using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetByMemberId;

public sealed class GetTeamsByMemberIdQueryHandler
    : IQueryHandler<GetTeamsByMemberIdQuery, List<EmployeeTeamResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetTeamsByMemberIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EmployeeTeamResponse>>> Handle(
        GetTeamsByMemberIdQuery request,
        CancellationToken cancellationToken
    )
    {
        List<EmployeeTeamResponse> teams = await _context
            .TeamMembers.Include(tm => tm.Team)
            .ThenInclude(t => t!.Creator)
            .Include(tm => tm.Team)
            .ThenInclude(t => t!.Members)
            .ThenInclude(m => m.EmployeeProfile)
            .ThenInclude(ep => ep!.User)
            .Include(tm => tm.Team)
            .ThenInclude(t => t!.Members)
            .ThenInclude(m => m.EmployeeProfile)
            .ThenInclude(ep => ep!.SpecializedRoles)
            .ThenInclude(sr => sr.SpecializedRole)
            .ThenInclude(sr => sr.TechnicalArea)
            .Include(tm => tm.Team)
            .ThenInclude(t => t!.Members)
            .ThenInclude(m => m.EmployeeProfile)
            .ThenInclude(ep => ep!.Technologies)
            .ThenInclude(et => et.Technology)
            .ThenInclude(t => t.Category)
            .Include(tm => tm.Team)
            .ThenInclude(t => t!.Members)
            .ThenInclude(m => m.EmployeeProfile)
            .ThenInclude(ep => ep!.Languages)
            .Include(tm => tm.Team)
            .ThenInclude(t => t!.Members)
            .ThenInclude(m => m.EmployeeProfile)
            .ThenInclude(ep => ep!.PersonalInterests)
            .Where(tm => tm.EmployeeProfileId == request.EmployeeProfileId)
            .Select(tm => new EmployeeTeamResponse
            {
                TeamId = tm.Team!.Id,
                TeamName = tm.Team.Name,
                CreatorName = tm.Team.Creator.Email,
                CompatibilityScore = tm.Team.CompatibilityScore,
                IsActive = tm.Team.IsActive,
                IsCurrentUserLeader = tm.IsLeader,
                Teammates = tm
                    .Team.Members.Where(m => m.EmployeeProfile != null)
                    .Select(m => new TeammateDto
                    {
                        EmployeeProfileId = m.EmployeeProfileId,
                        FirstName = m.EmployeeProfile!.FirstName,
                        LastName = m.EmployeeProfile.LastName,
                        Email = m.EmployeeProfile.User.Email,
                        Role = m.Role,
                        SfiaLevel = m.SfiaLevel,
                        IsLeader = m.IsLeader,
                        Country = m.EmployeeProfile.Country,
                        Timezone = m.EmployeeProfile.Timezone,
                        Mbti = m.EmployeeProfile.Mbti,
                        Availability = m.EmployeeProfile.Availability,
                        SfiaLevelGeneral = m.EmployeeProfile.SfiaLevelGeneral,
                        ProfilePictureUrl = m.EmployeeProfile.User.ProfilePictureUrl,
                        SpecializedRoles = m
                            .EmployeeProfile.SpecializedRoles.Select(
                                sr => new TeammateSpecializedRoleDto
                                {
                                    RoleName = sr.SpecializedRole.Name,
                                    TechnicalArea = sr.SpecializedRole.TechnicalArea.Name,
                                    Level = sr.Level,
                                    YearsExperience = sr.YearsExperience,
                                }
                            )
                            .ToList(),
                        Technologies = m
                            .EmployeeProfile.Technologies.Select(et => new TeammateTechnologyDto
                            {
                                TechnologyName = et.Technology.Name,
                                CategoryName = et.Technology.Category.Name,
                                SfiaLevel = et.SfiaLevel,
                                YearsExperience = et.YearsExperience,
                                Version = et.Version,
                            })
                            .ToList(),
                        Languages = m
                            .EmployeeProfile.Languages.Select(el => new TeammateLanguageDto
                            {
                                Language = el.Language,
                                Proficiency = el.Proficiency,
                            })
                            .ToList(),
                        PersonalInterests = m
                            .EmployeeProfile.PersonalInterests.Select(pi => pi.Name)
                            .ToList(),
                    })
                    .ToList(),
            })
            .ToListAsync(cancellationToken);

        return Result.Success(teams);
    }
}
