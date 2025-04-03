using Domain.Entities.Enums;
using SharedKernel.Domain;

namespace Domain.Entities.Users;

public sealed class User : Entity
{
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required Role Role { get; init; }
    public Uri? ProfilePictureUrl { get; init; }
    public bool IsActive { get; init; } = true;
}
