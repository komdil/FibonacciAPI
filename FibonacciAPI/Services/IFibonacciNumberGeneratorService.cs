namespace FibonacciAPI.Services
{
    public interface IFibonacciNumberGeneratorService
    {
        Task<(long numberBeforeFirstPosition, long numberOfFirstPosition)> GenerateFirstPositionAndBeforeFirstPosition(int n);
    }
}
