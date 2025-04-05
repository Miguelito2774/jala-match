using SharedKernel.Domain;

namespace Domain.Entities.Technologies;

public sealed  class TechnologyCategory : Entity
{
    public required string Name { get; set; } = null!;
    public required ICollection<Technology> Technologies { get; set; } = new List<Technology>();
}
