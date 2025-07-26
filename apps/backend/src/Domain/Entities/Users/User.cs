using Domain.Entities.Enums;
using Domain.Entities.Invitations;
using Domain.Entities.Privacy;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using SharedKernel.Domain;

namespace Domain.Entities.Users;

public sealed class User : Entity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required Role Role { get; set; }
    public required Uri? ProfilePictureUrl { get; set; }
    public string? ProfilePicturePublicId { get; set; }

    // Navigation properties
    public EmployeeProfile? EmployeeProfile { get; set; }
    public List<Team> CreatedTeams { get; set; } = new();
    public List<ProfileVerification> Reviews { get; set; } = new();
    public List<InvitationLink> CreatedInvitations { get; set; } = new();

    // Privacy-related navigation properties
    public UserPrivacyConsent? PrivacyConsent { get; set; }
    public List<DataDeletionOrder> DataDeletionRequests { get; set; } = new();
    public List<PrivacyAuditLog> PrivacyAuditLogs { get; set; } = new();
}
