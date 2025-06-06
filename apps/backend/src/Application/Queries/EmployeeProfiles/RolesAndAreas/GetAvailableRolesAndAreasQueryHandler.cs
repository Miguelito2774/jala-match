using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.RolesAndAreas;

public sealed class GetAvailableRolesAndAreasQueryHandler
    : IQueryHandler<GetAvailableRolesAndAreasQuery, AvailableRolesAndAreasResponse>
{
    private readonly IApplicationDbContext _context;

    public GetAvailableRolesAndAreasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AvailableRolesAndAreasResponse>> Handle(
        GetAvailableRolesAndAreasQuery request,
        CancellationToken cancellationToken
    )
    {
        string[] sfiaLevels = new[] { "Junior", "Staff", "Senior", "Architect" };

        // Get all technical areas with their specialized roles from database
        List<TechnicalArea> technicalAreas = await _context
            .TechnicalAreas.Include(ta => ta.Roles)
            .ToListAsync(cancellationToken);

        var roles = new List<RoleWithAreasDto>();

        // Group roles by role name and collect all areas for each role
        var roleGroups = technicalAreas
            .SelectMany(ta => ta.Roles.Select(r => new { RoleName = r.Name, AreaName = ta.Name }))
            .GroupBy(x => x.RoleName)
            .ToList();

        foreach (var roleGroup in roleGroups)
        {
            roles.Add(
                new RoleWithAreasDto
                {
                    Role = roleGroup.Key,
                    Areas = roleGroup.Select(x => x.AreaName).Distinct().ToList(),
                    Levels = sfiaLevels.ToList(),
                }
            );
        }

        var response = new AvailableRolesAndAreasResponse { Roles = roles };

        return Result.Success(response);
    }
}
