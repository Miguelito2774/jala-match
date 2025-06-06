using Domain.Entities.Enums;

namespace Application.DTOs;

// DTOs para el listado de solicitudes pendientes
public record PendingVerificationDto
{
    public Guid EmployeeProfileId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string EmployeeEmail { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }
    public string Country { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
    public int SfiaLevelGeneral { get; init; }
    public List<string> SpecializedRoles { get; init; } = new();
    public int YearsExperienceTotal { get; init; }
}

public record PendingVerificationsResponse
{
    public List<PendingVerificationDto> PendingVerifications { get; init; } = new();
    public int TotalCount { get; init; }
}

public record ProfileForVerificationDto
{
    public Guid EmployeeProfileId { get; init; }
    public Guid UserId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
    public int SfiaLevelGeneral { get; init; }
    public string Mbti { get; init; } = string.Empty;
    public DateTime RequestedAt { get; init; }

    // Roles especializados
    public List<SpecializedRoleForVerificationDto> SpecializedRoles { get; init; } = new();

    // Experiencia laboral resumida
    public List<WorkExperienceSummaryDto> WorkExperiences { get; init; } = new();

    // Tecnologías
    public List<TechnologyForVerificationDto> Technologies { get; init; } = new();

    // Estadísticas calculadas
    public int TotalYearsExperience { get; init; }
    public int TotalProjects { get; init; }
}

public record SpecializedRoleForVerificationDto
{
    public string RoleName { get; init; } = string.Empty;
    public string TechnicalAreaName { get; init; } = string.Empty;
    public ExperienceLevel Level { get; init; }
    public int YearsExperience { get; init; }
}

public record WorkExperienceSummaryDto
{
    public string ProjectName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public List<string> MainTechnologies { get; init; } = new();
    public int DurationMonths { get; init; }
}

public record TechnologyForVerificationDto
{
    public string TechnologyName { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public int SfiaLevel { get; init; }
    public decimal YearsExperience { get; init; }
}

// DTOs para aprobar/rechazar
public record ApproveProfileVerificationRequest
{
    public Guid EmployeeProfileId { get; init; }
    public int? SfiaProposed { get; init; }
    public string? Notes { get; init; }
}

public record RejectProfileVerificationRequest
{
    public Guid EmployeeProfileId { get; init; }
    public required string Notes { get; init; }
}

public record VerificationDecisionResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public VerificationStatus NewStatus { get; init; }
}
