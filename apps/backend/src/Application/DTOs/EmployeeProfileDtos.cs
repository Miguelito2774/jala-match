using Domain.Entities.Enums;

namespace Application.DTOs;

public record EmployeeProfileGeneralInfoDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public bool Availability { get; init; }
    public string Country { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
    public Uri? ProfilePictureUrl { get; init; }
    public List<EmployeeLanguageDto> Languages { get; init; } = new();
    public List<EmployeeSpecializedRoleDto> SpecializedRoles { get; init; } = new();
}

public record EmployeeLanguageDto
{
    public Guid Id { get; init; }
    public string Language { get; init; } = string.Empty;
    public string Proficiency { get; init; } = string.Empty;
}

// DTOs para respuestas de Roles Especializados
public record EmployeeSpecializedRoleDto
{
    public Guid Id { get; init; }
    public Guid SpecializedRoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public string TechnicalAreaName { get; init; } = string.Empty;
    public Domain.Entities.Enums.ExperienceLevel Level { get; init; }
    public int YearsExperience { get; init; }
}

// DTOs para Perfil Técnico
public record EmployeeProfileTechnicalDto
{
    public int SfiaLevelGeneral { get; init; }
    public string Mbti { get; init; } = string.Empty;

    public decimal YearsExperience { get; init; }
    public List<EmployeeTechnologyDto> Technologies { get; init; } = new();
    public List<EmployeeSpecializedRoleDto> SpecializedRoles { get; init; } = new();
}

public record EmployeeTechnologyDto
{
    public Guid Id { get; init; }
    public Guid TechnologyId { get; init; }
    public string TechnologyName { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;

    public decimal YearsExperience { get; init; }
    public int SfiaLevel { get; init; }
    public string Version { get; init; } = string.Empty;
}

// DTOs para Experiencia Laboral
public record WorkExperienceDto
{
    public Guid Id { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<string> Tools { get; init; } = new();
    public List<string> ThirdParties { get; init; } = new();
    public List<string> Frameworks { get; init; } = new();
    public string? VersionControl { get; init; }
    public string? ProjectManagement { get; init; }
    public List<string> Responsibilities { get; init; } = new();
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}

// DTOs para Intereses Personales
public record PersonalInterestDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int? SessionDurationMinutes { get; init; }
    public string? Frequency { get; init; }
    public int? InterestLevel { get; init; } // Escala Likert 1-5
}

// DTOs para respuestas completas
public record EmployeeProfileCompleteDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public EmployeeProfileGeneralInfoDto GeneralInfo { get; init; } = new();
    public EmployeeProfileTechnicalDto TechnicalProfile { get; init; } = new();
    public List<WorkExperienceDto> WorkExperiences { get; init; } = new();
    public List<PersonalInterestDto> PersonalInterests { get; init; } = new();
    public VerificationStatus VerificationStatus { get; init; }
    public string? VerificationNotes { get; init; }
    public bool HasVerificationRequests { get; init; }
}

// DTOs para operaciones de creación/actualización
public record CreateEmployeeProfileRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
    public int SfiaLevelGeneral { get; init; }
    public string Mbti { get; init; } = string.Empty;
}

public record UpdateEmployeeProfileGeneralInfoRequest
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Timezone { get; init; } = string.Empty;
}

public record UpdateEmployeeProfileTechnicalRequest
{
    public int SfiaLevelGeneral { get; init; }
    public string Mbti { get; init; } = string.Empty;
}

public record AddEmployeeTechnologyRequest
{
    public Guid TechnologyId { get; init; }
    public int SfiaLevel { get; init; }
    public decimal YearsExperience { get; init; }
    public string Version { get; init; } = string.Empty;
}

public record AddEmployeeSpecializedRoleRequest
{
    public Guid SpecializedRoleId { get; init; }
    public ExperienceLevel Level { get; init; }
    public int YearsExperience { get; init; }
}

public record AddEmployeeLanguageRequest
{
    public string Language { get; init; } = string.Empty;
    public string Proficiency { get; init; } = string.Empty;
}

public record UpdateEmployeeLanguageRequest
{
    public string Language { get; init; } = string.Empty;
    public string Proficiency { get; init; } = string.Empty;
}

public record AddWorkExperienceRequest
{
    public string ProjectName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<string> Tools { get; init; } = new();
    public List<string> ThirdParties { get; init; } = new();
    public List<string> Frameworks { get; init; } = new();
    public string? VersionControl { get; init; }
    public string? ProjectManagement { get; init; }
    public List<string> Responsibilities { get; init; } = new();
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}

public record AddPersonalInterestRequest
{
    public string Name { get; init; } = string.Empty;
    public int? SessionDurationMinutes { get; init; }
    public string? Frequency { get; init; }
    public int? InterestLevel { get; init; }
}

// DTOs para respuestas de disponibilidad
public record AvailableRolesAndAreasResponse
{
    public List<RoleWithAreasDto> Roles { get; init; } = new();
}

public record RoleWithAreasDto
{
    public string Role { get; init; } = string.Empty;
    public List<string> Areas { get; init; } = new();
    public List<string> Levels { get; init; } = new();
}

// DTOs para mapeo de roles especializados
public record SpecializedRolesMappingResponse
{
    public List<SpecializedRoleMappingDto> SpecializedRoles { get; init; } = new();
}

public record SpecializedRoleMappingDto
{
    public Guid Id { get; init; }
    public string RoleName { get; init; } = string.Empty;
    public string TechnicalArea { get; init; } = string.Empty;
}

// DTOs para importación JSON
public record ImportTechnologiesRequest
{
    public List<TechnologyImportDto> Technologies { get; init; } = new();
}

public record TechnologyImportDto
{
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int SfiaLevel { get; init; }
    public decimal YearsExperience { get; init; }
    public string Version { get; init; } = string.Empty;
}

public record ImportWorkExperiencesRequest
{
    public List<WorkExperienceImportDto> WorkExperiences { get; init; } = new();
}

public record WorkExperienceImportDto
{
    public string ProjectName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<string> Tools { get; init; } = new();
    public List<string> ThirdParties { get; init; } = new();
    public List<string> Frameworks { get; init; } = new();
    public string? VersionControl { get; init; }
    public string? ProjectManagement { get; init; }
    public List<string> Responsibilities { get; init; } = new();
    public string StartDate { get; init; } = string.Empty; // Se parseará a DateOnly
    public string? EndDate { get; init; } // Se parseará a DateOnly
}

public record ImportPersonalInterestsRequest
{
    public List<PersonalInterestImportDto> PersonalInterests { get; init; } = new();
}

public record PersonalInterestImportDto
{
    public string Name { get; init; } = string.Empty;
    public int? SessionDurationMinutes { get; init; }
    public string? Frequency { get; init; }
    public int? InterestLevel { get; init; }
}

public record UpdateEmployeeTechnologyRequest
{
    public int SfiaLevel { get; init; }
    public decimal YearsExperience { get; init; }
    public string Version { get; init; } = string.Empty;
}

public record UpdatePersonalInterestRequest
{
    public string Name { get; init; } = string.Empty;
    public int? SessionDurationMinutes { get; init; }
    public string? Frequency { get; init; }
    public int? InterestLevel { get; init; }
}

public record UpdateWorkExperienceRequest
{
    public string ProjectName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public List<string> Tools { get; init; } = new();
    public List<string> ThirdParties { get; init; } = new();
    public List<string> Frameworks { get; init; } = new();
    public string? VersionControl { get; init; }
    public string? ProjectManagement { get; init; }
    public List<string> Responsibilities { get; init; } = new();
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}

// DTOs para operaciones de Roles Especializados

public record UpdateEmployeeSpecializedRoleRequest
{
    public ExperienceLevel Level { get; init; }
    public int YearsExperience { get; init; }
}

// DTOs para Verificaciones de Perfil
public record ProfileVerificationDto
{
    public Guid Id { get; init; }
    public int SfiaProposed { get; init; }
    public VerificationStatus Status { get; init; }
    public string? Notes { get; init; }
    public DateTime RequestedAt { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public string? ReviewerName { get; init; }
    public string? ReviewerEmail { get; init; }
}

public record ProfileVerificationStatusDto
{
    public VerificationStatus Status { get; init; }
    public string? Notes { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public bool HasPendingRequest { get; init; }
}
