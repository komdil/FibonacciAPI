using FibonacciAPI.Responses;

namespace FibonacciAPI.Services
{
    public interface IFibonacciNumberGeneratorService
    {
        Task<FibonacciNumberPosition> GenerateFibonacciNumberPositionFromIndex(int n);
    }
}
