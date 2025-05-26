using System.Text.Json;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.DTOs;
using Domain.Entities.Teams;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetByCreatorId;

internal sealed class GetTeamsByCreatorIdQueryHandler
    : IQueryHandler<GetTeamsByCreatorIdQuery, List<TeamResponse>>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamsByCreatorIdQueryHandler(ITeamRepository teamRepository) =>
        _teamRepository = teamRepository;

    public async Task<Result<List<TeamResponse>>> Handle(
        GetTeamsByCreatorIdQuery query,
        CancellationToken cancellationToken
    )
    {
        List<Team> teams = await _teamRepository.GetByCreatorIdAsync(
            query.CreatorId,
            cancellationToken
        );

        var response = teams
            .Select(team => new TeamResponse
            {
                TeamId = team.Id,
                Name = team.Name,
                CreatorId = team.CreatorId,
                CompatibilityScore = team.CompatibilityScore,
                Members = team
                    .Members.Select(m => new TeamMemberDto(
                        m.EmployeeProfileId,
                        m.Name,
                        m.Role,
                        m.SfiaLevel,
                        m.IsLeader
                    ))
                    .ToList(),
                RequiredTechnologies = team
                    .RequiredTechnologies.Select(rt => rt.Technology.Name)
                    .ToList(),
                Analysis =
                    team.AiAnalysis != null
                        ? JsonSerializer.Deserialize<AiTeamAnalysis>(team.AiAnalysis)
                        : null,
                Weights =
                    team.WeightCriteria != null
                        ? JsonSerializer.Deserialize<WeightCriteria>(team.WeightCriteria)
                        : null,
                IsActive = team.IsActive,
            })
            .ToList();

        return Result.Success(response);
    }
}
