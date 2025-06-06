using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.SpecializedRoles.GetById;

public sealed class GetEmployeeSpecializedRoleByIdQueryHandler
    : IQueryHandler<GetEmployeeSpecializedRoleByIdQuery, EmployeeSpecializedRoleDto>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeSpecializedRoleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmployeeSpecializedRoleDto>> Handle(
        GetEmployeeSpecializedRoleByIdQuery request,
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
            return Result.Failure<EmployeeSpecializedRoleDto>(
                new Error(
                    "EmployeeSpecializedRole.NotFound",
                    "Specialized role entry not found",
                    ErrorType.Failure
                )
            );
        }

        var dto = new EmployeeSpecializedRoleDto
        {
            Id = entity.Id,
            SpecializedRoleId = entity.SpecializedRoleId,
            Level = entity.Level,
            YearsExperience = entity.YearsExperience,
        };

        return Result.Success(dto);
    }
}
