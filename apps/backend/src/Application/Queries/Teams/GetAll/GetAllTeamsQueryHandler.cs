using System.Text.Json;
using Application.Abstractions.Messaging;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Teams;
using SharedKernel.Results;

namespace Application.Queries.Teams.GetAll;

internal sealed class GetAllTeamsQueryHandler : IQueryHandler<GetAllTeamsQuery, List<TeamResponse>>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IImageStorageService _imageStorageService;

    public GetAllTeamsQueryHandler(ITeamRepository teamRepository, IImageStorageService imageStorageService)
    {
        _teamRepository = teamRepository;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<List<TeamResponse>>> Handle(
        GetAllTeamsQuery query,
        CancellationToken cancellationToken
    )
    {
        List<Team> teams = await _teamRepository.GetAllAsync(cancellationToken);
        var response = teams
            .Select(team => new TeamResponse
            {
                TeamId = team.Id,
                Name = team.Name,
                CreatorId = team.CreatorId,
                CompatibilityScore = team.CompatibilityScore,
                IsBlended = team.IsBlended,
                Members = team
                    .Members.Select(m => new TeamMemberDto(
                        m.EmployeeProfileId,
                        m.Name,
                        m.Role,
                        m.SfiaLevel,
                        m.IsLeader,
                        _imageStorageService.GenerateImageUrl(m.EmployeeProfile?.User?.ProfilePicturePublicId)
                    ))
                    .ToList(),
                RequiredTechnologies = team
                    .RequiredTechnologies.Select(rt => rt.Technology.Name)
                    .ToList(),
                Analysis =
                    team.AiAnalysis != null
                        ? JsonSerializer.Deserialize<AiTeamAnalysis>(team.AiAnalysis)
                        : null,
                Weights =
                    team.WeightCriteria != null
                        ? JsonSerializer.Deserialize<WeightCriteria>(team.WeightCriteria)
                        : null,
                IsActive = team.IsActive,
            })
            .ToList();

        return Result.Success(response);
    }
}
