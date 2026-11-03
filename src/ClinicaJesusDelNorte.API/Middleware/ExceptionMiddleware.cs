using ClinicaJesusDelNorte.API.Exceptions;
using ClinicaJesusDelNorte.API.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ClinicaJesusDelNorte.API.Middleware;

/// <summary>
/// Manejo centralizado de excepciones y de errores HTTP sin cuerpo,
/// para que toda respuesta de error tenga la misma estructura.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly JsonOptions _jsonOptions;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IOptions<JsonOptions> jsonOptions)
    {
        _next = next;
        _logger = logger;
        _jsonOptions = jsonOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (!context.Response.HasStarted)
            {
                var message = context.Response.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => "Debe iniciar sesión: envíe el token en la cabecera Authorization: Bearer {token}.",
                    StatusCodes.Status403Forbidden => "Su rol no tiene permiso para esta operación.",
                    StatusCodes.Status404NotFound => "El recurso solicitado no existe.",
                    StatusCodes.Status405MethodNotAllowed => "El método HTTP no está permitido en esta ruta.",
                    StatusCodes.Status415UnsupportedMediaType => "El tipo de contenido no es compatible. Use application/json.",
                    _ => null
                };

                if (message is not null)
                {
                    await EscribirErrorAsync(context, context.Response.StatusCode, message);
                }
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, exception.Message),
            BusinessException => (StatusCodes.Status422UnprocessableEntity, exception.Message),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, exception.Message),
            ForbiddenException => (StatusCodes.Status403Forbidden, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Error no controlado en la API CitaSync");
        }
        else
        {
            _logger.LogWarning("Solicitud rechazada ({StatusCode}): {Message}", statusCode, message);
        }

        if (context.Response.HasStarted)
        {
            _logger.LogWarning("La respuesta ya había comenzado; no se pudo escribir el error.");
            return;
        }

        await EscribirErrorAsync(context, statusCode, message);
    }

    private Task EscribirErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail(message),
            _jsonOptions.JsonSerializerOptions);
    }
}
