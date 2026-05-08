using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetTeamsPendingReview;

internal sealed class GetTeamsPendingReviewQueryHandler
    : IQueryHandler<GetTeamsPendingReviewQuery, List<TeamWithReviewStatusResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IImageStorageService _imageStorageService;

    public GetTeamsPendingReviewQueryHandler(
        IApplicationDbContext context,
        IImageStorageService imageStorageService
    )
    {
        _context = context;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<List<TeamWithReviewStatusResponse>>> Handle(
        GetTeamsPendingReviewQuery query,
        CancellationToken cancellationToken
    )
    {
        List<Team> teams = await _context.Teams
            .Where(t => t.PsychologistReviewRequested && t.IsActive)
            .Include(t => t.Members)
                .ThenInclude(m => m.EmployeeProfile!)
                    .ThenInclude(ep => ep.User)
            .Include(t => t.Creator)
            .Include(t => t.RequiredTechnologies)
                .ThenInclude(rt => rt.Technology)
            .Include(t => t.PsychologistReviews)
                .ThenInclude(r => r.Psychologist)
            .OrderByDescending(t => t.Id)
            .ToListAsync(cancellationToken);

        var result = teams.Select(team =>
        {
            Domain.Entities.Teams.PsychologistReview? latestReview = team.PsychologistReviews
                .OrderByDescending(r => r.ReviewedAt ?? r.CreatedAt)
                .FirstOrDefault();

            return new TeamWithReviewStatusResponse
            {
                Team = MapToTeamResponse(team),
                PsychologistReviewRequested = team.PsychologistReviewRequested,
                LatestReview = latestReview is null ? null : MapToReviewDto(latestReview),
            };
        }).ToList();

        return Result.Success(result);
    }

    private TeamResponse MapToTeamResponse(Team team) =>
        new()
        {
            TeamId = team.Id,
            Name = team.Name,
            CreatorId = team.CreatorId,
            CompatibilityScore = team.CompatibilityScore,
            Members = team.Members
                .Select(m => new TeamMemberDto(
                    m.EmployeeProfileId,
                    m.Name,
                    m.Role,
                    m.SfiaLevel,
                    m.IsLeader,
                    _imageStorageService.GenerateImageUrl(m.EmployeeProfile?.User?.ProfilePicturePublicId)
                ))
                .ToList(),
            RequiredTechnologies = team.RequiredTechnologies.Select(rt => rt.Technology.Name).ToList(),
            Analysis = team.AiAnalysis != null
                ? JsonSerializer.Deserialize<AiTeamAnalysis>(team.AiAnalysis)
                : null,
            Weights = team.WeightCriteria != null
                ? JsonSerializer.Deserialize<WeightCriteria>(team.WeightCriteria)
                : null,
            IsActive = team.IsActive,
            PsychologistReviewRequested = team.PsychologistReviewRequested,
        };

    private static PsychologistReviewDto MapToReviewDto(Domain.Entities.Teams.PsychologistReview r) =>
        new()
        {
            ReviewId = r.Id,
            TeamId = r.TeamId,
            PsychologistId = r.PsychologistId,
            PsychologistEmail = r.Psychologist?.Email ?? string.Empty,
            Comment = r.Comment,
            Status = r.Status.ToString(),
            ReviewedAt = r.ReviewedAt,
            CreatedAt = r.CreatedAt,
        };
}
