using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.GetTeamsPendingReview;

/// <summary>Returns all teams that have PsychologistReviewRequested = true,
/// enriched with the latest review if it exists.</summary>
public sealed record GetTeamsPendingReviewQuery(Guid PsychologistId) : IQuery<List<TeamWithReviewStatusResponse>>;
