using Domain.Entities.Profiles;
using SharedKernel.Domain;

namespace Domain.Entities.Technologies;

public sealed class EmployeeTechnology : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required EmployeeProfile EmployeeProfile { get; set; } = null!;
    public required Guid TechnologyId { get; set; }
    public required Technology Technology { get; set; } = null!;
    public required int SfiaLevel { get; set; }
    public required decimal YearsExperience { get; set; }
}
