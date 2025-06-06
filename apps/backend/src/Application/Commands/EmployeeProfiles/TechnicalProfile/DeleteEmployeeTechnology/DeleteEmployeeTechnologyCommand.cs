using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.DeleteEmployeeTechnology;

public sealed record DeleteEmployeeTechnologyCommand(Guid EmployeeTechnologyId, Guid UserId)
    : ICommand;
