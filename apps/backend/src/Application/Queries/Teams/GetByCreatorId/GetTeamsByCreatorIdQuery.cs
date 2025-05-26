using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.GetByCreatorId;

public sealed record GetTeamsByCreatorIdQuery(Guid CreatorId) : IQuery<List<TeamResponse>>;
