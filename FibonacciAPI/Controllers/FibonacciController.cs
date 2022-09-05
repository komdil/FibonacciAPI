using FibonacciAPI.Queries;
using FibonacciAPI.Responses;
using FibonacciAPI.Services.SequenceGenerator;
using FibonacciAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FibonacciAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FibonacciController : ControllerBase
    {
        private readonly IFibonacciSequenceService _fibonacciSequenceService;

        public FibonacciController(IFibonacciSequenceService fibonacciSequenceService)
        {
            _fibonacciSequenceService = fibonacciSequenceService;
        }

        [HttpGet(Name = "Numbers")]
        public async Task<ServerResponse<List<long>>> GetNumbers([FromQuery] GetSubsequenceQuery getSubsequenceQuery)
        {
            return await _fibonacciSequenceService.GetSubsequenceAsync(getSubsequenceQuery);
        }
    }
}