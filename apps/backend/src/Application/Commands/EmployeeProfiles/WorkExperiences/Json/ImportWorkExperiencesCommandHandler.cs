using System.Globalization;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.WorkExperiences.Json;

internal sealed class ImportWorkExperiencesCommandHandler(IApplicationDbContext context)
    : ICommandHandler<ImportWorkExperiencesCommand, List<Guid>>
{
    public async Task<Result<List<Guid>>> Handle(
        ImportWorkExperiencesCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? employeeProfile = await context.EmployeeProfiles.FirstOrDefaultAsync(
            ep => ep.UserId == request.UserId,
            cancellationToken
        );

        if (employeeProfile is null)
        {
            return Result.Failure<List<Guid>>(
                Error.NotFound("Profile.NotFound", "Employee profile not found")
            );
        }

        var workExperiences = new List<WorkExperience>();
        var createdIds = new List<Guid>();

        foreach (WorkExperienceImportDto dto in request.WorkExperiences)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            if (
                !DateOnly.TryParse(
                    dto.StartDate,
                    culture,
                    DateTimeStyles.None,
                    out DateOnly startDate
                )
            )
            {
                continue;
            }

            DateOnly? endDate = null;
            if (
                !string.IsNullOrEmpty(dto.EndDate)
                && DateOnly.TryParse(
                    dto.EndDate,
                    culture,
                    DateTimeStyles.None,
                    out DateOnly parsedEndDate
                )
            )
            {
                endDate = parsedEndDate;
            }

            var workExperience = new WorkExperience
            {
                Id = Guid.NewGuid(),
                EmployeeProfileId = employeeProfile.Id,
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                Tools = dto.Tools,
                ThirdParties = dto.ThirdParties,
                Frameworks = dto.Frameworks,
                VersionControl = dto.VersionControl,
                ProjectManagement = dto.ProjectManagement,
                Responsibilities = dto.Responsibilities,
                StartDate = startDate,
                EndDate = endDate,
                EmployeeProfile = employeeProfile,
            };

            workExperiences.Add(workExperience);
            createdIds.Add(workExperience.Id);
        }

        context.WorkExperiences.AddRange(workExperiences);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(createdIds);
    }
}
