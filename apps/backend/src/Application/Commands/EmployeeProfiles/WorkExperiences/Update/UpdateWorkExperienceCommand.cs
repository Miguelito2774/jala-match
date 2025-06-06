using Application.Abstractions.Messaging;

namespace Application.Commands.EmployeeProfiles.WorkExperiences.Update;

public sealed record UpdateWorkExperienceCommand(
    Guid WorkExperienceId,
    Guid UserId,
    string ProjectName,
    string? Description,
    List<string> Tools,
    List<string> ThirdParties,
    List<string> Frameworks,
    string? VersionControl,
    string? ProjectManagement,
    List<string> Responsibilities,
    DateOnly StartDate,
    DateOnly? EndDate
) : ICommand;
