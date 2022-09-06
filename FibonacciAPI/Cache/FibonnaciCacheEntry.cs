namespace FibonacciAPI.Cache;

public class FibonnaciCacheEntry
{
    public FibonnaciCacheEntry(int indexOfFirstProperty, int indexOfLastProperty, List<long> sequence)
    {
        IndexOfFirstProperty = indexOfFirstProperty;
        IndexOfLastProperty = indexOfLastProperty;
        Sequence = sequence;
    }

    public int IndexOfFirstProperty { get; set; }
    public int IndexOfLastProperty { get; set; }
    public List<long> Sequence { get; }
}