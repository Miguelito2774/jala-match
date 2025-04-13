using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public class EmployeeLanguage : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required string Language { get; set; }
    public required string Proficiency { get; set; } // A1, A2, B1, B2, C1, C2, Native

    public required EmployeeProfile EmployeeProfile { get; set; }
}
