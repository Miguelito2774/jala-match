using Application.Abstractions.Repositories;
using Domain.Entities.Privacy;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PrivacyAuditLogRepository : IPrivacyAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public PrivacyAuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PrivacyAuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .PrivacyAuditLogs
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<PrivacyAuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context
            .PrivacyAuditLogs
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PrivacyAuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.PrivacyAuditLogs.AddAsync(auditLog, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
