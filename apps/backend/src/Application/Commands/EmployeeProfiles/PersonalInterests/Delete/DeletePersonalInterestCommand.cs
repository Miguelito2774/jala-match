using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Delete;

public sealed record DeletePersonalInterestCommand(Guid PersonalInterestId, Guid UserId) : ICommand;
