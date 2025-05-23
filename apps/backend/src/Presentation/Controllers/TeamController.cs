﻿using Application.Commands.Teams.Create;
using Application.Commands.Teams.GenerateTeams;
using Application.Commands.Teams.Reanalyze;
using Application.DTOs;
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
    private static readonly string[] item = new[] { "Junior", "Staff", "Senior", "Architect" };
    private static readonly string[] itemArray = new[] 
                {
                    "Web Development",
                    "Mobile Development",
                    "Backend Development",
                    "Frontend Development",
                    "DevOps & Infrastructure"
                };
    private static readonly string[] itemArray0 = new[]
                {
                    "Test Automation",
                    "Performance Testing",
                    "Security Testing",
                    "Scripting"
                };
    private static readonly string[] itemArray1 = new[]
                {
                    "Functional Testing",
                    "Exploratory Testing",
                    "Regression Testing"
                };
    private static readonly string[] itemArray2 = new[]
                {
                    "User Research",
                    "Wireframing",
                    "Visual Design",
                    "Interaction Design"
                };
    private static readonly string[] itemArray3 = new[]
                {
                    "Data Pipelines",
                    "ETL",
                    "Big Data"
                };
    private static readonly string[] itemArray4 = new[]
                {
                    "Machine Learning",
                    "Data Analysis",
                    "AI/ML Operations"
                };

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
            request.Requirements,
            request.SfiaLevel,
            request.Technologies,
            request.Weights
        );

        Result<AiServiceResponse> result = await _sender.Send(command);

        return result.Match(Results.Ok, error => CustomResults.Problem(error));
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
            new 
            {
                Role = "Developer",
                Areas = itemArray,
                Levels = item
            },
            new 
            {
                Role = "QA Automation",
                Areas = itemArray0,
                Levels = item
            },
            new 
            {
                Role = "QA Manual",
                Areas = itemArray1,
                Levels = item
            },
            new 
            {
                Role = "UX/UI Designer",
                Areas = itemArray2,
                Levels = item
            },
            new 
            {
                Role = "Data Engineer",
                Areas = itemArray3,
                Levels = item
            },
            new 
            {
                Role = "Data Scientist",
                Areas = itemArray4,
                Levels = item
            }
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
}
