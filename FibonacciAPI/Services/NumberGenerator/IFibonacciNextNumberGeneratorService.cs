namespace FibonacciAPI.Services.NumberGenerator
{
    public interface IFibonacciNextNumberGeneratorService
    {
        long GenerateNextNumber(long previousNumber, long currentNumber);
    }
}
