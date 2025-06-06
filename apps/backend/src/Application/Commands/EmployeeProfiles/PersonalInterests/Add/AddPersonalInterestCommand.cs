using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Add;

public sealed record AddPersonalInterestCommand(
    Guid UserId,
    string Name,
    int? SessionDurationMinutes,
    string? Frequency,
    int? InterestLevel
) : ICommand<Guid>;
