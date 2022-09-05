using FibonacciAPI.Responses;

namespace FibonacciAPI.Services
{
    public interface IFibonacciPositionGeneratorService
    {
        Task<FibonacciNumberPosition> GenerateFibonacciNumberPositionFromIndexAsync(int index, CancellationToken cancellationToken);
    }
}
