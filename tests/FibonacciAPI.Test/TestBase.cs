using FibonacciAPI.Queries;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace FibonacciAPI.Test
{
    public class TestBase
    {
        protected static readonly List<int> fibonacciNumbers = new() { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711 };

        protected GetSubsequenceQuery GetQuery(int indexOfFirstNumber, int indexOfLastNumber, int maxAmountOfMemory = int.MaxValue, int firstGenerationTimeout = int.MaxValue, bool useCache = false)
        {
            return new GetSubsequenceQuery
            {
                IndexOfFirstNumber = indexOfFirstNumber,
                IndexOfLastNumber = indexOfLastNumber,
                FirstGenerationTimeout = firstGenerationTimeout,
                MaxAmountOfMemory = maxAmountOfMemory,
                UseCache = useCache
            };
        }
    }
}
