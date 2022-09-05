using FibonacciAPI.Responses;

namespace FibonacciAPI.Services
{
    public interface IFibonacciNumberGeneratorService
    {
        Task<FibonacciNumberPosition> GenerateFibonacciNumberPositionFromIndexAsync(int n);
    }
}
