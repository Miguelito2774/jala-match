using Application.Abstractions.Messaging;

namespace Application.Queries.EmployeeProfiles.Exists;

public sealed record CheckEmployeeProfileExistsQuery(Guid UserId) : IQuery<bool>;
