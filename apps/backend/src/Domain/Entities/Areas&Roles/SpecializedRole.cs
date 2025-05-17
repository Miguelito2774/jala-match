using SharedKernel.Domain;

namespace Domain.Entities.Areas_Roles;

public class SpecializedRole : Entity
{
    public required string Name { get; set; }
    public required Guid TechnicalAreaId { get; set; }
    public required TechnicalArea TechnicalArea { get; set; }
    public required List<SpecializedRoleSkill> RequiredSkills { get; set; } = new();
}
