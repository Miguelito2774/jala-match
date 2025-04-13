using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.GenerateTeams;

public sealed record GenerateTeamsCommand(
    Guid CreatorId,
    int TeamSize,
    List<TeamRoleRequest> Roles,
    List<string> Technologies,
    int SfiaLevel,
    WeightCriteria Weights,
    bool Availability = true
) : ICommand<AiServiceResponse>;
