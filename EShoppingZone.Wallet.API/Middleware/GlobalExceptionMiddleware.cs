namespace EShoppingZone.Wallet.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                UnauthorizedAccessException => (401, "Unauthorized access."),
                KeyNotFoundException => (404, ex.Message),
                ArgumentException => (400, ex.Message),
                _ => (500, "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = statusCode;

            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                statusCode,
                message
            });

            return context.Response.WriteAsync(result);
        }
    }
}