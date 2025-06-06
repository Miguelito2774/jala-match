using Application.Abstractions.Messaging;
using Domain.Entities.Enums;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.UpdateEmployeeTechnology;

public sealed record UpdateEmployeeTechnologyCommand(
    Guid EmployeeTechnologyId,
    Guid UserId,
    int SfiaLevel,
    decimal YearsExperience,
    string Version
) : ICommand;
