using Application.Abstractions.Messaging;

namespace Application.Queries.Technologies.GetAll;

public sealed record GetAllTechnologiesQuery : IQuery<List<TechnologyResponse>>;
