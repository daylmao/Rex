using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Rex.Application.Utilities;

public static class DistributedCache
{
    private static DistributedCacheEntryOptions CacheExpiration => new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(1)
    };

    public static async Task<T> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> factory,
        ILogger logger,
        DistributedCacheEntryOptions options = null!,
        CancellationToken cancellationToken = default
    )
    {
        var cachedData = await cache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrEmpty(cachedData))
        {
            logger.LogInformation("Cache hit for key '{Key}'", key);
            return JsonSerializer.Deserialize<T>(cachedData)!;
        }

        logger.LogInformation("Cache miss for key '{Key}', fetching data...", key);
        var data = await factory();

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(data),
            options ?? CacheExpiration,
            cancellationToken
        );

        logger.LogInformation("Data cached for key '{Key}'", key);

        return data;
    }

    public static async Task<int> GetVersionAsync(
        this IDistributedCache cache,
        string resourceType,
        Guid resourceId,
        CancellationToken cancellationToken = default
    )
    {
        var versionKey = $"cache-version:{resourceType}:{resourceId}";
        var versionStr = await cache.GetStringAsync(versionKey, cancellationToken);

        if (string.IsNullOrEmpty(versionStr))
        {
            await cache.SetStringAsync(versionKey, "1", CacheExpiration, cancellationToken);
            return 1;
        }

        return int.Parse(versionStr);
    }

    public static async Task IncrementVersionAsync(
        this IDistributedCache cache,
        string resourceType,
        Guid resourceId,
        ILogger logger,
        CancellationToken cancellationToken = default
    )
    {
        var versionKey = $"cache-version:{resourceType}:{resourceId}";
        var current = await cache.GetVersionAsync(resourceType, resourceId, cancellationToken);
        var newVersion = current + 1;

        await cache.SetStringAsync(versionKey, newVersion.ToString(), CacheExpiration, cancellationToken);

        logger.LogInformation(
            "Cache version incremented from {OldVersion} to {NewVersion} for {ResourceType}:{ResourceId}",
            current, newVersion, resourceType, resourceId
        );
    }
}