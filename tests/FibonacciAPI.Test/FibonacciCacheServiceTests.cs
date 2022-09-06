using FibonacciAPI.Cache;
using FibonacciAPI.Services.CacheService;
using FibonacciAPI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using System;

namespace FibonacciAPI.Test
{
    public class FibonacciCacheServiceTests : TestBase
    {
        private IMemoryCache MockMemoryCacheWithGetValue(object expectedValue)
        {
            var mockMemoryCache = new Mock<IMemoryCache>();
            mockMemoryCache
                .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue))
                .Returns(true);
            return mockMemoryCache.Object;
        }

        private IMemoryCache MockMemoryCacheWithSetValue(Action<object> setActionCallBack, object expectedValue = null)
        {
            var mockCache = new Mock<IMemoryCache>();
            var mockCacheEntry = new Mock<ICacheEntry>();
            string keyPayload = null;
            mockCache
                .Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Callback((object k) => keyPayload = (string)k)
                .Returns(mockCacheEntry.Object); // this should address your null reference exception

            if (expectedValue != null)
            {
                mockCache
                    .Setup(x => x.TryGetValue(It.IsAny<object>(), out expectedValue))
                    .Returns(true);
            }

            mockCacheEntry
                .SetupSet(mce => mce.Value = It.IsAny<object>())
                .Callback(setActionCallBack);
            return mockCache.Object;

        }

        private FibonacciCacheService GetCacheService(IMemoryCache memoryCache)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection().Build();
            return new FibonacciCacheService(memoryCache, configuration);
        }

        private FibonacciCache CreateFibonacciCache(int indexFrom, int indexTo)
        {
            FibonacciCache fibonacciCache = new();
            fibonacciCache.CacheEntries.Add(new
                FibonnaciCacheEntry(indexFrom, indexTo, EnumerableHelper.RangeLong(indexFrom, indexTo)));
            return fibonacciCache;
        }

        [TestCase(1, 100, 100)]
        [TestCase(50, 60, 11)]
        [TestCase(2, 99, 98)]
        [TestCase(10, 20, 11)]
        public void FibonacciCacheService_ShouldReturnNumbersFromCache_IfIndexesAreMatch(int startIndex, int endIndex, int expectedCount)
        {
            //arrange
            var query = GetQuery(startIndex, endIndex, useCache: true);
            var expectedCache = CreateFibonacciCache(1, 100);
            var memoryCache = MockMemoryCacheWithGetValue(expectedCache);
            var service = GetCacheService(memoryCache);

            //act
            var isCached = service.TryGetValue(query, out List<long> numbers);

            //assert
            Assert.That(isCached, Is.True);
            Assert.That(numbers.Count, Is.EqualTo(expectedCount));
        }

        [TestCase(1, 20)]
        [TestCase(10, 101)]
        [TestCase(100, 200)]
        public void FibonacciCacheService_ShouldNotReturnNumbersFromCache_IfIndexesAreNotMatch(int startIndex, int endIndex)
        {
            //arrange
            var query = GetQuery(startIndex, endIndex, useCache: true);
            var expectedCache = CreateFibonacciCache(10, 100);
            var memoryCache = MockMemoryCacheWithGetValue(expectedCache);
            var service = GetCacheService(memoryCache);

            //act
            var isCached = service.TryGetValue(query, out _);

            //assert
            Assert.That(isCached, Is.False);
        }

        [Test]
        public void FibonacciCacheService_CacheShouldReturnValues()
        {
            //arrange
            var query = GetQuery(1, 100, useCache: true);
            var longRangeToCache = EnumerableHelper.RangeLong(1, 100);
            FibonacciCache fibonacciCache = null;
            var memoryCacheMock = MockMemoryCacheWithSetValue(p =>
            {
                fibonacciCache = (FibonacciCache)p;
            });
            var cacheService = GetCacheService(memoryCacheMock);

            // Act
            cacheService.Set(query, longRangeToCache);

            //assert
            Assert.That(fibonacciCache, Is.Not.Null);
            Assert.That(fibonacciCache.CacheEntries.Count, Is.EqualTo(1));
            Assert.That(fibonacciCache.CacheEntries[0].Sequence.Count, Is.EqualTo(100));
        }

        [TestCase(100, 150, 1, 140, 151, "from 100 to 150 = 51 items, after adding from 1 to 140. Only 100 of them will be added. " +
            "So, result should be 51+100=151")]
        [TestCase(100, 150, 140, 200, 101, "from 100 to 150 = 51 items, after adding from 140 to 200. Only 50 of them will be added." +
            "So, result should be 51+50=101")]
        public void FibonacciCacheService_ShouldExtendCachedIndex_WhenStartIndexIsMatched(int existingCacheStartIndex,
            int existingCacheEndIndex, int cachingStartIndex, int cachingEndIndex, int expectedCount, string message)
        {
            //arrange
            var query = GetQuery(cachingStartIndex, cachingEndIndex, useCache: true);
            var longRangeToCache = EnumerableHelper.RangeLong(cachingStartIndex, cachingEndIndex);
            FibonacciCache fibonacciCache = CreateFibonacciCache(existingCacheStartIndex, existingCacheEndIndex);
            var memoryCacheMock = MockMemoryCacheWithGetValue(fibonacciCache);
            var cacheService = GetCacheService(memoryCacheMock);

            // Act
            cacheService.Set(query, longRangeToCache);

            //assert
            Assert.That(fibonacciCache.CacheEntries.Count, Is.EqualTo(1));
            Assert.That(fibonacciCache.CacheEntries[0].Sequence.Count, Is.EqualTo(expectedCount), message);
        }

        [Test]
        public void FibonacciCacheService_TwoEntriesShouldBeCached_WhenIndexesDoesNotMatch()
        {
            //arrange
            var query = GetQuery(200, 300, useCache: true);
            FibonacciCache fibonacciCache = CreateFibonacciCache(100, 150);
            var longRangeToCache = EnumerableHelper.RangeLong(200, 300);
            var memoryCacheMock = MockMemoryCacheWithSetValue(p => { }, fibonacciCache);
            var cacheService = GetCacheService(memoryCacheMock);

            // Act
            cacheService.Set(query, longRangeToCache);

            //assert
            Assert.That(fibonacciCache, Is.Not.Null);
            Assert.That(fibonacciCache.CacheEntries.Count, Is.EqualTo(2));
        }
    }
}
