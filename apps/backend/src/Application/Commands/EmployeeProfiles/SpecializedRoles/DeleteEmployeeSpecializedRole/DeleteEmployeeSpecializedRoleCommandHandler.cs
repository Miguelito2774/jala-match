using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.EmployeeProfiles.SpecializedRoles.DeleteEmployeeSpecializedRole;

public sealed class DeleteEmployeeSpecializedRoleCommandHandler
    : ICommandHandler<DeleteEmployeeSpecializedRoleCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmployeeSpecializedRoleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(
        DeleteEmployeeSpecializedRoleCommand request,
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

        _context.EmployeeSpecializedRoles.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
