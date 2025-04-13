using Application.Queries.Technologies.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("api/technologies")]
public sealed class TechnologiesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllTechnologiesQuery();

        Result<List<TechnologyResponse>> result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
