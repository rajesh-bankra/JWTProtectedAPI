using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace JWTProtectedAPI
{
   
    public class SanitizeInputMiddleware
    {
        private readonly RequestDelegate _next;

        public SanitizeInputMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Example of input sanitization (trimming and filtering out unwanted characters)
            if (context.Request.ContentType?.Contains("application/json") == true)
            {
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();

                // Example sanitization (remove unwanted characters or SQL injections)
                body = SanitizeInput(body);

                // Rewind the body stream so that the next middleware can read it again.
                context.Request.Body = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }

        private static string SanitizeInput(string input)
        {
            // Example of sanitizing input, you can modify it to your needs
            // Remove any potentially dangerous characters (e.g., <, >, ;, --)
            var sanitized = input.Replace("<", "").Replace(">", "").Replace(";", "").Replace("--", "");

            // You can add more complex sanitization rules as per your requirements.
            return sanitized;
        }
    }

}
