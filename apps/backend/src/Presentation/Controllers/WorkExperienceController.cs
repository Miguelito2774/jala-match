using System.Security.Claims;
using Application.Commands.EmployeeProfiles.WorkExperiences.Add;
using Application.Commands.EmployeeProfiles.WorkExperiences.Delete;
using Application.Commands.EmployeeProfiles.WorkExperiences.Json;
using Application.Commands.EmployeeProfiles.WorkExperiences.Update;
using Application.DTOs;
using Application.Queries.EmployeeProfiles.WorkExperience.GetById;
using Application.Queries.EmployeeProfiles.WorkExperience.GetByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("work-experiences")]
[Authorize]
public sealed class WorkExperienceController(ISender sender) : ControllerBase
{
    #region GET Endpoints

    [HttpGet("user/{userId:guid}")]
    public async Task<IResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetWorkExperiencesByUserIdQuery(userId);
        Result<List<WorkExperienceDto>> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{workExperienceId:guid}")]
    public async Task<IResult> GetById(Guid workExperienceId, CancellationToken cancellationToken)
    {
        Guid userId = GetCurrentUserId();
        var query = new GetWorkExperienceByIdQuery(workExperienceId, userId);
        Result<WorkExperienceDto> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    #endregion

    #region POST Endpoints

    [HttpPost("user/{userId:guid}")]
    public async Task<IResult> Add(
        Guid userId,
        [FromBody] AddWorkExperienceRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AddWorkExperienceCommand(
            userId,
            request.ProjectName,
            request.Description,
            request.Tools,
            request.ThirdParties,
            request.Frameworks,
            request.VersionControl,
            request.ProjectManagement,
            request.Responsibilities,
            request.StartDate,
            request.EndDate
        );

        Result<Guid> result = await sender.Send(command, cancellationToken);
        return result.Match(
            id => Results.Created($"/work-experiences/{id}", new { Id = id }),
            CustomResults.Problem
        );
    }

    [HttpPost("user/{userId:guid}/import")]
    public async Task<IResult> ImportWorkExperiences(
        Guid userId,
        [FromBody] ImportWorkExperiencesRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new ImportWorkExperiencesCommand(userId, request.WorkExperiences);
        Result<List<Guid>> result = await sender.Send(command, cancellationToken);
        return result.Match(
            ids => Results.Ok(new { ImportedIds = ids, ids.Count }),
            CustomResults.Problem
        );
    }

    #endregion

    #region PUT Endpoints

    [HttpPut("{workExperienceId:guid}")]
    public async Task<IResult> Update(
        Guid workExperienceId,
        [FromBody] UpdateWorkExperienceRequest request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = GetCurrentUserId();
        var command = new UpdateWorkExperienceCommand(
            workExperienceId,
            userId,
            request.ProjectName,
            request.Description,
            request.Tools,
            request.ThirdParties,
            request.Frameworks,
            request.VersionControl,
            request.ProjectManagement,
            request.Responsibilities,
            request.StartDate,
            request.EndDate
        );

        Result result = await sender.Send(command, cancellationToken);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    #endregion

    #region DELETE Endpoints

    [HttpDelete("{workExperienceId:guid}")]
    public async Task<IResult> Delete(Guid workExperienceId, CancellationToken cancellationToken)
    {
        Guid userId = GetCurrentUserId();
        var command = new DeleteWorkExperienceCommand(workExperienceId, userId);
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
