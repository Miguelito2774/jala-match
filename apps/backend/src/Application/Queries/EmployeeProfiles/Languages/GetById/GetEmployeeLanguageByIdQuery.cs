using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.Languages.GetById;

public sealed record GetEmployeeLanguageByIdQuery(Guid LanguageId) : IQuery<EmployeeLanguageDto>;
