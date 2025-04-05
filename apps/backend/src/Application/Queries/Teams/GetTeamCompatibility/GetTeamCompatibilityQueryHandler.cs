using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetTeamCompatibility;

internal sealed class GetTeamCompatibilityQueryHandler(
    IApplicationDbContext context,
    ITeamService teamService) 
    : IQueryHandler<GetTeamCompatibilityQuery, TeamCompatibilityResponse>
{
    public async Task<Result<TeamCompatibilityResponse>> Handle(
        GetTeamCompatibilityQuery query,
        CancellationToken cancellationToken)
    {
        Team? team = await context.Teams
            .FirstOrDefaultAsync(t => t.Id == query.TeamId, cancellationToken);
            
        if (team is null)
        {
            return Result.Failure<TeamCompatibilityResponse>(TeamErrors.NotFound(query.TeamId));
        }

        EmployeeProfile? member = await context.EmployeeProfiles
            .Include(ep => ep.User)
            .Include(ep => ep.Technologies)
            .ThenInclude(et => et.Technology)
            .FirstOrDefaultAsync(ep => ep.Id == query.MemberId, cancellationToken);
            
        if (member is null)
        {
            return Result.Failure<TeamCompatibilityResponse>(TeamErrors.InvalidMember(query.MemberId));
        }

        var memberData = new TeamMemberData(
            member.Id,
            $"{member.FirstName} {member.LastName}",
            member.User.Role.ToString(),
            member.Technologies.Select(t => t.Technology.Name).ToList(),
            member.SfiaLevelGeneral ?? 0,
            member.Mbti,
            JsonSerializer.Deserialize<List<string>>(member.PersonalInterests) ?? new());

        return await teamService.CalculateCompatibility(
            new TeamCompatibilityRequest(
                JsonSerializer.Deserialize<List<Guid>>(team.Members) ?? new(),
                memberData));
    }
}
