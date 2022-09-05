using FibonacciAPI.Extentions;
using FibonacciAPI.Queries;
using FibonacciAPI.Responses;
using FibonacciAPI.Utilities;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using FibonacciAPI.Cache;

namespace FibonacciAPI.Services
{
    public class FibonacciSequenceService : IFibonacciSequenceService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IValidator<GetSubsequenceQuery> _fibonacciSequenceValidator;
        private readonly IFibonacciPositionGeneratorService _fibonacciNumberGeneratorService;
        private readonly IFibonacciNextNumberGeneratorService _fibonacciNextNumberGeneratorService;
        public FibonacciSequenceService(
            IFibonacciPositionGeneratorService fibonacciNumberGeneratorService,
            IFibonacciNextNumberGeneratorService fibonacciNextNumberGeneratorService,
            IValidator<GetSubsequenceQuery> validator,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _fibonacciSequenceValidator = validator;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _fibonacciNumberGeneratorService = fibonacciNumberGeneratorService;
            _fibonacciNextNumberGeneratorService = fibonacciNextNumberGeneratorService;
        }

        public async Task<ServerResponse<List<long>>> GetSubsequenceAsync(GetSubsequenceQuery query)
        {
            var validateStatus = await _fibonacciSequenceValidator.ValidateAsync(query);
            if (!validateStatus.IsValid)
                return ServerResponse<List<long>>.GetFailResponse(validateStatus.GetErrors());

            if (TryGetFromCache(query, out List<long> fromCache))
                return ServerResponse<List<long>>.GetSuccessResponse(fromCache);

            try
            {
                using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(query.FirstGenerationTimeout));
                var firstPositionGenerationTask = await _fibonacciNumberGeneratorService.GenerateFibonacciNumberPositionFromIndexAsync(query.IndexOfFirstNumber, timeoutCancellationTokenSource.Token);
                List<long> numbers = await GetSequences(query, firstPositionGenerationTask.FirstNumberPosition, firstPositionGenerationTask.SecondNumberPosition);
                SetCache(query, numbers);
                return ServerResponse<List<long>>.GetSuccessResponse(numbers);
            }
            catch (TaskCanceledException)
            {
                return GetTimeOutResponse();
            }
        }

        private ServerResponse<List<long>> GetTimeOutResponse()
        {
            return ServerResponse<List<long>>.GetFailResponse(new List<ErrorResponse>
            {
                new ErrorResponse(Constants.TimeoutErrorPropertyName, new List<string>{ Constants.TimeoutError })
            });
        }

        private async Task<List<long>> GetSequences(GetSubsequenceQuery query, long numberBeforeFirstPosition, long numberOfFirstPosition)
        {
            List<long> results = new() { numberOfFirstPosition };
            for (int i = query.IndexOfFirstNumber; i < query.IndexOfLastNumber; i++)
            {
                var currentNumber = await Task.Factory.StartNew(() => _fibonacciNextNumberGeneratorService.GenerateNextNumber(numberBeforeFirstPosition, numberOfFirstPosition));
                numberBeforeFirstPosition = numberOfFirstPosition;
                numberOfFirstPosition = currentNumber;
                results.Add(currentNumber);
                if (results.Count * 8 >= query.MaxAmountOfMemory)//long is 64 bits or 8 bytes
                    break;
            }
            return results;
        }

        private bool TryGetFromCache(GetSubsequenceQuery query, out List<long> fromCache)
        {
            fromCache = null;

            if (query.UseCache
                && _memoryCache.TryGetValue(Constants.CacheKey, out FibonnaciCacheEntry cacheEntry)
                && cacheEntry.StartValue >= query.IndexOfFirstNumber && cacheEntry.EndValue <= query.IndexOfLastNumber)
            {
                // fromCache = cacheEntry.Sequence.Skip(DifferenceBetweenOurValues).Take(UntillLimitReached);
                return true;
            }
            return false;
        }

        private void SetCache(GetSubsequenceQuery query, List<long> numbers)
        {
            if (query.UseCache && query.UseCache)
            {
                MemoryCacheEntryOptions memoryCacheEntryOptions = new();
                if (TryGetCacheTimeoutFromConfiguration(out TimeSpan? timeOut))
                    memoryCacheEntryOptions.SetAbsoluteExpiration(timeOut.Value);

                var cacheEntry = new FibonnaciCacheEntry
                {
                    StartValue = query.IndexOfFirstNumber,
                    EndValue = query.IndexOfLastNumber,
                    Sequence = numbers
                };
                _memoryCache.Set(Constants.CacheKey, cacheEntry, memoryCacheEntryOptions);
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
