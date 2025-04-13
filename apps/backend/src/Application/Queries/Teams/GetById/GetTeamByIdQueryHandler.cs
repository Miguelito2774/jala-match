using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetById;

internal sealed class GetTeamByIdQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetTeamByIdQuery, TeamResponse>
{
    public async Task<Result<TeamResponse>> Handle(
        GetTeamByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        Team? team = await context
            .Teams.AsNoTracking()
            .Include(t => t.Creator)
            .Include(t => t.Members)
            .ThenInclude(m => m.EmployeeProfile)
            .ThenInclude(ep => ep.User)
            .Include(t => t.Members)
            .ThenInclude(m => m.EmployeeProfile)
            .ThenInclude(ep => ep.Technologies)
            .ThenInclude(et => et.Technology)
            .Include(t => t.RequiredTechnologies)
            .ThenInclude(rt => rt.Technology)
            .FirstOrDefaultAsync(t => t.Id == query.TeamId, cancellationToken);

        if (team is null)
        {
            return Result.Failure<TeamResponse>(TeamErrors.NotFound(query.TeamId));
        }

        var requiredTechnologies = team
            .RequiredTechnologies.Select(rt => rt.Technology.Name)
            .ToList();

        if (!requiredTechnologies.Any() && !string.IsNullOrEmpty(team.RequiredTechnologiesJson))
        {
            requiredTechnologies =
                JsonSerializer.Deserialize<List<string>>(team.RequiredTechnologiesJson) ?? new();
        }

        List<TeamMemberDto> members = GetTeamMembersFromEntity(team);

        if (!members.Any() && !string.IsNullOrEmpty(team.MembersJson))
        {
            members = await GetTeamMembersFromJson(context, team.MembersJson, cancellationToken);
        }

        if (team.AiAnalysis != null)
        {
            return new TeamResponse(
                team.Id,
                team.Name,
                team.CreatorId,
                requiredTechnologies,
                members,
                team.CompatibilityScore,
                team.IsActive,
                team.AiAnalysis
            );
        }

        return new TeamResponse(
            team.Id,
            team.Name,
            team.CreatorId,
            requiredTechnologies,
            members,
            team.CompatibilityScore,
            team.IsActive,
            "{}"
        );
    }

    private static List<TeamMemberDto> GetTeamMembersFromEntity(Team team)
    {
        return team
            .Members.Select(m => new TeamMemberDto(
                m.EmployeeProfile.Id,
                $"{m.EmployeeProfile.FirstName} {m.EmployeeProfile.LastName}",
                m.EmployeeProfile.User.Role.ToString(),
                m.EmployeeProfile.Technologies.Select(t => t.Technology.Name).ToList(),
                m.EmployeeProfile.SfiaLevelGeneral,
                m.EmployeeProfile.Mbti,
                m.EmployeeProfile.PersonalInterests.Select(pi => pi.Name).ToList()
            ))
            .ToList();
    }

    private static async Task<List<TeamMemberDto>> GetTeamMembersFromJson(
        IApplicationDbContext context,
        string membersJson,
        CancellationToken cancellationToken
    )
    {
        List<Guid> memberIds = JsonSerializer.Deserialize<List<Guid>>(membersJson) ?? new();

        List<EmployeeProfile> employeeProfiles = await context
            .EmployeeProfiles.Include(ep => ep.User)
            .Include(ep => ep.Technologies)
            .ThenInclude(et => et.Technology)
            .Include(ep => ep.PersonalInterests)
            .Where(ep => memberIds.Contains(ep.Id))
            .ToListAsync(cancellationToken);

        return employeeProfiles
            .Select(ep => new TeamMemberDto(
                ep.Id,
                $"{ep.FirstName} {ep.LastName}",
                ep.User.Role.ToString(),
                ep.Technologies.Select(t => t.Technology.Name).ToList(),
                ep.SfiaLevelGeneral,
                ep.Mbti,
                ep.PersonalInterests.Select(pi => pi.Name).ToList()
            ))
            .ToList();
    }
}
