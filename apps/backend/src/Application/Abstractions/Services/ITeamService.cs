using Application.DTOs;
using Application.Queries.Teams.GetTeamCompatibility;
using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface ITeamService
{
    Task<Result<TeamCompositionResponse>> GenerateTeams(TeamGenerationRequest request);
    Task<Result<TeamCompatibilityResponse>> CalculateCompatibility(TeamCompatibilityRequest request);
}
