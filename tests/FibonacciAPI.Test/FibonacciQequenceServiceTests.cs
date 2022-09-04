using FibonacciAPI.Queries;
using FibonacciAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FibonacciAPI.Test
{
    public class FibonacciSequenceServiceTests
    {
        private static readonly List<int> fibonacciNumbers = new() { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711 };

        private FibonacciSequenceService CreateTestFibonacciSequenceService(IFibonacciNumberGeneratorService fibonacciNumberGeneratorService, GetSubsequenceQuery query)
        {
            var _validatorMock = new Mock<IValidator<GetSubsequenceQuery>>();
            _validatorMock.Setup(s => s.Validate(query)).Returns(new ValidationResult());

            var mockedMemoryCache = Mock.Of<IMemoryCache>();
            var configurationMock = Mock.Of<IConfiguration>();
            return new FibonacciSequenceService(fibonacciNumberGeneratorService, _validatorMock.Object, mockedMemoryCache, configurationMock);
        }

        private FibonacciSequenceService CreateTestFibonacciSequenceService(GetSubsequenceQuery query)
        {
            return CreateTestFibonacciSequenceService(new FibonacciNumberGeneratorService(), query);
        }

        private GetSubsequenceQuery GetQuery(int indexOfFirstNumber, int indexOfLastNumber, int maxAmountOfMemory, int firstGenerationTimeout)
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

        [Test]
        public async Task FibonacciSequenceService_ShouldReturnNumbersAsExpected()
        {
            //act
            int sizeOfTenNumbersInBytes = 10 * 8;//10 numbers with 8 byte size
            var query = GetQuery(0, 19, sizeOfTenNumbersInBytes, 3000);
            var _fibonacciSequenceService = CreateTestFibonacciSequenceService(query);

            //arrange
            var response = await _fibonacciSequenceService.GetSubsequence(query);

            //assert
            Assert.That(response.Success, Is.True);

            var numbers = response.Data;
            Assert.That(numbers, Has.Count.EqualTo(10));

            for (int i = 0; i <= 9; i++)
            {
                Assert.That(numbers[i], Is.EqualTo(fibonacciNumbers[i]));
            }
        }

        [Test]
        public async Task FibonacciSequenceService_ShouldReturnTimeOutResponse_WhenGenerationOfFirstNumberTimesOut()
        {
            //act
            var query = GetQuery(0, 7, 100, 100);
            var mockFibonacciNumberGeneratorService = new Mock<IFibonacciNumberGeneratorService>();
            mockFibonacciNumberGeneratorService.Setup(s => s.GenerateFibonacciNumberPositionFromIndex(query.IndexOfFirstNumber.Value))
                .Returns(() => GetDelayTask(query.FirstGenerationTimeout.Value + 50));
            var _fibonacciSequenceService = CreateTestFibonacciSequenceService(mockFibonacciNumberGeneratorService.Object, query);

            //arrange
            var response = await _fibonacciSequenceService.GetSubsequence(query);

            //assert
            Assert.That(response.Success, Is.False);
            Assert.That(response.Messages.Any(s => s.PropertyName == FibonacciSequenceService.TimeoutErrorPropertyName), Is.True);

            async Task<(long numberBeforeFirstPosition, long numberOfFirstPosition)> GetDelayTask(int delay)
            {
                await Task.Delay(delay);
                return (1, 1);
            }
        }

        [Test]
        public async Task FibonacciSequenceService_ShouldAbortExecution_WhenNumbersAreOutOfMemory()
        {
            //act
            var query = GetQuery(0, 7, 100, 3000);
            var _fibonacciSequenceService = CreateTestFibonacciSequenceService(query);

            //arrange
            var response = await _fibonacciSequenceService.GetSubsequence(query);

            //assert
            Assert.That(response.Success, Is.True);

            var numbers = response.Data;
            Assert.That(numbers, Has.Count.EqualTo(8));

            for (int i = 0; i <= 7; i++)
            {
                Assert.That(numbers[i], Is.EqualTo(fibonacciNumbers[i]));
            }
        }
    }
}
