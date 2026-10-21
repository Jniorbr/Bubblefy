using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bubblefy.WebApi.Middleware;

internal sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisicao invalida"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflito"),
            _ => (0, string.Empty)
        };

        if (statusCode == 0)
            return false;

        logger.LogWarning(exception, "Erro de requisicao tratado com status {StatusCode}.", statusCode);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        }, cancellationToken);
        return true;
    }
}
