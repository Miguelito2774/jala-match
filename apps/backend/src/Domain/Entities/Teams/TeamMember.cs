using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using SharedKernel.Domain;

namespace Domain.Entities.Teams;

public class TeamMember : Entity
{
    public required Guid TeamId { get; set; }
    public required Guid EmployeeProfileId { get; set; }
    public required TeamMemberRole Role { get; set; }
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    public required Team Team { get; set; }
    public required EmployeeProfile EmployeeProfile { get; set; }
}
