using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.RemoveTeamMember;

public record RemoveTeamMemberCommand(Guid TeamId, Guid EmployeeProfileId) : ICommand<TeamResponse>;
