using Domain.Services;
using Microsoft.Extensions.Caching.Hybrid;

namespace Infrastructure.Services;

internal sealed class CacheService(HybridCache hybridCache) : ICacheService
{
    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        IEnumerable<string> tags,
        CancellationToken cancellationToken = default
    )
    {
        T result = await hybridCache.GetOrCreateAsync(
            key,
            factory,
            tags: tags,
            cancellationToken: cancellationToken
        );

        return result;
    }

    public async Task EvictByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveByTagAsync(tag, cancellationToken);
    }
}
