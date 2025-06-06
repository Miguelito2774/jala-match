using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Update;

public sealed record UpdatePersonalInterestCommand(
    Guid PersonalInterestId,
    Guid UserId,
    string Name,
    int? SessionDurationMinutes,
    string? Frequency,
    int? InterestLevel
) : ICommand;
