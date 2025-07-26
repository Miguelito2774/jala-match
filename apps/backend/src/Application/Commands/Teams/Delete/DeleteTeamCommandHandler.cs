using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Domain.Entities.Teams;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Teams.Delete;

internal sealed class DeleteTeamCommandHandler : ICommandHandler<DeleteTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeleteTeamCommandHandler> _logger;

    public DeleteTeamCommandHandler(
        ITeamRepository teamRepository,
        IEmployeeProfileRepository employeeProfileRepository,
        INotificationService notificationService,
        ILogger<DeleteTeamCommandHandler> logger
    )
    {
        _teamRepository = teamRepository;
        _employeeProfileRepository = employeeProfileRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

        public async Task<Result> Handle(DeleteTeamCommand command, CancellationToken cancellationToken)
    {
        Team? team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
        if (team == null)
        {
            return Result.Failure(
                new Error("Team.NotFound", "Equipo no encontrado", ErrorType.Failure)
            );
        }

        // Collect member data before deletion for background notifications
        var memberNotificationData = new List<(string Email, string Name)>();
        
        foreach (TeamMember member in team.Members)
        {
            Domain.Entities.Profiles.EmployeeProfile? profile =
                await _employeeProfileRepository.GetByIdAsync(member.EmployeeProfileId);

            if (profile?.User.Email != null)
            {
                memberNotificationData.Add((profile.User.Email, $"{profile.FirstName} {profile.LastName}"));
            }
        }

        string teamName = team.Name;
        string managerEmail = team.Creator?.Email ?? "No disponible";

        // Delete the team first (fast response)
        await _teamRepository.DeleteAsync(team, cancellationToken);
        await _teamRepository.SaveChangesAsync(cancellationToken);

        // Fire and forget: Send notifications in background without blocking response
        _ = Task.Run(async () =>
        {
            try
            {
                if (memberNotificationData.Count > 0)
                {
                    var memberEmails = memberNotificationData.Select(m => m.Email).ToList();
                    var memberNames = memberNotificationData.Select(m => m.Name).ToList();

                    await _notificationService.SendTeamDeletedNotificationAsync(
                        memberEmails,
                        memberNames,
                        teamName,
                        managerEmail,
                        CancellationToken.None
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending team deletion notifications for team {TeamName}",
                    teamName
                );
                // Don't fail the operation just because notifications failed
            }
        }, CancellationToken.None);

        return Result.Success();
    }
}
