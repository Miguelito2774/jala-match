using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public class EmployeeLanguage : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required string Language { get; set; }
    public required string Proficiency { get; set; }

    public EmployeeProfile EmployeeProfile { get; set; }
}
