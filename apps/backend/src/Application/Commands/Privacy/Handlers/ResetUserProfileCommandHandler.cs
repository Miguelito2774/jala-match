using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Commands.Privacy;
using Domain.Entities.Enums;
using Domain.Entities.Privacy;
using Domain.Entities.Privacy.Enums;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Privacy.Handlers;

internal sealed class ResetUserProfileCommandHandler
    : ICommandHandler<ResetUserProfileCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ResetUserProfileCommandHandler> _logger;

    public ResetUserProfileCommandHandler(
        IApplicationDbContext context,
        IPrivacyAuditLogRepository auditLogRepository,
        ILogger<ResetUserProfileCommandHandler> logger
    )
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        ResetUserProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // Get the user's employee profile
            EmployeeProfile? employeeProfile = await _context
                .EmployeeProfiles.Include(ep => ep.Technologies)
                .Include(ep => ep.WorkExperiences)
                .Include(ep => ep.PersonalInterests)
                .FirstOrDefaultAsync(ep => ep.UserId == request.UserId, cancellationToken);

            if (employeeProfile == null)
            {
                return Result.Failure<bool>(
                    new Error(
                        "ProfileReset.ProfileNotFound",
                        "Perfil de empleado no encontrado",
                        ErrorType.NotFound
                    )
                );
            }

            // Clear profile data based on requested data types
            foreach (string dataType in request.DataTypes)
            {
                switch (dataType.ToUpperInvariant())
                {
                    case "PROFILE":
                        // Reset verification status and clear general profile info
                        employeeProfile.VerificationStatus = VerificationStatus.NotRequested;
                        employeeProfile.VerificationNotes = null;
                        employeeProfile.Availability = true;
                        employeeProfile.Country = string.Empty;
                        employeeProfile.Timezone = string.Empty;
                        employeeProfile.SfiaLevelGeneral = 1;
                        employeeProfile.Mbti = string.Empty;
                        break;

                    case "TECHNOLOGIES":
                        // Remove all technologies
                        List<Domain.Entities.Technologies.EmployeeTechnology> technologies =
                            await _context
                                .EmployeeTechnologies.Where(et =>
                                    et.EmployeeProfileId == employeeProfile.Id
                                )
                                .ToListAsync(cancellationToken);
                        _context.EmployeeTechnologies.RemoveRange(technologies);
                        break;

                    case "EXPERIENCES":
                        // Remove all work experiences
                        List<WorkExperience> experiences = await _context
                            .WorkExperiences.Where(we => we.EmployeeProfileId == employeeProfile.Id)
                            .ToListAsync(cancellationToken);
                        _context.WorkExperiences.RemoveRange(experiences);
                        break;

                    case "INTERESTS":
                        // Remove all personal interests
                        List<PersonalInterest> interests = await _context
                            .PersonalInterests.Where(pi =>
                                pi.EmployeeProfileId == employeeProfile.Id
                            )
                            .ToListAsync(cancellationToken);
                        _context.PersonalInterests.RemoveRange(interests);
                        break;

                    case "LANGUAGES":
                        List<EmployeeLanguage> languages = await _context
                            .EmployeeLanguages.Where(el =>
                                el.EmployeeProfileId == employeeProfile.Id
                            )
                            .ToListAsync(cancellationToken);
                        _context.EmployeeLanguages.RemoveRange(languages);
                        break;
                }
            }

            // Save all changes
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Profile reset completed for user {UserId}. Data types: {DataTypes}",
                request.UserId,
                string.Join(", ", request.DataTypes)
            );

            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting profile for user {UserId}", request.UserId);
            return Result.Failure<bool>(
                new Error(
                    "ProfileReset.ResetFailed",
                    "Error al reiniciar el perfil",
                    ErrorType.Failure
                )
            );
        }
    }
}
