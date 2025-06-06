using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.EmployeeProfiles.WorkExperiences.Json;

public sealed record ImportWorkExperiencesCommand(
    Guid UserId,
    List<WorkExperienceImportDto> WorkExperiences
) : ICommand<List<Guid>>;
