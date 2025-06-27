using Application.Abstractions.Messaging;
using SharedKernel.Results;

namespace Application.Commands.Privacy;

public sealed record ResetUserProfileCommand(
    Guid UserId,
    List<string> DataTypes,
    string? Reason = null
) : ICommand<bool>;
