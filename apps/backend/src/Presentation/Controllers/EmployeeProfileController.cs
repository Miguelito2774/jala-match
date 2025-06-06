using System.Security.Claims;
using Application.Commands.EmployeeProfiles.GeneralInformation.Create;
using Application.Commands.EmployeeProfiles.GeneralInformation.Update;
using Application.Commands.EmployeeProfiles.TechnicalProfile.UpdateEmployeeProfile;
using Application.Commands.EmployeeProfiles.Verifications;
using Application.DTOs;
using Application.Queries.EmployeeProfiles.Complete;
using Application.Queries.EmployeeProfiles.GeneralInformation;
using Application.Queries.EmployeeProfiles.RolesAndAreas;
using Application.Queries.EmployeeProfiles.SpecializedRoles;
using Application.Queries.EmployeeProfiles.TechnicalProfile;
using Application.Queries.EmployeeProfiles.Verifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("employee-profiles")]
[Authorize]
public sealed class EmployeeProfileController(ISender sender) : ControllerBase
{
    #region GET Endpoints

    [HttpGet("user/{userId:guid}")]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> GetCompleteProfile(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeProfileByUserIdQuery(userId);
        Result<EmployeeProfileCompleteDto> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("user/{userId:guid}/general-info")]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> GetGeneralInfo(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeProfileGeneralInfoQuery(userId);
        Result<EmployeeProfileGeneralInfoDto> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("user/{userId:guid}/technical")]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> GetTechnicalProfile(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeProfileTechnicalQuery(userId);
        Result<EmployeeProfileTechnicalDto> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("available-roles-areas")]
    [Authorize(Roles = "Employee,Manager")]
    public async Task<IResult> GetAvailableRolesAndAreas(CancellationToken cancellationToken)
    {
        var query = new GetAvailableRolesAndAreasQuery();
        Result<AvailableRolesAndAreasResponse> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("user/{userId:guid}/verification-history")]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> GetVerificationHistory(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeVerificationHistoryQuery(userId);
        Result<List<ProfileVerificationDto>> result = await sender.Send(query, cancellationToken);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    #endregion

    #region POST Endpoints

    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> CreateProfile(
        [FromBody] CreateEmployeeProfileRequest request,
        CancellationToken cancellationToken
    )
    {
        Guid userId = GetCurrentUserId();
        var command = new CreateEmployeeProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.Country,
            request.Timezone,
            request.SfiaLevelGeneral,
            request.Mbti
        );

        Result<Guid> result = await sender.Send(command, cancellationToken);
        return result.Match(
            id => Results.Created($"/employee-profiles/user/{userId}", new { Id = id }),
            CustomResults.Problem
        );
    }

    [HttpPost("user/{userId:guid}/verification-request")]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> RequestVerification(Guid userId, CancellationToken cancellationToken)
    {
        var command = new RequestProfileVerificationCommand(userId);
        Result result = await sender.Send(command, cancellationToken);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    #endregion

    #region PUT Endpoints

    [HttpPut("user/{userId:guid}/general-info")]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> UpdateGeneralInfo(
        Guid userId,
        [FromBody] UpdateEmployeeProfileGeneralInfoRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateEmployeeProfileGeneralInfoCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.Country,
            request.Timezone
        );

        Result result = await sender.Send(command, cancellationToken);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
    }

    [HttpPut("user/{userId:guid}/technical")]
    [Authorize(Roles = "Employee")]
    public async Task<IResult> UpdateTechnicalProfile(
        Guid userId,
        [FromBody] UpdateEmployeeProfileTechnicalRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateEmployeeProfileTechnicalCommand(
            userId,
            request.SfiaLevelGeneral,
            request.Mbti
        );

        Result result = await sender.Send(command, cancellationToken);
        return result.Match(() => Results.Ok(), CustomResults.Problem);
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
