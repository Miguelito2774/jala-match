using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using SharedKernel.Domain;

namespace Domain.Entities.Technologies;

public class EmployeeTechnology : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required Guid TechnologyId { get; set; }
    public required int SfiaLevel { get; set; } // 1-7
    public required decimal YearsExperience { get; set; }
    public required string Version { get; set; }
    public required ExperienceLevel ExperienceLevel { get; set; }

    // Navigation
    public required EmployeeProfile EmployeeProfile { get; set; }
    public required Technology Technology { get; set; }
}
