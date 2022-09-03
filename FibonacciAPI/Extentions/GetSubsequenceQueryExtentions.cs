using FibonacciAPI.Queries;

namespace FibonacciAPI.Extentions
{
    public static class GetSubsequenceQueryExtentions
    {
        public static string GetCacheKey(this GetSubsequenceQuery query)
        {
            return $"{query.IndexOfFirstNumber},{query.IndexOfLastNumber},{query.MaxAmountOfMemory}";
        }
    }
}
