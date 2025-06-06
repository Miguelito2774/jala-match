using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Profiles;
using Domain.Entities.Technologies;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.UpdateEmployeeTechnology;

public sealed class UpdateEmployeeTechnologyCommandHandler
    : ICommandHandler<UpdateEmployeeTechnologyCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmployeeTechnologyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateEmployeeTechnologyCommand request,
        CancellationToken cancellationToken
    )
    {
        // verify profile exists
        EmployeeProfile? profile = await _context.EmployeeProfiles.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            cancellationToken
        );
        if (profile == null)
        {
            return Result.Failure(
                Error.Unauthorized("EmployeeTechnology.Unauthorized", "Employee profile not found")
            );
        }
        // find the technology entry by Id and verify ownership
        EmployeeTechnology? entity = await _context.EmployeeTechnologies.FirstOrDefaultAsync(
            et => et.Id == request.EmployeeTechnologyId && et.EmployeeProfileId == profile.Id,
            cancellationToken
        );
        if (entity == null)
        {
            return Result.Failure(
                Error.NotFound(
                    "EmployeeTechnology.NotFound",
                    "Technology not found for this profile"
                )
            );
        }

        // apply updates
        entity.SfiaLevel = request.SfiaLevel;
        entity.YearsExperience = request.YearsExperience;
        entity.Version = request.Version;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
