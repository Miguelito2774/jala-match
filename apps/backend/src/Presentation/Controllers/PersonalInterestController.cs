using System.Security.Claims;
using Application.Commands.EmployeeProfiles.PersonalInterests.Add;
using Application.Commands.EmployeeProfiles.PersonalInterests.Delete;
using Application.Commands.EmployeeProfiles.PersonalInterests.Json;
using Application.Commands.EmployeeProfiles.PersonalInterests.Update;
using Application.DTOs;
using Application.Queries.EmployeeProfiles.PersonalInterests.GetById;
using Application.Queries.EmployeeProfiles.PersonalInterests.GetByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("personal-interests")]
[Authorize]
public sealed class PersonalInterestController(ISender sender) : ControllerBase
{
    #region GET Endpoints

    [HttpGet("user/{userId:guid}")]
    public async Task<IResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetPersonalInterestsByUserIdQuery(userId);
        Result<List<PersonalInterestDto>> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{personalInterestId:guid}")]
    public async Task<IResult> GetById(Guid personalInterestId, CancellationToken cancellationToken)
    {
        Guid userId = GetCurrentUserId();
        var query = new GetPersonalInterestByIdQuery(personalInterestId, userId);
        Result<PersonalInterestDto> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    #endregion

    #region POST Endpoints

    [HttpPost]
    public async Task<IResult> Add(
        [FromBody] AddPersonalInterestRequest request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = GetCurrentUserId();
        var command = new AddPersonalInterestCommand(
            userId,
            request.Name,
            request.SessionDurationMinutes,
            request.Frequency,
            request.InterestLevel
        );

        Result<Guid> result = await sender.Send(command, cancellationToken);
        return result.Match(
            id => Results.Created($"/personal-interests/{id}", new { Id = id }),
            CustomResults.Problem
        );
    }

    [HttpPost("user/{userId:guid}/import")]
    public async Task<IResult> ImportPersonalInterests(
        Guid userId,
        [FromBody] ImportPersonalInterestsRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new ImportPersonalInterestsCommand(userId, request.PersonalInterests);
        Result<List<Guid>> result = await sender.Send(command, cancellationToken);
        return result.Match(
            ids => Results.Ok(new { ImportedIds = ids, ids.Count }),
            CustomResults.Problem
        );
    }

    #endregion

    #region PUT Endpoints

    [HttpPut("{personalInterestId:guid}")]
    public async Task<IResult> Update(
        Guid personalInterestId,
        [FromBody] UpdatePersonalInterestRequest request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = GetCurrentUserId();
        var command = new UpdatePersonalInterestCommand(
            personalInterestId,
            userId,
            request.Name,
            request.SessionDurationMinutes,
            request.Frequency,
            request.InterestLevel
        );

        Result result = await sender.Send(command, cancellationToken);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    #endregion

    #region DELETE Endpoints

    [HttpDelete("{personalInterestId:guid}")]
    public async Task<IResult> Delete(Guid personalInterestId, CancellationToken cancellationToken)
    {
        Guid userId = GetCurrentUserId();
        var command = new DeletePersonalInterestCommand(personalInterestId, userId);
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
