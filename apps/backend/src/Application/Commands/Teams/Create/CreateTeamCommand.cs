using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Teams;

namespace Application.Commands.Teams.Create;

public record CreateTeamCommand(
    string Name,
    Guid CreatorId,
    List<TeamMemberDto> Members,
    Guid LeaderId,
    AiTeamAnalysis Analysis,
    int CompatibilityScore,
    WeightCriteria Weights,
    List<string> RequiredTechnologies
) : ICommand<TeamResponse>;
