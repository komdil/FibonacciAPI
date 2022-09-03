using FibonacciAPI.Queries;
using FibonacciAPI.Responses;
using FibonacciAPI.Services;
using FibonacciAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FibonacciAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FibonacciController : ControllerBase
    {
        private readonly IFibonacciSequenceService _fibonacciSequenceService;

        private readonly ILogger<ApplicationLog> _logger;

        public FibonacciController(ILogger<ApplicationLog> logger, IFibonacciSequenceService fibonacciSequenceService)
        {
            _logger = logger;
            _fibonacciSequenceService = fibonacciSequenceService;
        }

        [HttpGet(Name = "Numbers")]
        public async Task<ServerResponse<List<int>>> GetNumbers([FromQuery] GetSubsequenceQuery getSubsequenceQuery)
        {
            return await _fibonacciSequenceService.GetSubsequence(getSubsequenceQuery);
        }
    }
}