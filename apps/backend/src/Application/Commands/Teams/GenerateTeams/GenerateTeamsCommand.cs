using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.GenerateTeams;

public sealed record GenerateTeamsCommand(
    Guid CreatorId,
    int TeamSize,
    List<TeamRequirements> Requirements,
    int SfiaLevel,
    List<string> Technologies,
    WeightCriteria Weights,
    bool Availability = true
) : ICommand<AiServiceResponse>;
