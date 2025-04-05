using Application.Abstractions.Messaging;

namespace Application.Queries.Teams.GetTeamCompatibility;

public sealed record GetTeamCompatibilityQuery(
    Guid TeamId,
    Guid MemberId) : IQuery<TeamCompatibilityResponse>;
