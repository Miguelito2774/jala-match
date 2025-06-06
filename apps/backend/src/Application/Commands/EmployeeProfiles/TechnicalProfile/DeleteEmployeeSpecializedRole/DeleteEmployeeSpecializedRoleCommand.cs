using Application.Abstractions.Messaging;
using System;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.DeleteEmployeeSpecializedRole;

public sealed record DeleteEmployeeSpecializedRoleCommand(
    Guid EmployeeSpecializedRoleId,
    Guid UserId
) : ICommand;
