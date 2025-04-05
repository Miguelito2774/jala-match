using Application.Abstractions.Messaging;

namespace Application.Commands.Teams.Create;

public sealed record CreateTeamCommand(
    string Name,
    Guid CreatorId,
    List<string> RequiredTechnologies,
    List<Guid> MemberIds) : ICommand<Guid>;

