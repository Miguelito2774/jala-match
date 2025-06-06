using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.PersonalInterests.Add;

public sealed class AddPersonalInterestCommandHandler
    : ICommandHandler<AddPersonalInterestCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddPersonalInterestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        AddPersonalInterestCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context.EmployeeProfiles.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            cancellationToken
        );

        if (profile == null)
        {
            return Result.Failure<Guid>(
                new Error("Profile.NotFound", "Employee profile not found", ErrorType.Failure)
            );
        }

        var personalInterest = new PersonalInterest
        {
            EmployeeProfileId = profile.Id,
            Name = request.Name,
            SessionDurationMinutes = request.SessionDurationMinutes,
            Frequency = request.Frequency,
            InterestLevel = request.InterestLevel,
        };

        _context.PersonalInterests.Add(personalInterest);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(personalInterest.Id);
    }
}
