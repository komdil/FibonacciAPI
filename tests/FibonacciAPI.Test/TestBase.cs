using FibonacciAPI.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciAPI.Test
{
    public class TestBase
    {
        protected static readonly List<int> fibonacciNumbers = new() { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711 };

        protected GetSubsequenceQuery GetQuery(int indexOfFirstNumber, int indexOfLastNumber, int maxAmountOfMemory, int firstGenerationTimeout)
        {
            return new GetSubsequenceQuery
            {
                IndexOfFirstNumber = indexOfFirstNumber,
                IndexOfLastNumber = indexOfLastNumber,
                FirstGenerationTimeout = firstGenerationTimeout,
                MaxAmountOfMemory = maxAmountOfMemory,
                UseCache = false
            };
        }
    }
}
