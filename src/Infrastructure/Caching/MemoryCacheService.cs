using HotelManagement.Application.Abstractions.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManagement.Infrastructure.Caching;

public sealed class MemoryCacheService(IMemoryCache cache) : ICacheService
{
    public T? Get<T>(string key) => cache.TryGetValue(key, out T? v) ? v : default;
    public void Set<T>(string key, T value, TimeSpan duration) => cache.Set(key, value, duration);
    public void Remove(string key) => cache.Remove(key);
}