namespace FibonacciAPI.Cache;

public class FibonnaciCacheEntry
{
    public FibonnaciCacheEntry(int indexOfFirstProperty, int indexOfLastProperty, List<long> sequence)
    {
        IndexOfFirstProperty = indexOfFirstProperty;
        IndexOfLastProperty = indexOfLastProperty;
        Sequence = sequence;
    }

    public int IndexOfFirstProperty { get; }
    public int IndexOfLastProperty { get; }
    public List<long> Sequence { get; }
}