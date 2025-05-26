using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using SharedKernel.Results;

namespace Application.Commands.Teams.RemoveTeamMember;

internal sealed class RemoveTeamMemberCommandHandler
    : ICommandHandler<RemoveTeamMemberCommand, TeamResponse>
{
    private readonly ITeamService _teamService;

    public RemoveTeamMemberCommandHandler(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<Result<TeamResponse>> Handle(
        RemoveTeamMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = new RemoveTeamMemberRequest
        {
            TeamId = command.TeamId,
            EmployeeProfileId = command.EmployeeProfileId,
        };

        return await _teamService.RemoveTeamMember(request, cancellationToken);
    }
}
