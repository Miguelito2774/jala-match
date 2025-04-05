using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public sealed  class Team : Entity
{
    public required string Name { get; set; } = null!;
    public required Guid CreatorId { get; set; }
    public required User Creator { get; set; } = null!;
    public required string RequiredTechnologies { get; set; } = "[]";
    public required string Members { get; set; } = "[]";
    public required string AiAnalysis { get; set; } = "{}";
    public required int? CompatibilityScore { get; set; }
    public required bool IsActive { get; set; } = true;
}
