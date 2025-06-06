using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.Verifications;

public sealed record GetPendingVerificationsQuery(int PageSize = 20, int PageNumber = 1)
    : IQuery<PendingVerificationsResponse>;
