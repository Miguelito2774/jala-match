using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.GetAll;

public record GetAllTeamsQuery : IQuery<List<TeamResponse>>;
