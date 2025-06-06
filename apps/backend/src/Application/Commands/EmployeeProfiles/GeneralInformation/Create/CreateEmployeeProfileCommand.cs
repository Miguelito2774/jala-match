using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.GeneralInformation.Create;

public sealed record CreateEmployeeProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Country,
    string Timezone,
    int SfiaLevelGeneral,
    string Mbti
) : ICommand<Guid>;
