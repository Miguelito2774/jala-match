using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;

namespace Application.Commands.Teams.RemoveTeamMember;

internal sealed class RemoveTeamMemberCommandHandler
    : ICommandHandler<RemoveTeamMemberCommand, TeamResponse>
{
    private readonly ITeamService _teamService;
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<RemoveTeamMemberCommandHandler> _logger;

    public RemoveTeamMemberCommandHandler(
        ITeamService teamService,
        ITeamRepository teamRepository,
        IEmployeeProfileRepository employeeProfileRepository,
        INotificationService notificationService,
        ILogger<RemoveTeamMemberCommandHandler> logger
    )
    {
        _teamService = teamService;
        _teamRepository = teamRepository;
        _employeeProfileRepository = employeeProfileRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<TeamResponse>> Handle(
        RemoveTeamMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = new RemoveTeamMemberRequest
        {
            TeamId = command.TeamId,
            EmployeeProfileId = command.EmployeeProfileId,
        };

        // Get team and member info before removal for notifications
        Team? team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
        if (team == null)
        {
            return Result.Failure<TeamResponse>(
                new SharedKernel.Errors.Error("Team.NotFound", "Team not found", SharedKernel.Errors.ErrorType.NotFound)
            );
        }

        EmployeeProfile? memberProfile = await _employeeProfileRepository.GetByIdAsync(command.EmployeeProfileId);
        string memberEmail = memberProfile?.User?.Email ?? "";
        string memberName = memberProfile != null ? $"{memberProfile.FirstName} {memberProfile.LastName}" : "";
        string teamName = team.Name;
        string managerEmail = team.Creator?.Email ?? "manager@jalamatch.com";

        Result<TeamResponse> result = await _teamService.RemoveTeamMember(request, cancellationToken);
        
        if (result.IsFailure)
        {
            return result;
        }

        // Send notification to removed team member
        _ = Task.Run(async () =>
        {
            try
            {
                if (!string.IsNullOrEmpty(memberEmail))
                {
                    await _notificationService.SendTeamMemberRemovedNotificationAsync(
                        memberEmail,
                        memberName,
                        teamName,
                        managerEmail,
                        CancellationToken.None
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending remove team member notification for member {MemberId} from team {TeamId}", 
                    command.EmployeeProfileId, command.TeamId);
            }
        }, CancellationToken.None);

        return result;
    }
}
