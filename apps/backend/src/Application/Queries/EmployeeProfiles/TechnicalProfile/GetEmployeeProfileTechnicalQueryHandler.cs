using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.TechnicalProfile;

public sealed class GetEmployeeProfileTechnicalQueryHandler
    : IQueryHandler<GetEmployeeProfileTechnicalQuery, EmployeeProfileTechnicalDto>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeProfileTechnicalQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmployeeProfileTechnicalDto>> Handle(
        GetEmployeeProfileTechnicalQuery request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context
            .EmployeeProfiles.Include(p => p.Technologies)
            .ThenInclude(t => t.Technology)
            .ThenInclude(t => t.Category)
            .Include(p => p.SpecializedRoles)
            .ThenInclude(sr => sr.SpecializedRole)
            .ThenInclude(r => r.TechnicalArea)
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

        if (profile == null)
        {
            return Result.Failure<EmployeeProfileTechnicalDto>(
                new Error("Profile.NotFound", "Employee profile not found", ErrorType.Failure)
            );
        }

        var dto = new EmployeeProfileTechnicalDto
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
        };

        return Result.Success(dto);
    }
}
