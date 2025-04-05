using Application.Abstractions.Messaging;

namespace Application.Queries.Teams.GetById;

public sealed record GetTeamByIdQuery(Guid TeamId) : IQuery<TeamResponse>;
