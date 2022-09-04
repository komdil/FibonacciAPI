namespace FibonacciAPI.Responses
{
    public record FibonacciNumberPosition
    {
        public FibonacciNumberPosition(long firstNumberPosition, long secondNumberPosition)
        {
            FirstNumberPosition = firstNumberPosition;
            SecondNumberPosition = secondNumberPosition;
        }

        public long FirstNumberPosition { get; }

        public long SecondNumberPosition { get; }
    }
}
