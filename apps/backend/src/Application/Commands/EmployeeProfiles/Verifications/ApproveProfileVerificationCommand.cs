using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.EmployeeProfiles.Verifications;

public sealed record ApproveProfileVerificationCommand(
    Guid ReviewerId,
    Guid EmployeeProfileId,
    int? SfiaProposed,
    string? Notes
) : ICommand<VerificationDecisionResponse>;
