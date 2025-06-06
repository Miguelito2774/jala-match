using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.Complete;

public sealed record GetEmployeeProfileByUserIdQuery(Guid UserId)
    : IQuery<EmployeeProfileCompleteDto>;
