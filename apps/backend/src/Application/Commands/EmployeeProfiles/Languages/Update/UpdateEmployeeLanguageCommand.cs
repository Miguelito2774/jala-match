using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.Languages.Update;

public sealed record UpdateEmployeeLanguageCommand(
    Guid EmployeeLanguageId,
    Guid UserId,
    string Language,
    string Proficiency
) : ICommand;
