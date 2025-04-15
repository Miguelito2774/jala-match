using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Teams.GenerateTeams;

internal sealed class GenerateTeamsCommandHandler
    : ICommandHandler<GenerateTeamsCommand, AiServiceResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITeamService _teamService;
    private readonly ISfiaCalculatorService _sfiaCalculator;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public GenerateTeamsCommandHandler(
        IApplicationDbContext dbContext,
        ITeamService teamService,
        ISfiaCalculatorService sfiaCalculator
    )
    {
        _dbContext = dbContext;
        _teamService = teamService;
        _sfiaCalculator = sfiaCalculator;
    }

    public async Task<Result<AiServiceResponse>> Handle(
        GenerateTeamsCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            List<EmployeeProfile> filteredEmployees = await FilterEmployeesByRequirements(
                command.Roles,
                command.Technologies,
                command.Availability,
                cancellationToken
            );

            if (!filteredEmployees.Any())
            {
                return Result.Failure<AiServiceResponse>(
                    new Error(
                        "Teams.Generation.NoEligibleEmployees",
                        "No employees match the specified criteria",
                        ErrorType.NotFound
                    )
                );
            }

            var membersData = new List<TeamMemberGenerated>();
            foreach (EmployeeProfile employee in filteredEmployees)
            {
                int calculatedSfia = await _sfiaCalculator.CalculateAverageSfiaForRequirements(
                    employee.Id,
                    command.Technologies,
                    cancellationToken
                );

                TeamMemberGenerated memberData = MapEmployeeToMemberData(employee, calculatedSfia);
                membersData.Add(memberData);
            }

            var eligibleMembers = membersData
                .OrderByDescending(m => m.SfiaLevel)
                .ThenBy(m => Guid.NewGuid())
                .ToList();

            if (!eligibleMembers.Any())
            {
                return Result.Failure<AiServiceResponse>(
                    new Error(
                        "Teams.Generation.NoEligibleEmployees",
                        "No employees meet the minimum SFIA level requirement for the specified technologies",
                        ErrorType.NotFound
                    )
                );
            }

            Result<AiServiceResponse> response = await _teamService.GenerateTeams(
                command.Roles,
                command.Technologies,
                command.SfiaLevel,
                command.TeamSize,
                eligibleMembers,
                command.Weights,
                cancellationToken
            );

            if (response.IsFailure)
            {
                return Result.Failure<AiServiceResponse>(response.Error);
            }

            return response;
        }
        catch (Exception ex)
        {
            return Result.Failure<AiServiceResponse>(
                new Error("Teams.Generation.Failed", ex.Message, ErrorType.Failure)
            );
        }
    }

    private async Task<List<EmployeeProfile>> FilterEmployeesByRequirements(
        List<TeamRoleRequest> roles,
        List<string> technologies,
        bool availabilityRequired,
        CancellationToken cancellationToken
    )
    {
        IQueryable<EmployeeProfile> query = _dbContext
            .EmployeeProfiles.Include(p => p.Technologies)
            .ThenInclude(t => t.Technology)
            .Include(p => p.PersonalInterests)
            .Include(p => p.User);

        if (availabilityRequired)
        {
            query = query.Where(p => p.Availability);
        }

        if (roles.Any())
        {
            IEnumerable<string> roleNames = roles.Select(r => r.Role).Distinct();
            query = query.Where(p => roleNames.Contains(p.Specialization));
        }

        if (technologies.Any())
        {
            query = query.Where(p =>
                p.Technologies.Any(t => technologies.Contains(t.Technology.Name))
            );
        }

        return await query.ToListAsync(cancellationToken);
    }

    private TeamMemberGenerated MapEmployeeToMemberData(
        EmployeeProfile employee,
        int calculatedSfia
    )
    {
        return new TeamMemberGenerated(
            Id: employee.Id,
            Name: $"{employee.FirstName} {employee.LastName}",
            Role: employee.Specialization,
            Technologies: employee.Technologies.Select(t => t.Technology.Name).ToList(),
            SfiaLevel: calculatedSfia,
            Mbti: employee.Mbti,
            Interests: employee.PersonalInterests.Select(pi => pi.Name).ToList(),
            Timezone: employee.Timezone,
            Country: employee.Country
        );
    }

    // private async Task StoreTeamSuggestion(
    //     TeamCompositionResponse teamComposition,
    //     Guid creatorId,
    //     List<TeamRoleRequest> roles,
    //     List<string> requiredTechnologies,
    //     int sfiaLevel,
    //     int teamSize,
    //     WeightCriteria weights,
    //     CancellationToken cancellationToken)
    // {
    //     // TODO
    // }

    public async Task<Result<TeamCompatibilityResponse>> AddMemberToTeam(
        Guid teamId,
        Guid newMemberId,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Team? team = await _dbContext
                .Teams.Include(t => t.Members)
                .ThenInclude(m => m.EmployeeProfile)
                .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);

            if (team == null)
            {
                return Result.Failure<TeamCompatibilityResponse>(
                    new Error("Teams.Member.TeamNotFound", "Team not found", ErrorType.NotFound)
                );
            }

            EmployeeProfile? newMember = await _dbContext
                .EmployeeProfiles.Include(p => p.Technologies)
                .ThenInclude(t => t.Technology)
                .FirstOrDefaultAsync(p => p.Id == newMemberId, cancellationToken);

            if (newMember == null)
            {
                return Result.Failure<TeamCompatibilityResponse>(
                    new Error(
                        "Teams.Member.MemberNotFound",
                        "Team member not found",
                        ErrorType.NotFound
                    )
                );
            }

            if (team.RequiredTechnologiesJson != null)
            {
                List<string> requiredTechnologies =
                    JsonSerializer.Deserialize<List<string>>(
                        team.RequiredTechnologiesJson,
                        _jsonOptions
                    ) ?? new List<string>();

                var currentMembers = team
                    .Members.Select(m =>
                        MapEmployeeToMemberData(
                            m.EmployeeProfile,
                            m.EmployeeProfile.SfiaLevelGeneral
                        )
                    )
                    .ToList();

                int calculatedSfia = await _sfiaCalculator.CalculateAverageSfiaForRequirements(
                    newMember.Id,
                    requiredTechnologies,
                    cancellationToken
                );

                TeamMemberGenerated newMemberData = MapEmployeeToMemberData(
                    newMember,
                    calculatedSfia
                );

                Result<TeamCompatibilityResponse> compatibilityResult =
                    await _teamService.CalculateCompatibility(
                        currentMembers,
                        newMemberData,
                        requiredTechnologies,
                        cancellationToken
                    );

                if (compatibilityResult.IsFailure)
                {
                    return Result.Failure<TeamCompatibilityResponse>(compatibilityResult.Error);
                }

                if (compatibilityResult.Value.CompatibilityScore > 0.5)
                {
                    team.Members.Add(
                        new TeamMember
                        {
                            TeamId = team.Id,
                            Team = team,
                            EmployeeProfileId = newMember.Id,
                            EmployeeProfile = newMember,
                            Role = TeamMemberRole.Developer,
                            JoinedDate = DateTime.UtcNow,
                        }
                    );

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }

                return Result.Success(compatibilityResult.Value);
            }
        }
        catch (Exception ex)
        {
            return Result.Failure<TeamCompatibilityResponse>(
                new Error("Teams.Member.AddFailed", ex.Message, ErrorType.Failure)
            );
        }

        return Result.Failure<TeamCompatibilityResponse>(
            new Error("Teams.Member.AddFailed", "Failed to add member to team", ErrorType.Failure)
        );
    }
}
