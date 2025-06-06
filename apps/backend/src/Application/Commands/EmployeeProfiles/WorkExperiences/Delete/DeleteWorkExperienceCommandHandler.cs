using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.WorkExperiences.Delete;

internal sealed class DeleteWorkExperienceCommandHandler(IApplicationDbContext context)
    : ICommandHandler<DeleteWorkExperienceCommand>
{
    public async Task<Result> Handle(
        DeleteWorkExperienceCommand request,
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

        context.WorkExperiences.Remove(workExperience);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
