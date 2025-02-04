using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
namespace JWTProtectedAPI
{
  
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An unhandled exception occurred.");

                // Set the status code to 500 (Internal Server Error)
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // Create an error response object
                var errorResponse = new
                {
                    Message = "An unexpected error occurred.",
                    Details = ex.Message // You can add more details or hide them in production
                };

                // Set content type to JSON
                context.Response.ContentType = "application/json";

                // Return the error response as JSON
                var responseJson = JsonConvert.SerializeObject(errorResponse);
                await context.Response.WriteAsync(responseJson);
            }
        }
    }

}
