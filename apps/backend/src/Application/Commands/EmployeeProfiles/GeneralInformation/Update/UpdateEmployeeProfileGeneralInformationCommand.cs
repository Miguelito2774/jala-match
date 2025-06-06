using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.GeneralInformation.Update;

public sealed record UpdateEmployeeProfileGeneralInfoCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Country,
    string Timezone
) : ICommand;
