using FibonacciAPI.Responses;
using FibonacciAPI.Utilities;
using Newtonsoft.Json;
using System.Net;

namespace FibonacciAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApplicationLog> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ApplicationLog> logger)
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
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsync(GetUnhundledErrorString());
        }

        string GetUnhundledErrorString()
        {
            var errorResponse = new ErrorResponse(Constants.UnhundledErrorUnknownProperty, new List<string>
            {
                Constants.UnhundledErrorMessage
            });
            var serverResponse = ServerResponse<string>.GetFailResponse(new List<ErrorResponse>() { errorResponse });
            return JsonConvert.SerializeObject(serverResponse, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
        }
    }
}
