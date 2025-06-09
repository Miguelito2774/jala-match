using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.Verifications;

public sealed class ApproveProfileVerificationCommandHandler
    : ICommandHandler<ApproveProfileVerificationCommand, VerificationDecisionResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ApproveProfileVerificationCommandHandler(
        IApplicationDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<VerificationDecisionResponse>> Handle(
        ApproveProfileVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        // Verificar que el profile existe y está pendiente
        EmployeeProfile? profile = await _context.EmployeeProfiles
            .Include(ep => ep.User)
            .FirstOrDefaultAsync(
                ep => ep.Id == request.EmployeeProfileId,
                cancellationToken
            );

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
            Status = VerificationStatus.Approved,
            SfiaProposed = request.SfiaProposed,
            Notes = request.Notes,
            ReviewedAt = DateTime.UtcNow,
            EmployeeProfile = profile,
            Reviewer = reviewer,
        };

        // Actualizar el perfil
        profile.VerificationStatus = VerificationStatus.Approved;
        profile.VerificationNotes = request.Notes;
        profile.Availability = true; // El perfil ya está disponible para asignaciones

        // Guardar cambios
        _context.ProfileVerifications.Add(verification);
        await _context.SaveChangesAsync(cancellationToken);

        // Send notification email
        try
        {
            await _notificationService.SendProfileApprovedNotificationAsync(
                profile.User.Email,
                $"{profile.FirstName} {profile.LastName}",
                request.SfiaProposed,
                request.Notes,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            // Log the exception but don't fail the operation
            // TODO: Add proper logging
            Console.WriteLine($"Failed to send notification email: {ex.Message}");
        }

        var response = new VerificationDecisionResponse
        {
            Success = true,
            Message = "Profile approved successfully",
            NewStatus = VerificationStatus.Approved,
        };

        return Result.Success(response);
    }
}
