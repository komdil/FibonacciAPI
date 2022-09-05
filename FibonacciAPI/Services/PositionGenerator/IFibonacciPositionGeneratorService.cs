namespace FibonacciAPI.Services.PositionGenerator
{
    public interface IFibonacciPositionGeneratorService
    {
        Task<FibonacciNumberPosition> GenerateFibonacciNumberPositionFromIndexAsync(int index, CancellationToken cancellationToken);
    }
}
