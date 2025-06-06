using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
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

    public ApproveProfileVerificationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<VerificationDecisionResponse>> Handle(
        ApproveProfileVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        // Verificar que el profile existe y está pendiente
        EmployeeProfile? profile = await _context.EmployeeProfiles.FirstOrDefaultAsync(
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

        var response = new VerificationDecisionResponse
        {
            Success = true,
            Message = "Profile approved successfully",
            NewStatus = VerificationStatus.Approved,
        };

        return Result.Success(response);
    }
}
