using Application.Abstractions.Messaging;
using Domain.Entities.Enums;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.AddEmployeeTechnology;

public sealed record AddEmployeeTechnologyCommand(
    Guid UserId,
    Guid TechnologyId,
    int SfiaLevel,
    decimal YearsExperience,
    string Version
) : ICommand<Guid>;
