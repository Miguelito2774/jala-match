using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Entities.Users;
using Domain.Identifiers;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Results;

namespace Application.Queries.Users.GetById;

internal sealed class GetUserByIdQueryHandler(
    IApplicationDbContext context,
    ICacheService cacheService
) : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    private async Task<Result<UserResponse>> GetUserByIdAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        User? user = await context
            .Users.AsNoTracking()
            .Where(u => u.Id == query.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(query.UserId));
        }

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = user.Role.ToString(),
        };
    }

    public async Task<Result<UserResponse>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        return await cacheService.GetOrCreateAsync(
            $"users:{query.UserId}",
            async ct => await GetUserByIdAsync(query, ct),
            [Tags.Users],
            cancellationToken
        );
    }
}
