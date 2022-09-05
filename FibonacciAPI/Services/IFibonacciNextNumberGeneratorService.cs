namespace FibonacciAPI.Services
{
    public interface IFibonacciNextNumberGeneratorService
    {
        long GenerateNextNumber(long previousNumber, long currentNumber);
    }
}
