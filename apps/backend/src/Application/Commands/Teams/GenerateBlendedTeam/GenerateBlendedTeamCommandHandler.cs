using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Teams.GenerateBlendedTeam;

internal sealed class GenerateBlendedTeamCommandHandler
    : ICommandHandler<GenerateBlendedTeamCommand, AiServiceResponse>
{
    private readonly ITeamService _teamService;

    public GenerateBlendedTeamCommandHandler(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public async Task<Result<AiServiceResponse>> Handle(
        GenerateBlendedTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Result<AiServiceResponse> response = await _teamService.GenerateBlendedTeam(
                command.CreatorId,
                command.TeamSize,
                command.Technologies,
                command.ProjectComplexity,
                command.SfiaLevel,
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
                new Error("Teams.BlendedGeneration.Failed", ex.Message, ErrorType.Failure)
            );
        }
    }
}
