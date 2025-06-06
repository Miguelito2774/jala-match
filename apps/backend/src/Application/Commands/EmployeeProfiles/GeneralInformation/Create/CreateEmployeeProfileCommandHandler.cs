using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.GeneralInformation.Create;

internal sealed class CreateEmployeeProfileCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateEmployeeProfileCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateEmployeeProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        User? user = await context.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            cancellationToken
        );

        if (user is null)
        {
            return Result.Failure<Guid>(Error.NotFound("User.NotFound", "User not found"));
        }

        EmployeeProfile? existingProfile = await context.EmployeeProfiles.FirstOrDefaultAsync(
            ep => ep.UserId == request.UserId,
            cancellationToken
        );

        if (existingProfile is not null)
        {
            return Result.Failure<Guid>(
                Error.Conflict(
                    "Profile.AlreadyExists",
                    "Employee profile already exists for this user"
                )
            );
        }

        var employeeProfile = new EmployeeProfile
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Availability = true,  // automatically enabled for new profiles
            Country = request.Country,
            Timezone = request.Timezone,
            SfiaLevelGeneral = request.SfiaLevelGeneral,
            Mbti = request.Mbti,
            User = user,
        };

        context.EmployeeProfiles.Add(employeeProfile);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(employeeProfile.Id);
    }
}
