using System.Security.Claims;
using System.Text.Json;
using Application.Commands.Privacy;
using Application.DTOs;
using Application.Queries.Privacy;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrivacyController : ControllerBase
{
    private readonly IMediator _mediator;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public PrivacyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("consents")]
    public async Task<IActionResult> GetConsents()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
        {
            return Unauthorized();
        }

        var query = new GetUserConsentQuery(userGuid);
        Result<ConsentSettingsDto> result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return StatusCode(500, new { message = result.Error.Description });
        }

        return Ok(result.Value);
    }

    [HttpPut("consents")]
    public async Task<IActionResult> UpdateConsents([FromBody] UpdateConsentRequestDto request)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
        {
            return Unauthorized();
        }

        var command = new UpdateConsentCommand(userGuid, request);
        Result result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return StatusCode(500, new { message = result.Error.Description });
        }

        return Ok(new { message = "Consent settings updated successfully" });
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportData()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
        {
            return Unauthorized();
        }

        var query = new ExportUserDataQuery(userGuid);
        Result<DataExportDto> result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return StatusCode(500, new { message = result.Error.Description });
        }

        string json = JsonSerializer.Serialize(result.Value, JsonOptions);

        string fileName = $"jala-match-data-export-{userGuid}-{DateTime.UtcNow:yyyyMMdd}.json";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        return File(bytes, "application/json", fileName);
    }

    [HttpPost("delete-request")]
    public async Task<IActionResult> RequestDataDeletion([FromBody] DataDeletionRequestDto request)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
        {
            return Unauthorized();
        }

        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        string userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new CreateDataDeletionRequestCommand(userGuid, request, ipAddress, userAgent);
        Result<DataDeletionResponseDto> result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return StatusCode(500, new { message = result.Error.Description });
        }

        return Ok(result.Value);
    }

    [HttpDelete("delete-request/{requestId}")]
    public async Task<IActionResult> CancelDataDeletion(string requestId)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(requestId, out Guid requestGuid))
        {
            return BadRequest(new { message = "Invalid request ID format" });
        }

        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        string userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var command = new CancelDataDeletionRequestCommand(
            userGuid,
            requestGuid,
            ipAddress,
            userAgent
        );
        Result result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return StatusCode(500, new { message = result.Error.Description });
        }

        return Ok(new { message = "Data deletion request cancelled successfully" });
    }

    [HttpPost("reset-profile")]
    public async Task<IActionResult> ResetProfile([FromBody] DataDeletionRequestDto request)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
        {
            return Unauthorized();
        }

        var command = new ResetUserProfileCommand(userGuid, request.DataTypes, request.Reason);
        Result<bool> result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return StatusCode(500, new { message = result.Error.Description });
        }

        return Ok(new { 
            success = true,
            message = "Perfil reiniciado exitosamente. Puedes completar tu información nuevamente." 
        });
    }
}
