using Application.DTOs;
using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface ITeamService
{
    Task<Result<AiServiceResponse>> GenerateTeams(
        Guid creatorId,
        List<TeamRoleRequest> roles,
        List<string> technologies,
        int sfiaLevel,
        int teamSize,
        WeightCriteria weights,
        CancellationToken cancellationToken,
        bool availability = true
    );
}
