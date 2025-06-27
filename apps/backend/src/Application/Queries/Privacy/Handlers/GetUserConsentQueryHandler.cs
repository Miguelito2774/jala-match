using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.DTOs;
using Application.Queries.Privacy;
using Domain.Entities.Privacy;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.Privacy.Handlers;

internal sealed class GetUserConsentQueryHandler
    : IQueryHandler<GetUserConsentQuery, ConsentSettingsDto>
{
    private readonly IUserPrivacyConsentRepository _consentRepository;
    private readonly ILogger<GetUserConsentQueryHandler> _logger;

    public GetUserConsentQueryHandler(
        IUserPrivacyConsentRepository consentRepository,
        ILogger<GetUserConsentQueryHandler> logger
    )
    {
        _consentRepository = consentRepository;
        _logger = logger;
    }

    public async Task<Result<ConsentSettingsDto>> Handle(
        GetUserConsentQuery request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            UserPrivacyConsent? consent = await _consentRepository.GetByUserIdAsync(
                request.UserId,
                cancellationToken
            );

            if (consent == null)
            {
                // Return default consent settings if none exist
                var defaultConsent = new ConsentSettingsDto(
                    TeamMatchingAnalysis: true,
                    LastUpdated: DateTime.UtcNow,
                    Version: "1.0"
                );

                return Result.Success(defaultConsent);
            }

            var consentDto = new ConsentSettingsDto(
                consent.TeamMatchingAnalysis,
                consent.LastUpdated,
                consent.Version
            );

            return Result.Success(consentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving consent settings for user {UserId}",
                request.UserId
            );
            return Result.Failure<ConsentSettingsDto>(
                new Error(
                    "Privacy.ConsentRetrievalFailed",
                    "Error al obtener la configuración de consentimiento",
                    ErrorType.Failure
                )
            );
        }
    }
}
