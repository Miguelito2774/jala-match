using SharedKernel.Domain;

namespace Domain.Entities.Profiles;

public class WorkExperience : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required string ProjectName { get; set; }
    public string? Description { get; set; }

    public List<string> Tools { get; set; } = new();
    public List<string> ThirdParties { get; set; } = new();
    public List<string> Frameworks { get; set; } = new();
    public string? VersionControl { get; set; }
    public string? ProjectManagement { get; set; }

    public required List<string> Responsibilities { get; set; } = new();

    public required DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public required EmployeeProfile EmployeeProfile { get; set; }
}
