namespace Logic_Layer.Services.Abstractions;

public interface ICacheService
{
    Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiration = null);
    Task<T> GetAsync<T>(string cacheKey);
}