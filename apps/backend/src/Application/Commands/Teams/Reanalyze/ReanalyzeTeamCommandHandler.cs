using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Teams.Reanalyze;

internal sealed class ReanalyzeTeamCommandHandler
    : ICommandHandler<ReanalyzeTeamCommand, AiServiceResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITeamService _teamService;
    private readonly ISfiaCalculatorService _sfiaCalculator;

    public ReanalyzeTeamCommandHandler(
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
        ReanalyzeTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            List<EmployeeProfile> members = await _dbContext
                .EmployeeProfiles.Include(p => p.Technologies)
                .ThenInclude(t => t.Technology)
                .Include(p => p.PersonalInterests)
                .Include(p => p.User)
                .Where(p => command.MemberIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            if (members.Count == 0)
            {
                return Result.Failure<AiServiceResponse>(
                    new Error(
                        "Teams.Reanalysis.NoMembers",
                        "No members found for the specified IDs",
                        ErrorType.NotFound
                    )
                );
            }

            var memberTechnologies = members
                .SelectMany(m => m.Technologies.Select(t => t.Technology.Name))
                .Distinct()
                .ToList();

            // Convert to TeamMemberGenerated format
            var membersData = new List<TeamMemberGenerated>();
            foreach (EmployeeProfile employee in members)
            {
                int calculatedSfia = await _sfiaCalculator.CalculateAverageSfiaForRequirements(
                    employee.Id,
                    memberTechnologies,
                    cancellationToken
                );

                var memberData = new TeamMemberGenerated(
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
                membersData.Add(memberData);
            }

            var roles = members
                .Select(m => m.Specialization)
                .Distinct()
                .Select(role => new TeamRoleRequest(role, "Any"))
                .ToList();

            int avgSfiaLevel = (int)Math.Round(membersData.Average(m => m.SfiaLevel));

            var weights = new WeightCriteria(20, 20, 15, 15, 10, 10, 10);

            Result<AiServiceResponse> response = await _teamService.ReanalyzeTeam(
                command.TeamId,
                roles,
                memberTechnologies,
                avgSfiaLevel,
                membersData.Count,
                membersData,
                command.LeaderId,
                weights,
                cancellationToken
            );

            return response;
        }
        catch (Exception ex)
        {
            return Result.Failure<AiServiceResponse>(
                new Error("Teams.Reanalysis.Failed", ex.Message, ErrorType.Failure)
            );
        }
    }
}
