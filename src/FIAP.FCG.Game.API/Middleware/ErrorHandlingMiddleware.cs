using FIAP.FCG.Game.Infrastructure.Logger;
using FIAP.FCG.Game.Service.Exceptions;

namespace FIAP.FCG.Game.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IBaseLogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, IBaseLogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning($"Erro de validação: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex.Message,
                errors = ex.Errors
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning($"Recurso não encontrado: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning($"Acesso não autorizado: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (ForbiddenException ex)
        {
            _logger.LogWarning($"Acesso proibido: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro interno não tratado: {ex.Message}");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Erro interno no servidor. Tente novamente mais tarde."
            });
        }
    }
}
