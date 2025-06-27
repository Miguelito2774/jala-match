using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.DTOs;
using Domain.Entities.Privacy;
using Domain.Entities.Privacy.Enums;
using Domain.Entities.Profiles;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.Privacy.Handlers;

internal sealed class ExportUserDataQueryHandler : IQueryHandler<ExportUserDataQuery, DataExportDto>
{
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly IUserPrivacyConsentRepository _consentRepository;
    private readonly IPrivacyAuditLogRepository _auditLogRepository;
    private readonly ILogger<ExportUserDataQueryHandler> _logger;

    public ExportUserDataQueryHandler(
        IEmployeeProfileRepository employeeProfileRepository,
        IUserPrivacyConsentRepository consentRepository,
        IPrivacyAuditLogRepository auditLogRepository,
        ILogger<ExportUserDataQueryHandler> logger
    )
    {
        _employeeProfileRepository = employeeProfileRepository;
        _consentRepository = consentRepository;
        _auditLogRepository = auditLogRepository;
        _logger = logger;
    }

    public async Task<Result<DataExportDto>> Handle(
        ExportUserDataQuery request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            EmployeeProfile? employeeProfile =
                await _employeeProfileRepository.GetByUserIdWithAllDataAsync(
                    request.UserId,
                    cancellationToken
                );
            if (employeeProfile == null)
            {
                return Result.Failure<DataExportDto>(
                    new Error(
                        "Privacy.ProfileNotFound",
                        "Perfil de empleado no encontrado",
                        ErrorType.NotFound
                    )
                );
            }

            UserPrivacyConsent? consent = await _consentRepository.GetByUserIdAsync(
                request.UserId,
                cancellationToken
            );

            // Build personal info
            var personalInfo = new EmployeeProfileGeneralInfoDto
            {
                FirstName = employeeProfile.FirstName,
                LastName = employeeProfile.LastName,
                Availability = employeeProfile.Availability,
                Country = employeeProfile.Country,
                Timezone = employeeProfile.Timezone,
                ProfilePictureUrl = employeeProfile.User?.ProfilePictureUrl,
                Languages = employeeProfile
                    .Languages.Select(el => new EmployeeLanguageDto
                    {
                        Id = el.Id,
                        Language = el.Language,
                        Proficiency = el.Proficiency,
                    })
                    .ToList(),
                SpecializedRoles = employeeProfile
                    .SpecializedRoles.Select(esr => new EmployeeSpecializedRoleDto
                    {
                        Id = esr.Id,
                        SpecializedRoleId = esr.SpecializedRoleId,
                        RoleName = esr.SpecializedRole?.Name ?? "Unknown Role",
                        TechnicalAreaName = esr.SpecializedRole?.TechnicalArea?.Name ?? "Unknown Area",
                        Level = esr.Level,
                        YearsExperience = esr.YearsExperience,
                    })
                    .ToList(),
            };

            // Build technologies list
            var technologies = employeeProfile
                .Technologies.Select(et => new EmployeeTechnologyDto
                {
                    Id = et.Id,
                    TechnologyId = et.TechnologyId,
                    TechnologyName = et.Technology?.Name ?? "Unknown Technology",
                    CategoryName = et.Technology?.Category?.Name ?? "Unknown Category",
                    YearsExperience = et.YearsExperience,
                    SfiaLevel = et.SfiaLevel,
                    Version = et.Version,
                })
                .ToList();

            // Build interests list
            var interests = employeeProfile
                .PersonalInterests.Select(pi => new PersonalInterestDto
                {
                    Id = pi.Id,
                    Name = pi.Name,
                    SessionDurationMinutes = pi.SessionDurationMinutes,
                    Frequency = pi.Frequency,
                    InterestLevel = pi.InterestLevel,
                })
                .ToList();

            // Build work experience list
            var workExperiences = employeeProfile
                .WorkExperiences.Select(we => new WorkExperienceDto
                {
                    Id = we.Id,
                    ProjectName = we.ProjectName,
                    Description = we.Description,
                    Tools = we.Tools,
                    ThirdParties = we.ThirdParties,
                    Frameworks = we.Frameworks,
                    VersionControl = we.VersionControl,
                    ProjectManagement = we.ProjectManagement,
                    Responsibilities = we.Responsibilities,
                    StartDate = we.StartDate,
                    EndDate = we.EndDate,
                })
                .ToList();

            // Build team participation list
            var teamParticipation = employeeProfile
                .TeamMemberships.Select(tm => new TeamParticipationDto(
                    tm.Team?.Name ?? "Unknown Team",
                    tm.Role,
                    tm.SfiaLevel,
                    tm.IsLeader,
                    tm.Team?.RequiredTechnologies?.Select(rt => rt.Technology?.Name ?? "Unknown Technology").ToList() ?? new List<string>()
                ))
                .ToList();

            // Build consent settings
            ConsentSettingsDto consentSettings =
                consent != null
                    ? new ConsentSettingsDto(
                        consent.TeamMatchingAnalysis,
                        consent.LastUpdated,
                        consent.Version
                    )
                    : new ConsentSettingsDto(
                        true,
                        DateTime.UtcNow,
                        "1.0"
                    );

            var exportData = new DataExportDto(
                personalInfo,
                technologies,
                interests,
                workExperiences,
                teamParticipation,
                consentSettings,
                DateTime.UtcNow,
                "JSON"
            );

            // Create audit log for data export
            var auditLog = new PrivacyAuditLog
            {
                UserId = request.UserId,
                Action = PrivacyAction.DataExported,
                Details = "User data exported as JSON file",
                Timestamp = DateTime.UtcNow,
            };

            await _auditLogRepository.AddAsync(auditLog, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Data exported for user {UserId}", request.UserId);

            return Result.Success(exportData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data for user {UserId}", request.UserId);
            return Result.Failure<DataExportDto>(
                new Error(
                    "Privacy.DataExportFailed",
                    "Error al exportar los datos",
                    ErrorType.Failure
                )
            );
        }
    }
}
