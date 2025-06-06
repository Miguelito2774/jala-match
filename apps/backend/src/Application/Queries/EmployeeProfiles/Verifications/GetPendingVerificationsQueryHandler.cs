using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.Verifications;

public sealed class GetPendingVerificationsQueryHandler
    : IQueryHandler<GetPendingVerificationsQuery, PendingVerificationsResponse>
{
    private readonly IApplicationDbContext _context;

    public GetPendingVerificationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PendingVerificationsResponse>> Handle(
        GetPendingVerificationsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Only get profiles that have explicit verification requests in pending status
        // This ensures we only show profiles where employees actually clicked "Request Verification"
        IIncludableQueryable<EmployeeProfile, IEnumerable<ProfileVerification>> query = _context
            .EmployeeProfiles.Where(ep => 
                ep.VerificationStatus == VerificationStatus.Pending &&
                ep.Verifications.Any(v => v.Status == VerificationStatus.Pending))
            .Include(ep => ep.User)
            .Include(ep => ep.SpecializedRoles)
            .ThenInclude(esr => esr.SpecializedRole)
            .Include(ep => ep.WorkExperiences)
            .Include(ep => ep.Verifications.Where(v => v.Status == VerificationStatus.Pending));

        int totalCount = await query.CountAsync(cancellationToken);

        List<PendingVerificationDto> profiles = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(ep => new PendingVerificationDto
            {
                EmployeeProfileId = ep.Id,
                EmployeeName = $"{ep.FirstName} {ep.LastName}",
                EmployeeEmail = ep.User.Email,
                Country = ep.Country,
                Timezone = ep.Timezone,
                SfiaLevelGeneral = ep.SfiaLevelGeneral,
                SpecializedRoles = ep
                    .SpecializedRoles.Select(esr => $"{esr.SpecializedRole.Name} ({esr.Level})")
                    .ToList(),
                RequestedAt = ep
                    .Verifications.Where(v => v.Status == VerificationStatus.Pending)
                    .OrderByDescending(v => v.RequestedAt)
                    .Select(v => v.RequestedAt)
                    .FirstOrDefault(),
                YearsExperienceTotal = ep.SpecializedRoles.Sum(sr => sr.YearsExperience),
            })
            .ToListAsync(cancellationToken);

        var response = new PendingVerificationsResponse
        {
            PendingVerifications = profiles,
            TotalCount = totalCount,
        };

        return Result.Success(response);
    }
}
