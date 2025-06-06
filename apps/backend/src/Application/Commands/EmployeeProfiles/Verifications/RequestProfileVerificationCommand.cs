using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.Verifications;

public sealed record RequestProfileVerificationCommand(Guid UserId) : ICommand;
