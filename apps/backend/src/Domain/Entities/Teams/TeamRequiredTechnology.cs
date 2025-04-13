using Domain.Entities.Technologies;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class TeamRequiredTechnology : Entity
{
    public required Guid TeamId { get; set; }
    public required Guid TechnologyId { get; set; }
    public int MinimumSfiaLevel { get; set; } = 3;
    public bool IsMandatory { get; set; } = true;

    public required Team Team { get; set; }
    public required Technology Technology { get; set; }
}
