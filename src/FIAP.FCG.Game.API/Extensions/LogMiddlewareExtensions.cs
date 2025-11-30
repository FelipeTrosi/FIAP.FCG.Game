using FIAP.FCG.Game.API.Middleware;

namespace FIAP.FCG.Game.API.Extensions;

public static class LogMiddlewareExtensions
{
    public static IApplicationBuilder UseLogMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<LogMiddleware>();

}