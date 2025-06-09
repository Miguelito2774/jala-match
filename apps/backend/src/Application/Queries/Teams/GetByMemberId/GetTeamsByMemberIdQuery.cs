using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.GetByMemberId;

public record GetTeamsByMemberIdQuery(Guid EmployeeProfileId) : IQuery<List<EmployeeTeamResponse>>;
