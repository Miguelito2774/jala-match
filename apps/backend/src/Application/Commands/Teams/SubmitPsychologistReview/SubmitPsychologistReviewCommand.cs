using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.SubmitPsychologistReview;

public sealed record SubmitPsychologistReviewCommand(
    Guid TeamId,
    Guid PsychologistId,
    string Comment,
    string Status
) : ICommand<PsychologistReviewDto>;
