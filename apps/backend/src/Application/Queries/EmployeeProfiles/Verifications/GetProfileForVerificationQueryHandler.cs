using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.Verifications;

public sealed class GetProfileForVerificationQueryHandler
    : IQueryHandler<GetProfileForVerificationQuery, ProfileForVerificationDto>
{
    private readonly IApplicationDbContext _context;

    public GetProfileForVerificationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ProfileForVerificationDto>> Handle(
        GetProfileForVerificationQuery request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context
            .EmployeeProfiles.Where(ep =>
                ep.Id == request.EmployeeProfileId
                && ep.VerificationStatus == VerificationStatus.Pending
                && ep.Verifications.Any(v => v.Status == VerificationStatus.Pending)
            )
            .Include(ep => ep.User)
            .Include(ep => ep.SpecializedRoles)
            .ThenInclude(esr => esr.SpecializedRole)
            .ThenInclude(sr => sr.TechnicalArea)
            .Include(ep => ep.WorkExperiences)
            .Include(ep => ep.Technologies)
            .ThenInclude(et => et.Technology)
            .ThenInclude(t => t.Category)
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            return Result.Failure<ProfileForVerificationDto>(
                new Error(
                    "ProfileVerification.NotFound",
                    "Profile not found or not pending verification",
                    ErrorType.NotFound
                )
            );
        }

        var dto = new ProfileForVerificationDto
        {
            EmployeeProfileId = profile.Id,
            UserId = profile.UserId,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Email = profile.User.Email,
            Country = profile.Country,
            Timezone = profile.Timezone,
            SfiaLevelGeneral = profile.SfiaLevelGeneral,
            Mbti = profile.Mbti,
            RequestedAt = profile.Verifications
                .Where(v => v.Status == VerificationStatus.Pending)
                .OrderByDescending(v => v.RequestedAt)
                .Select(v => v.RequestedAt)
                .FirstOrDefault(),
            SpecializedRoles = profile
                .SpecializedRoles.Select(esr => new SpecializedRoleForVerificationDto
                {
                    RoleName = esr.SpecializedRole.Name,
                    TechnicalAreaName = esr.SpecializedRole.TechnicalArea.Name,
                    Level = esr.Level,
                    YearsExperience = esr.YearsExperience,
                })
                .ToList(),
            WorkExperiences = profile
                .WorkExperiences.Select(we => new WorkExperienceSummaryDto
                {
                    ProjectName = we.ProjectName,
                    Description = we.Description,
                    StartDate = we.StartDate,
                    EndDate = we.EndDate,
                    MainTechnologies = we.Tools.Take(5).ToList(),
                    DurationMonths = we.EndDate.HasValue
                        ? (we.EndDate.Value.Year - we.StartDate.Year) * 12
                            + (we.EndDate.Value.Month - we.StartDate.Month)
                        : (DateOnly.FromDateTime(DateTime.UtcNow).Year - we.StartDate.Year) * 12
                            + (DateOnly.FromDateTime(DateTime.UtcNow).Month - we.StartDate.Month),
                })
                .ToList(),
            Technologies = profile
                .Technologies.Select(et => new TechnologyForVerificationDto
                {
                    TechnologyName = et.Technology.Name,
                    CategoryName = et.Technology.Category.Name,
                    SfiaLevel = et.SfiaLevel,
                    YearsExperience = et.YearsExperience,
                })
                .ToList(),
            TotalYearsExperience =
                profile.WorkExperiences.Sum(we =>
                    we.EndDate.HasValue
                        ? (we.EndDate.Value.Year - we.StartDate.Year) * 12
                            + (we.EndDate.Value.Month - we.StartDate.Month)
                        : (DateOnly.FromDateTime(DateTime.UtcNow).Year - we.StartDate.Year) * 12
                            + (DateOnly.FromDateTime(DateTime.UtcNow).Month - we.StartDate.Month)
                ) / 12,
            TotalProjects = profile.WorkExperiences.Count,
        };

        return Result.Success(dto);
    }
}
