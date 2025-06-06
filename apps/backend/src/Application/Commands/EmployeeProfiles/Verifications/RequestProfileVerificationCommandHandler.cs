using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.Verifications;

public sealed class RequestProfileVerificationCommandHandler
    : ICommandHandler<RequestProfileVerificationCommand>
{
    private readonly IApplicationDbContext _context;

    public RequestProfileVerificationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        RequestProfileVerificationCommand request,
        CancellationToken cancellationToken
    )
    {
        // Get the employee profile with all necessary data for validation
        EmployeeProfile? profile = await _context
            .EmployeeProfiles.Include(p => p.SpecializedRoles)
            .Include(p => p.Technologies)
            .Include(p => p.WorkExperiences)
            .Include(p => p.PersonalInterests)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure(
                new Error(
                    "EmployeeProfile.NotFound",
                    "Employee profile not found",
                    ErrorType.Failure
                )
            );
        }

        // Check if profile is already verified or pending
        if (profile.VerificationStatus == Domain.Entities.Enums.VerificationStatus.Approved)
        {
            return Result.Failure(
                new Error(
                    "ProfileVerification.AlreadyVerified",
                    "Profile is already verified",
                    ErrorType.Failure
                )
            );
        }

        if (profile.VerificationStatus == Domain.Entities.Enums.VerificationStatus.Pending)
        {
            return Result.Failure(
                new Error(
                    "ProfileVerification.AlreadyRequested",
                    "Verification request is already pending",
                    ErrorType.Failure
                )
            );
        }

        // Validate all 4 sections are complete
        Result validationResult = ValidateProfileCompletion(profile);
        if (validationResult.IsFailure)
        {
            return validationResult;
        }

        // Update verification status to pending
        profile.VerificationStatus = Domain.Entities.Enums.VerificationStatus.Pending;

        // Create a verification request record
        var verificationRequest = new ProfileVerification
        {
            Id = Guid.NewGuid(),
            EmployeeProfileId = profile.Id,
            ReviewerId = null,
            SfiaProposed = profile.SfiaLevelGeneral, // Agregar el nivel SFIA propuesto
            Status = Domain.Entities.Enums.VerificationStatus.Pending,
            RequestedAt = DateTime.UtcNow,
            ReviewedAt = null,
            EmployeeProfile = profile,
            Reviewer = null,
        };

        _context.ProfileVerifications.Add(verificationRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result ValidateProfileCompletion(EmployeeProfile profile)
    {
        var missingRequirements = new List<string>();

        // 1. General Information validation
        if (
            string.IsNullOrWhiteSpace(profile.FirstName)
            || string.IsNullOrWhiteSpace(profile.LastName)
            || string.IsNullOrWhiteSpace(profile.Country)
            || string.IsNullOrWhiteSpace(profile.Timezone)
        )
        {
            missingRequirements.Add(
                "Complete general information (first name, last name, country, timezone)"
            );
        }

        // 2. Technical Profile validation
        if (
            profile.SfiaLevelGeneral <= 0
            || string.IsNullOrWhiteSpace(profile.Mbti)
            || profile.SpecializedRoles == null
            || profile.SpecializedRoles.Count == 0
        )
        {
            missingRequirements.Add(
                "Complete technical profile (SFIA level, MBTI, at least one specialized role)"
            );
        }

        // 3. Work Experience validation
        if (profile.WorkExperiences.Count == 0)
        {
            missingRequirements.Add("Add at least one work experience");
        }

        // 4. Personal Interests validation
        if (profile.PersonalInterests.Count == 0)
        {
            missingRequirements.Add("Add at least one personal interest");
        }

        if (missingRequirements.Count > 0)
        {
            return Result.Failure(
                new Error(
                    "ProfileVerification.IncompleteProfile",
                    $"Profile is incomplete. Missing requirements: {string.Join(", ", missingRequirements)}",
                    ErrorType.Validation
                )
            );
        }

        return Result.Success();
    }
}
