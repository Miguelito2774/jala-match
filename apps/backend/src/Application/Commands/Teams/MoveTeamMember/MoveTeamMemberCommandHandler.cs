using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;

namespace Application.Commands.Teams.MoveTeamMember;

internal sealed class MoveTeamMemberCommandHandler
    : ICommandHandler<MoveTeamMemberCommand, (TeamResponse SourceTeam, TeamResponse TargetTeam)>
{
    private readonly ITeamService _teamService;
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<MoveTeamMemberCommandHandler> _logger;

    public MoveTeamMemberCommandHandler(
        ITeamService teamService,
        ITeamRepository teamRepository,
        IEmployeeProfileRepository employeeProfileRepository,
        INotificationService notificationService,
        ILogger<MoveTeamMemberCommandHandler> logger
    )
    {
        _teamService = teamService;
        _teamRepository = teamRepository;
        _employeeProfileRepository = employeeProfileRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<(TeamResponse SourceTeam, TeamResponse TargetTeam)>> Handle(
        MoveTeamMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var request = new MoveTeamMemberRequest
        {
            SourceTeamId = command.SourceTeamId,
            TargetTeamId = command.TargetTeamId,
            EmployeeProfileId = command.EmployeeProfileId,
        };

        // Get teams and member info before move for notifications
        Team? sourceTeam = await _teamRepository.GetByIdAsync(command.SourceTeamId, cancellationToken);
        Team? targetTeam = await _teamRepository.GetByIdAsync(command.TargetTeamId, cancellationToken);
        
        if (sourceTeam == null || targetTeam == null)
        {
            return Result.Failure<(TeamResponse, TeamResponse)>(
                new SharedKernel.Errors.Error("Team.NotFound", "Source or target team not found", SharedKernel.Errors.ErrorType.NotFound)
            );
        }

        EmployeeProfile? memberProfile = await _employeeProfileRepository.GetByIdAsync(command.EmployeeProfileId);
        string memberEmail = memberProfile?.User?.Email ?? "";
        string memberName = memberProfile != null ? $"{memberProfile.FirstName} {memberProfile.LastName}" : "";
        string sourceTeamName = sourceTeam.Name;
        string targetTeamName = targetTeam.Name;
        string managerEmail = targetTeam.Creator?.Email ?? "manager@jalamatch.com";

        Result<(TeamResponse SourceTeam, TeamResponse TargetTeam)> result = await _teamService.MoveTeamMember(request, cancellationToken);
        
        if (result.IsFailure)
        {
            return result;
        }

        // Send notification to moved team member
        _ = Task.Run(async () =>
        {
            try
            {
                if (!string.IsNullOrEmpty(memberEmail))
                {
                    await _notificationService.SendTeamMemberMovedNotificationAsync(
                        memberEmail,
                        memberName,
                        sourceTeamName,
                        targetTeamName,
                        managerEmail,
                        CancellationToken.None
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending move team member notification for member {MemberId} from team {SourceTeamId} to team {TargetTeamId}", 
                    command.EmployeeProfileId, command.SourceTeamId, command.TargetTeamId);
            }
        }, CancellationToken.None);

        return result;
    }
}
