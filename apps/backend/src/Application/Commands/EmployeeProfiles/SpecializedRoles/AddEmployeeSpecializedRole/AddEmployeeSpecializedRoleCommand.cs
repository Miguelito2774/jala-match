using Application.Abstractions.Messaging;
using Domain.Entities.Enums;

namespace Application.Commands.EmployeeProfiles.SpecializedRoles.AddEmployeeSpecializedRole;

public sealed record AddEmployeeSpecializedRoleCommand(
    Guid UserId,
    Guid SpecializedRoleId,
    ExperienceLevel Level,
    int YearsExperience
) : ICommand<Guid>;
