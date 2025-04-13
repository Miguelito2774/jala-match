namespace Application.Queries.Technologies.GetAll;

public sealed record TechnologyResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid CategoryId { get; init; }
    public required string CategoryName { get; init; }
}
