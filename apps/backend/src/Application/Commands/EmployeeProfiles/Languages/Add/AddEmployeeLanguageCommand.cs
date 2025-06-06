using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.Languages.Add;

public sealed record AddEmployeeLanguageCommand(Guid UserId, string Language, string Proficiency)
    : ICommand<Guid>;
