using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Commands.Privacy;
using Application.DTOs;
using Domain.Entities.Privacy;
using Domain.Entities.Privacy.Enums;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Privacy.Handlers;

internal sealed class CreateDataDeletionRequestCommandHandler
    : ICommandHandler<CreateDataDeletionRequestCommand, DataDeletionResponseDto>
{
    private readonly IDataDeletionRequestRepository _deletionRequestRepository;
    private readonly IPrivacyAuditLogRepository _auditLogRepository;
    private readonly ILogger<CreateDataDeletionRequestCommandHandler> _logger;

    public CreateDataDeletionRequestCommandHandler(
        IDataDeletionRequestRepository deletionRequestRepository,
        IPrivacyAuditLogRepository auditLogRepository,
        ILogger<CreateDataDeletionRequestCommandHandler> logger
    )
    {
        _deletionRequestRepository = deletionRequestRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result<DataDeletionResponseDto>> Handle(
        CreateDataDeletionRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // Check if user already has a pending deletion request
            DataDeletionOrder? existingRequest =
                await _deletionRequestRepository.GetPendingByUserIdAsync(
                    request.UserId,
                    cancellationToken
                );
            if (existingRequest != null)
            {
                return Result.Failure<DataDeletionResponseDto>(
                    new Error(
                        "Privacy.DeletionRequestExists",
                        "Ya existe una solicitud de eliminación pendiente",
                        ErrorType.Conflict
                    )
                );
            }

            DateTime now = DateTime.UtcNow;
            DateTime scheduledDeletionDate = now.AddDays(30); // 30-day grace period

            var deletionRequest = new DataDeletionOrder
            {
                UserId = request.UserId,
                Status = DataDeletionStatus.Pending,
                RequestDate = now,
                ScheduledDeletionDate = scheduledDeletionDate,
                DataTypes = request.Request.DataTypes,
                Reason = request.Request.Reason,
            };

            await _deletionRequestRepository.AddAsync(deletionRequest, cancellationToken);

            // Create audit log
            var auditLog = new PrivacyAuditLog
            {
                UserId = request.UserId,
                Action = PrivacyAction.DataDeletionRequested,
                Details =
                    $"Data deletion requested for types: {string.Join(", ", request.Request.DataTypes)}. Scheduled for: {scheduledDeletionDate:yyyy-MM-dd}",
                Timestamp = now,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
            };

            await _auditLogRepository.AddAsync(auditLog, cancellationToken);

            await _deletionRequestRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Data deletion request created for user {UserId} with ID {RequestId}",
                request.UserId,
                deletionRequest.Id
            );

            var response = new DataDeletionResponseDto(
                deletionRequest.Id.ToString(),
                scheduledDeletionDate,
                "Solicitud de eliminación de datos enviada exitosamente. Tus datos serán eliminados en 30 días."
            );

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating data deletion request for user {UserId}",
                request.UserId
            );
            return Result.Failure<DataDeletionResponseDto>(
                new Error(
                    "Privacy.DeletionRequestFailed",
                    "Error al procesar la solicitud de eliminación",
                    ErrorType.Failure
                )
            );
        }
    }
}
