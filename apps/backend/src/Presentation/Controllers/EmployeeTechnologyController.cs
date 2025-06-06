using System.Security.Claims;
using Application.Commands.EmployeeProfiles.TechnicalProfile.AddEmployeeTechnology;
using Application.Commands.EmployeeProfiles.TechnicalProfile.DeleteEmployeeTechnology;
using Application.Commands.EmployeeProfiles.TechnicalProfile.Json;
using Application.Commands.EmployeeProfiles.TechnicalProfile.UpdateEmployeeTechnology;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("employee-technologies")]
[Authorize]
public sealed class EmployeeTechnologyController(ISender sender) : ControllerBase
{
    #region POST Endpoints

    [HttpPost("user/{userId:guid}")]
    public async Task<IResult> Add(
        Guid userId,
        [FromBody] AddEmployeeTechnologyRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AddEmployeeTechnologyCommand(
            userId,
            request.TechnologyId,
            request.SfiaLevel,
            request.YearsExperience,
            request.Version
        );

        Result<Guid> result = await sender.Send(command, cancellationToken);
        return result.Match(
            id => Results.Created($"/employee-technologies/{id}", new { Id = id }),
            CustomResults.Problem
        );
    }

    [HttpPost("user/{userId:guid}/import")]
    public async Task<IResult> ImportTechnologies(
        Guid userId,
        [FromBody] ImportTechnologiesRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new ImportTechnologiesCommand(userId, request.Technologies);
        Result<List<Guid>> result = await sender.Send(command, cancellationToken);
        return result.Match(
            ids => Results.Ok(new { ImportedIds = ids, ids.Count }),
            CustomResults.Problem
        );
    }

    #endregion

    #region PUT Endpoints

    [HttpPut("{employeeTechnologyId:guid}")]
    public async Task<IResult> Update(
        Guid employeeTechnologyId,
        [FromBody] UpdateEmployeeTechnologyRequest request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = GetCurrentUserId();
        var command = new UpdateEmployeeTechnologyCommand(
            employeeTechnologyId,
            userId,
            request.SfiaLevel,
            request.YearsExperience,
            request.Version
        );

        Result result = await sender.Send(command, cancellationToken);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    #endregion

    #region DELETE Endpoints

    [HttpDelete("{employeeTechnologyId:guid}")]
    public async Task<IResult> Delete(
        Guid employeeTechnologyId,
        CancellationToken cancellationToken
    )
    {
        Guid userId = GetCurrentUserId();
        var command = new DeleteEmployeeTechnologyCommand(employeeTechnologyId, userId);
        Result result = await sender.Send(command, cancellationToken);
        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }

    #endregion

    private Guid GetCurrentUserId()
    {
        string? userIdClaim =
            User.FindFirst("userId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAccessException("Usuario no autenticado correctamente");
        }

        return userId;
    }
}
