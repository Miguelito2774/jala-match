using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Areas_Roles;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.TechnicalProfile.AddEmployeeSpecializedRole;

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
        // Get employee profile
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

        // Check specialized role exists
        EmployeeSpecializedRole? specializedRole =
            await _context.EmployeeSpecializedRoles.FirstOrDefaultAsync(
                sr => sr.Id == request.SpecializedRoleId,
                cancellationToken
            );
        if (specializedRole == null)
        {
            return Result.Failure<Guid>(
                new Error(
                    "SpecializedRole.NotFound",
                    "Specialized role not found",
                    ErrorType.Failure
                )
            );
        }

        // Check for duplicates
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
                    "Specialization.AlreadyExists",
                    "Specialization already exists for this employee",
                    ErrorType.Failure
                )
            );
        }

        // Create and save
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
