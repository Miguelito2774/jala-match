using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.GetAvailableTeamsForMember;

public record GetAvailableTeamsForMemberQuery(
    Guid EmployeeProfileId,
    Guid CreatorId,
    Guid? ExcludeTeamId = null
) : IQuery<List<AvailableTeamDto>>;
