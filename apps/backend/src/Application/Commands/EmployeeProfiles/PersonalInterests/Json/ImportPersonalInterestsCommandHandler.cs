using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Json;

internal sealed class ImportPersonalInterestsCommandHandler(IApplicationDbContext context)
    : ICommandHandler<ImportPersonalInterestsCommand, List<Guid>>
{
    public async Task<Result<List<Guid>>> Handle(
        ImportPersonalInterestsCommand request,
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

        var personalInterests = new List<PersonalInterest>();
        var createdIds = new List<Guid>();

        foreach (PersonalInterestImportDto dto in request.PersonalInterests)
        {
            var personalInterest = new PersonalInterest
            {
                Id = Guid.NewGuid(),
                EmployeeProfileId = employeeProfile.Id,
                Name = dto.Name,
                SessionDurationMinutes = dto.SessionDurationMinutes,
                Frequency = dto.Frequency,
                InterestLevel = dto.InterestLevel,
                EmployeeProfile = employeeProfile,
            };

            personalInterests.Add(personalInterest);
            createdIds.Add(personalInterest.Id);
        }

        context.PersonalInterests.AddRange(personalInterests);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(createdIds);
    }
}
