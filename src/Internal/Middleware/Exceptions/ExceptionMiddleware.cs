using Internal.Models;

namespace Internal.Middleware.Exceptions;

internal class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex?.InnerException?.Message ?? ex?.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(new Problem()
        {
            Status = context.Response.StatusCode,
            Detail = exception?.InnerException?.Message ?? exception?.Message,
            Type = exception?.InnerException?.GetType().Name ?? exception?.GetType().Name
        }.ToString());
    }
}