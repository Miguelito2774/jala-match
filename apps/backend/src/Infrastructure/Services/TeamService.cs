using System.Net.Http.Json;
using Application.Abstractions.Services;
using Application.DTOs;
using Application.Queries.Teams.GetTeamCompatibility;
using Microsoft.Extensions.Configuration;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Infrastructure.Services;

public class TeamService : ITeamService
{
    private readonly HttpClient _httpClient;

    public TeamService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["AIService:BaseUrl"]!);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<Result<TeamCompositionResponse>> GenerateTeams(TeamGenerationRequest request)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/generate-teams", request);
            return await ProcessResponse<TeamCompositionResponse>(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<TeamCompositionResponse>(
                Error.Failure("AI.ServiceError", ex.Message));
        }
    }

    public async Task<Result<TeamCompatibilityResponse>> CalculateCompatibility(TeamCompatibilityRequest request)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/calculate-compatibility", request);
            return await ProcessResponse<TeamCompatibilityResponse>(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<TeamCompatibilityResponse>(
                Error.Failure("AI.ServiceError", ex.Message));
        }
    }

    private static async Task<Result<T>> ProcessResponse<T>(HttpResponseMessage response) where T : class
    {
        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<T>(
                Error.Failure("AI.ServiceError", await response.Content.ReadAsStringAsync()));
        }

        return await response.Content.ReadFromJsonAsync<T>() ?? 
               Result.Failure<T>(Error.Failure("AI.InvalidResponse", "Invalid response from AI service"));
    }
}
