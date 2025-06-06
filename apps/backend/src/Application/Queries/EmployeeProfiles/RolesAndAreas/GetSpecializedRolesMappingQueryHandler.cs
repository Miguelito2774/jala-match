using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.RolesAndAreas;

public sealed class GetSpecializedRolesMappingQueryHandler
    : IQueryHandler<GetSpecializedRolesMappingQuery, SpecializedRolesMappingResponse>
{
    private readonly IApplicationDbContext _context;

    public GetSpecializedRolesMappingQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SpecializedRolesMappingResponse>> Handle(
        GetSpecializedRolesMappingQuery request,
        CancellationToken cancellationToken
    )
    {
        List<SpecializedRoleMappingDto> specializedRoles = await _context
            .SpecializedRoles.Include(sr => sr.TechnicalArea)
            .Select(sr => new SpecializedRoleMappingDto
            {
                Id = sr.Id,
                RoleName = sr.Name,
                TechnicalArea = sr.TechnicalArea.Name,
            })
            .ToListAsync(cancellationToken);

        var response = new SpecializedRolesMappingResponse { SpecializedRoles = specializedRoles };

        return Result.Success(response);
    }
}
