using ClinicaJesusDelNorte.API.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace ClinicaJesusDelNorte.API.Middleware;

/// <summary>
/// Convierte los errores de validación del modelo en una respuesta ApiResponse.
/// </summary>
public static class ValidationResponseFactory
{
    private const string CuerpoInvalido = "El cuerpo de la solicitud es obligatorio y debe ser un JSON válido.";

    public static IActionResult Crear(ActionContext context)
    {
        var errors = context.ModelState
            .SelectMany(entry => entry.Value!.Errors.Select(error => Traducir(entry.Key, error.ErrorMessage)))
            .OfType<string>()
            .Distinct()
            .ToList();

        if (errors.Count == 0)
        {
            errors.Add(CuerpoInvalido);
        }

        return new BadRequestObjectResult(
            ApiResponse<object>.Fail("Error de validación de datos de entrada.", errors));
    }

    private static string? Traducir(string campo, string mensaje)
    {
        // "dto" y "" son errores implícitos de un cuerpo vacío o ilegible.
        if (campo is "dto" or "" or "$")
        {
            return null;
        }

        if (campo.StartsWith("$."))
        {
            return $"El valor del campo {campo[2..]} no es válido.";
        }

        return string.IsNullOrWhiteSpace(mensaje) ? $"El valor del campo {campo} no es válido." : mensaje;
    }
}
