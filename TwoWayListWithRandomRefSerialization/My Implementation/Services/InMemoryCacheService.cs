using Logic_Layer.Services.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Logic_Layer.Services;

public class InMemoryCacheService : ICacheService
{
    private const int DEFAULT_SLIDING_EXPIRATION_IN_MIN = 5;
    
    private readonly IMemoryCache cache;
    private readonly MemoryCacheEntryOptions cacheEntryOptions;

    public InMemoryCacheService(int defaultSlidingExpirationInMin = DEFAULT_SLIDING_EXPIRATION_IN_MIN)
    {
        var cacheOptions = new MemoryCacheOptions();
        cache = new MemoryCache(cacheOptions);
        cacheEntryOptions = new MemoryCacheEntryOptions();
        cacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(defaultSlidingExpirationInMin);
    }
    
    public void Set<T>(string cacheKey, T value, TimeSpan? expiration = null)
    {
        if (expiration == null)
        {
            cache.Set(cacheKey, value, cacheEntryOptions);
            return;
        }

        var newCacheEntryOptions = new MemoryCacheEntryOptions();
        newCacheEntryOptions.SlidingExpiration = expiration;
        cache.Set(cacheKey, value, newCacheEntryOptions);
    }
    
    public T Get<T>(string cacheKey) => cache.Get<T>(cacheKey);

    public Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null)
    {
        Set(cacheKey, value, expiration);
        return Task.CompletedTask;
    }

    public Task<T> GetAsync<T>(string cacheKey) => Task.FromResult(Get<T>(cacheKey));
}