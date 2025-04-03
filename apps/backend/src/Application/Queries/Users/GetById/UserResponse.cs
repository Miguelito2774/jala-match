namespace Application.Queries.Users.GetById;

public sealed record UserResponse
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required Uri? ProfilePictureUrl { get; init; }
    public required string Role { get; init; }
}
