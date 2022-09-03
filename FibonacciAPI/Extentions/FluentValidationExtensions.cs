using FibonacciAPI.Responses;
using FluentValidation.Results;

namespace FibonacciAPI.Extentions
{
    public static class FluentValidationExtensions
    {
        public static List<ErrorResponse> GetErrors(this ValidationResult validationResult)
        {
            return validationResult.Errors
         .GroupBy(x => x.PropertyName)
         .Select(s => new ErrorResponse(s.Key, s.Select(s => s.ErrorMessage).ToList())).ToList();
        }
    }
}
