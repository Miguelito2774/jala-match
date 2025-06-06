using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Areas_Roles;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.SpecializedRoles.AddEmployeeSpecializedRole;

public sealed class AddEmployeeSpecializedRoleCommandHandler
    : ICommandHandler<AddEmployeeSpecializedRoleCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddEmployeeSpecializedRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        AddEmployeeSpecializedRoleCommand request,
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
                new Error(
                    "EmployeeProfile.NotFound",
                    "Employee profile not found",
                    ErrorType.Failure
                )
            );
        }

        bool exists = await _context.EmployeeSpecializedRoles.AnyAsync(
            esr =>
                esr.EmployeeProfileId == profile.Id
                && esr.SpecializedRoleId == request.SpecializedRoleId,
            cancellationToken
        );

        if (exists)
        {
            return Result.Failure<Guid>(
                new Error(
                    "EmployeeSpecializedRole.AlreadyExists",
                    "Specialized role already added for this employee",
                    ErrorType.Failure
                )
            );
        }

        var entity = new EmployeeSpecializedRole
        {
            EmployeeProfileId = profile.Id,
            SpecializedRoleId = request.SpecializedRoleId,
            Level = request.Level,
            YearsExperience = request.YearsExperience,
        };

        _context.EmployeeSpecializedRoles.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
