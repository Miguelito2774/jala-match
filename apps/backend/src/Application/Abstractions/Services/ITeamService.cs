using Application.DTOs;
using Domain.Entities.Technologies;
using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface ITeamService
{
    Task<Result<AiServiceResponse>> GenerateTeams(Guid creatorId,
        List<TeamRequirements> requirements,
        int sfiaLevel,
        int teamSize,
        List<string> technologies,
        WeightCriteria weights,
        CancellationToken cancellationToken,
        bool availability = true);
}
