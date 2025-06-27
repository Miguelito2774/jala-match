using Domain.Entities.Privacy;

namespace Application.Abstractions.Repositories;

public interface IDataDeletionRequestRepository
{
    Task<DataDeletionOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DataDeletionOrder?> GetPendingByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
    Task<List<DataDeletionOrder>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
    Task<List<DataDeletionOrder>> GetPendingRequestsAsync(
        CancellationToken cancellationToken = default
    );
    Task AddAsync(DataDeletionOrder order, CancellationToken cancellationToken = default);
    Task UpdateAsync(DataDeletionOrder order, CancellationToken cancellationToken = default);
    Task DeleteAsync(DataDeletionOrder order, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
