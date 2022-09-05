
using FibonacciAPI.Cache;
using FibonacciAPI.Services.CacheService;
using FibonacciAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Reflection.Metadata;

namespace FibonacciAPI.Test
{
    public class FibonacciCacheServiceTests
    {
        FibonacciCacheService GetCacheService(IMemoryCache memoryCache)
        {
            IConfiguration configuration = Mock.Of<IConfiguration>();
            return new FibonacciCacheService(memoryCache, configuration);
        }

        [Test]
        public void FibonacciCacheService_ShouldReturnNumbersFromCache_IfIndexesAreMatch()
        {
            //TODO
        }
    }
}
