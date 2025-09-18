using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;

namespace Application.Commands.Teams.AddTeamMember;

public class AddTeamMemberCommandHandler : ICommandHandler<AddTeamMemberCommand, TeamResponse>
{
    private readonly ITeamService _teamService;
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AddTeamMemberCommandHandler> _logger;

    public AddTeamMemberCommandHandler(
        ITeamService teamService,
        ITeamRepository teamRepository,
        IEmployeeProfileRepository employeeProfileRepository,
        INotificationService notificationService,
        ILogger<AddTeamMemberCommandHandler> logger
    )
    {
        _teamService = teamService;
        _teamRepository = teamRepository;
        _employeeProfileRepository = employeeProfileRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<TeamResponse>> Handle(
        AddTeamMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = new TeamMemberUpdateRequest
        {
            TeamId = command.TeamId,
            Members = command.Members,
        };

        // Get current team to identify new members
        Team? team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
        if (team == null)
        {
            return Result.Failure<TeamResponse>(
                new SharedKernel.Errors.Error("Team.NotFound", "Team not found", SharedKernel.Errors.ErrorType.NotFound)
            );
        }

        // Identify which members are actually new
        var newMemberIds = new List<Guid>();
        foreach (TeamMemberDto member in command.Members)
        {
            if (!team.Members.Any(m => m.EmployeeProfileId == member.EmployeeProfileId))
            {
                newMemberIds.Add(member.EmployeeProfileId);
            }
        }

        Result<TeamResponse> result = await _teamService.AddTeamMember(request, cancellationToken);
        
        if (result.IsFailure)
        {
            return result;
        }

        // Send notifications to new team members only
        _ = Task.Run(async () =>
        {
            try
            {
                // Get manager email
                string managerEmail = team.Creator?.Email ?? "manager@jalamatch.com";
                
                // Send notifications only to actually new members
                foreach (Guid newMemberId in newMemberIds)
                {
                    EmployeeProfile? memberProfile = await _employeeProfileRepository.GetByIdAsync(newMemberId);
                    
                    if (memberProfile?.User?.Email != null)
                    {
                        string memberName = $"{memberProfile.FirstName} {memberProfile.LastName}";
                        
                        await _notificationService.SendTeamMemberAddedNotificationAsync(
                            memberProfile.User.Email,
                            memberName,
                            team.Name,
                            managerEmail,
                            CancellationToken.None
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending add team member notifications for team {TeamId}", command.TeamId);
            }
        }, CancellationToken.None);

        return result;
    }
}
