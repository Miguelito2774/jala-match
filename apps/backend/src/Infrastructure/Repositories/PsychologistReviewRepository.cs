using Application.Abstractions.Repositories;
using Domain.Entities.Teams;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PsychologistReviewRepository : IPsychologistReviewRepository
{
    private readonly ApplicationDbContext _context;

    public PsychologistReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PsychologistReview review, CancellationToken ct)
    {
        await _context.PsychologistReviews.AddAsync(review, ct);
    }

    public Task UpdateAsync(PsychologistReview review, CancellationToken ct)
    {
        _context.PsychologistReviews.Update(review);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task<PsychologistReview?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.PsychologistReviews
            .Include(r => r.Psychologist)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<List<PsychologistReview>> GetByTeamIdAsync(Guid teamId, CancellationToken ct)
    {
        return await _context.PsychologistReviews
            .Include(r => r.Psychologist)
            .Where(r => r.TeamId == teamId)
            .ToListAsync(ct);
    }

    public async Task<List<PsychologistReview>> GetPendingByPsychologistIdAsync(Guid psychologistId, CancellationToken ct)
    {
        return await _context.PsychologistReviews
            .Include(r => r.Team)
            .Where(r => r.PsychologistId == psychologistId && r.Status == Domain.Entities.Enums.ReviewStatus.Pending)
            .ToListAsync(ct);
    }

    public async Task<PsychologistReview?> GetLatestByTeamIdAsync(Guid teamId, CancellationToken ct)
    {
        return await _context.PsychologistReviews
            .Include(r => r.Psychologist)
            .Where(r => r.TeamId == teamId)
            .OrderByDescending(r => r.ReviewedAt ?? r.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }
}
