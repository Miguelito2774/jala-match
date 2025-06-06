using Application.Abstractions.Messaging;
using Domain.Entities.Enums;
using System;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.AddEmployeeSpecializedRole;

public sealed record AddEmployeeSpecializedRoleCommand(
    Guid UserId,
    Guid SpecializedRoleId,
    ExperienceLevel Level,
    int YearsExperience
) : ICommand<Guid>;
