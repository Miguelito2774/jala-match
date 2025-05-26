using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.AddTeamMember;

public record AddTeamMemberCommand(Guid TeamId, List<TeamMemberDto> Members)
    : ICommand<TeamResponse>;
