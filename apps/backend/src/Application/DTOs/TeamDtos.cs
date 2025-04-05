namespace Application.DTOs;

public sealed record TeamCompositionResponse(
    List<TeamComposition> Teams);
public sealed record TeamComposition(
    string Name,
    List<string> Members,
    double TeamCompatibility,
    Dictionary<string, double> MemberCompatibilities,
    string Justification);

public sealed record TeamMemberCompatibilityResponse(
    double CompatibilityScore,
    string Justification);

public sealed record TeamGenerationRequest(
    List<string> Roles,
    List<string> Technologies,
    int SfiaLevel,
    bool Availability,
    List<TeamMemberData> MembersData,
    Dictionary<string, int> CriteriaWeights);
public sealed record TeamMemberData(
    Guid Id,
    string Name,
    string Role,
    List<string> Technologies,
    int SfiaLevel,
    string? Mbti,
    List<string> Interests);

public sealed record TeamCreationRequest(
    string Name,
    Guid CreatorId,
    List<string> RequiredTechnologies,
    List<Guid> MemberIds);
public sealed record TeamCompatibilityRequest(
    List<Guid> TeamMemberIds,
    TeamMemberData NewMember);
public sealed record GenerateTeamsRequest(
    List<string> Roles,
    List<string> Technologies,
    int SfiaLevel,
    bool Availability,
    int TechnicalWeight,
    int PsychologicalWeight,
    int InterestsWeight);

public sealed record CreateTeamsRequest(
    string Name,
    Guid CreatorId,
    List<string> RequiredTechnologies,
    List<Guid> MemberIds);
    
    
