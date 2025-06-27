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

internal sealed class UpdateConsentCommandHandler : ICommandHandler<UpdateConsentCommand>
{
    private readonly IUserPrivacyConsentRepository _consentRepository;
    private readonly IPrivacyAuditLogRepository _auditLogRepository;
    private readonly ILogger<UpdateConsentCommandHandler> _logger;

    public UpdateConsentCommandHandler(
        IUserPrivacyConsentRepository consentRepository,
        IPrivacyAuditLogRepository auditLogRepository,
        ILogger<UpdateConsentCommandHandler> logger
    )
    {
        _consentRepository = consentRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateConsentCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            UserPrivacyConsent? existingConsent = await _consentRepository.GetByUserIdAsync(
                request.UserId,
                cancellationToken
            );
            DateTime now = DateTime.UtcNow;

            if (existingConsent == null)
            {
                // Create new consent record
                var newConsent = new UserPrivacyConsent
                {
                    UserId = request.UserId,
                    TeamMatchingAnalysis = request.Request.TeamMatchingAnalysis,
                    LastUpdated = now,
                    Version = "1.0",
                    CreatedAt = now,
                };

                await _consentRepository.AddAsync(newConsent, cancellationToken);
            }
            else
            {
                // Update existing consent
                existingConsent.TeamMatchingAnalysis = request.Request.TeamMatchingAnalysis;
                existingConsent.LastUpdated = now;

                await _consentRepository.UpdateAsync(existingConsent, cancellationToken);
            }

            // Create audit log
            var auditLog = new PrivacyAuditLog
            {
                UserId = request.UserId,
                Action = PrivacyAction.ConsentUpdated,
                Details =
                    $"Privacy consents updated: TeamMatching={request.Request.TeamMatchingAnalysis}",
                Timestamp = now,
            };

            await _auditLogRepository.AddAsync(auditLog, cancellationToken);

            await _consentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Privacy consent updated for user {UserId}", request.UserId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating privacy consent for user {UserId}",
                request.UserId
            );
            return Result.Failure(
                new Error(
                    "Privacy.ConsentUpdateFailed",
                    "Error al actualizar el consentimiento",
                    ErrorType.Failure
                )
            );
        }
    }
}
