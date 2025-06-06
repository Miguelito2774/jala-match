using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.EmployeeProfiles.Exists;

public sealed class CheckEmployeeProfileExistsQueryHandler
    : IQueryHandler<CheckEmployeeProfileExistsQuery, bool>
{
    private readonly IApplicationDbContext _context;

    public CheckEmployeeProfileExistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> Handle(
        CheckEmployeeProfileExistsQuery request,
        CancellationToken cancellationToken
    )
    {
        bool exists = await _context.EmployeeProfiles.AnyAsync(
            p => p.UserId == request.UserId,
            cancellationToken
        );

        return Result.Success(exists);
    }
}
