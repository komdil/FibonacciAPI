using FluentValidation;

namespace FibonacciAPI.Queries
{
    public class GetSubsequenceQueryValidator : AbstractValidator<GetSubsequenceQuery>
    {
        public GetSubsequenceQueryValidator()
        {
            RuleFor(x => x.FirstGenerationTimeout).GreaterThan(0);
            RuleFor(x => x.IndexOfFirstNumber);
            RuleFor(x => x.IndexOfLastNumber).GreaterThan(s => s.IndexOfFirstNumber);
            RuleFor(x => x.UseCache);
            RuleFor(x => x.MaxAmountOfMemory).GreaterThan(0);
        }
    }
}
