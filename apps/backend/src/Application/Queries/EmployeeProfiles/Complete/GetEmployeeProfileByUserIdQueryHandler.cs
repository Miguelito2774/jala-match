using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.Complete;

public sealed class GetEmployeeProfileByUserIdQueryHandler
    : IQueryHandler<GetEmployeeProfileByUserIdQuery, EmployeeProfileCompleteDto>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeProfileByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmployeeProfileCompleteDto>> Handle(
        GetEmployeeProfileByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context
            .EmployeeProfiles.Include(p => p.User)
            .Include(p => p.Languages)
            .Include(p => p.SpecializedRoles)
            .ThenInclude(sr => sr.SpecializedRole)
            .ThenInclude(sr => sr.TechnicalArea)
            .Include(p => p.Technologies)
            .ThenInclude(t => t.Technology)
            .ThenInclude(t => t.Category)
            .Include(p => p.WorkExperiences)
            .Include(p => p.PersonalInterests)
            .Include(p => p.Verifications)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure<EmployeeProfileCompleteDto>(
                new Error("Profile.NotFound", "Employee profile not found", ErrorType.Failure)
            );
        }

        var dto = new EmployeeProfileCompleteDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            GeneralInfo = new EmployeeProfileGeneralInfoDto
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Availability = profile.Availability,
                Country = profile.Country,
                Timezone = profile.Timezone,
                ProfilePictureUrl = profile.User.ProfilePictureUrl,
                Languages = profile
                    .Languages.Select(l => new EmployeeLanguageDto
                    {
                        Id = l.Id,
                        Language = l.Language,
                        Proficiency = l.Proficiency,
                    })
                    .ToList(),
                SpecializedRoles = profile
                    .SpecializedRoles.Select(sr => new EmployeeSpecializedRoleDto
                    {
                        Id = sr.Id,
                        SpecializedRoleId = sr.SpecializedRoleId,
                        RoleName = sr.SpecializedRole.Name,
                        TechnicalAreaName = sr.SpecializedRole.TechnicalArea.Name,
                        Level = sr.Level,
                        YearsExperience = sr.YearsExperience,
                    })
                    .ToList(),
            },
            TechnicalProfile = new EmployeeProfileTechnicalDto
            {
                SfiaLevelGeneral = profile.SfiaLevelGeneral,
                Mbti = profile.Mbti,
                Technologies = profile
                    .Technologies.Select(t => new EmployeeTechnologyDto
                    {
                        Id = t.Id,
                        TechnologyId = t.TechnologyId,
                        TechnologyName = t.Technology.Name,
                        CategoryName = t.Technology.Category.Name,
                        SfiaLevel = t.SfiaLevel,
                        YearsExperience = t.YearsExperience,
                        Version = t.Version,
                    })
                    .ToList(),
                SpecializedRoles = profile
                    .SpecializedRoles.Select(sr => new EmployeeSpecializedRoleDto
                    {
                        Id = sr.Id,
                        SpecializedRoleId = sr.SpecializedRoleId,
                        RoleName = sr.SpecializedRole.Name,
                        TechnicalAreaName = sr.SpecializedRole.TechnicalArea.Name,
                        Level = sr.Level,
                        YearsExperience = sr.YearsExperience,
                    })
                    .ToList(),
            },
            WorkExperiences = profile
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
                .ToList(),
            PersonalInterests = profile
                .PersonalInterests.Select(pi => new PersonalInterestDto
                {
                    Id = pi.Id,
                    Name = pi.Name,
                    SessionDurationMinutes = pi.SessionDurationMinutes,
                    Frequency = pi.Frequency,
                    InterestLevel = pi.InterestLevel,
                })
                .ToList(),
            VerificationStatus = profile.VerificationStatus,
            VerificationNotes = profile.VerificationNotes,
            HasVerificationRequests = profile.Verifications.Any(),
        };

        return Result.Success(dto);
    }
}
