using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.PersonalInterests.GetByUserId;

public sealed record GetPersonalInterestsByUserIdQuery(Guid UserId)
    : IQuery<List<PersonalInterestDto>>;
