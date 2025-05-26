using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.FindTeamMembers;

public record FindTeamMembersQuery(
    Guid TeamId,
    string Role,
    string Area,
    string Level,
    List<string> Technologies
) : IQuery<List<TeamMemberRecommendation>>;
