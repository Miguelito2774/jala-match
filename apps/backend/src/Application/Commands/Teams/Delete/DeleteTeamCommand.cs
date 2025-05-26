using Application.Abstractions.Messaging;

namespace Application.Commands.Teams.Delete;

public record DeleteTeamCommand(Guid TeamId) : ICommand;
