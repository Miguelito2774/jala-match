using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.GenerateTeams;

public sealed record GenerateTeamsCommand(
    List<string> Roles,
    List<string> Technologies,
    int SfiaLevel,
    bool Availability,
    Dictionary<string, int> CriteriaWeights
) : ICommand<TeamCompositionResponse>;
