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

        // Send notifications before deleting the team
        try
        {
            var memberEmails = new List<string>();
            var memberNames = new List<string>();

            foreach (TeamMember member in team.Members)
            {
                Domain.Entities.Profiles.EmployeeProfile? profile =
                    await _employeeProfileRepository.GetByIdAsync(member.EmployeeProfileId);

                if (profile?.User.Email != null)
                {
                    memberEmails.Add(profile.User.Email);
                    memberNames.Add($"{profile.FirstName} {profile.LastName}");
                }
            }

            if (memberEmails.Count > 0)
            {
                string managerEmail = team.Creator?.Email ?? "No disponible";

                await _notificationService.SendTeamDeletedNotificationAsync(
                    memberEmails,
                    memberNames,
                    team.Name,
                    managerEmail,
                    cancellationToken
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error sending team deletion notifications for team {TeamId}",
                team.Id
            );
        }

        await _teamRepository.DeleteAsync(team, cancellationToken);
        await _teamRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
