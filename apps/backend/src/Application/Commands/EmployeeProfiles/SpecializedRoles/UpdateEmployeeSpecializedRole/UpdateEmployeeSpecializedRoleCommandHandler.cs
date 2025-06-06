using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.SpecializedRoles.UpdateEmployeeSpecializedRole;

public sealed class UpdateEmployeeSpecializedRoleCommandHandler
    : ICommandHandler<UpdateEmployeeSpecializedRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmployeeSpecializedRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        UpdateEmployeeSpecializedRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        EmployeeSpecializedRole? entity =
            await _context.EmployeeSpecializedRoles.FirstOrDefaultAsync(
                esr => esr.Id == request.RoleId,
                cancellationToken
            );

        if (entity == null)
        {
            return Result.Failure(
                new Error(
                    "EmployeeSpecializedRole.NotFound",
                    "Specialized role entry not found",
                    ErrorType.Failure
                )
            );
        }

        entity.Level = request.Level;
        entity.YearsExperience = request.YearsExperience;

        _context.EmployeeSpecializedRoles.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
