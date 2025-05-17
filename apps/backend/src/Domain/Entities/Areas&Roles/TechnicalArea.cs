using SharedKernel.Domain;

namespace Domain.Entities.Areas_Roles;

public class TechnicalArea : Entity
{
    public required string Name { get; set; }
    public required List<SpecializedRole> Roles { get; set; } = new();
}
