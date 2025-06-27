namespace Application.DTOs;

public sealed record ConsentSettingsDto(
    bool TeamMatchingAnalysis,
    DateTime LastUpdated,
    string Version
);

public sealed record UpdateConsentRequestDto(
    bool TeamMatchingAnalysis
);

public sealed record DataExportDto(
    EmployeeProfileGeneralInfoDto PersonalInfo,
    List<EmployeeTechnologyDto> Technologies,
    List<PersonalInterestDto> Interests,
    List<WorkExperienceDto> WorkExperience,
    List<TeamParticipationDto> TeamParticipation,
    ConsentSettingsDto Consents,
    DateTime ExportDate,
    string DataFormat
);

public sealed record TeamParticipationDto(
    string TeamName,
    string Role,
    int SfiaLevel,
    bool IsLeader,
    List<string> Technologies
);

public sealed record DataDeletionRequestDto(List<string> DataTypes, string? Reason);

public sealed record DataDeletionResponseDto(
    string RequestId,
    DateTime ScheduledDeletionDate,
    string Message
);
