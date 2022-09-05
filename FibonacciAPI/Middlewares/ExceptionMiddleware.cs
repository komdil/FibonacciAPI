using FibonacciAPI.Responses;
using FibonacciAPI.Utilities;
using Newtonsoft.Json;
using System.Net;

namespace FibonacciAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                await HandleExceptionAsync(httpContext);
            }
        }

        async Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(GetUnhandledErrorString());
        }

        string GetUnhandledErrorString()
        {
            var errorResponse = new ErrorResponse(Constants.UnhandledErrorUnknownProperty, new List<string>
            {
                Constants.UnhandledErrorMessage
            });
            var serverResponse = ServerResponse<string>.GetFailResponse(new List<ErrorResponse>() { errorResponse });
            return JsonConvert.SerializeObject(serverResponse, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
        }
    }
}
