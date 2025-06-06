using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.Languages.AddEmployeeLanguage;

public sealed record AddEmployeeLanguageCommand(
    Guid UserId,
    string Language,
    string Proficiency
) : ICommand<Guid>;
