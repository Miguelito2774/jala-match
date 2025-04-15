using Application.Commands.Teams.Create;
using Application.Commands.Teams.GenerateTeams;
using Application.Commands.Teams.Reanalyze;
using Application.DTOs;
using Application.Queries.Teams.GetTeamCompatibility;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Errors;
using SharedKernel.Results;
using TeamCompatibilityResponse = Application.DTOs.TeamCompatibilityResponse;

namespace Presentation.Controllers;

[ApiController]
[Route("api/teams")]
public sealed class TeamsController : ControllerBase
{
    private readonly ISender _sender;
    private static readonly string[] item = new[] { "Junior", "Staff", "Senior" };

    public TeamsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("generate")]
    public async Task<IResult> GenerateTeams([FromBody] GenerateTeamsRequest request)
    {
        var command = new GenerateTeamsCommand(
            request.CreatorId,
            request.TeamSize,
            request.Roles,
            request.Technologies,
            request.SfiaLevel,
            request.Weights
        );

        Result<AiServiceResponse> result = await _sender.Send(command);

        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpGet("{teamId}/compatibility/{memberId}")]
    public async Task<IResult> GetCompatibility(Guid teamId, Guid memberId)
    {
        var query = new GetTeamCompatibilityQuery(teamId, memberId);

        try
        {
            if (await _sender.Send(query) is not Result<TeamCompatibilityResponse> result)
            {
                return CustomResults.Problem(
                    Result.Failure<TeamCompatibilityResponse>(
                        new Error(
                            "Compatibility.NullResult",
                            "Received null response from service",
                            ErrorType.Failure
                        )
                    )
                );
            }

            return result.Match(
                success => Results.Ok(success),
                error => CustomResults.Problem(result)
            );
        }
        catch (Exception ex)
        {
            return CustomResults.Problem(
                Result.Failure<TeamCompatibilityResponse>(
                    new Error("Compatibility.Error", ex.Message, ErrorType.Failure)
                )
            );
        }
    }

    [HttpPost]
    public async Task<IResult> CreateTeam([FromBody] CreateTeamsRequest request)
    {
        var command = new CreateTeamCommand(
            request.Name,
            request.CreatorId,
            request.RequiredTechnologies,
            request.MemberIds
        );

        Result<Guid> result = await _sender.Send(command);
        return result.Match(
            teamId => Results.Created($"/api/teams/{teamId}", teamId),
            CustomResults.Problem
        );
    }

    [HttpGet("available-roles")]
    public Task<IResult> GetAvailableRoles()
    {
        var roles = new List<object>
        {
            new { Role = "Developer", Levels = item },
            new { Role = "QA", Levels = item },
        };

        return Task.FromResult(Results.Ok(roles));
    }

    [HttpGet("weight-criteria")]
    public Task<IResult> GetWeightCriteria()
    {
        var criteria = new List<object>
        {
            new
            {
                Id = "sfiaWeight",
                Name = "Nivel SFIA",
                DefaultValue = 15,
            },
            new
            {
                Id = "technicalWeight",
                Name = "Tech Stack",
                DefaultValue = 20,
            },
            new
            {
                Id = "psychologicalWeight",
                Name = "MBTI",
                DefaultValue = 15,
            },
            new
            {
                Id = "experienceWeight",
                Name = "Experiencia",
                DefaultValue = 15,
            },
            new
            {
                Id = "languageWeight",
                Name = "Idioma",
                DefaultValue = 10,
            },
            new
            {
                Id = "interestsWeight",
                Name = "Intereses Personales",
                DefaultValue = 15,
            },
            new
            {
                Id = "timezoneWeight",
                Name = "Ubicacion - Zona horaria",
                DefaultValue = 10,
            },
        };

        return Task.FromResult(Results.Ok(criteria));
    }

    [HttpPost("reanalyze")]
    public async Task<IResult> ReanalyzeTeam([FromBody] ReanalyzeTeamRequest request)
    {
        var command = new ReanalyzeTeamCommand(request.TeamId, request.MemberIds, request.LeaderId);

        Result<AiServiceResponse> result = await _sender.Send(command);

        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }
}
