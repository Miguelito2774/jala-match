using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Teams;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Commands.Teams.SubmitPsychologistReview;

internal sealed class SubmitPsychologistReviewCommandHandler
    : ICommandHandler<SubmitPsychologistReviewCommand, PsychologistReviewDto>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IPsychologistReviewRepository _reviewRepository;

    public SubmitPsychologistReviewCommandHandler(
        ITeamRepository teamRepository,
        IPsychologistReviewRepository reviewRepository
    )
    {
        _teamRepository = teamRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<PsychologistReviewDto>> Handle(
        SubmitPsychologistReviewCommand command,
        CancellationToken cancellationToken
    )
    {
        Team? team = await _teamRepository.GetByIdAsync(command.TeamId, cancellationToken);

        if (team is null)
        {
            return Result.Failure<PsychologistReviewDto>(
                new Error("Team.NotFound", "Equipo no encontrado", ErrorType.Failure)
            );
        }

        if (!team.PsychologistReviewRequested)
        {
            return Result.Failure<PsychologistReviewDto>(
                new Error(
                    "Team.ReviewNotRequested",
                    "Este equipo no tiene una solicitud de revisión psicológica activa",
                    ErrorType.Validation
                )
            );
        }

        if (!Enum.TryParse<ReviewStatus>(command.Status, out ReviewStatus reviewStatus)
            || reviewStatus == ReviewStatus.Pending)
        {
            return Result.Failure<PsychologistReviewDto>(
                new Error(
                    "Review.InvalidStatus",
                    "El estado de revisión debe ser 'Approved' o 'FlaggedConcerns'",
                    ErrorType.Validation
                )
            );
        }

        var review = new PsychologistReview
        {
            Id = Guid.NewGuid(),
            TeamId = command.TeamId,
            PsychologistId = command.PsychologistId,
            Comment = command.Comment,
            Status = reviewStatus,
            ReviewedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        var dto = new PsychologistReviewDto
        {
            ReviewId = review.Id,
            TeamId = review.TeamId,
            PsychologistId = review.PsychologistId,
            PsychologistEmail = string.Empty, // populated by query, not needed here
            Comment = review.Comment,
            Status = review.Status.ToString(),
            ReviewedAt = review.ReviewedAt,
            CreatedAt = review.CreatedAt,
        };

        return Result.Success(dto);
    }
}
