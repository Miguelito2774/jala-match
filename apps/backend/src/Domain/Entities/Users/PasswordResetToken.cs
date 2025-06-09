using SharedKernel.Domain;

namespace Domain.Entities.Users;

public sealed class PasswordResetToken : Entity
{
    public required string Token { get; set; }
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }

    public required User User { get; set; }
}
