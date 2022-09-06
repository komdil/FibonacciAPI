namespace FibonacciAPI.Services.PositionGenerator
{
    public class FibonacciPositionGeneratorService : IFibonacciPositionGeneratorService
    {
        IFibonacciNextNumberGeneratorService _fibonacciNextNumberGeneratorService;
        public FibonacciPositionGeneratorService(IFibonacciNextNumberGeneratorService fibonacciNextNumberGeneratorService)
        {
            _fibonacciNextNumberGeneratorService = fibonacciNextNumberGeneratorService;
        }

        public async Task<FibonacciNumberPosition> GenerateFibonacciNumberPositionFromIndexAsync(int index,
            CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                long firstnumber = 0, secondnumber = 1, result = 0;

                if (index == 0) return new FibonacciNumberPosition(1, 0);
                if (index == 1) return new FibonacciNumberPosition(0, 1);

                for (int i = 2; i <= index; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    result = await Task.Factory.StartNew(() =>
                    _fibonacciNextNumberGeneratorService.GenerateNextNumber(firstnumber, secondnumber));
                    firstnumber = secondnumber;
                    secondnumber = result;
                }

                return new FibonacciNumberPosition(firstnumber, result);
            });
        }
    }
}
