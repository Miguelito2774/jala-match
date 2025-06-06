using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.TechnicalProfile;

public sealed record GetEmployeeProfileTechnicalQuery(Guid UserId)
    : IQuery<EmployeeProfileTechnicalDto>;
