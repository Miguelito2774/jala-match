using System.Security.Claims;
using Application.Commands.EmployeeProfiles.SpecializedRoles.AddEmployeeSpecializedRole;
using Application.Commands.EmployeeProfiles.TechnicalProfile.DeleteEmployeeSpecializedRole;
using Application.Commands.EmployeeProfiles.SpecializedRoles.UpdateEmployeeSpecializedRole;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("employee-profiles/user/{userId:guid}/specialized-roles")]
[Authorize(Roles = "Employee")]
public sealed class EmployeeSpecializedRoleController : ControllerBase
{
    private readonly ISender _sender;

    public EmployeeSpecializedRoleController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IResult> Add(
        Guid userId,
        [FromBody] AddEmployeeSpecializedRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AddEmployeeSpecializedRoleCommand(
            userId,
            request.SpecializedRoleId,
            request.Level,
            request.YearsExperience
        );

        Result<Guid> result = await _sender.Send(command, cancellationToken);
        return result.Match(
            id =>
                Results.Created(
                    $"/employee-profiles/user/{userId}/specialized-roles/{id}",
                    new { Id = id }
                ),
            CustomResults.Problem
        );
    }

    [HttpPut("{entityId:guid}")]
    public async Task<IResult> Update(
        Guid userId,
        Guid entityId,
        [FromBody] UpdateEmployeeSpecializedRoleRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateEmployeeSpecializedRoleCommand(
            entityId,
            request.Level,
            request.YearsExperience
        );

        Result result = await _sender.Send(command, cancellationToken);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    [HttpDelete("{entityId:guid}")]
    public async Task<IResult> Delete(
        Guid userId,
        Guid entityId,
        CancellationToken cancellationToken
    )
    {
        var command = new DeleteEmployeeSpecializedRoleCommand(entityId, userId);
        Result result = await _sender.Send(command, cancellationToken);
        return result.Match(() => Results.NoContent(), CustomResults.Problem);
    }
}
