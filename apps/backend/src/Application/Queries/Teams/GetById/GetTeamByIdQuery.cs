using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.GetById;

public sealed record GetTeamByIdQuery(Guid TeamId) : IQuery<TeamResponse>;
