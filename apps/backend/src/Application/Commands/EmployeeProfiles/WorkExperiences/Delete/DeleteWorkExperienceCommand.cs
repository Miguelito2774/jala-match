using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.WorkExperiences.Delete;

public sealed record DeleteWorkExperienceCommand(Guid WorkExperienceId, Guid UserId) : ICommand;
