using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.WorkExperience.GetById;

public sealed record GetWorkExperienceByIdQuery(Guid WorkExperienceId, Guid UserId)
    : IQuery<WorkExperienceDto>;
