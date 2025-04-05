using Application.Commands.Teams.Create;
using Application.Commands.Teams.GenerateTeams;
using Application.DTOs;
using Application.Queries.Teams.GetTeamCompatibility;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("api/teams")]
public sealed class TeamsController : ControllerBase
{
    private readonly ISender _sender;

    public TeamsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("generate")]
    public async Task<IResult> GenerateTeams([FromBody] GenerateTeamsRequest request)
    {
        var command = new GenerateTeamsCommand(
            request.Roles,
            request.Technologies,
            request.SfiaLevel,
            request.Availability,
            new Dictionary<string, int>
            {
                { "technical", request.TechnicalWeight },
                { "psychological", request.PsychologicalWeight },
                { "interests", request.InterestsWeight }
            });
            
        Result<TeamCompositionResponse> result = await _sender.Send(command);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpGet("{teamId}/compatibility/{memberId}")]
    public async Task<IResult> GetCompatibility(Guid teamId, Guid memberId)
    {
        var query = new GetTeamCompatibilityQuery(teamId, memberId);
        Result<TeamCompatibilityResponse> result = await _sender.Send(query);
        return result.Match(Results.Ok, CustomResults.Problem);
    }

    [HttpPost]
    public async Task<IResult> CreateTeam([FromBody] CreateTeamsRequest request)
    {
        var command = new CreateTeamCommand(
            request.Name,
            request.CreatorId,
            request.RequiredTechnologies,
            request.MemberIds);
            
        Result<Guid> result = await _sender.Send(command);
        return result.Match(
            teamId => Results.Created($"/api/teams/{teamId}", teamId),
            CustomResults.Problem);
    }
}
