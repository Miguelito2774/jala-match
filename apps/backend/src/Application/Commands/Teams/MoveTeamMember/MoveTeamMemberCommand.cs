using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.MoveTeamMember;

public record MoveTeamMemberCommand(Guid SourceTeamId, Guid TargetTeamId, Guid EmployeeProfileId)
    : ICommand<(TeamResponse SourceTeam, TeamResponse TargetTeam)>;
