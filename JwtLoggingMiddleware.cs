namespace JWTProtectedAPI
{
    public class JwtLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                // Log the Authorization header to see the token
                Console.WriteLine("Authorization Header: " + authorizationHeader);
            }

            // Continue to the next middleware
            await _next(context);
        }
    }

}
