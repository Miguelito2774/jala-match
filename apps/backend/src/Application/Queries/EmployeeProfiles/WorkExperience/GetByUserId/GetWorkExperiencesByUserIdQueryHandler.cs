using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.WorkExperience.GetByUserId;

public sealed class GetWorkExperiencesByUserIdQueryHandler
    : IQueryHandler<GetWorkExperiencesByUserIdQuery, List<WorkExperienceDto>>
{
    private readonly IApplicationDbContext _context;

    public GetWorkExperiencesByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<WorkExperienceDto>>> Handle(
        GetWorkExperiencesByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        List<WorkExperienceDto> workExperiences = await _context
            .WorkExperiences.Where(we => we.EmployeeProfile.UserId == request.UserId)
            .OrderByDescending(we => we.StartDate)
            .Select(we => new WorkExperienceDto
            {
                Id = we.Id,
                ProjectName = we.ProjectName,
                Description = we.Description,
                Tools = we.Tools,
                ThirdParties = we.ThirdParties,
                Frameworks = we.Frameworks,
                VersionControl = we.VersionControl,
                ProjectManagement = we.ProjectManagement,
                Responsibilities = we.Responsibilities,
                StartDate = we.StartDate,
                EndDate = we.EndDate,
            })
            .ToListAsync(cancellationToken);

        return Result.Success(workExperiences);
    }
}
