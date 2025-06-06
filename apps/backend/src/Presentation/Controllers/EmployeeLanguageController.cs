using Application.Commands.EmployeeProfiles.Languages.AddEmployeeLanguage;
using Application.Commands.EmployeeProfiles.Languages.DeleteEmployeeLanguage;
using Application.Commands.EmployeeProfiles.Languages.UpdateEmployeeLanguage;
using Application.DTOs;
using Application.Queries.EmployeeProfiles.Languages.GetById;
using Application.Queries.EmployeeProfiles.Languages.GetByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/employee-languages")]
public sealed class EmployeeLanguageController : ControllerBase
{
    private readonly ISender _sender;

    public EmployeeLanguageController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IResult> GetByUser(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeLanguagesByUserIdQuery(userId);
        Result<List<EmployeeLanguageDto>> result = await _sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{languageId:guid}")]
    public async Task<IResult> GetById(Guid languageId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeLanguageByIdQuery(languageId);
        Result<EmployeeLanguageDto> result = await _sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("user/{userId:guid}")]
    public async Task<IResult> CreateForUser(
        Guid userId,
        [FromBody] AddEmployeeLanguageRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AddEmployeeLanguageCommand(userId, request.Language, request.Proficiency);
        Result<Guid> result = await _sender.Send(command, cancellationToken);
        return result.Match(
            id => Results.Created($"/employee-languages/{id}", id),
            CustomResults.Problem
        );
    }

    [HttpPut("{languageId:guid}")]
    public async Task<IResult> Update(
        Guid languageId,
        [FromBody] UpdateEmployeeLanguageRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateEmployeeLanguageCommand(
            languageId,
            request.Language,
            request.Proficiency
        );
        Result result = await _sender.Send(command, cancellationToken);
        return result.Match(Results.NoContent, CustomResults.Problem);
    }

    [HttpDelete("{languageId:guid}")]
    public async Task<IResult> Delete(Guid languageId, CancellationToken cancellationToken)
    {
        var command = new DeleteEmployeeLanguageCommand(languageId);
        Result result = await _sender.Send(command, cancellationToken);
        return result.Match(Results.NoContent, CustomResults.Problem);
    }
}
