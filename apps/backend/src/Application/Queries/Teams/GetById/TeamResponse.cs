namespace Application.Queries.Teams.GetById;

public sealed record TeamResponse(
    Guid Id,
    string Name,
    Guid CreatorId,
    List<string> RequiredTechnologies,
    List<TeamMemberDto> Members,
    double CompatibilityScore,
    bool IsActive,
    string AiAnalysis
);

public sealed record TeamMemberDto(
    Guid Id,
    string Name,
    string Role,
    List<string> Technologies,
    int SfiaLevel,
    string? Mbti,
    List<string> Interests
);
