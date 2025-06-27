using Application.Abstractions.Repositories;
using Domain.Entities.Privacy;
using Domain.Entities.Privacy.Enums;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DataDeletionRequestRepository : IDataDeletionRequestRepository
{
    private readonly ApplicationDbContext _context;

    public DataDeletionRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DataDeletionOrder?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .DataDeletionRequests.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<DataDeletionOrder?> GetPendingByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.DataDeletionRequests.FirstOrDefaultAsync(
            r => r.UserId == userId && r.Status == DataDeletionStatus.Pending,
            cancellationToken
        );
    }

    public async Task<List<DataDeletionOrder>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .DataDeletionRequests.Where(r => r.UserId == userId)
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<DataDeletionOrder>> GetPendingRequestsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .DataDeletionRequests.Include(r => r.User)
            .Where(r =>
                r.Status == DataDeletionStatus.Pending && r.ScheduledDeletionDate <= DateTime.UtcNow
            )
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        DataDeletionOrder order,
        CancellationToken cancellationToken = default
    )
    {
        await _context.DataDeletionRequests.AddAsync(order, cancellationToken);
    }

    public Task UpdateAsync(DataDeletionOrder order, CancellationToken cancellationToken = default)
    {
        _context.DataDeletionRequests.Update(order);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(DataDeletionOrder order, CancellationToken cancellationToken = default)
    {
        _context.DataDeletionRequests.Remove(order);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
