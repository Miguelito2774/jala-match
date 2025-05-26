using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetAvailableTeamsForMember;

internal sealed class GetAvailableTeamsForMemberQueryHandler
    : IQueryHandler<GetAvailableTeamsForMemberQuery, List<AvailableTeamDto>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;

    public GetAvailableTeamsForMemberQueryHandler(
        ITeamRepository teamRepository,
        IEmployeeProfileRepository employeeProfileRepository
    )
    {
        _teamRepository = teamRepository;
        _employeeProfileRepository = employeeProfileRepository;
    }

    public async Task<Result<List<AvailableTeamDto>>> Handle(
        GetAvailableTeamsForMemberQuery query,
        CancellationToken cancellationToken
    )
    {
        try
        {
            EmployeeProfile? employee = await _employeeProfileRepository.GetByIdAsync(
                query.EmployeeProfileId
            );
            if (employee == null)
            {
                return Result.Failure<List<AvailableTeamDto>>(
                    new Error(
                        "Employee.NotFound",
                        $"No se encontró el empleado con ID {query.EmployeeProfileId}",
                        ErrorType.NotFound
                    )
                );
            }

            List<Team> allTeams = await _teamRepository.GetAllAsync(cancellationToken);
            var activeTeams = allTeams.Where(t => t.IsActive).ToList();

            if (query.ExcludeTeamId.HasValue)
            {
                activeTeams = activeTeams.Where(t => t.Id != query.ExcludeTeamId.Value).ToList();
            }

            var availableTeams = activeTeams
                .Select(team => new AvailableTeamDto
                {
                    TeamId = team.Id,
                    Name = team.Name,
                    CurrentMemberCount = team.Members.Count,
                    HasMember = team.Members.Any(m =>
                        m.EmployeeProfileId == query.EmployeeProfileId
                    ),
                    CreatorName = $"Creator: {team.CreatorId}" // Podrías mejorar esto obteniendo el nombre real del creator
                })
                .ToList();

            return Result.Success(availableTeams);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<AvailableTeamDto>>(
                new Error(
                    "Teams.GetAvailableTeams.Failed",
                    $"Error al obtener equipos disponibles: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }
}
