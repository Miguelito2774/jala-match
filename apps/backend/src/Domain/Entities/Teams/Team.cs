using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class Team : Entity
{
    public required string Name { get; set; }
    public required Guid CreatorId { get; set; }
    public string? Description { get; set; }
    public double CompatibilityScore { get; set; }
    public bool IsActive { get; set; } = true;
    public string? AiAnalysis { get; set; }
    public string? WeightCriteria { get; set; }

    public int TeamSize { get; set; }
    public string? RequiredRoles { get; set; }
    public int MinimumSfiaLevel { get; set; }

    // Weight properties
    public int SfiaWeight { get; set; }
    public int TechnicalWeight { get; set; }
    public int PsychologicalWeight { get; set; }
    public int ExperienceWeight { get; set; }
    public int LanguageWeight { get; set; }
    public int InterestsWeight { get; set; }
    public int TimezoneWeight { get; set; }

    public string? MembersJson { get; set; }
    public string? RequiredTechnologiesJson { get; set; }

    // Navigation
    public required User Creator { get; set; }
    public List<TeamRequiredTechnology> RequiredTechnologies { get; set; } = new();
    public List<TeamMember> Members { get; set; } = new();
}
