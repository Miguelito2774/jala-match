using Domain.Entities.Enums;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;
using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public class EmployeeProfile : Entity
{
    public required Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required bool Availability { get; set; }
    public required string Country { get; set; }
    public required string Timezone { get; set; }

    public required int SfiaLevelGeneral { get; set; }
    public required string Specialization { get; set; }
    public required string Mbti { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public string? VerificationNotes { get; set; }

    public required User User { get; set; }
    public List<WorkExperience> WorkExperiences { get; set; } = new();
    public List<PersonalInterest> PersonalInterests { get; set; } = new();
    public List<EmployeeTechnology> Technologies { get; set; } = new();
    public List<EmployeeLanguage> Languages { get; set; } = new();
    public List<TeamMember> TeamMemberships { get; set; } = new();
    public List<ProfileVerification> Verifications { get; set; } = new();
}
