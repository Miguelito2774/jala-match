using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IResult Get()
    {
        return Results.NoContent();
    }
}
