using FibonacciAPI.Extentions;
using FibonacciAPI.Queries;
using FibonacciAPI.Responses;
using FibonacciAPI.Utilities;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.InteropServices;

namespace FibonacciAPI.Services
{
    public class FibonacciSequenceService : IFibonacciSequenceService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IValidator<GetSubsequenceQuery> _fibonacciSequenceValidator;
        private readonly IFibonacciNumberGeneratorService _fibonacciNumberGeneratorService;
        public FibonacciSequenceService(IFibonacciNumberGeneratorService fibonacciNumberGeneratorService, IValidator<GetSubsequenceQuery> validator, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _fibonacciSequenceValidator = validator;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _fibonacciNumberGeneratorService = fibonacciNumberGeneratorService;
        }

        public async Task<ServerResponse<List<long>>> GetSubsequenceAsync(GetSubsequenceQuery query)
        {
            var validateStatus = _fibonacciSequenceValidator.Validate(query);
            if (!validateStatus.IsValid)
                return ServerResponse<List<long>>.GetFailResponse(validateStatus.GetErrors());

            if (TryGetFromCache(query, out List<long> fromCache))
                return ServerResponse<List<long>>.GetSuccessResponse(fromCache);

            var firstPositionGenerationTask = _fibonacciNumberGeneratorService.GenerateFibonacciNumberPositionFromIndexAsync(query.IndexOfFirstNumber.Value);
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                if (await Task.WhenAny(firstPositionGenerationTask, Task.Delay(query.FirstGenerationTimeout.Value, timeoutCancellationTokenSource.Token)) == firstPositionGenerationTask)
                {
                    timeoutCancellationTokenSource.Cancel();
                    var firstPosition = firstPositionGenerationTask.Result;
                    List<long> numbers = GetSequences(query, firstPosition.FirstNumberPosition, firstPosition.SecondNumberPosition);
                    SetCache(query, numbers);

                    return ServerResponse<List<long>>.GetSuccessResponse(numbers);
                }
                else
                {
                    return GetTimeOutResponse();
                }
            }
        }

        public const string TimeoutErrorPropertyName = "Timeout";
        public const string TimeoutError = "Timeout during generation first number";
        private ServerResponse<List<long>> GetTimeOutResponse()
        {
            return ServerResponse<List<long>>.GetFailResponse(new List<ErrorResponse>
            {
                new ErrorResponse(TimeoutErrorPropertyName, new List<string>{ TimeoutError })
            });
        }

        private List<long> GetSequences(GetSubsequenceQuery query, long numberBeforeFirstPosition, long numberOfFirstPosition)
        {
            List<long> results = new() { numberOfFirstPosition };
            int memorySize = Marshal.SizeOf(numberOfFirstPosition);
            for (int i = query.IndexOfFirstNumber.Value; i < query.IndexOfLastNumber; i++)
            {
                var currentNumber = numberBeforeFirstPosition + numberOfFirstPosition;
                numberBeforeFirstPosition = numberOfFirstPosition;
                numberOfFirstPosition = currentNumber;
                results.Add(currentNumber);
                memorySize += Marshal.SizeOf(currentNumber);
                if (memorySize >= query.MaxAmountOfMemory)
                    break;
            }
            return results;
        }

        private bool TryGetFromCache(GetSubsequenceQuery query, out List<long> fromCache)
        {
            fromCache = null;
            if (query.UseCache.HasValue && query.UseCache.Value
                && _memoryCache.TryGetValue(query.GetCacheKey(), out fromCache))
            {
                return true;
            }
            return false;
        }

        private void SetCache(GetSubsequenceQuery query, List<long> numbers)
        {
            if (query.UseCache.HasValue && query.UseCache.Value)
            {
                MemoryCacheEntryOptions memoryCacheEntryOptions = new();
                if (TryGetCacheTimeoutFromConfiguration(out TimeSpan? timeOut))
                    memoryCacheEntryOptions.SetAbsoluteExpiration(timeOut.Value);

                _memoryCache.Set(query.GetCacheKey(), numbers, memoryCacheEntryOptions);
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
