using System.Security.Claims;
using Application.Commands.Teams.AddTeamMember;
using Application.Commands.Teams.Create;
using Application.Commands.Teams.Delete;
using Application.Commands.Teams.GenerateTeams;
using Application.Commands.Teams.MoveTeamMember;
using Application.Commands.Teams.RemoveTeamMember;
using Application.DTOs;
using Application.Queries.Teams.FindTeamMembers;
using Application.Queries.Teams.GetAll;
using Application.Queries.Teams.GetAvailableTeamsForMember;
using Application.Queries.Teams.GetByCreatorId;
using Application.Queries.Teams.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using Presentation.Infrastructure;
using SharedKernel.Results;

namespace Presentation.Controllers;

[ApiController]
[Route("api/teams")]
[Authorize]
public sealed class TeamsController : ControllerBase
{
    private readonly ISender _sender;
    private static readonly string[] SfiaLevels = new[]
    {
        "Junior",
        "Staff",
        "Senior",
        "Architect",
    };
    private static readonly string[] DeveloperAreas = new[]
    {
        "Web Development",
        "Mobile Development",
        "Backend Development",
        "Frontend Development",
        "DevOps & Infrastructure",
    };
    private static readonly string[] QAAutomationAreas = new[]
    {
        "Test Automation",
        "Performance Testing",
        "Security Testing",
        "Scripting",
    };
    private static readonly string[] QAManualAreas = new[]
    {
        "Functional Testing",
        "Exploratory Testing",
        "Regression Testing",
    };
    private static readonly string[] DesignerAreas = new[]
    {
        "User Research",
        "Wireframing",
        "Visual Design",
        "Interaction Design",
    };
    private static readonly string[] DataEngineerAreas = new[]
    {
        "Data Pipelines",
        "ETL",
        "Big Data",
    };
    private static readonly string[] DataScientistAreas = new[]
    {
        "Machine Learning",
        "Data Analysis",
        "AI/ML Operations",
    };

    public TeamsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Manager")]
    public async Task<IResult> GenerateTeams([FromBody] GenerateTeamsRequest request)
    {
        Guid currentUserId = GetCurrentUserId();

        var command = new GenerateTeamsCommand(
            currentUserId,
            request.TeamSize,
            request.Requirements,
            request.SfiaLevel,
            request.Technologies,
            request.Weights
        );

        Result<AiServiceResponse> result = await _sender.Send(command);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpPost("create")]
    [Authorize(Roles = "Manager")]
    public async Task<IResult> CreateTeam([FromBody] CreateTeamsRequest request)
    {
        Guid currentUserId = GetCurrentUserId();

        var command = new CreateTeamCommand(
            request.Name,
            currentUserId,
            request.Members,
            request.LeaderId,
            request.Analysis,
            request.CompatibilityScore,
            request.Weights,
            request.RequiredTechnologies
        );

        Result<TeamResponse> result = await _sender.Send(command);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Manager")]
    public async Task<IResult> DeleteTeam(Guid id)
    {
        var command = new DeleteTeamCommand(id);
        Result result = await _sender.Send(command);
        return result.Match(Results.NoContent, error => CustomResults.Problem(error));
    }

    [HttpGet("by-id/{id}")]
    public async Task<IResult> GetTeamById(Guid id)
    {
        var query = new GetTeamByIdQuery(id);
        Result<TeamResponse> result = await _sender.Send(query);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpGet("all")]
    public async Task<IResult> GetAllTeams()
    {
        Guid currentUserId = GetCurrentUserId();
        string userRole = GetCurrentUserRole();

        if (userRole == "Admin")
        {
            var adminQuery = new GetAllTeamsQuery();
            Result<List<TeamResponse>> adminResult = await _sender.Send(adminQuery);
            return adminResult.Match(Results.Ok, error => CustomResults.Problem(error));
        }

        var query = new GetTeamsByCreatorIdQuery(currentUserId);
        Result<List<TeamResponse>> result = await _sender.Send(query);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpGet("by-creator/{creatorId}")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IResult> GetTeamsByCreatorId(Guid creatorId)
    {
        Guid currentUserId = GetCurrentUserId();
        string userRole = GetCurrentUserRole();

        if (userRole == "Manager" && creatorId != currentUserId)
        {
            return Results.Forbid();
        }

        var query = new GetTeamsByCreatorIdQuery(creatorId);
        Result<List<TeamResponse>> result = await _sender.Send(query);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpPost("find")]
    [Authorize(Roles = "Manager")]
    public async Task<IResult> FindTeamMembers([FromBody] FindTeamMemberRequest request)
    {
        var query = new FindTeamMembersQuery(
            request.TeamId,
            request.Role,
            request.Area,
            request.Level,
            request.Technologies
        );

        Result<List<TeamMemberRecommendation>> result = await _sender.Send(query);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpPost("add")]
    [Authorize(Roles = "Manager")]
    public async Task<IResult> AddTeamMember([FromBody] TeamMemberUpdateRequest request)
    {
        var command = new AddTeamMemberCommand(request.TeamId, request.Members);
        Result<TeamResponse> result = await _sender.Send(command);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpDelete("{teamId}/members/{employeeId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IResult> RemoveTeamMember(Guid teamId, Guid employeeId)
    {
        var command = new RemoveTeamMemberCommand(teamId, employeeId);
        Result<TeamResponse> result = await _sender.Send(command);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpPost("move-member")]
    [Authorize(Roles = "Manager")]
    public async Task<IResult> MoveTeamMember([FromBody] MoveTeamMemberRequest request)
    {
        var command = new MoveTeamMemberCommand(
            request.SourceTeamId,
            request.TargetTeamId,
            request.EmployeeProfileId
        );

        Result<(TeamResponse SourceTeam, TeamResponse TargetTeam)> result = await _sender.Send(
            command
        );

        return result.Match(
            success => Results.Ok(new { success.SourceTeam, success.TargetTeam }),
            error => CustomResults.Problem(error)
        );
    }

    [HttpGet("available-for-member/{employeeId}")]
    public async Task<IResult> GetAvailableTeamsForMember(
        Guid employeeId,
        [FromQuery] Guid? excludeTeamId = null
    )
    {
        Guid currentUserId = GetCurrentUserId();
        var query = new GetAvailableTeamsForMemberQuery(employeeId, currentUserId, excludeTeamId);
        Result<List<AvailableTeamDto>> result = await _sender.Send(query);
        return result.Match(Results.Ok, error => CustomResults.Problem(error));
    }

    [HttpGet("available-roles")]
    [Authorize]
    public Task<IResult> GetAvailableRoles()
    {
        var roles = new List<object>
        {
            new
            {
                Role = "Developer",
                Areas = DeveloperAreas,
                Levels = SfiaLevels,
            },
            new
            {
                Role = "QA Automation",
                Areas = QAAutomationAreas,
                Levels = SfiaLevels,
            },
            new
            {
                Role = "QA Manual",
                Areas = QAManualAreas,
                Levels = SfiaLevels,
            },
            new
            {
                Role = "UX/UI Designer",
                Areas = DesignerAreas,
                Levels = SfiaLevels,
            },
            new
            {
                Role = "Data Engineer",
                Areas = DataEngineerAreas,
                Levels = SfiaLevels,
            },
            new
            {
                Role = "Data Scientist",
                Areas = DataScientistAreas,
                Levels = SfiaLevels,
            },
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

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value
            ?? throw new UnauthorizedAccessException("Rol de usuario no encontrado");
    }
}
