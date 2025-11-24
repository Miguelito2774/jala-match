using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.GenerateBlendedTeam;

public sealed record GenerateBlendedTeamCommand(
    Guid CreatorId,
    int TeamSize,
    List<string> Technologies,
    string ProjectComplexity,
    int SfiaLevel,
    WeightCriteria Weights,
    bool Availability = true
) : ICommand<AiServiceResponse>;
