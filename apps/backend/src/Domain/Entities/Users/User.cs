using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using SharedKernel.Domain;

namespace Domain.Entities.Users;

public sealed class User : Entity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required Role Role { get; set; }
    public required Uri ProfilePictureUrl { get; set; }

    // Navigation properties
    public EmployeeProfile? EmployeeProfile { get; set; }
    public List<Team> CreatedTeams { get; set; } = new();
    public List<ProfileVerification> Reviews { get; set; } = new();
}
