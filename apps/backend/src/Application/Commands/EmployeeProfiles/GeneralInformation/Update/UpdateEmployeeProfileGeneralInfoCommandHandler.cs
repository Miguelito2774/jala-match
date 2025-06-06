using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.GeneralInformation.Update;

internal sealed class UpdateEmployeeProfileGeneralInformationCommandHandler(
    IApplicationDbContext context
) : ICommandHandler<UpdateEmployeeProfileGeneralInfoCommand>
{
    public async Task<Result> Handle(
        UpdateEmployeeProfileGeneralInfoCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? employeeProfile = await context.EmployeeProfiles.FirstOrDefaultAsync(
            ep => ep.UserId == request.UserId,
            cancellationToken
        );

        if (employeeProfile is null)
        {
            return Result.Failure(Error.NotFound("Profile.NotFound", "Employee profile not found"));
        }

        employeeProfile.FirstName = request.FirstName;
        employeeProfile.LastName = request.LastName;
        employeeProfile.Country = request.Country;
        employeeProfile.Timezone = request.Timezone;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
