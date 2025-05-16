using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Teams.GenerateTeams;

internal sealed class GenerateTeamsCommandHandler
    : ICommandHandler<GenerateTeamsCommand, AiServiceResponse>
{
    private readonly ITeamService _teamService;

    public GenerateTeamsCommandHandler(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<Result<AiServiceResponse>> Handle(
        GenerateTeamsCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Result<AiServiceResponse> response = await _teamService.GenerateTeams(
                command.CreatorId,
                command.Roles,
                command.Technologies,
                command.SfiaLevel,
                command.TeamSize,
                command.Weights,
                cancellationToken,
                command.Availability
            );

            if (response.IsFailure)
            {
                return Result.Failure<AiServiceResponse>(response.Error);
            }

            return response;
        }
        catch (Exception ex)
        {
            return Result.Failure<AiServiceResponse>(
                new Error("Teams.Generation.Failed", ex.Message, ErrorType.Failure)
            );
        }
    }
}
