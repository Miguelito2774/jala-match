using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Domain.Entities.Teams;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Teams.Delete;

internal sealed class DeleteTeamCommandHandler : ICommandHandler<DeleteTeamCommand>
{
    private readonly ITeamRepository _teamRepository;

    public DeleteTeamCommandHandler(ITeamRepository teamRepository) =>
        _teamRepository = teamRepository;

    public async Task<Result> Handle(DeleteTeamCommand command, CancellationToken cancellationToken)
    {
        Team? team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);
        if (team == null)
        {
            return Result.Failure(
                new Error("Team.NotFound", "Equipo no encontrado", ErrorType.Failure)
            );
        }

        await _teamRepository.DeleteAsync(team, cancellationToken);
        await _teamRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
