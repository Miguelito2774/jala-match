using Domain.Entities.Privacy;

namespace Application.Abstractions.Repositories;

public interface IUserPrivacyConsentRepository
{
    Task<UserPrivacyConsent?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserPrivacyConsent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(UserPrivacyConsent consent, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserPrivacyConsent consent, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserPrivacyConsent consent, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
