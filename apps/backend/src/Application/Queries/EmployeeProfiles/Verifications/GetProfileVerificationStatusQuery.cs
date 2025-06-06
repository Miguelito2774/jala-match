using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.Verifications;

public sealed record GetProfileVerificationStatusQuery(Guid UserId)
    : IQuery<ProfileVerificationStatusDto>;
