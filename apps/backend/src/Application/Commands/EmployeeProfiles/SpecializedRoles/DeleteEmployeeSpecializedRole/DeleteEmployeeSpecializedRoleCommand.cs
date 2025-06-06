using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.SpecializedRoles.DeleteEmployeeSpecializedRole;

public sealed record DeleteEmployeeSpecializedRoleCommand(Guid RoleId) : ICommand;
