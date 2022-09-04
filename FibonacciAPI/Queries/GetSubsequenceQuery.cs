namespace FibonacciAPI.Queries
{
    public class GetSubsequenceQuery
    {
        public int? FirstGenerationTimeout { get; set; }

        public int? IndexOfFirstNumber { get; set; }

        public int? IndexOfLastNumber { get; set; }

        public bool? UseCache { get; set; }

        public int? MaxAmountOfMemory { get; set; }
    }
}
