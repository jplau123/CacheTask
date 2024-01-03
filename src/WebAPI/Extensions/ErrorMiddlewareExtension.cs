using WebAPI.Middlewares;

namespace WebAPI.Extensions;

public static class ErrorMiddlewareExtension
{
    public static IApplicationBuilder UseErrorMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorMiddleware>();
    }
}
