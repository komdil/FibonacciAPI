using FibonacciAPI.Extentions;
using FibonacciAPI.Queries;
using FibonacciAPI.Responses;
using FluentValidation;

namespace FibonacciAPI.Services
{
    public class FibonacciSequenceService : IFibonacciSequenceService
    {
        private readonly IValidator<GetSubsequenceQuery> _fibonacciSequenceValidator;
        public FibonacciSequenceService(IValidator<GetSubsequenceQuery> validator)
        {
            _fibonacciSequenceValidator = validator;
        }

        public async Task<ServerResponse<List<int>>> GetSubsequence(GetSubsequenceQuery query)
        {
            throw new Exception("Bitch");
            var validateStatus = _fibonacciSequenceValidator.Validate(query);
            if (!validateStatus.IsValid)
                return new ServerResponse<List<int>>(validateStatus.GetErrors());
            return null;
        }
    }
}
