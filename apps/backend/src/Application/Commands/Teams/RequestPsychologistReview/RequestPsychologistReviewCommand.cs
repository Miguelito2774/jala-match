using Application.Abstractions.Messaging;
using Application.DTOs;

namespace Application.Commands.Teams.RequestPsychologistReview;

public sealed record RequestPsychologistReviewCommand(Guid TeamId, Guid RequestedByUserId) : ICommand<TeamResponse>;
