using System.Text.Json;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.DTOs;
using Domain.Entities.Teams;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetById;

internal sealed class GetTeamByIdQueryHandler : IQueryHandler<GetTeamByIdQuery, TeamResponse>
{
    private readonly ITeamRepository _teamRepository;

    public GetTeamByIdQueryHandler(ITeamRepository teamRepository) =>
        _teamRepository = teamRepository;

    public async Task<Result<TeamResponse>> Handle(
        GetTeamByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        Team? team = await _teamRepository.GetByIdAsync(query.TeamId, cancellationToken);
        if (team is null)
        {
            return Result.Failure<TeamResponse>(
                new Error("Team.NotFound", "Equipo no encontrado", ErrorType.Failure)
            );
        }

        var response = new TeamResponse
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
        };

        return Result.Success(response);
    }
}
