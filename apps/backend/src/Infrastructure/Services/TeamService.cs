using System.Text;
using System.Text.Json;
using Application.Abstractions.Services;
using Application.DTOs;
using Domain.Entities.Technologies;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Infrastructure.Services;

public class TeamService : ITeamService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TeamService> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TeamService(
        IHttpClientFactory httpClientFactory,
        ILogger<TeamService> logger
    )
    {
        _httpClient = httpClientFactory.CreateClient("AIService");
        _logger = logger;
    }

    public async Task<Result<AiServiceResponse>> GenerateTeams(Guid creatorId,
        List<TeamRequirements> requirements,
        int sfiaLevel,
        int teamSize,
        List<string> technologies,
        WeightCriteria weights,
        CancellationToken cancellationToken,
        bool availability = true)
    {
        try
        {
            var request = new
            {
                CreatorId = creatorId,
                Requirements = requirements,
                Technologies = technologies,
                SfiaLevel = sfiaLevel,
                TeamSize = teamSize,
                Weights = new 
                {
                    weights.SfiaWeight,
                    weights.TechnicalWeight,
                    weights.PsychologicalWeight,
                    weights.InterestsWeight,
                    weights.ExperienceWeight,
                    weights.LanguageWeight,
                    weights.TimezoneWeight,
                },
                Availability = availability
            };

            string requestJson = JsonSerializer.Serialize(request, _jsonOptions);
            _logger.LogInformation(
                "Request payload for team generation: {RequestPayload}",
                requestJson
            );

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to AI Service for team generation");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(
                    "/generate-teams",
                    content,
                    cancellationToken
                );
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(
                    httpEx,
                    "HTTP request failed while calling AI Service: {Message}",
                    httpEx.Message
                );
                return Result.Failure<AiServiceResponse>(
                    new Error(
                        "Teams.Generation.HttpRequestFailed",
                        $"Failed to connect to AI Service: {httpEx.Message}",
                        ErrorType.Failure
                    )
                );
            }

            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation(
                "Received response from AI Service with status code {StatusCode}",
                response.StatusCode
            );
            _logger.LogDebug("Response content: {Content}", responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "AI Service returned error: {StatusCode} - {Content}",
                    response.StatusCode,
                    responseContent
                );
                return Result.Failure<AiServiceResponse>(
                    new Error(
                        "Teams.Generation.Failed",
                        $"AI Service responded with {response.StatusCode}: {responseContent}",
                        ErrorType.Failure
                    )
                );
            }

            try
            {
                AiServiceResponse aiResponse = JsonSerializer.Deserialize<AiServiceResponse>(
                    responseContent,
                    _jsonOptions
                );

                if (aiResponse == null)
                {
                    _logger.LogWarning("Failed to deserialize AI Service response");
                    return Result.Failure<AiServiceResponse>(
                        new Error(
                            "Teams.Generation.DeserializationFailed",
                            "Failed to deserialize response from AI Service",
                            ErrorType.Failure
                        )
                    );
                }

                return aiResponse;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(
                    jsonEx,
                    "Failed to deserialize response: {Message}",
                    jsonEx.Message
                );
                return Result.Failure<AiServiceResponse>(
                    new Error(
                        "Teams.Generation.DeserializationFailed",
                        $"Failed to parse AI Service response: {jsonEx.Message}",
                        ErrorType.Failure
                    )
                );
            }
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<AiServiceResponse>(
                new Error(
                    "Teams.Generation.Canceled",
                    "The operation was canceled",
                    ErrorType.Failure
                )
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while generating teams: {Message}", ex.Message);
            return Result.Failure<AiServiceResponse>(
                new Error(
                    "Teams.Generation.Failed",
                    $"Unexpected exception: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }
}
