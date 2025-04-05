using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using SharedKernel.Domain;

namespace Domain.Entities.Users;

public sealed class User : Entity
{
    public required string Email { get; set; } = null!;
    public required string PasswordHash { get; set; } = null!;
    public required  Role Role { get; set; }
    public required Uri ProfilePictureUrl { get; set; }
    public required EmployeeProfile? EmployeeProfile { get; set; }
    public required ICollection<Team> CreatedTeams { get; set; } = new List<Team>();
    public required ICollection<ProfileVerification> Reviews { get; set; } = new List<ProfileVerification>();
}
