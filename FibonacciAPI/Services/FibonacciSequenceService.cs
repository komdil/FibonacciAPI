using FibonacciAPI.Extentions;
using FibonacciAPI.Queries;
using FibonacciAPI.Responses;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace FibonacciAPI.Services
{
    public class FibonacciSequenceService : IFibonacciSequenceService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IValidator<GetSubsequenceQuery> _fibonacciSequenceValidator;
        public FibonacciSequenceService(IValidator<GetSubsequenceQuery> validator, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _fibonacciSequenceValidator = validator;
            _memoryCache = memoryCache;
            _configuration = configuration;
        }

        public async Task<ServerResponse<List<long>>> GetSubsequence(GetSubsequenceQuery query)
        {
            var validateStatus = _fibonacciSequenceValidator.Validate(query);
            if (!validateStatus.IsValid)
                return ServerResponse<List<long>>.GetFailResponse(validateStatus.GetErrors());

            if (TryGetFromCache(query, out List<long> fromCache))
                return ServerResponse<List<long>>.GetSuccessResponse(fromCache);

            List<long> numbers = await GetSequence(query);

            SetCache(query, numbers);

            return ServerResponse<List<long>>.GetSuccessResponse(numbers);
        }

        async Task<List<long>> GetSequence(GetSubsequenceQuery query)
        {
            (long numberBeforeFirstPosition, long numberOfFirstPosition) = getNumberOfFirstPositionAndBeforeFirstPosition(query.IndexOfFirstNumber.Value);
            List<long> results = new() { numberOfFirstPosition };
            for (int i = query.IndexOfFirstNumber.Value; i < query.IndexOfLastNumber; i++)
            {
                var currentNumber = numberBeforeFirstPosition + numberOfFirstPosition;
                numberBeforeFirstPosition = numberOfFirstPosition;
                numberOfFirstPosition = currentNumber;
                results.Add(currentNumber);
            }
            return results;
        }

        (long numberBeforeFirstPosition, long numberOfFirstPosition) getNumberOfFirstPositionAndBeforeFirstPosition(int firstPositionIndex)
        {
            long firstnumber = 0, secondnumber = 1, result = 0;

            if (firstPositionIndex == 0) return (1, 0);
            if (firstPositionIndex == 1) return (0, 1);

            for (int i = 2; i <= firstPositionIndex; i++)
            {
                result = firstnumber + secondnumber;
                firstnumber = secondnumber;
                secondnumber = result;
            }

            return (firstnumber, result);
        }

        bool TryGetFromCache(GetSubsequenceQuery query, out List<long> fromCache)
        {
            fromCache = null;
            if (query.UseCache.HasValue && query.UseCache.Value
                && _memoryCache.TryGetValue(query.GetCacheKey(), out fromCache))
            {
                return true;
            }
            return false;
        }

        void SetCache(GetSubsequenceQuery query, List<long> numbers)
        {
            if (query.UseCache.HasValue && query.UseCache.Value)
            {
                MemoryCacheEntryOptions memoryCacheEntryOptions = new();
                if (TryGetCacheTimeout(out TimeSpan? timeOut))
                    memoryCacheEntryOptions.SetAbsoluteExpiration(timeOut.Value);

                _memoryCache.Set(query.GetCacheKey(), numbers, memoryCacheEntryOptions);
            }
        }

        bool TryGetCacheTimeout(out TimeSpan? timeSpan)
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
