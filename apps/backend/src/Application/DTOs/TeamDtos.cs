using System.Text.Json.Serialization;
using Domain.Entities.Enums;
using Domain.Entities.Teams;
using Domain.Entities.Technologies;

namespace Application.DTOs;

public sealed record WeightCriteria(
    int SfiaWeight,
    int TechnicalWeight,
    int PsychologicalWeight,
    int ExperienceWeight,
    int LanguageWeight,
    int InterestsWeight,
    int TimezoneWeight
);

public sealed record TeamRoleRequest(string Role, string Level);

public sealed record GenerateTeamsRequest(
    Guid CreatorId,
    int TeamSize,
    List<TeamRequirements> Requirements,
    List<string> Technologies,
    int SfiaLevel,
    WeightCriteria Weights
);

public sealed record CreateTeamsRequest(
    string Name,
    Guid CreatorId,
    List<TeamMemberDto> Members,
    Guid LeaderId,
    AiTeamAnalysis Analysis,
    int CompatibilityScore,
    WeightCriteria Weights,
    List<string> RequiredTechnologies
);

public sealed record TeamMemberGenerated(
    Guid Id,
    string Name,
    string Role,
    List<string> Technologies,
    int SfiaLevel,
    string? Mbti,
    List<string> Interests,
    string? Timezone,
    string? Country
);

public class AiTeamMemberGenerated
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public int Sfia_Level { get; set; }
}

public class AiTeamGenereted
{
    public Guid Team_Id { get; set; }
    public List<AiTeamMemberGenerated> Members { get; set; }
}

public class AiRecommendedLeader
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Rationale { get; set; }
}

public class AiTeamAnalysis
{
    public List<string> Strengths { get; set; }
    public List<string> Weaknesses { get; set; }
    public string Compatibility { get; set; }
}

public class AiServiceResponse
{
    public List<AiTeamGenereted> Teams { get; set; }
    public AiRecommendedLeader Recommended_Leader { get; set; }
    public AiTeamAnalysis Team_Analysis { get; set; }
    public int Compatibility_Score { get; set; }

    public List<AiRecommendedMember> recommended_members { get; set; }
}

public class AiRecommendedMember
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Compatibility_Score { get; set; }
    public string Analysis { get; set; }
    public List<string> Potential_Conflicts { get; set; }
    public string Team_Impact { get; set; }
}

public class ReanalyzeTeamRequest
{
    public Guid TeamId { get; set; }
    public List<Guid> MemberIds { get; set; }
    public Guid LeaderId { get; set; }
}

public record TeamRequirements(
    string Role,
    string Area,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] ExperienceLevel Level
);

public class TeamResponse
{
    public Guid TeamId { get; set; }
    public string Name { get; set; }
    public Guid CreatorId { get; set; }
    public double CompatibilityScore { get; set; }
    public List<TeamMemberDto> Members { get; set; } = new();
    public List<string>? RequiredTechnologies { get; set; } = new();

    public AiTeamAnalysis? Analysis { get; set; }

    public WeightCriteria? Weights { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public record TeamMemberDto(
    Guid EmployeeProfileId,
    string Name,
    string Role,
    int SfiaLevel,
    bool? IsLeader
);

public class FindTeamMemberRequest
{
    public Guid TeamId { get; set; }
    public string Role { get; set; }
    public string Area { get; set; }
    public string Level { get; set; }

    public List<string> Technologies { get; set; } = new();
}

public class TeamMemberRecommendation
{
    [JsonPropertyName("employee_id")]
    public Guid EmployeeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("area")]
    public string Area { get; set; }

    [JsonPropertyName("technologies")]
    public List<string> Technologies { get; set; }

    [JsonPropertyName("sfia_level")]
    public int SfiaLevel { get; set; }

    [JsonPropertyName("compatibility_score")]
    public int CompatibilityScore { get; set; }

    [JsonPropertyName("analysis")]
    public string Analysis { get; set; }
}

public class TeamMemberUpdateRequest
{
    public Guid TeamId { get; set; }

    public List<TeamMemberDto> Members { get; set; } = new();
}

public class TeamWithUpdatedAnalysisResponse
{
    public TeamResponse Team { get; set; }
    public int CompatibilityScore { get; set; }
    public List<string>? UpdatedStrengths { get; set; }
    public List<string>? UpdatedWeaknesses { get; set; }
    public string? DetailedAnalysis { get; set; }
    public List<string>? Recommendations { get; set; }
}

public class AiReanalysisResponse
{
    public int NewCompatibilityScore { get; set; }
    public List<string> UpdatedStrengths { get; set; }
    public List<string> UpdatedWeaknesses { get; set; }
    public string DetailedAnalysis { get; set; }
    public List<string> Recommendations { get; set; }
}

public class RemoveTeamMemberRequest
{
    public Guid TeamId { get; set; }
    public Guid EmployeeProfileId { get; set; }
}

public class MoveTeamMemberRequest
{
    public Guid SourceTeamId { get; set; }
    public Guid TargetTeamId { get; set; }
    public Guid EmployeeProfileId { get; set; }
}

public class AvailableTeamDto
{
    public Guid TeamId { get; set; }
    public string Name { get; set; }
    public int CurrentMemberCount { get; set; }
    public bool HasMember { get; set; }
    public string CreatorName { get; set; }
}

public class GetAvailableTeamsForMemberRequest
{
    public Guid EmployeeProfileId { get; set; }
    public Guid? ExcludeTeamId { get; set; }
}
