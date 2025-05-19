using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class TeamMember : Entity
{
    public Guid TeamId { get; set; }
    public Guid EmployeeProfileId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public int SfiaLevel { get; set; }
    public bool IsLeader { get; set; }
    public Team? Team { get; set; }
    public EmployeeProfile? EmployeeProfile { get; set; }
}
