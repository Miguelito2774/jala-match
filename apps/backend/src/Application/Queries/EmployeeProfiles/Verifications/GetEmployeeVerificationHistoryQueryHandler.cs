using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Errors;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.Verifications;

public sealed class GetEmployeeVerificationHistoryQueryHandler
    : IQueryHandler<GetEmployeeVerificationHistoryQuery, List<ProfileVerificationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeVerificationHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ProfileVerificationDto>>> Handle(
        GetEmployeeVerificationHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        EmployeeProfile? profile = await _context.EmployeeProfiles.FirstOrDefaultAsync(
            p => p.UserId == request.UserId,
            cancellationToken
        );

        if (profile == null)
        {
            return Result.Failure<List<ProfileVerificationDto>>(
                new Error("Profile.NotFound", "Employee profile not found", ErrorType.Failure)
            );
        }

        List<ProfileVerification> rawVerifications = await _context
            .ProfileVerifications.Where(pv => pv.EmployeeProfileId == profile.Id)
            .Include(pv => pv.Reviewer)
            .OrderByDescending(pv => pv.RequestedAt)
            .ToListAsync(cancellationToken);

        var verifications = rawVerifications
            .Select(pv => new ProfileVerificationDto
            {
                Id = pv.Id,
                Status = pv.Status,
                Notes = pv.Notes,
                SfiaProposed = pv.SfiaProposed ?? 0,
                RequestedAt = pv.RequestedAt,
                ReviewedAt = pv.ReviewedAt,
                ReviewerName = pv.Reviewer?.Email,
                ReviewerEmail = pv.Reviewer?.Email,
            })
            .ToList();

        return Result.Success(verifications);
    }
}
