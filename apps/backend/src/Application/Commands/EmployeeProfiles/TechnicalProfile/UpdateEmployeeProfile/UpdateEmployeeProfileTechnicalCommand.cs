using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.UpdateEmployeeProfile;

public sealed record UpdateEmployeeProfileTechnicalCommand(
    Guid UserId,
    int SfiaLevelGeneral,
    string Mbti
) : ICommand;
