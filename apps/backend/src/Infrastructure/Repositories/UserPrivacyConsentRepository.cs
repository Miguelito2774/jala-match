using Application.Abstractions.Repositories;
using Domain.Entities.Privacy;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserPrivacyConsentRepository : IUserPrivacyConsentRepository
{
    private readonly ApplicationDbContext _context;

    public UserPrivacyConsentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserPrivacyConsent?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.UserPrivacyConsents.FirstOrDefaultAsync(
            c => c.UserId == userId,
            cancellationToken
        );
    }

    public async Task<UserPrivacyConsent?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        return await _context.UserPrivacyConsents.FirstOrDefaultAsync(
            c => c.Id == id,
            cancellationToken
        );
    }

    public async Task AddAsync(
        UserPrivacyConsent consent,
        CancellationToken cancellationToken = default
    )
    {
        await _context.UserPrivacyConsents.AddAsync(consent, cancellationToken);
    }

    public Task UpdateAsync(
        UserPrivacyConsent consent,
        CancellationToken cancellationToken = default
    )
    {
        _context.UserPrivacyConsents.Update(consent);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        UserPrivacyConsent consent,
        CancellationToken cancellationToken = default
    )
    {
        _context.UserPrivacyConsents.Remove(consent);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
