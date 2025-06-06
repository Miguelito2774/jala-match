using Domain.Entities.Enums;
using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public class ProfileVerification : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public Guid? ReviewerId { get; set; }
    public int? SfiaProposed { get; set; }
    public required VerificationStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public required EmployeeProfile EmployeeProfile { get; set; }
    public User? Reviewer { get; set; }
}
