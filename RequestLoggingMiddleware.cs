using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JWTProtectedAPI
{  

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log Request Information (e.g., method, path)
            _logger.LogInformation("Request Information: Method - {Method}, Path - {Path}", context.Request.Method, context.Request.Path);

            // Optionally log the body (if it's a POST, PUT, etc.)
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                context.Request.EnableBuffering(); // Enable buffering to read the body multiple times

                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var body = await reader.ReadToEndAsync();
                    _logger.LogInformation("Request Body: {RequestBody}", body);

                    context.Request.Body.Seek(0, SeekOrigin.Begin); // Rewind the body stream so the next middleware can read it
                }
            }

            // Log Headers (optional)
            foreach (var header in context.Request.Headers)
            {
                _logger.LogInformation("Request Header: {Header} - {Value}", header.Key, header.Value);
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

}
