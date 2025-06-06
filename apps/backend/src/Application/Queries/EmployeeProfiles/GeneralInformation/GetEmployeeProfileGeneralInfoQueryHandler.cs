using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.GeneralInformation;

public sealed class GetEmployeeProfileGeneralInfoQueryHandler
    : IQueryHandler<GetEmployeeProfileGeneralInfoQuery, EmployeeProfileGeneralInfoDto>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeProfileGeneralInfoQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmployeeProfileGeneralInfoDto>> Handle(
        GetEmployeeProfileGeneralInfoQuery request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context
            .EmployeeProfiles.Include(p => p.User)
            .Include(p => p.Languages)
            .Include(p => p.SpecializedRoles)
            .ThenInclude(sr => sr.SpecializedRole)
            .ThenInclude(sr => sr.TechnicalArea)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure<EmployeeProfileGeneralInfoDto>(
                new Error("Profile.NotFound", "Employee profile not found", ErrorType.Failure)
            );
        }

        var dto = new EmployeeProfileGeneralInfoDto
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
        };

        return Result.Success(dto);
    }
}
