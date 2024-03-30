using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ManyInOneAPI.Infrastructure
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Exception occurred : {Messsage}", exception.Message);

            var problemDetails = new ProblemDetails() { };

            if (exception is OperationCanceledException) // TaskCanceledException
            {
                problemDetails.Title = "Request was Canceled!";
                problemDetails.Status = StatusCodes.Status499ClientClosedRequest;
                problemDetails.Instance = "API";
                problemDetails.Detail = $"Error : {exception.Message}";
                    //Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
            }
            else
            {
                    problemDetails.Title = "Server Error !!";
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Instance = "API";
                    problemDetails.Detail = $"Error : {exception.Message}";
                    problemDetails.Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
            }

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            Console.WriteLine(httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken));
            return true;
        }
    }
}
