using Domain.Entities.Technologies;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class TeamRequiredTechnology : Entity
{
    public TeamRequiredTechnology(Guid teamId, Guid technologyId)
    {
        TeamId = teamId;
        TechnologyId = technologyId;
        MinimumSfiaLevel = 3;
        IsMandatory = true;
    }

    public Guid TeamId { get; private set; }
    public Guid TechnologyId { get; private set; }

    public int MinimumSfiaLevel { get; set; }
    public bool IsMandatory { get; set; }

    public Team Team { get; set; }
    public Technology Technology { get; set; }
}
