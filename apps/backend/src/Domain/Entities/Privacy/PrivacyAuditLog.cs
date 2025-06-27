using Domain.Entities.Privacy.Enums;
using SharedKernel.Domain;

namespace Domain.Entities.Privacy;

public sealed class PrivacyAuditLog : Entity
{
    public required Guid UserId { get; set; }
    public required PrivacyAction Action { get; set; }
    public required string Details { get; set; }
    public required DateTime Timestamp { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    // Navigation property
    public Users.User User { get; set; } = null!;
}
