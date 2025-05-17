using Domain.Entities.Technologies;
using SharedKernel.Domain;

namespace Domain.Entities.Areas_Roles;

public class SpecializedRoleSkill : Entity
{
    public required Guid SpecializedRoleId { get; set; }
    public required  Guid TechnologyId { get; set; }
    public required int MinimumLevel { get; set; }
    
    public required SpecializedRole SpecializedRole { get; set; }
    public required Technology Technology { get; set; }
}
