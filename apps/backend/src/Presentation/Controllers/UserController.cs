using Application.Queries.Users.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("users")]
public sealed class UserController(ISender sender) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<IResult> Get(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);

        Result<UserResponse> result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
