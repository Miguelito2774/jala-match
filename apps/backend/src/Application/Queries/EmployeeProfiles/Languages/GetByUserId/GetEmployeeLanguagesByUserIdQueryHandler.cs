using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.Languages.GetByUserId;

public sealed class GetEmployeeLanguagesByUserIdQueryHandler
    : IQueryHandler<GetEmployeeLanguagesByUserIdQuery, List<EmployeeLanguageDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeLanguagesByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<EmployeeLanguageDto>>> Handle(
        GetEmployeeLanguagesByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        List<EmployeeLanguageDto> list = await _context
            .EmployeeLanguages.Where(el => el.EmployeeProfile.UserId == request.UserId)
            .Select(el => new EmployeeLanguageDto
            {
                Id = el.Id,
                Language = el.Language,
                Proficiency = el.Proficiency,
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}
