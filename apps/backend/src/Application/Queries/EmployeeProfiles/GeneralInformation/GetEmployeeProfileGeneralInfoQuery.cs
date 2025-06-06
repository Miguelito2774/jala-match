using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.GeneralInformation;

public sealed record GetEmployeeProfileGeneralInfoQuery(Guid UserId)
    : IQuery<EmployeeProfileGeneralInfoDto>;
