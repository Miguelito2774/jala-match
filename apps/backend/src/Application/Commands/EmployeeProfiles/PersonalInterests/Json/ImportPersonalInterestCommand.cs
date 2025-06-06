using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Json;

public sealed record ImportPersonalInterestsCommand(
    Guid UserId,
    List<PersonalInterestImportDto> PersonalInterests
) : ICommand<List<Guid>>;
