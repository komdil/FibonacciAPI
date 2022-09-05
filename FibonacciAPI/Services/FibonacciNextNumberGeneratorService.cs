namespace FibonacciAPI.Services
{
    public class FibonacciNextNumberGeneratorService : IFibonacciNextNumberGeneratorService
    {
        public long GenerateNextNumber(long previousNumber, long currentNumber)
        {
            return previousNumber + currentNumber;
        }
    }
}
