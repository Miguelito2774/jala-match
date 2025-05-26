using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using SharedKernel.Results;

namespace Application.Queries.Teams.FindTeamMembers;

public class FindTeamMembersQueryHandler
    : IQueryHandler<FindTeamMembersQuery, List<TeamMemberRecommendation>>
{
    private readonly ITeamService _teamService;

    public FindTeamMembersQueryHandler(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<Result<List<TeamMemberRecommendation>>> Handle(
        FindTeamMembersQuery query,
        CancellationToken cancellationToken
    )
    {
        var request = new FindTeamMemberRequest
        {
            TeamId = query.TeamId,
            Role = query.Role,
            Area = query.Area,
            Level = query.Level,
            Technologies = query.Technologies,
        };

        return await _teamService.FindTeamMembers(request, cancellationToken);
    }
}
