using Domain.Entities.Teams;

namespace Application.Abstractions.Repositories;

public interface ITeamRepository
{
    Task AddAsync(Team team, CancellationToken ct);
    Task UpdateAsync(Team team, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<Team?> GetByIdAsync(Guid id, CancellationToken ct);
    Task DeleteAsync(Team team, CancellationToken ct);
    Task<List<Team>> GetAllAsync(CancellationToken ct);
    Task<List<Team>> GetByCreatorIdAsync(Guid creatorId, CancellationToken ct);
}
