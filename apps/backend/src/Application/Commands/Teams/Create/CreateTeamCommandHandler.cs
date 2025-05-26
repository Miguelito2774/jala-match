using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using SharedKernel.Results;

namespace Application.Commands.Teams.Create;

internal sealed class CreateTeamCommandHandler : ICommandHandler<CreateTeamCommand, TeamResponse>
{
    private readonly ITeamService _teamService;

    public CreateTeamCommandHandler(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<Result<TeamResponse>> Handle(
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        return await _teamService.CreateTeam(command, cancellationToken);
    }
}
