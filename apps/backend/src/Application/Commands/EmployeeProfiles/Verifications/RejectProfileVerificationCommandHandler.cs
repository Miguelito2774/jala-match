using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.Verifications;

public sealed class RejectProfileVerificationCommandHandler
    : ICommandHandler<RejectProfileVerificationCommand, VerificationDecisionResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly ILogger<RejectProfileVerificationCommandHandler> _logger;

    public RejectProfileVerificationCommandHandler(
        IApplicationDbContext context,
        INotificationService notificationService,
        ILogger<RejectProfileVerificationCommandHandler> logger
    )
    {
        _context = context;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<VerificationDecisionResponse>> Handle(
        RejectProfileVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context
            .EmployeeProfiles.Include(ep => ep.User)
            .FirstOrDefaultAsync(ep => ep.Id == request.EmployeeProfileId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure<VerificationDecisionResponse>(
                new Error(
                    "EmployeeProfile.NotFound",
                    "Employee profile not found",
                    ErrorType.NotFound
                )
            );
        }

        if (profile.VerificationStatus != VerificationStatus.Pending)
        {
            return Result.Failure<VerificationDecisionResponse>(
                new Error(
                    "ProfileVerification.InvalidStatus",
                    "Profile is not pending verification",
                    ErrorType.Validation
                )
            );
        }

        // Verificar que el reviewer existe y es manager
        User? reviewer = await _context.Users.FirstOrDefaultAsync(
            u => u.Id == request.ReviewerId,
            cancellationToken
        );

        if (reviewer == null || reviewer.Role != Role.Manager)
        {
            return Result.Failure<VerificationDecisionResponse>(
                new Error(
                    "ProfileVerification.InvalidReviewer",
                    "Reviewer not found or is not a manager",
                    ErrorType.Validation
                )
            );
        }

        // Crear registro de verificación
        var verification = new ProfileVerification
        {
            EmployeeProfileId = request.EmployeeProfileId,
            ReviewerId = request.ReviewerId,
            Status = VerificationStatus.Rejected,
            Notes = request.Notes,
            ReviewedAt = DateTime.UtcNow,
            EmployeeProfile = profile,
            Reviewer = reviewer,
        };

        // Actualizar el perfil - rechazado puede solicitar nuevamente
        profile.VerificationStatus = VerificationStatus.Rejected;
        profile.VerificationNotes = request.Notes;
        profile.Availability = false; // No disponible hasta nueva aprobación

        // Guardar cambios
        _context.ProfileVerifications.Add(verification);
        await _context.SaveChangesAsync(cancellationToken);

        // Fire and forget: Send rejection notification in background without blocking response
        _ = Task.Run(async () =>
        {
            try
            {
                await _notificationService.SendProfileRejectedNotificationAsync(
                    profile.User.Email,
                    $"{profile.FirstName} {profile.LastName}",
                    request.Notes ?? "No se proporcionaron notas adicionales",
                    CancellationToken.None
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending profile rejection notification for employee {EmployeeId}",
                    profile.Id
                );
            }
        }, CancellationToken.None);

        var response = new VerificationDecisionResponse
        {
            Success = true,
            Message = "Profile rejected successfully",
            NewStatus = VerificationStatus.Rejected,
        };

        return Result.Success(response);
    }
}
