using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.DTOs;
using Domain.Entities.Teams;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetTeamReviews;

internal sealed class GetTeamReviewsQueryHandler
    : IQueryHandler<GetTeamReviewsQuery, List<PsychologistReviewDto>>
{
    private readonly IPsychologistReviewRepository _reviewRepository;

    public GetTeamReviewsQueryHandler(IPsychologistReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<List<PsychologistReviewDto>>> Handle(
        GetTeamReviewsQuery query,
        CancellationToken cancellationToken
    )
    {
        List<PsychologistReview> reviews = await _reviewRepository.GetByTeamIdAsync(
            query.TeamId,
            cancellationToken
        );

        var dtos = reviews
            .OrderByDescending(r => r.ReviewedAt ?? r.CreatedAt)
            .Select(r => new PsychologistReviewDto
            {
                ReviewId = r.Id,
                TeamId = r.TeamId,
                PsychologistId = r.PsychologistId,
                PsychologistEmail = r.Psychologist?.Email ?? string.Empty,
                Comment = r.Comment,
                Status = r.Status.ToString(),
                ReviewedAt = r.ReviewedAt,
                CreatedAt = r.CreatedAt,
            })
            .ToList();

        return Result.Success(dtos);
    }
}
