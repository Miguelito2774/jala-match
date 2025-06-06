using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.Languages.UpdateEmployeeLanguage;

public sealed record UpdateEmployeeLanguageCommand(
    Guid LanguageId,
    string Language,
    string Proficiency
) : ICommand;
