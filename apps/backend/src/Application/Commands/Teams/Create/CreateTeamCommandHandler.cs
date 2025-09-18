using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Microsoft.Extensions.Logging;
using SharedKernel.Results;

namespace Application.Commands.Teams.Create;

internal sealed class CreateTeamCommandHandler : ICommandHandler<CreateTeamCommand, TeamResponse>
{
    private readonly ITeamService _teamService;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CreateTeamCommandHandler> _logger;

    public CreateTeamCommandHandler(
        ITeamService teamService,
        IEmployeeProfileRepository employeeProfileRepository,
        INotificationService notificationService,
        ILogger<CreateTeamCommandHandler> logger
    )
    {
        _teamService = teamService;
        _employeeProfileRepository = employeeProfileRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<TeamResponse>> Handle(
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<TeamResponse> result = await _teamService.CreateTeam(command, cancellationToken);
        
        if (result.IsFailure)
        {
            return result;
        }

        // Send notifications after successful team creation
        _ = Task.Run(async () =>
        {
            try
            {
                // Get creator email for manager information
                EmployeeProfile? creatorProfile = await _employeeProfileRepository.GetByIdAsync(command.CreatorId);
                string managerEmail = creatorProfile?.User?.Email ?? "manager@jalamatch.com";
                
                // Send notifications to all team members
                foreach (TeamMemberDto member in command.Members)
                {
                    EmployeeProfile? memberProfile = await _employeeProfileRepository.GetByIdAsync(member.EmployeeProfileId);
                    
                    if (memberProfile?.User?.Email != null)
                    {
                        string memberName = $"{memberProfile.FirstName} {memberProfile.LastName}";
                        
                        await _notificationService.SendTeamMemberAddedNotificationAsync(
                            memberProfile.User.Email,
                            memberName,
                            result.Value.Name,
                            managerEmail,
                            CancellationToken.None
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending team creation notifications for team {TeamName}", result.Value.Name);
            }
        }, CancellationToken.None);

        return result;
    }
}
