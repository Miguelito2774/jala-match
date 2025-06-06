using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.PersonalInterests.GetById;

public sealed record GetPersonalInterestByIdQuery(Guid PersonalInterestId, Guid UserId)
    : IQuery<PersonalInterestDto>;
