namespace FibonacciAPI.Services.NumberGenerator
{
    public class FibonacciNextNumberGeneratorService : IFibonacciNextNumberGeneratorService
    {
        public long GenerateNextNumber(long previousNumber, long currentNumber)
        {
            return previousNumber + currentNumber;
        }
    }
}
