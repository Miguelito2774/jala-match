using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using SharedKernel.Results;

namespace Application.Commands.Teams.AddTeamMember;

public class AddTeamMemberCommandHandler : ICommandHandler<AddTeamMemberCommand, TeamResponse>
{
    private readonly ITeamService _teamService;

    public AddTeamMemberCommandHandler(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<Result<TeamResponse>> Handle(
        AddTeamMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = new TeamMemberUpdateRequest
        {
            TeamId = command.TeamId,
            Members = command.Members,
        };

        return await _teamService.AddTeamMember(request, cancellationToken);
    }
}
