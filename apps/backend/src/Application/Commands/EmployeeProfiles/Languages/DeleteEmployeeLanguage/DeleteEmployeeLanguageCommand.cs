using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.Languages.DeleteEmployeeLanguage;

public sealed record DeleteEmployeeLanguageCommand(Guid LanguageId) : ICommand;
