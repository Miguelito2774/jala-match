using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.SpecializedRoles.GetById;

public sealed record GetEmployeeSpecializedRoleByIdQuery(Guid RoleId)
    : IQuery<EmployeeSpecializedRoleDto>;
