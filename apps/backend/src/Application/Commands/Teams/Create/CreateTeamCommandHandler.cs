using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Teams;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Commands.Teams.Create;

internal sealed class CreateTeamCommandHandler(
    IApplicationDbContext context) 
    : ICommandHandler<CreateTeamCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTeamCommand command, 
        CancellationToken cancellationToken)
    {
        User? creator = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.CreatorId, cancellationToken);
        
        if (creator is null)
        {
            return Result.Failure<Guid>(TeamErrors.CreatorNotFound(command.CreatorId));
        }

        int existingMembersCount = await context.EmployeeProfiles
            .Where(ep => command.MemberIds.Contains(ep.Id))
            .CountAsync(cancellationToken);
        
        if (existingMembersCount != command.MemberIds.Count)
        {
            return Result.Failure<Guid>(TeamErrors.InvalidMembers);
        }

        var team = new Team
        {
            Name = command.Name,
            CreatorId = command.CreatorId,
            Creator = creator,
            RequiredTechnologies = JsonSerializer.Serialize(command.RequiredTechnologies),
            Members = JsonSerializer.Serialize(command.MemberIds),
            AiAnalysis = "{}",
            CompatibilityScore = null, 
            IsActive = true
        };

        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);

        return team.Id;
    }
}

