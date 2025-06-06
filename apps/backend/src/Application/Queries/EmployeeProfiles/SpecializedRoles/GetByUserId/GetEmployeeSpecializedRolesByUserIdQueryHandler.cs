using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.SpecializedRoles.GetByUserId;

public sealed class GetEmployeeSpecializedRolesByUserIdQueryHandler
    : IQueryHandler<GetEmployeeSpecializedRolesByUserIdQuery, List<EmployeeSpecializedRoleDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeSpecializedRolesByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EmployeeSpecializedRoleDto>>> Handle(
        GetEmployeeSpecializedRolesByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        List<EmployeeSpecializedRoleDto> list = await _context
            .EmployeeSpecializedRoles.Where(esr => esr.EmployeeProfile.UserId == request.UserId)
            .Select(esr => new EmployeeSpecializedRoleDto
            {
                Id = esr.Id,
                SpecializedRoleId = esr.SpecializedRoleId,
                Level = esr.Level,
                YearsExperience = esr.YearsExperience,
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}
