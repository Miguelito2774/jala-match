using Application.Abstractions.Messaging;
using Domain.Entities.Enums;

namespace Application.Commands.EmployeeProfiles.SpecializedRoles.UpdateEmployeeSpecializedRole;

public sealed record UpdateEmployeeSpecializedRoleCommand(
    Guid RoleId,
    ExperienceLevel Level,
    int YearsExperience
) : ICommand;
