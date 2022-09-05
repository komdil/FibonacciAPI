using FibonacciAPI.Queries;
using FibonacciAPI.Services.CacheService;
using FibonacciAPI.Services.NumberGenerator;
using FibonacciAPI.Services.PositionGenerator;
using FibonacciAPI.Services.SequenceGenerator;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FibonacciAPI.Test
{
    public class FibonacciSequenceServiceTests : TestBase
    {
        private FibonacciSequenceService CreateTestFibonacciSequenceService(IFibonacciPositionGeneratorService fibonacciNumberGeneratorService, GetSubsequenceQuery query)
        {
            var _validatorMock = new Mock<IValidator<GetSubsequenceQuery>>();
            _validatorMock.Setup(s => s.ValidateAsync(query, default)).Returns(Task.FromResult(new ValidationResult()));

            var mockedCacheService = Mock.Of<IFibonacciCacheService>();
            var nextNumberGeneratorService = new FibonacciNextNumberGeneratorService();
            return new FibonacciSequenceService(fibonacciNumberGeneratorService, nextNumberGeneratorService, _validatorMock.Object, mockedCacheService);
        }

        private FibonacciSequenceService CreateTestFibonacciSequenceService(GetSubsequenceQuery query)
        {
            var nextNumberGeneratorService = new FibonacciNextNumberGeneratorService();
            return CreateTestFibonacciSequenceService(new FibonacciPositionGeneratorService(nextNumberGeneratorService), query);
        }

       

        [TestCase(0, 7)]
        [TestCase(3, 9)]
        [TestCase(5, 20)]
        public async Task FibonacciSequenceService_ShouldReturnNumbersAsExpected(int indexOfFirstNumber, int indexOfLastNumber)
        {
            //arrange
            var query = GetQuery(indexOfFirstNumber, indexOfLastNumber, int.MaxValue, int.MaxValue);
            var _fibonacciSequenceService = CreateTestFibonacciSequenceService(query);

            //act
            var response = await _fibonacciSequenceService.GetSubsequenceAsync(query);

            //assert
            Assert.That(response.Success, Is.True);

            var numbers = response.Data;
            var expectedCount = indexOfLastNumber - indexOfFirstNumber + 1;

            Assert.That(numbers, Has.Count.EqualTo(expectedCount));

            var expectedFibonacciNumbers = fibonacciNumbers.Skip(indexOfFirstNumber).Take(expectedCount).ToList();
            for (int i = 0; i < expectedCount; i++)
            {
                Assert.That(numbers[i], Is.EqualTo(expectedFibonacciNumbers[i]));
            }
        }

        [Test]
        public async Task FibonacciSequenceService_ShouldAbortExecution_WhenNumbersAreOutOfMemory()
        {
            //arrange
            int sizeOfTenNumbersInBytes = 10 * 8;//10 numbers with 8 byte size
            var query = GetQuery(0, 19, sizeOfTenNumbersInBytes, 3000);
            var _fibonacciSequenceService = CreateTestFibonacciSequenceService(query);

            //act
            var response = await _fibonacciSequenceService.GetSubsequenceAsync(query);

            //assert
            Assert.That(response.Success, Is.True);

            var numbers = response.Data;
            Assert.That(numbers, Has.Count.EqualTo(10));

            for (int i = 0; i <= 9; i++)
            {
                Assert.That(numbers[i], Is.EqualTo(fibonacciNumbers[i]));
            }
        }
    }
}
