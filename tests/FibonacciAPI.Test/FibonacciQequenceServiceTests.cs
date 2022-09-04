using FibonacciAPI.Queries;
using FibonacciAPI.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace FibonacciAPI.Test
{
    public class FibonacciSequenceServiceTests
    {
        static readonly List<int> fibonacciNumbers = new() { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711 };

        FibonacciSequenceService _fibonacciSequenceService;
        Mock<IValidator<GetSubsequenceQuery>> _validatorMock;
        [SetUp]
        public void SetUp()
        {
            _validatorMock = new Mock<IValidator<GetSubsequenceQuery>>();
            var mockedMemoryCache = Mock.Of<IMemoryCache>();
            _fibonacciSequenceService = new FibonacciSequenceService(_validatorMock.Object, mockedMemoryCache);
        }

        [Test]
        public async Task FibonacciSequenceService_ShouldReturnNumbersAsExpected()
        {
            //act
            var query = new GetSubsequenceQuery();
            query.IndexOfFirstNumber = 0;
            query.IndexOfLastNumber = 7;
            query.MaxAmountOfMemory = 100;
            _validatorMock.Setup(s => s.Validate(query)).Returns(new ValidationResult());

            //arrange
            var response = await _fibonacciSequenceService.GetSubsequence(query);

            //assert
            Assert.That(response.Success, Is.True);

            var numbers = response.Data;
            Assert.That(numbers.Count, Is.EqualTo(8));

            for (int i = 0; i <= 7; i++)
            {
                Assert.That(numbers[i], Is.EqualTo(fibonacciNumbers[i]));
            }
        }
    }
}
