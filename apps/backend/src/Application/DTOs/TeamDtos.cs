using System.Text.Json.Serialization;
using Domain.Entities.Enums;
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
    List<string> RequiredTechnologies,
    List<Guid> MemberIds
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
    
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ExperienceLevel Level
);
