using Domain.Entities.Teams;

namespace Application.Abstractions.Repositories;

public interface IPsychologistReviewRepository
{
    Task AddAsync(PsychologistReview review, CancellationToken ct);
    Task UpdateAsync(PsychologistReview review, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<PsychologistReview?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<PsychologistReview>> GetByTeamIdAsync(Guid teamId, CancellationToken ct);
    Task<List<PsychologistReview>> GetPendingByPsychologistIdAsync(Guid psychologistId, CancellationToken ct);
    Task<PsychologistReview?> GetLatestByTeamIdAsync(Guid teamId, CancellationToken ct);
}
