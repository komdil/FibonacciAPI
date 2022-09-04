namespace FibonacciAPI.Services
{
    public class FibonacciNumberGeneratorService : IFibonacciNumberGeneratorService
    {
        public async Task<(long numberBeforeFirstPosition, long numberOfFirstPosition)> GenerateFirstPositionAndBeforeFirstPosition(int n)
        {
            return await Task.Run(() =>
            {
                long firstnumber = 0, secondnumber = 1, result = 0;

                if (n == 0) return (1, 0);
                if (n == 1) return (0, 1);

                for (int i = 2; i <= n; i++)
                {
                    result = firstnumber + secondnumber;
                    firstnumber = secondnumber;
                    secondnumber = result;
                }

                return (firstnumber, result);
            });
        }
    }
}
