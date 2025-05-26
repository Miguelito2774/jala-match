using Domain.Entities.Teams;
using SharedKernel.Domain;

namespace Domain.Entities.Technologies;

public sealed class Technology : Entity
{
    public required string Name { get; set; }
    public Guid CategoryId { get; set; }
    public string? Version { get; set; }
    public string? Description { get; set; }

    public TechnologyCategory Category { get; set; }
    public List<EmployeeTechnology> EmployeeTechnologies { get; set; } = new();
    public List<TeamRequiredTechnology> TeamRequiredTechnologies { get; set; } = new();
}
