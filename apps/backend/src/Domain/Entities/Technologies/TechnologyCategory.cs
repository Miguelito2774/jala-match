using SharedKernel.Domain;

namespace Domain.Entities.Technologies;

public sealed class TechnologyCategory : Entity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<Technology> Technologies { get; set; } = new();
}
