namespace FibonacciAPI.Cache;

public class FibonnaciCacheEntry
{
    public int StartValue { get; set; }
    public int EndValue { get; set; }
    public List<long> Sequence { get; set; }
}