using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Queries.Teams.GetTeamReviews;

public sealed record GetTeamReviewsQuery(Guid TeamId) : IQuery<List<PsychologistReviewDto>>;
