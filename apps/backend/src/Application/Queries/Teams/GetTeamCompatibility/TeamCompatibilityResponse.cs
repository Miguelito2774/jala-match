namespace Application.Queries.Teams.GetTeamCompatibility;

public sealed record TeamCompatibilityResponse(
    double CompatibilityScore,
    string Justification);
