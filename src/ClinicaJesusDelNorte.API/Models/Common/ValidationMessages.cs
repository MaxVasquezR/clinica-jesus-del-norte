namespace ClinicaJesusDelNorte.API.Models.Common;

/// <summary>
/// Mensajes de validación compartidos por los DTOs. {0} es el nombre del campo.
/// </summary>
public static class ValidationMessages
{
    public const string Obligatorio = "El campo {0} es obligatorio.";
    public const string LongitudMaxima = "El campo {0} admite como máximo {1} caracteres.";
    public const string LongitudRango = "El campo {0} debe tener entre {2} y {1} caracteres.";
    public const string Rango = "El campo {0} debe estar entre {1} y {2}.";
    public const string Correo = "El campo {0} no contiene un correo electrónico válido.";
    public const string ValorNoPermitido = "El campo {0} tiene un valor no permitido.";
    public const string Hora = "El campo {0} debe tener el formato HH:mm (00:00 a 23:59).";
    public const string Telefono = "El campo {0} solo admite de 7 a 15 dígitos, con + inicial opcional.";
}
