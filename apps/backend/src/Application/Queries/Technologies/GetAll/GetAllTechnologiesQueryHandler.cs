using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Identifiers;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.Technologies.GetAll;

internal sealed class GetAllTechnologiesQueryHandler(
    IApplicationDbContext context,
    ICacheService cacheService
) : IQueryHandler<GetAllTechnologiesQuery, List<TechnologyResponse>>
{
    private async Task<Result<List<TechnologyResponse>>> GetAllTechnologiesAsync(
        CancellationToken cancellationToken
    )
    {
        List<TechnologyResponse> technologies = await context
            .Technologies.AsNoTracking()
            .Include(t => t.Category)
            .Select(t => new TechnologyResponse
            {
                Id = t.Id,
                Name = t.Name,
                CategoryId = t.CategoryId,
                CategoryName = t.Category.Name,
            })
            .ToListAsync(cancellationToken);

        return technologies;
    }

    public async Task<Result<List<TechnologyResponse>>> Handle(
        GetAllTechnologiesQuery query,
        CancellationToken cancellationToken
    )
    {
        return await cacheService.GetOrCreateAsync(
            "technologies:all",
            async ct => await GetAllTechnologiesAsync(ct),
            [Tags.Technologies],
            cancellationToken
        );
    }
}
