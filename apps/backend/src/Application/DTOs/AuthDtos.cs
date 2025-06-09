using Domain.Entities.Enums;

namespace Application.DTOs;

public sealed record AuthResponse
{
    public required string Token { get; init; }
    public required UserDto User { get; init; }
    public required DateTime ExpiresAt { get; init; }
}

public sealed record UserDto
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required Role Role { get; init; }
    public required bool HasProfile { get; init; }
    public required bool IsProfileVerified { get; init; }
}

public sealed record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public sealed record RegisterEmployeeRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public sealed record RegisterManagerRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public sealed record CreateInvitationRequest
{
    public required string Email { get; init; }
    public required Role TargetRole { get; init; }
}

public sealed record ForgotPasswordRequest
{
    public required string Email { get; init; }
}

public sealed record ResetPasswordRequest
{
    public required string Token { get; init; }
    public required string NewPassword { get; init; }
}
