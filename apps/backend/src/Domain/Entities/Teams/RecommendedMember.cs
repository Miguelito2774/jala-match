using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class RecommendedMember : Entity
{
    public Guid UserId { get; set; }
    public required string Name { get; set; }
    public int CompatibilityScore { get; set; }
    public required string Analysis { get; set; }
}
