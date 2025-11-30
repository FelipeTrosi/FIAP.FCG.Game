namespace FIAP.FCG.Game.API.Middleware;

public class LogMiddleware
{
    private readonly RequestDelegate _next;

    public LogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext) 
        => _next(httpContext);
}

