using FluentValidation;

namespace FibonacciAPI.Queries
{
    public class GetSubsequenceQueryValidator : AbstractValidator<GetSubsequenceQuery>
    {
        public GetSubsequenceQueryValidator()
        {
            RuleFor(x => x.FirstGenerationTimeout).NotNull().GreaterThan(0);
            RuleFor(x => x.IndexOfFirstNumber).NotNull();
            RuleFor(x => x.IndexOfLastNumber).NotNull().GreaterThan(s => s.IndexOfFirstNumber);
            RuleFor(x => x.UseCache).NotNull();
            RuleFor(x => x.MaxAmountOfMemory).NotNull().GreaterThan(0);
        }
    }
}
