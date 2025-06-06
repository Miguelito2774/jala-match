using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.EmployeeProfiles.Verifications;

public sealed record RejectProfileVerificationCommand(
    Guid ReviewerId,
    Guid EmployeeProfileId,
    string Notes
) : ICommand<VerificationDecisionResponse>;
