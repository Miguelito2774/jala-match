using Domain.Entities.Privacy.Enums;
using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Privacy;

public sealed class DataDeletionOrder : Entity
{
    public required Guid UserId { get; set; }
    public required DataDeletionStatus Status { get; set; }
    public required DateTime RequestDate { get; set; }
    public DateTime? ScheduledDeletionDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public required List<string> DataTypes { get; set; } = new();
    public string? CancellationReason { get; set; }
    public string? Reason { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
