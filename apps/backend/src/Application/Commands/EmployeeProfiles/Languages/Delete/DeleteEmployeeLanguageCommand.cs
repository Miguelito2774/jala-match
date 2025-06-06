using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.Languages.Delete;

public sealed record DeleteEmployeeLanguageCommand(Guid EmployeeLanguageId, Guid UserId) : ICommand;
