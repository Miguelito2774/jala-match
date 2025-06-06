using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public class PersonalInterest : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required string Name { get; set; }
    public int? SessionDurationMinutes { get; set; }
    public string? Frequency { get; set; }
    public int? InterestLevel { get; set; }

    public EmployeeProfile EmployeeProfile { get; set; }
}
