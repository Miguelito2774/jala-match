using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.WorkExperiences.Add;

internal sealed class AddWorkExperienceCommandHandler(IApplicationDbContext context)
    : ICommandHandler<AddWorkExperienceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        AddWorkExperienceCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? employeeProfile = await context.EmployeeProfiles.FirstOrDefaultAsync(
            ep => ep.UserId == request.UserId,
            cancellationToken
        );

        if (employeeProfile is null)
        {
            return Result.Failure<Guid>(
                Error.NotFound("Profile.NotFound", "Employee profile not found")
            );
        }

        var workExperience = new WorkExperience
        {
            Id = Guid.NewGuid(),
            EmployeeProfileId = employeeProfile.Id,
            ProjectName = request.ProjectName,
            Description = request.Description,
            Tools = request.Tools,
            ThirdParties = request.ThirdParties,
            Frameworks = request.Frameworks,
            VersionControl = request.VersionControl,
            ProjectManagement = request.ProjectManagement,
            Responsibilities = request.Responsibilities,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            EmployeeProfile = employeeProfile,
        };

        context.WorkExperiences.Add(workExperience);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(workExperience.Id);
    }
}
