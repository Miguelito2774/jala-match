using Application.Abstractions.Services;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request)
    {
        Result<AuthResponse> result = await _authService.LoginAsync(
            request.Email,
            request.Password,
            HttpContext.RequestAborted
        );
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpPost("register/employee")]
    public async Task<IResult> RegisterEmployee([FromBody] RegisterEmployeeRequest request)
    {
        Result<AuthResponse> result = await _authService.RegisterEmployeeAsync(
            request,
            HttpContext.RequestAborted
        );
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpPost("register/manager")]
    public async Task<IResult> RegisterManager(
        [FromBody] RegisterManagerRequest request,
        [FromQuery] string invitationToken
    )
    {
        Result<AuthResponse> result = await _authService.RegisterManagerAsync(
            request,
            invitationToken,
            HttpContext.RequestAborted
        );
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpPost("invitations")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> CreateInvitation([FromBody] CreateInvitationRequest request)
    {
        var adminId = Guid.Parse(
            User.FindFirst("userId")?.Value ?? throw new UnauthorizedAccessException()
        );
        Result<string> result = await _authService.CreateInvitationLinkAsync(
            adminId,
            request.Email,
            request.TargetRole,
            HttpContext.RequestAborted
        );
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpGet("validate-invitation/{token}")]
    public async Task<IResult> ValidateInvitationToken(string token)
    {
        Result<bool> result = await _authService.ValidateInvitationTokenAsync(
            token,
            HttpContext.RequestAborted
        );
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }
}
