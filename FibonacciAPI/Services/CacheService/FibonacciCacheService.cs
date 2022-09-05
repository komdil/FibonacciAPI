using Microsoft.Extensions.Caching.Memory;

namespace FibonacciAPI.Services.CacheService
{
    public class FibonacciCacheService : IFibonacciCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        public FibonacciCacheService(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public bool TryGetValue(GetSubsequenceQuery query, out List<long> numbers)
        {
            numbers = null;
            if (query.UseCache && _memoryCache.TryGetValue(Constants.CacheKey, out FibonacciCache cache))
            {
                var cacheEnry = cache.CacheEntries.FirstOrDefault(s => s.IndexOfFirstProperty >= query.IndexOfFirstNumber && s.IndexOfLastProperty <= query.IndexOfLastNumber);
                if (cacheEnry != null)
                {
                    numbers = cacheEnry.Sequence.Skip(query.IndexOfFirstNumber).Take(query.IndexOfLastNumber).ToList();
                    return true;
                }
            }
            return false;
        }

        public void Set(GetSubsequenceQuery query, List<long> numbers)
        {
            if (query.UseCache)
            {
                MemoryCacheEntryOptions memoryCacheEntryOptions = new();
                if (TryGetCacheTimeoutFromConfiguration(out TimeSpan? timeOut))
                    memoryCacheEntryOptions.SetAbsoluteExpiration(timeOut.Value);

                if (_memoryCache.TryGetValue(Constants.CacheKey, out FibonacciCache cache))
                {
                    //TODO
                }
                else
                {
                    FibonacciCache fibonacciCache = new();
                    fibonacciCache.CacheEntries.Add(new
                        FibonnaciCacheEntry(query.IndexOfFirstNumber, query.IndexOfLastNumber, numbers));
                    _memoryCache.Set(Constants.CacheKey, fibonacciCache);
                }
            }
        }

        private bool TryGetCacheTimeoutFromConfiguration(out TimeSpan? timeSpan)
        {
            timeSpan = null;
            var timeOut = _configuration.GetValue<int?>(Constants.TimeOutConfigurationKey);
            if (timeOut.HasValue)
            {
                timeSpan = TimeSpan.FromMinutes(timeOut.Value);
                return true;
            }
            return false;
        }
    }
}
