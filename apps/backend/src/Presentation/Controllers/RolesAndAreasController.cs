using Application.DTOs;
using Application.Queries.EmployeeProfiles.RolesAndAreas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("api/roles-and-areas")]
[Authorize(Roles = "Employee")]
public sealed class RolesAndAreasController : ControllerBase
{
    private readonly ISender _sender;

    public RolesAndAreasController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("available")]
    public async Task<IResult> GetAvailableRolesAndAreas(CancellationToken cancellationToken)
    {
        var query = new GetAvailableRolesAndAreasQuery();
        Result<AvailableRolesAndAreasResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.Match(data => Results.Ok(data), CustomResults.Problem);
    }

    [HttpGet("mapping")]
    public async Task<IResult> GetSpecializedRolesMapping(CancellationToken cancellationToken)
    {
        var query = new GetSpecializedRolesMappingQuery();
        Result<SpecializedRolesMappingResponse> result = await _sender.Send(
            query,
            cancellationToken
        );

        return result.Match(data => Results.Ok(data), CustomResults.Problem);
    }
}
