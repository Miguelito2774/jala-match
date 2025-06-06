using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.SpecializedRoles.GetByUserId;

public sealed record GetEmployeeSpecializedRolesByUserIdQuery(Guid UserId)
    : IQuery<List<EmployeeSpecializedRoleDto>>;
