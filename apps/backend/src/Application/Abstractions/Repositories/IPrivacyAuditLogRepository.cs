using Domain.Entities.Privacy;

namespace Application.Abstractions.Repositories;

public interface IPrivacyAuditLogRepository
{
    Task<PrivacyAuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<PrivacyAuditLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(PrivacyAuditLog auditLog, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
