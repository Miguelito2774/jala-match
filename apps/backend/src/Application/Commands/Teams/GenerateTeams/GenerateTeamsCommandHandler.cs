using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Commands.Teams.GenerateTeams;

internal sealed class GenerateTeamsCommandHandler(
    IApplicationDbContext context,
    ITeamService teamService)
    : ICommandHandler<GenerateTeamsCommand, TeamCompositionResponse>
{
    public async Task<Result<TeamCompositionResponse>> Handle(
        GenerateTeamsCommand command,
        CancellationToken cancellationToken)
    {
        IQueryable<EmployeeProfile> membersQuery = context.EmployeeProfiles
            .Include(ep => ep.User)
            .Include(ep => ep.Technologies)
            .ThenInclude(et => et.Technology)
            .Where(ep =>
                ep.User.Role.ToString() == "Employee" &&
                ep.Availability == command.Availability &&
                ep.VerificationStatus == Domain.Entities.Enums.VerificationStatus.Approved &&
                (ep.SfiaLevelGeneral >= command.SfiaLevel ||
                 ep.Technologies.Any(t =>
                     command.Technologies.Contains(t.Technology.Name) &&
                     t.SfiaLevel >= command.SfiaLevel)));

        // Primero obtenemos los perfiles de la base de datos
        List<EmployeeProfile> employeeProfiles = await membersQuery.ToListAsync(cancellationToken);
        
        // Luego hacemos la proyección en memoria
        var membersData = employeeProfiles
            .Select(ep => new TeamMemberData(
                ep.Id,
                string.Concat(ep.FirstName, " ", ep.LastName),
                ep.User.Role.ToString(),
                ep.Technologies.Select(t => t.Technology.Name).ToList(),
                ep.SfiaLevelGeneral ?? 0,
                ep.Mbti,
                JsonSerializer.Deserialize<List<string>>(ep.PersonalInterests) ?? new List<string>()
            ))
            .ToList();

        return await teamService.GenerateTeams(
            new TeamGenerationRequest(
                command.Roles,
                command.Technologies,
                command.SfiaLevel,
                command.Availability,
                membersData,
                command.CriteriaWeights));
    }
}
