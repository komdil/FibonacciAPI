using FibonacciAPI.Services.CacheService;

namespace FibonacciAPI.Services.SequenceGenerator
{
    public class FibonacciSequenceService : IFibonacciSequenceService
    {
        private readonly IValidator<GetSubsequenceQuery> _fibonacciSequenceValidator;
        private readonly IFibonacciPositionGeneratorService _fibonacciNumberGeneratorService;
        private readonly IFibonacciNextNumberGeneratorService _fibonacciNextNumberGeneratorService;
        private readonly IFibonacciCacheService _fibonacciCacheService;
        public FibonacciSequenceService(
            IFibonacciPositionGeneratorService fibonacciNumberGeneratorService,
            IFibonacciNextNumberGeneratorService fibonacciNextNumberGeneratorService,
            IValidator<GetSubsequenceQuery> validator,
            IFibonacciCacheService fibonacciCacheService)
        {
            _fibonacciSequenceValidator = validator;
            _fibonacciCacheService = fibonacciCacheService;
            _fibonacciNumberGeneratorService = fibonacciNumberGeneratorService;
            _fibonacciNextNumberGeneratorService = fibonacciNextNumberGeneratorService;
        }

        public async Task<ServerResponse<List<long>>> GetSubsequenceAsync(GetSubsequenceQuery query)
        {
            var validateStatus = await _fibonacciSequenceValidator.ValidateAsync(query);
            if (!validateStatus.IsValid)
                return ServerResponse<List<long>>.GetFailResponse(validateStatus.GetErrors());

            if (_fibonacciCacheService.TryGetValue(query, out List<long> fromCache))
                return ServerResponse<List<long>>.GetSuccessResponse(fromCache);

            try
            {
                using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(query.FirstGenerationTimeout));
                var firstPositionGenerationTask = await _fibonacciNumberGeneratorService.GenerateFibonacciNumberPositionFromIndexAsync(query.IndexOfFirstNumber, timeoutCancellationTokenSource.Token);
                List<long> numbers = await GetSequences(query, firstPositionGenerationTask.FirstNumberPosition, firstPositionGenerationTask.SecondNumberPosition);
                _fibonacciCacheService.Set(query, numbers);
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
    }
}
