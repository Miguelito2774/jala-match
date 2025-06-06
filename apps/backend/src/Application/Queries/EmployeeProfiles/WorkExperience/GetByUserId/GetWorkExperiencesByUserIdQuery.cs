using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.EmployeeProfiles.WorkExperience.GetByUserId;

public sealed record GetWorkExperiencesByUserIdQuery(Guid UserId) : IQuery<List<WorkExperienceDto>>;
