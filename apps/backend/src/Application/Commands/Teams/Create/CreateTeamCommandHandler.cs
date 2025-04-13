using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Commands.Teams.Create;

internal sealed class CreateTeamCommandHandler(IApplicationDbContext context)
    : ICommandHandler<CreateTeamCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        User? creator = await context.Users.FirstOrDefaultAsync(
            u => u.Id == command.CreatorId,
            cancellationToken
        );

        if (creator is null)
        {
            return Result.Failure<Guid>(TeamErrors.CreatorNotFound(command.CreatorId));
        }

        int existingMembersCount = await context
            .EmployeeProfiles.Where(ep => command.MemberIds.Contains(ep.Id))
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
            MembersJson = JsonSerializer.Serialize(command.MemberIds),
            RequiredTechnologiesJson = JsonSerializer.Serialize(command.RequiredTechnologies),
            AiAnalysis = "{}",
            CompatibilityScore = 0,
            IsActive = true,
        };

        if (command.RequiredTechnologies?.Any() == true)
        {
            foreach (string techId in command.RequiredTechnologies)
            {
                var technologyId = Guid.Parse(techId);
                Technology? technology = await context.Technologies.FindAsync(
                    new object[] { technologyId },
                    cancellationToken
                );
                if (technology != null)
                {
                    team.RequiredTechnologies.Add(
                        new TeamRequiredTechnology
                        {
                            TeamId = team.Id,
                            Team = team,
                            TechnologyId = technologyId,
                            Technology = technology,
                            MinimumSfiaLevel = 3,
                            IsMandatory = true,
                        }
                    );
                }
            }
        }

        if (command.MemberIds?.Any() == true)
        {
            foreach (Guid memberId in command.MemberIds)
            {
                EmployeeProfile? profile = await context.EmployeeProfiles.FindAsync(
                    new object[] { memberId },
                    cancellationToken
                );
                if (profile != null)
                {
                    team.Members.Add(
                        new TeamMember
                        {
                            TeamId = team.Id,
                            Team = team,
                            EmployeeProfileId = memberId,
                            EmployeeProfile = profile,
                            Role = TeamMemberRole.Developer,
                            JoinedDate = DateTime.UtcNow,
                        }
                    );
                }
            }
        }

        context.Teams.Add(team);
        await context.SaveChangesAsync(cancellationToken);

        return team.Id;
    }
}
