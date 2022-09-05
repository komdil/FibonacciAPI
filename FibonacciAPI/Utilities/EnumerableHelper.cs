namespace FibonacciAPI.Utilities
{
    public static class EnumerableHelper
    {
        public static List<long> RangeLong(long start, long end)
        {
            List<long> longs = new();

            for (long i = start; i <= end; i++)
            {
                longs.Add(i);
            }
            return longs;
        }
    }
}
