using System.Security.Claims;
using Application.Commands.EmployeeProfiles.Verifications;
using Application.DTOs;
using Application.Queries.EmployeeProfiles.Verifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("api/profile-verifications")]
[Authorize(Roles = "Manager")]
public sealed class ProfileVerificationController(ISender sender) : ControllerBase
{
    [HttpGet("pending")]
    public async Task<IResult> GetPendingVerifications(
        [FromQuery] int pageSize = 20,
        [FromQuery] int pageNumber = 1,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetPendingVerificationsQuery(pageSize, pageNumber);
        Result<PendingVerificationsResponse> result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{employeeProfileId:guid}")]
    public async Task<IResult> GetProfileForVerification(
        Guid employeeProfileId,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetProfileForVerificationQuery(employeeProfileId);
        Result<ProfileForVerificationDto> result = await sender.Send(query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("{employeeProfileId:guid}/approve")]
    public async Task<IResult> ApproveProfile(
        Guid employeeProfileId,
        [FromBody] ApproveProfileVerificationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.EmployeeProfileId != employeeProfileId)
        {
            return Results.BadRequest("Employee profile ID mismatch");
        }

        Guid reviewerId = GetCurrentUserId();
        var command = new ApproveProfileVerificationCommand(
            reviewerId,
            employeeProfileId,
            request.SfiaProposed,
            request.Notes
        );

        Result<VerificationDecisionResponse> result = await sender.Send(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost("{employeeProfileId:guid}/reject")]
    public async Task<IResult> RejectProfile(
        Guid employeeProfileId,
        [FromBody] RejectProfileVerificationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (request.EmployeeProfileId != employeeProfileId)
        {
            return Results.BadRequest("Employee profile ID mismatch");
        }

        Guid reviewerId = GetCurrentUserId();
        var command = new RejectProfileVerificationCommand(
            reviewerId,
            employeeProfileId,
            request.Notes
        );

        Result<VerificationDecisionResponse> result = await sender.Send(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private Guid GetCurrentUserId()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated");
        }
        return userId;
    }
}
