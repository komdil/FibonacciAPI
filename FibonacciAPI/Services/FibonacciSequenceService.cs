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
        private readonly IValidator<GetSubsequenceQuery> _fibonacciSequenceValidator;
        public FibonacciSequenceService(IValidator<GetSubsequenceQuery> validator, IMemoryCache memoryCache)
        {
            _fibonacciSequenceValidator = validator;
            _memoryCache = memoryCache;
        }

        public async Task<ServerResponse<List<int>>> GetSubsequence(GetSubsequenceQuery query)
        {
            var validateStatus = _fibonacciSequenceValidator.Validate(query);
            if (!validateStatus.IsValid)
                return new ServerResponse<List<int>>(validateStatus.GetErrors());

            if (TryGetFromCache(query, out List<int> fromCache))
                return new ServerResponse<List<int>>(fromCache);

            return null;
        }

        bool TryGetFromCache(GetSubsequenceQuery query, out List<int> fromCache)
        {
            fromCache = null;
            if (query.UseCache.HasValue && query.UseCache.Value
                && _memoryCache.TryGetValue(query.GetCacheKey(), out fromCache))
            {
                return true;
            }
            return false;
        }

        void SetCache(GetSubsequenceQuery query, List<int> numbers)
        {
            if (query.UseCache.HasValue && query.UseCache.Value)
                _memoryCache.Set(query.GetCacheKey(), numbers);
        }
    }
}
