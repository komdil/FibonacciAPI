namespace FibonacciAPI.Utilities
{
    public static class EnumerableHelper
    {
        public static List<long> RangeLong(int start, int end)
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
