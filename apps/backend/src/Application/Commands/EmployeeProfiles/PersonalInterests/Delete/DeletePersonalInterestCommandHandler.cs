using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Delete;

public sealed class DeletePersonalInterestCommandHandler
    : ICommandHandler<DeletePersonalInterestCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePersonalInterestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeletePersonalInterestCommand request,
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
                Error.NotFound("PersonalInterest.NotFound", "Personal interest not found")
            );
        }

        // Optional: Verify ownership by checking if the personal interest belongs to the user's profile
        EmployeeProfile? profile = await _context.EmployeeProfiles.FindAsync(
            new object[] { personalInterest.EmployeeProfileId },
            cancellationToken
        );

        if (profile == null || profile.UserId != request.UserId)
        {
            return Result.Failure(
                Error.Forbidden(
                    "PersonalInterest.Forbidden",
                    "You don't have permission to delete this personal interest"
                )
            );
        }

        _context.PersonalInterests.Remove(personalInterest);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
