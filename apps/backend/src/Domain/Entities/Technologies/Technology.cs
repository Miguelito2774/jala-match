using SharedKernel.Domain;

namespace Domain.Entities.Technologies;

public sealed class Technology : Entity
{
    public required string Name { get; set; } = null!;
    public required Guid CategoryId { get; set; }
    public required TechnologyCategory Category { get; set; } = null!;
    public required string? Version { get; set; }
    public required ICollection<EmployeeTechnology> EmployeeTechnologies { get; set; } = new List<EmployeeTechnology>();
}
