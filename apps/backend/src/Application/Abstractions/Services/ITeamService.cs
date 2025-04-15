using Application.DTOs;
using SharedKernel.Results;

namespace Application.Abstractions.Services;

public interface ITeamService
{
    Task<Result<AiServiceResponse>> GenerateTeams(
        List<TeamRoleRequest> roles,
        List<string> technologies,
        int sfiaLevel,
        int teamSize,
        List<TeamMemberGenerated> membersData,
        WeightCriteria weights,
        CancellationToken cancellationToken
    );

    Task<Result<TeamCompatibilityResponse>> CalculateCompatibility(
        List<TeamMemberGenerated> teamMembers,
        TeamMemberGenerated newMember,
        List<string> requiredTechnologies,
        CancellationToken cancellationToken
    );

    Task<Result<AiServiceResponse>> ReanalyzeTeam(
        Guid teamId,
        List<TeamRoleRequest> roles,
        List<string> technologies,
        int sfiaLevel,
        int teamSize,
        List<TeamMemberGenerated> membersData,
        Guid leaderId,
        WeightCriteria weights,
        CancellationToken cancellationToken
    );
}
