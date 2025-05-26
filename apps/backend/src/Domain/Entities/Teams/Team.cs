using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class Team : Entity
{
    public string Name { get; set; }
    public Guid CreatorId { get; set; }
    public double CompatibilityScore { get; set; }
    public bool IsActive { get; set; } = true;
    public string? AiAnalysis { get; set; }
    public string? WeightCriteria { get; set; }

    public int TeamSize { get; set; }

    // Navigation
    public User Creator { get; set; }
    public List<TeamRequiredTechnology> RequiredTechnologies { get; set; } = new();
    public List<TeamMember> Members { get; set; } = new();
}
