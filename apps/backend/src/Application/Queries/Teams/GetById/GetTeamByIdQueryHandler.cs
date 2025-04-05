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
        CancellationToken cancellationToken)
    {
        Team? team = await context.Teams
            .AsNoTracking()
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == query.TeamId, cancellationToken);

        if (team is null)
        {
            return Result.Failure<TeamResponse>(TeamErrors.NotFound(query.TeamId));
        }

        List<TeamMemberDto> members = await GetTeamMembers(context, team.Members, cancellationToken);

        return new TeamResponse(
            team.Id,
            team.Name,
            team.CreatorId,
            JsonSerializer.Deserialize<List<string>>(team.RequiredTechnologies) ?? new(),
            members,
            team.CompatibilityScore,
            team.IsActive,
            team.AiAnalysis);
    }

    private static async Task<List<TeamMemberDto>> GetTeamMembers(
        IApplicationDbContext context,
        string membersJson,
        CancellationToken cancellationToken)
    {
        List<Guid> memberIds = JsonSerializer.Deserialize<List<Guid>>(membersJson) ?? new();

        List<EmployeeProfile> employeeProfiles = await context.EmployeeProfiles
            .Include(ep => ep.User)
            .Include(ep => ep.Technologies)
            .ThenInclude(et => et.Technology)
            .Where(ep => memberIds.Contains(ep.Id))
            .ToListAsync(cancellationToken);

        return employeeProfiles
            .Select(ep => new TeamMemberDto(
                ep.Id,
                $"{ep.FirstName} {ep.LastName}",
                ep.User.Role.ToString(),
                ep.Technologies.Select(t => t.Technology.Name).ToList(),
                ep.SfiaLevelGeneral ?? 0,
                ep.Mbti,
                JsonSerializer.Deserialize<List<string>>(ep.PersonalInterests) ?? new()
            ))
            .ToList();
    }
}
