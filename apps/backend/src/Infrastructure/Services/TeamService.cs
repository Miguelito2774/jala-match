using System.Text;
using System.Text.Json;
using Application.Abstractions.Services;
using Application.DTOs;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Infrastructure.Services;

public class TeamService : ITeamService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TeamService> _logger;
    private readonly ISfiaCalculatorService _sfiaCalculator;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TeamService(
        IHttpClientFactory httpClientFactory,
        ILogger<TeamService> logger,
        ISfiaCalculatorService sfiaCalculator
    )
    {
        _httpClient = httpClientFactory.CreateClient("AIService");
        _logger = logger;
        _sfiaCalculator = sfiaCalculator;
    }

    public async Task<Result<AiServiceResponse>> GenerateTeams(
        List<TeamRoleRequest> roles,
        List<string> technologies,
        int sfiaLevel,
        int teamSize,
        List<TeamMemberGenerated> membersData,
        WeightCriteria weights,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var roleStrings = roles.Select(r => r.Role).ToList();

            var request = new
            {
                roles = roleStrings,
                technologies,
                sfia_level = sfiaLevel,
                team_size = teamSize,
                members_data = membersData,
                technical_weight = weights.TechnicalWeight,
                psychological_weight = weights.PsychologicalWeight,
                interests_weight = weights.InterestsWeight,
                sfia_weight = weights.SfiaWeight,
                experience_weight = weights.ExperienceWeight,
                language_weight = weights.LanguageWeight,
                timezone_weight = weights.TimezoneWeight,
                availability = true,
            };

            _logger.LogInformation(
                "Request payload: {RequestPayload}",
                JsonSerializer.Serialize(request, _jsonOptions)
            );

            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions),
                Encoding.UTF8,
                "application/json"
            );

            _logger.LogDebug("Members data count: {Count}", membersData.Count);
            _logger.LogDebug(
                "First member (if any): {Member}",
                membersData.Count > 0 ? JsonSerializer.Serialize(membersData[0]) : "None"
            );

            _logger.LogInformation("Sending request to AI Service for team generation");

            _logger.LogDebug("Request content: {Content}", JsonSerializer.Serialize(request));

            HttpResponseMessage response = await _httpClient.PostAsync(
                "/generate-teams",
                content,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Response content: {Content}", responseContent);
            _logger.LogInformation("Received response from AI Service");

            AiServiceResponse aiResponse =
                JsonSerializer.Deserialize<AiServiceResponse>(responseContent, _jsonOptions)
                ?? new AiServiceResponse
                {
                    Teams = new List<AiTeamGenereted>(),
                    Recommended_Leader = new AiRecommendedLeader(),
                    Team_Analysis = new AiTeamAnalysis(),
                    Compatibility_Score = 0,
                };

            return aiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate teams using AI Service");
            return Result.Failure<AiServiceResponse>(
                new Error(
                    "Teams.Generation.Failed",
                    $"AI Service error: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }

    public async Task<Result<TeamCompatibilityResponse>> CalculateCompatibility(
        List<TeamMemberGenerated> teamMembers,
        TeamMemberGenerated newMember,
        List<string> requiredTechnologies,
        CancellationToken cancellationToken
    )
    {
        try
        {
            int calculatedSfia = await _sfiaCalculator.CalculateAverageSfiaForRequirements(
                newMember.Id,
                requiredTechnologies,
                cancellationToken
            );

            newMember = newMember with { SfiaLevel = calculatedSfia };

            var request = new TeamCompatibilityRequest(
                teamMembers.Select(m => m.Id).ToList(),
                newMember
            );

            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions),
                Encoding.UTF8,
                "application/json"
            );

            _logger.LogInformation("Sending request to AI Service for compatibility calculation");

            HttpResponseMessage response = await _httpClient.PostAsync(
                "/calculate-compatibility",
                content,
                cancellationToken
            );
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Received compatibility response from AI Service");

            return Result.Success(
                JsonSerializer.Deserialize<TeamCompatibilityResponse>(responseContent, _jsonOptions)
            )!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to calculate compatibility using AI Service");
            return Result.Failure<TeamCompatibilityResponse>(
                new Error(
                    "Teams.Generation.Failed",
                    $"AI Service error: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }
}
