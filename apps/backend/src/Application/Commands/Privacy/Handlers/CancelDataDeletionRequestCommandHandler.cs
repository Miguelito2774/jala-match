using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Commands.Privacy;
using Domain.Entities.Privacy;
using Domain.Entities.Privacy.Enums;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Privacy.Handlers;

internal sealed class CancelDataDeletionRequestCommandHandler
    : ICommandHandler<CancelDataDeletionRequestCommand>
{
    private readonly IDataDeletionRequestRepository _deletionRequestRepository;
    private readonly IPrivacyAuditLogRepository _auditLogRepository;
    private readonly ILogger<CancelDataDeletionRequestCommandHandler> _logger;

    public CancelDataDeletionRequestCommandHandler(
        IDataDeletionRequestRepository deletionRequestRepository,
        IPrivacyAuditLogRepository auditLogRepository,
        ILogger<CancelDataDeletionRequestCommandHandler> logger
    )
    {
        _deletionRequestRepository = deletionRequestRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CancelDataDeletionRequestCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            DataDeletionOrder? deletionRequest = await _deletionRequestRepository.GetByIdAsync(
                request.RequestId,
                cancellationToken
            );
            if (deletionRequest == null)
            {
                return Result.Failure(
                    new Error(
                        "Privacy.DeletionRequestNotFound",
                        "Solicitud de eliminación no encontrada",
                        ErrorType.NotFound
                    )
                );
            }

            if (deletionRequest.UserId != request.UserId)
            {
                return Result.Failure(
                    new Error(
                        "Privacy.Unauthorized",
                        "No autorizado para cancelar esta solicitud",
                        ErrorType.Failure
                    )
                );
            }

            if (deletionRequest.Status != DataDeletionStatus.Pending)
            {
                return Result.Failure(
                    new Error(
                        "Privacy.DeletionRequestNotCancellable",
                        "La solicitud no puede ser cancelada en su estado actual",
                        ErrorType.Conflict
                    )
                );
            }

            deletionRequest.Status = DataDeletionStatus.Cancelled;
            deletionRequest.CancellationReason = "Cancelled by user";

            await _deletionRequestRepository.UpdateAsync(deletionRequest, cancellationToken);

            // Create audit log
            var auditLog = new PrivacyAuditLog
            {
                UserId = request.UserId,
                Action = PrivacyAction.DataDeletionCancelled,
                Details = $"Data deletion request {request.RequestId} cancelled by user",
                Timestamp = DateTime.UtcNow,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
            };

            await _auditLogRepository.AddAsync(auditLog, cancellationToken);

            await _deletionRequestRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Data deletion request {RequestId} cancelled by user {UserId}",
                request.RequestId,
                request.UserId
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error cancelling data deletion request {RequestId} for user {UserId}",
                request.RequestId,
                request.UserId
            );
            return Result.Failure(
                new Error(
                    "Privacy.CancellationFailed",
                    "Error al cancelar la solicitud de eliminación",
                    ErrorType.Failure
                )
            );
        }
    }
}
