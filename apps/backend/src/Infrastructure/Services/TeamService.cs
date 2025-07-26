using System.Text;
using System.Text.Json;
using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.Commands.Teams.Create;
using Application.DTOs;
using Domain.Entities.Profiles;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;
using Microsoft.Extensions.Logging;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Infrastructure.Services;

public class TeamService : ITeamService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TeamService> _logger;
    private readonly ITeamRepository _teamRepository;
    private readonly IEmployeeProfileRepository _employeeProfileRepository;
    private readonly ITechnologyRepository _technologyRepository;
    private readonly INotificationService _notificationService;
    private readonly IImageStorageService _imageStorageService;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public TeamService(
        IHttpClientFactory httpClientFactory,
        ILogger<TeamService> logger,
        ITeamRepository teamRepository,
        IEmployeeProfileRepository employeeProfileRepository,
        ITechnologyRepository technologyRepository,
        INotificationService notificationService,
        IImageStorageService imageStorageService
    )
    {
        _httpClient = httpClientFactory.CreateClient("AIService");
        _logger = logger;
        _teamRepository = teamRepository;
        _employeeProfileRepository = employeeProfileRepository;
        _technologyRepository = technologyRepository;
        _notificationService = notificationService;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<AiServiceResponse>> GenerateTeams(
        Guid creatorId,
        List<TeamRequirements> requirements,
        int sfiaLevel,
        int teamSize,
        List<string> technologies,
        WeightCriteria weights,
        CancellationToken cancellationToken,
        bool availability = true
    )
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
                Availability = availability,
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

                // Enrich recommended_members with profile pictures
                if (aiResponse.recommended_members?.Any() == true)
                {
                    _logger.LogInformation(
                        "Enriching {Count} recommended members with profile pictures",
                        aiResponse.recommended_members.Count
                    );

                    foreach (
                        AiRecommendedMember recommendedMember in aiResponse.recommended_members
                    )
                    {
                        try
                        {
                            EmployeeProfile? employeeProfile =
                                await _employeeProfileRepository.GetByIdAsync(recommendedMember.Id);
                            if (employeeProfile?.User?.ProfilePicturePublicId != null)
                            {
                                recommendedMember.ProfilePictureUrl =
                                    _imageStorageService.GenerateImageUrl(
                                        employeeProfile.User.ProfilePicturePublicId
                                    );
                                _logger.LogDebug(
                                    "Generated profile picture URL for recommended member {MemberId}: {Url}",
                                    recommendedMember.Id,
                                    recommendedMember.ProfilePictureUrl
                                );
                            }
                            else
                            {
                                _logger.LogDebug(
                                    "No profile picture found for recommended member {MemberId}",
                                    recommendedMember.Id
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(
                                ex,
                                "Failed to enrich profile picture for recommended member {MemberId}",
                                recommendedMember.Id
                            );
                            // Continue with other members even if one fails
                        }
                    }
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

    public async Task<Result<TeamResponse>> CreateTeam(
        CreateTeamCommand command,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (command.Members.All(m => m.EmployeeProfileId != command.LeaderId))
            {
                return Result.Failure<TeamResponse>(
                    new Error(
                        "Team.InvalidLeader",
                        "El líder debe ser parte de los miembros del equipo",
                        ErrorType.Validation
                    )
                );
            }

            var missingTechs = new List<string>();
            foreach (string techName in command.RequiredTechnologies.Distinct())
            {
                if (!await _technologyRepository.ExistsByNameAsync(techName))
                {
                    missingTechs.Add(techName);
                }
            }

            if (missingTechs.Any())
            {
                return Result.Failure<TeamResponse>(
                    new Error(
                        "Team.MissingTechnologies",
                        $"Las siguientes tecnologías no están registradas: {string.Join(", ", missingTechs)}",
                        ErrorType.Validation
                    )
                );
            }

            var team = new Team
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                CreatorId = command.CreatorId,
                AiAnalysis = JsonSerializer.Serialize(command.Analysis, _jsonOptions),
                WeightCriteria = JsonSerializer.Serialize(command.Weights, _jsonOptions),
                CompatibilityScore = command.CompatibilityScore,
                IsActive = true,
            };

            foreach (string techName in command.RequiredTechnologies.Distinct())
            {
                Technology? technology = await _technologyRepository.GetByNameAsync(techName);
                team.RequiredTechnologies.Add(new TeamRequiredTechnology(team.Id, technology!.Id));
            }

            foreach (TeamMemberDto memberDto in command.Members)
            {
                EmployeeProfile? profile = await _employeeProfileRepository.GetByIdAsync(
                    memberDto.EmployeeProfileId
                );
                if (profile is null)
                {
                    continue;
                }

                team.Members.Add(
                    new TeamMember
                    {
                        EmployeeProfileId = memberDto.EmployeeProfileId,
                        Name = $"{profile.FirstName} {profile.LastName}",
                        Role = memberDto.Role,
                        SfiaLevel = memberDto.SfiaLevel,
                        IsLeader = memberDto.EmployeeProfileId == command.LeaderId,
                    }
                );
            }

            await _teamRepository.AddAsync(team, cancellationToken);
            await _teamRepository.SaveChangesAsync(cancellationToken);

            // Fire and forget: Send notifications in background without blocking response
            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        EmployeeProfile? creatorProfile =
                            await _employeeProfileRepository.GetByIdAsync(command.CreatorId);
                        string managerEmail = creatorProfile?.User?.Email ?? "No disponible";

                        foreach (TeamMember member in team.Members)
                        {
                            EmployeeProfile? memberProfile =
                                await _employeeProfileRepository.GetByIdAsync(
                                    member.EmployeeProfileId
                                );

                            if (memberProfile?.User?.Email != null)
                            {
                                string memberName =
                                    $"{memberProfile.FirstName} {memberProfile.LastName}";

                                await _notificationService.SendTeamMemberAddedNotificationAsync(
                                    memberProfile.User.Email,
                                    memberName,
                                    team.Name,
                                    managerEmail,
                                    CancellationToken.None // Use CancellationToken.None for background task
                                );
                            }
                        }
                    }
                    catch (Exception notificationEx)
                    {
                        _logger.LogError(
                            notificationEx,
                            "Error sending team creation notifications for team {TeamId}",
                            team.Id
                        );
                        // Don't fail team creation if notifications fail
                    }
                },
                CancellationToken.None
            );

            return new TeamResponse
            {
                TeamId = team.Id,
                Name = team.Name,
                CreatorId = team.CreatorId,
                CompatibilityScore = team.CompatibilityScore,
                Members = team
                    .Members.Select(m => new TeamMemberDto(
                        m.EmployeeProfileId,
                        m.Name,
                        m.Role,
                        m.SfiaLevel,
                        m.IsLeader,
                        _imageStorageService.GenerateImageUrl(
                            m.EmployeeProfile?.User?.ProfilePicturePublicId
                        )
                    ))
                    .ToList(),
                RequiredTechnologies = command.RequiredTechnologies,
                Analysis = command.Analysis,
                Weights = command.Weights,
                IsActive = team.IsActive,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear equipo");
            return Result.Failure<TeamResponse>(
                new Error(
                    "Team.CreateError",
                    $"Error al crear equipo: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }

    public async Task<Result<List<TeamMemberRecommendation>>> FindTeamMembers(
        FindTeamMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Team? team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
            if (team == null)
            {
                return Result.Failure<List<TeamMemberRecommendation>>(
                    new Error(
                        "Team.NotFound",
                        $"No se encontró el equipo con ID {request.TeamId}",
                        ErrorType.NotFound
                    )
                );
            }

            var httpRequest = new
            {
                TeamId = request.TeamId.ToString(),
                request.Role,
                request.Area,
                request.Level,
                request.Technologies,
            };

            string requestJson = JsonSerializer.Serialize(httpRequest, _jsonOptions);
            _logger.LogInformation(
                "Request payload for team member search: {RequestPayload}",
                requestJson
            );

            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to AI Service for team member search");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(
                    "/find-team-members",
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
                return Result.Failure<List<TeamMemberRecommendation>>(
                    new Error(
                        "Teams.FindMembers.HttpRequestFailed",
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
                return Result.Failure<List<TeamMemberRecommendation>>(
                    new Error(
                        "Teams.FindMembers.Failed",
                        $"AI Service responded with {response.StatusCode}: {responseContent}",
                        ErrorType.Failure
                    )
                );
            }

            try
            {
                List<TeamMemberRecommendation>? recommendations = JsonSerializer.Deserialize<
                    List<TeamMemberRecommendation>
                >(responseContent, _jsonOptions);

                if (recommendations == null)
                {
                    _logger.LogWarning("Failed to deserialize AI Service response");
                    return Result.Failure<List<TeamMemberRecommendation>>(
                        new Error(
                            "Teams.FindMembers.DeserializationFailed",
                            "Failed to deserialize response from AI Service",
                            ErrorType.Failure
                        )
                    );
                }

                // Enrich recommendations with profile picture URLs
                foreach (TeamMemberRecommendation recommendation in recommendations)
                {
                    EmployeeProfile? employeeProfile =
                        await _employeeProfileRepository.GetByIdWithUserAsync(
                            recommendation.EmployeeId,
                            cancellationToken
                        );

                    if (employeeProfile?.User != null)
                    {
                        recommendation.ProfilePictureUrl = _imageStorageService.GenerateImageUrl(
                            employeeProfile.User.ProfilePicturePublicId
                        );
                    }
                }

                return recommendations;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(
                    jsonEx,
                    "Failed to deserialize response: {Message}",
                    jsonEx.Message
                );
                return Result.Failure<List<TeamMemberRecommendation>>(
                    new Error(
                        "Teams.FindMembers.DeserializationFailed",
                        $"Failed to parse AI Service response: {jsonEx.Message}",
                        ErrorType.Failure
                    )
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while finding team members: {Message}",
                ex.Message
            );
            return Result.Failure<List<TeamMemberRecommendation>>(
                new Error(
                    "Teams.FindMembers.Failed",
                    $"Unexpected exception: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }

    public async Task<Result<TeamResponse>> AddTeamMember(
        TeamMemberUpdateRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Team? team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
            if (team == null)
            {
                return Result.Failure<TeamResponse>(
                    new Error(
                        "Team.NotFound",
                        $"No se encontró el equipo con ID {request.TeamId}",
                        ErrorType.NotFound
                    )
                );
            }

            // Keep track of which members are actually new
            var newMemberIds = new List<Guid>();

            foreach (TeamMemberDto member in request.Members)
            {
                EmployeeProfile? employeeProfile = await _employeeProfileRepository.GetByIdAsync(
                    member.EmployeeProfileId
                );

                if (employeeProfile == null)
                {
                    return Result.Failure<TeamResponse>(
                        new Error(
                            "Employee.NotFound",
                            $"No se encontró el empleado con ID {member.EmployeeProfileId}",
                            ErrorType.NotFound
                        )
                    );
                }

                // Check if member is already in team BEFORE adding
                if (team.Members.Any(m => m.EmployeeProfileId == member.EmployeeProfileId))
                {
                    continue;
                }

                // This member is new, add to tracking list
                newMemberIds.Add(member.EmployeeProfileId);

                var newMember = new TeamMember
                {
                    Id = Guid.NewGuid(),
                    TeamId = request.TeamId,
                    EmployeeProfileId = member.EmployeeProfileId,
                    Role = member.Role,
                    SfiaLevel = member.SfiaLevel,
                    Name = member.Name,
                    IsLeader = false,
                };

                team.Members.Add(newMember);
            }

            await _teamRepository.UpdateAsync(team, cancellationToken);
            await _teamRepository.SaveChangesAsync(cancellationToken);

            // Fire and forget: Send notifications for team member additions in background
            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        foreach (TeamMemberDto memberDto in request.Members)
                        {
                            // Only process notifications for actually new members
                            if (!newMemberIds.Contains(memberDto.EmployeeProfileId))
                            {
                                continue;
                            }

                            EmployeeProfile? profile =
                                await _employeeProfileRepository.GetByIdAsync(
                                    memberDto.EmployeeProfileId
                                );

                            if (profile?.User.Email != null)
                            {
                                // Send notification to the new member
                                await _notificationService.SendTeamMemberAddedNotificationAsync(
                                    profile.User.Email,
                                    $"{profile.FirstName} {profile.LastName}",
                                    team.Name,
                                    team.Creator?.Email ?? "manager@jalamatch.com",
                                    CancellationToken.None
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error sending team member addition notifications for team {TeamId}",
                            team.Id
                        );
                        // Don't fail the operation just because notifications failed
                    }
                },
                CancellationToken.None
            );

            // Update availability for new members only
            foreach (Guid newMemberId in newMemberIds)
            {
                EmployeeProfile? ep = await _employeeProfileRepository.GetByIdAsync(newMemberId);
                if (ep != null && ep.TeamMemberships.Count >= 2 && ep.Availability)
                {
                    ep.Availability = false;
                    await _employeeProfileRepository.UpdateAsync(ep);
                }
            }
            await _teamRepository.SaveChangesAsync(cancellationToken);

            return Result.Success(
                new TeamResponse
                {
                    TeamId = team.Id,
                    Name = team.Name,
                    CreatorId = team.CreatorId,
                    CompatibilityScore = team.CompatibilityScore,
                    Members = team
                        .Members.Select(m => new TeamMemberDto(
                            m.EmployeeProfileId,
                            m.Name,
                            m.Role,
                            m.SfiaLevel,
                            m.IsLeader,
                            _imageStorageService.GenerateImageUrl(
                                m.EmployeeProfile?.User?.ProfilePicturePublicId
                            )
                        ))
                        .ToList(),
                    RequiredTechnologies = team
                        .RequiredTechnologies.Select(rt => rt.Technology.Name)
                        .ToList(),
                    Analysis = JsonSerializer.Deserialize<AiTeamAnalysis?>(
                        team.AiAnalysis ?? "{}",
                        _jsonOptions
                    ),
                    Weights = JsonSerializer.Deserialize<WeightCriteria>(
                        team.WeightCriteria ?? "{}",
                        _jsonOptions
                    ),
                    IsActive = team.IsActive,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al añadir miembros al equipo");
            return Result.Failure<TeamResponse>(
                new Error(
                    "Team.AddMemberError",
                    $"Error al añadir miembros al equipo: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }

    public async Task<Result<TeamResponse>> RemoveTeamMember(
        RemoveTeamMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Team? team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken);
            if (team == null)
            {
                return Result.Failure<TeamResponse>(
                    new Error(
                        "Team.NotFound",
                        $"No se encontró el equipo con ID {request.TeamId}",
                        ErrorType.NotFound
                    )
                );
            }

            TeamMember? memberToRemove = team.Members.FirstOrDefault(m =>
                m.EmployeeProfileId == request.EmployeeProfileId
            );

            if (memberToRemove == null)
            {
                return Result.Failure<TeamResponse>(
                    new Error(
                        "TeamMember.NotFound",
                        $"No se encontró el miembro con ID {request.EmployeeProfileId} en el equipo",
                        ErrorType.NotFound
                    )
                );
            }

            if (memberToRemove.IsLeader)
            {
                return Result.Failure<TeamResponse>(
                    new Error(
                        "TeamMember.CannotRemoveLeader",
                        "No se puede eliminar al líder del equipo. Primero debe asignar un nuevo líder",
                        ErrorType.Validation
                    )
                );
            }

            team.Members.Remove(memberToRemove);

            await _teamRepository.UpdateAsync(team, cancellationToken);
            await _teamRepository.SaveChangesAsync(cancellationToken);

            // Fire and forget: Send notifications for team member removal in background
            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        EmployeeProfile? profile = await _employeeProfileRepository.GetByIdAsync(
                            request.EmployeeProfileId
                        );

                        if (profile?.User.Email != null)
                        {
                            // Send notification to the removed member
                            await _notificationService.SendTeamMemberRemovedNotificationAsync(
                                profile.User.Email,
                                $"{profile.FirstName} {profile.LastName}",
                                team.Name,
                                team.Creator?.Email ?? "manager@jalamatch.com",
                                CancellationToken.None
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error sending team member removal notification for team {TeamId}",
                            team.Id
                        );
                    }
                },
                CancellationToken.None
            );

            return BuildTeamResponse(team);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar miembro del equipo");
            return Result.Failure<TeamResponse>(
                new Error(
                    "Team.RemoveMemberError",
                    $"Error al eliminar miembro del equipo: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }

    public async Task<Result<(TeamResponse SourceTeam, TeamResponse TargetTeam)>> MoveTeamMember(
        MoveTeamMemberRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            Team? sourceTeam = await _teamRepository.GetByIdAsync(
                request.SourceTeamId,
                cancellationToken
            );
            Team? targetTeam = await _teamRepository.GetByIdAsync(
                request.TargetTeamId,
                cancellationToken
            );

            if (sourceTeam == null)
            {
                return Result.Failure<(TeamResponse, TeamResponse)>(
                    new Error(
                        "SourceTeam.NotFound",
                        $"No se encontró el equipo origen con ID {request.SourceTeamId}",
                        ErrorType.NotFound
                    )
                );
            }

            if (targetTeam == null)
            {
                return Result.Failure<(TeamResponse, TeamResponse)>(
                    new Error(
                        "TargetTeam.NotFound",
                        $"No se encontró el equipo destino con ID {request.TargetTeamId}",
                        ErrorType.NotFound
                    )
                );
            }

            TeamMember? memberToMove = sourceTeam.Members.FirstOrDefault(m =>
                m.EmployeeProfileId == request.EmployeeProfileId
            );

            if (memberToMove == null)
            {
                return Result.Failure<(TeamResponse, TeamResponse)>(
                    new Error(
                        "TeamMember.NotFound",
                        $"No se encontró el miembro con ID {request.EmployeeProfileId} en el equipo origen",
                        ErrorType.NotFound
                    )
                );
            }

            if (memberToMove.IsLeader)
            {
                return Result.Failure<(TeamResponse, TeamResponse)>(
                    new Error(
                        "TeamMember.CannotMoveLeader",
                        "No se puede mover al líder del equipo. Primero debe asignar un nuevo líder",
                        ErrorType.Validation
                    )
                );
            }

            if (targetTeam.Members.Any(m => m.EmployeeProfileId == request.EmployeeProfileId))
            {
                return Result.Failure<(TeamResponse, TeamResponse)>(
                    new Error(
                        "TeamMember.AlreadyExists",
                        "El miembro ya pertenece al equipo destino",
                        ErrorType.Validation
                    )
                );
            }

            var newMember = new TeamMember
            {
                Id = Guid.NewGuid(),
                TeamId = request.TargetTeamId,
                EmployeeProfileId = memberToMove.EmployeeProfileId,
                Name = memberToMove.Name,
                Role = memberToMove.Role,
                SfiaLevel = memberToMove.SfiaLevel,
                IsLeader = false,
            };

            sourceTeam.Members.Remove(memberToMove);
            targetTeam.Members.Add(newMember);

            // Actualizar ambos equipos
            await _teamRepository.UpdateAsync(sourceTeam, cancellationToken);
            await _teamRepository.UpdateAsync(targetTeam, cancellationToken);
            await _teamRepository.SaveChangesAsync(cancellationToken);

            // Fire and forget: Send notifications for team member move in background
            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        EmployeeProfile? profile = await _employeeProfileRepository.GetByIdAsync(
                            request.EmployeeProfileId
                        );

                        if (profile?.User.Email != null)
                        {
                            // Send notification to the moved member
                            await _notificationService.SendTeamMemberMovedNotificationAsync(
                                profile.User.Email,
                                $"{profile.FirstName} {profile.LastName}",
                                sourceTeam.Name,
                                targetTeam.Name,
                                targetTeam.Creator?.Email ?? "manager@jalamatch.com",
                                CancellationToken.None
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error sending team member move notifications for teams {SourceTeamId} -> {TargetTeamId}",
                            sourceTeam.Id,
                            targetTeam.Id
                        );
                    }
                },
                CancellationToken.None
            );

            Result<TeamResponse> sourceTeamResponse = BuildTeamResponse(sourceTeam);
            Result<TeamResponse> targetTeamResponse = BuildTeamResponse(targetTeam);

            return Result.Success((sourceTeamResponse.Value, targetTeamResponse.Value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mover miembro entre equipos");
            return Result.Failure<(TeamResponse, TeamResponse)>(
                new Error(
                    "Team.MoveMemberError",
                    $"Error al mover miembro entre equipos: {ex.Message}",
                    ErrorType.Failure
                )
            );
        }
    }

    private Result<TeamResponse> BuildTeamResponse(Team team)
    {
        return Result.Success(
            new TeamResponse
            {
                TeamId = team.Id,
                Name = team.Name,
                CreatorId = team.CreatorId,
                CompatibilityScore = team.CompatibilityScore,
                Members = team
                    .Members.Select(m => new TeamMemberDto(
                        m.EmployeeProfileId,
                        m.Name,
                        m.Role,
                        m.SfiaLevel,
                        m.IsLeader,
                        _imageStorageService.GenerateImageUrl(
                            m.EmployeeProfile?.User?.ProfilePicturePublicId
                        )
                    ))
                    .ToList(),
                RequiredTechnologies = team
                    .RequiredTechnologies.Select(rt => rt.Technology.Name)
                    .ToList(),
                Analysis = JsonSerializer.Deserialize<AiTeamAnalysis?>(
                    team.AiAnalysis ?? "{}",
                    _jsonOptions
                ),
                Weights = JsonSerializer.Deserialize<WeightCriteria>(
                    team.WeightCriteria ?? "{}",
                    _jsonOptions
                ),
                IsActive = team.IsActive,
            }
        );
    }
}
