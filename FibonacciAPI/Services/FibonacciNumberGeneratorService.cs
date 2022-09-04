using FibonacciAPI.Responses;

namespace FibonacciAPI.Services
{
    public class FibonacciNumberGeneratorService : IFibonacciNumberGeneratorService
    {
        public async Task<FibonacciNumberPosition> GenerateFibonacciNumberPositionFromIndex(int index)
        {
            return await Task.Run(() =>
            {
                long firstnumber = 0, secondnumber = 1, result = 0;

                if (index == 0) return new FibonacciNumberPosition(1, 0);
                if (index == 1) return new FibonacciNumberPosition(0, 1);

                for (int i = 2; i <= index; i++)
                {
                    result = firstnumber + secondnumber;
                    firstnumber = secondnumber;
                    secondnumber = result;
                }

                return new FibonacciNumberPosition(firstnumber, result);
            });
        }
    }
}
