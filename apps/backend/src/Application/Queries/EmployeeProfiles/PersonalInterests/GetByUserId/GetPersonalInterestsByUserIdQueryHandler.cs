using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.DTOs;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.PersonalInterests.GetByUserId;

public sealed class GetPersonalInterestsByUserIdQueryHandler
    : IQueryHandler<GetPersonalInterestsByUserIdQuery, List<PersonalInterestDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPersonalInterestsByUserIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<PersonalInterestDto>>> Handle(
        GetPersonalInterestsByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        List<PersonalInterestDto> personalInterests = await _context
            .PersonalInterests.Where(pi => pi.EmployeeProfile.UserId == request.UserId)
            .Select(pi => new PersonalInterestDto
            {
                Id = pi.Id,
                Name = pi.Name,
                SessionDurationMinutes = pi.SessionDurationMinutes,
                Frequency = pi.Frequency,
                InterestLevel = pi.InterestLevel,
            })
            .ToListAsync(cancellationToken);

        return Result.Success(personalInterests);
    }
}
