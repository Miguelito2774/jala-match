using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Update;

public sealed class UpdatePersonalInterestCommandHandler
    : ICommandHandler<UpdatePersonalInterestCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdatePersonalInterestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdatePersonalInterestCommand request,
        CancellationToken cancellationToken
    )
    {
        PersonalInterest? personalInterest = await _context.PersonalInterests.FindAsync(
            new object[] { request.PersonalInterestId },
            cancellationToken
        );

        if (personalInterest == null)
        {
            return Result.Failure(
                Error.NotFound(
                    "EmployeeTechnology.NotFound",
                    "Technology not found for this profile"
                )
            );
        }

        personalInterest.Name = request.Name;
        personalInterest.SessionDurationMinutes = request.SessionDurationMinutes;
        personalInterest.Frequency = request.Frequency;
        personalInterest.InterestLevel = request.InterestLevel;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
