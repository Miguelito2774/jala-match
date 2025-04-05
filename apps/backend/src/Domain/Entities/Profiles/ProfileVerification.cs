using Domain.Entities.Enums;
using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public sealed class ProfileVerification : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required EmployeeProfile EmployeeProfile { get; set; } = null!;
    public required Guid ReviewerId { get; set; }
    public required User Reviewer { get; set; } = null!;
    public required int? SfiaProposed { get; set; }
    public required VerificationStatus Status { get; set; }
    public required string? Notes { get; set; }
    public required DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
}
