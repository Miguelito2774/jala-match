using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetTeamCompatibility;

internal sealed class GetTeamCompatibilityQueryHandler(
    IApplicationDbContext context,
    ITeamService teamService
) : IQueryHandler<GetTeamCompatibilityQuery, Application.DTOs.TeamCompatibilityResponse>
{
    public async Task<Result<DTOs.TeamCompatibilityResponse>> Handle(
        GetTeamCompatibilityQuery query,
        CancellationToken cancellationToken
    )
    {
        Team? team = await context
            .Teams.Include(t => t.Members)
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
            return Result.Failure<DTOs.TeamCompatibilityResponse>(
                TeamErrors.NotFound(query.TeamId)
            );
        }

        EmployeeProfile? newMember = await context
            .EmployeeProfiles.Include(ep => ep.User)
            .Include(ep => ep.Technologies)
            .ThenInclude(et => et.Technology)
            .Include(ep => ep.PersonalInterests)
            .FirstOrDefaultAsync(ep => ep.Id == query.MemberId, cancellationToken);

        if (newMember is null)
        {
            return Result.Failure<DTOs.TeamCompatibilityResponse>(
                TeamErrors.InvalidMember(query.MemberId)
            );
        }

        var teamMemberData = team
            .Members.Select(m => new TeamMemberGenerated(
                m.EmployeeProfile.Id,
                $"{m.EmployeeProfile.FirstName} {m.EmployeeProfile.LastName}",
                m.EmployeeProfile.Specialization ?? string.Empty,
                m.EmployeeProfile.Technologies.Select(t => t.Technology.Name).ToList(),
                m.EmployeeProfile.SfiaLevelGeneral,
                m.EmployeeProfile.Mbti,
                m.EmployeeProfile.PersonalInterests.Select(pi => pi.Name).ToList(),
                m.EmployeeProfile.Timezone,
                m.EmployeeProfile.Country
            ))
            .ToList();

        if (!teamMemberData.Any() && !string.IsNullOrEmpty(team.MembersJson))
        {
            List<Guid> memberIds =
                JsonSerializer.Deserialize<List<Guid>>(team.MembersJson) ?? new();

            List<EmployeeProfile> profiles = await context
                .EmployeeProfiles.Include(ep => ep.Technologies)
                .ThenInclude(t => t.Technology)
                .Include(ep => ep.PersonalInterests)
                .Where(ep => memberIds.Contains(ep.Id))
                .ToListAsync(cancellationToken);

            teamMemberData = profiles
                .Select(p => new TeamMemberGenerated(
                    p.Id,
                    $"{p.FirstName} {p.LastName}",
                    p.Specialization,
                    p.Technologies.Select(t => t.Technology.Name).ToList(),
                    p.SfiaLevelGeneral,
                    p.Mbti,
                    p.PersonalInterests.Select(pi => pi.Name).ToList(),
                    p.Timezone,
                    p.Country
                ))
                .ToList();
        }

        var newMemberData = new TeamMemberGenerated(
            newMember.Id,
            $"{newMember.FirstName} {newMember.LastName}",
            newMember.Specialization ?? string.Empty,
            newMember.Technologies.Select(t => t.Technology.Name).ToList(),
            newMember.SfiaLevelGeneral,
            newMember.Mbti,
            newMember.PersonalInterests.Select(pi => pi.Name).ToList(),
            newMember.Timezone,
            newMember.Country
        );

        var requiredTechnologies = team
            .RequiredTechnologies.Select(rt => rt.Technology.Name)
            .ToList();

        if (!requiredTechnologies.Any() && !string.IsNullOrEmpty(team.RequiredTechnologiesJson))
        {
            requiredTechnologies = JsonSerializer.Deserialize<List<string>>(
                team.RequiredTechnologiesJson
            );
        }

        if (requiredTechnologies != null)
        {
            return await teamService.CalculateCompatibility(
                teamMemberData,
                newMemberData,
                requiredTechnologies,
                cancellationToken
            );
        }

        return Result.Failure<DTOs.TeamCompatibilityResponse>(
            new Error(
                "MissingTechnologies",
                "Required technologies are missing.",
                ErrorType.Failure
            )
        );
    }
}
