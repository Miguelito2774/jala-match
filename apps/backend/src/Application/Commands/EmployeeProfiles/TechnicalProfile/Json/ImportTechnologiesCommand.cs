using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.Json;

public sealed record ImportTechnologiesCommand(Guid UserId, List<TechnologyImportDto> Technologies)
    : ICommand<List<Guid>>;
