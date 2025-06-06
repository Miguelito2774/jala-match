using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.WorkExperiences.Update;

internal sealed class UpdateWorkExperienceCommandHandler(IApplicationDbContext context)
    : ICommandHandler<UpdateWorkExperienceCommand>
{
    public async Task<Result> Handle(
        UpdateWorkExperienceCommand request,
        CancellationToken cancellationToken
    )
    {
        WorkExperience? workExperience = await context
            .WorkExperiences.Include(we => we.EmployeeProfile)
            .FirstOrDefaultAsync(
                we =>
                    we.Id == request.WorkExperienceId
                    && we.EmployeeProfile!.UserId == request.UserId,
                cancellationToken
            );

        if (workExperience is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "WorkExperience.NotFound",
                    "Work experience not found or access denied"
                )
            );
        }

        // Update properties
        workExperience.ProjectName = request.ProjectName;
        workExperience.Description = request.Description;
        workExperience.Tools = request.Tools;
        workExperience.ThirdParties = request.ThirdParties;
        workExperience.Frameworks = request.Frameworks;
        workExperience.VersionControl = request.VersionControl;
        workExperience.ProjectManagement = request.ProjectManagement;
        workExperience.Responsibilities = request.Responsibilities;
        workExperience.StartDate = request.StartDate;
        workExperience.EndDate = request.EndDate;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
