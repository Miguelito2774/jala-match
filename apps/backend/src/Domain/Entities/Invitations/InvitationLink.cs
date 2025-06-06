using Domain.Entities.Enums;
using Domain.Entities.Users;
using SharedKernel.Domain;

namespace Domain.Entities.Invitations;

public sealed class InvitationLink : Entity
{
    public required string Token { get; set; }
    public required Guid CreatedById { get; set; }
    public required Role TargetRole { get; set; }
    public required string Email { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }

    public required User? CreatedBy { get; set; }
}
