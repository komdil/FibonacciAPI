using FibonacciAPI.Services;
using Moq;

namespace FibonacciAPI.Test
{
    public class FibonacciNextPositionGeneratorServiceTests : TestBase
    {
        FibonacciPositionGeneratorService GetFibonacciPositionGeneratorService(IFibonacciNextNumberGeneratorService fibonacciNextNumberGeneratorService)
        {
            return new FibonacciPositionGeneratorService(fibonacciNextNumberGeneratorService);
        }

        FibonacciPositionGeneratorService GetFibonacciPositionGeneratorService()
        {
            var numberGeneratorService = new FibonacciNextNumberGeneratorService();
            return GetFibonacciPositionGeneratorService(numberGeneratorService);
        }

        [TestCase(3)]
        [TestCase(5)]
        [TestCase(15)]
        public async Task FibonacciNextPositionGeneratorService_ShouldReturnZeroAndOnePosition_WhenIndexIsOne(int index)
        {
            //arrange
            var _fibonacciPositionGeneratorService = GetFibonacciPositionGeneratorService();

            //act
            var numberPosition = await _fibonacciPositionGeneratorService.GenerateFibonacciNumberPositionFromIndexAsync(index, default);

            //assert
            Assert.That(numberPosition.FirstNumberPosition, Is.EqualTo(fibonacciNumbers[index - 1]));
            Assert.That(numberPosition.SecondNumberPosition, Is.EqualTo(fibonacciNumbers[index]));
        }

        [Test]
        public async Task FibonacciNextPositionGeneratorService_ShouldReturnZeroAndOnePosition_WhenIndexIsOne()
        {
            //arrange
            var _fibonacciPositionGeneratorService = GetFibonacciPositionGeneratorService();

            //act
            var numberPosition = await _fibonacciPositionGeneratorService.GenerateFibonacciNumberPositionFromIndexAsync(1, default);

            //assert
            Assert.That(numberPosition.FirstNumberPosition, Is.EqualTo(0));
            Assert.That(numberPosition.SecondNumberPosition, Is.EqualTo(1));
        }

        [Test]
        public async Task FibonacciNextPositionGeneratorService_ShouldReturnOneAndZeroPosition_WhenIndexIsZero()
        {
            //arrange
            var _fibonacciPositionGeneratorService = GetFibonacciPositionGeneratorService();

            //act
            var numberPosition = await _fibonacciPositionGeneratorService.GenerateFibonacciNumberPositionFromIndexAsync(0, default);

            //assert
            Assert.That(numberPosition.FirstNumberPosition, Is.EqualTo(1));
            Assert.That(numberPosition.SecondNumberPosition, Is.EqualTo(0));
        }

        [Test]
        public void FibonacciNextPositionGeneratorService_ShouldThrowCancelationExpception_WhenTimeoutOccurs()
        {
            //arrange
            var timeOutMilliSeconds = 200;
            var _nextNumberGeneration = new TestDelayedNumberGeneratorService(timeOutMilliSeconds);
            var _fibonacciPositionGeneratorService = GetFibonacciPositionGeneratorService(_nextNumberGeneration);

            //act
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                using (CancellationTokenSource cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeOutMilliSeconds)))
                {
                    await _fibonacciPositionGeneratorService.GenerateFibonacciNumberPositionFromIndexAsync(5, cancellation.Token);
                }
            });
        }
    }

    class TestDelayedNumberGeneratorService : IFibonacciNextNumberGeneratorService
    {
        readonly int delay;

        public TestDelayedNumberGeneratorService(int delay)
        {
            this.delay = delay;
        }

        public long GenerateNextNumber(long previousNumber, long currentNumber)
        {
            Task.Delay(delay).Wait();
            return previousNumber + currentNumber;
        }
    }
}
