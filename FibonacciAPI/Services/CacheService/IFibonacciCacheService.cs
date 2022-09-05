namespace FibonacciAPI.Services.CacheService
{
    public interface IFibonacciCacheService
    {
        bool TryGetValue(GetSubsequenceQuery query, out List<long> numbers);

        void Set(GetSubsequenceQuery query, List<long> numbers);
    }
}
