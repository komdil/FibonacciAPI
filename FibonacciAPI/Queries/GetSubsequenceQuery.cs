namespace FibonacciAPI.Queries
{
    public class GetSubsequenceQuery
    {
        /// <summary>
        /// Timeout of first generation in milliseconds
        /// </summary>
        public int FirstGenerationTimeout { get; set; }

        public int IndexOfFirstNumber { get; set; }

        public int IndexOfLastNumber { get; set; }

        public bool UseCache { get; set; }

        /// <summary>
        /// Max amount of memory of sequence in byte
        /// </summary>
        public int MaxAmountOfMemory { get; set; }
    }
}
