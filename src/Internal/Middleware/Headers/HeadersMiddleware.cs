namespace Internal.Middleware.Headers;

internal class HeadersMiddleware
{
    private readonly RequestDelegate _next;

    public HeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        httpContext.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        httpContext.Response.Headers.Append("Content-Security-Policy", "default-src 'self'");
        httpContext.Response.Headers.Append("Expect-CT", "max-age=31556952");
        httpContext.Response.Headers.Append("X-Xss-Protection", "1; mode=block");

        await _next(httpContext);
    }
}
