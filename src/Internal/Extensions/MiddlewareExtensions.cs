using Internal.Middleware.Exceptions;
using Internal.Middleware.Headers;

namespace Internal.Extensions;

internal static class MiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        => builder.UseMiddleware<ExceptionMiddleware>();

    public static IApplicationBuilder UseResponseHeaders(this IApplicationBuilder builder)
        => builder.UseMiddleware<HeadersMiddleware>();
}
