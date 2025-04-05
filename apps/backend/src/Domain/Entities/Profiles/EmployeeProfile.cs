using Domain.Entities.Enums;
using Domain.Entities.Technologies;
using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public sealed class EmployeeProfile : Entity
{
    public required Guid UserId { get; set; }
    public required User User { get; set; } = null!;
    public required string FirstName { get; set; } = null!;
    public required string LastName { get; set; } = null!;
    public required bool Availability { get; set; } = true;
    public required string? Country { get; set; }
    public required string? Timezone { get; set; }
    public required int? SfiaLevelGeneral { get; set; }
    public required string? Specialization { get; set; }
    public required string? Mbti { get; set; }
    public required string WorkExperience { get; set; } = "[]";
    public required string PersonalInterests { get; set; } = "[]";
    public required VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public required string? VerificationNotes { get; set; }
    public required ICollection<EmployeeTechnology> Technologies { get; set; } = new List<EmployeeTechnology>();
    public required ICollection<ProfileVerification> Verifications { get; set; } = new List<ProfileVerification>();
}
