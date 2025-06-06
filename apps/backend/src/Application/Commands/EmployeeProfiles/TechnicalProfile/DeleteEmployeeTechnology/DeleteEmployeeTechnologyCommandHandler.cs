using Application.Abstractions.Data;
using Domain.Entities.Profiles;
using Domain.Entities.Technologies;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.DeleteEmployeeTechnology;

public sealed class DeleteEmployeeTechnologyCommandHandler
    : IRequestHandler<DeleteEmployeeTechnologyCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmployeeTechnologyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteEmployeeTechnologyCommand request,
        CancellationToken cancellationToken
    )
    {
        // Verify employee profile exists
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

        // Find the technology relation by its Id and the profile Id
        EmployeeTechnology? entity = await _context.EmployeeTechnologies.FirstOrDefaultAsync(
            et => et.Id == request.EmployeeTechnologyId && et.EmployeeProfileId == profile.Id,
            cancellationToken
        );

        if (entity == null)
        {
            return Result.Failure(
                Error.NotFound(
                    "EmployeeTechnology.NotFound",
                    "Technology entry not found for this profile"
                )
            );
        }

        _context.EmployeeTechnologies.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
