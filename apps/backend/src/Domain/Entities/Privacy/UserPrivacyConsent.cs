using SharedKernel.Domain;

namespace Domain.Entities.Privacy;

public sealed class UserPrivacyConsent : Entity
{
    public required Guid UserId { get; set; }
    public required bool TeamMatchingAnalysis { get; set; }
    public required DateTime LastUpdated { get; set; }
    public required string Version { get; set; }
    public required DateTime CreatedAt { get; set; }
    
    // Navigation property
    public Users.User User { get; set; } = null!;
}
