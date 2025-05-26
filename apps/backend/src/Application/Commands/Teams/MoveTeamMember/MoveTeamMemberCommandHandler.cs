using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using SharedKernel.Results;

namespace Application.Commands.Teams.MoveTeamMember;

internal sealed class MoveTeamMemberCommandHandler
    : ICommandHandler<MoveTeamMemberCommand, (TeamResponse SourceTeam, TeamResponse TargetTeam)>
{
    private readonly ITeamService _teamService;

    public MoveTeamMemberCommandHandler(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<Result<(TeamResponse SourceTeam, TeamResponse TargetTeam)>> Handle(
        MoveTeamMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = new MoveTeamMemberRequest
        {
            SourceTeamId = command.SourceTeamId,
            TargetTeamId = command.TargetTeamId,
            EmployeeProfileId = command.EmployeeProfileId,
        };

        return await _teamService.MoveTeamMember(request, cancellationToken);
    }
}
