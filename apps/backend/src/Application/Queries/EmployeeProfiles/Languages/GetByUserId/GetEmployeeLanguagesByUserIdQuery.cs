using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.Languages.GetByUserId;

public sealed record GetEmployeeLanguagesByUserIdQuery(Guid UserId) : IQuery<List<EmployeeLanguageDto>>;
