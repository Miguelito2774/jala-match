using SharedKernel.Errors;

namespace Domain.Entities.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) =>
        Error.NotFound("Users.NotFound", $"The user with ID {userId} was not found");
}
