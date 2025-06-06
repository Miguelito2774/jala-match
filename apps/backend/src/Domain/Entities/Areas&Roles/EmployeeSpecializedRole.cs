using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using SharedKernel.Domain;

namespace Domain.Entities.Areas_Roles;

public class EmployeeSpecializedRole : Entity
{
    public required Guid EmployeeProfileId { get; set; }
    public required Guid SpecializedRoleId { get; set; }
    public required ExperienceLevel Level { get; set; }
    public required int YearsExperience { get; set; }

    public EmployeeProfile EmployeeProfile { get; set; }
    public SpecializedRole SpecializedRole { get; set; }
}
