using System.Globalization;
using ClinicaJesusDelNorte.API.Models.Enums;

namespace ClinicaJesusDelNorte.API.Models.Common;

/// <summary>
/// Conversión de las horas HH:mm de HorarioMedico y del día de la semana de una fecha.
/// </summary>
public static class Horas
{
    public static TimeOnly Parse(string hhmm) => TimeOnly.ParseExact(hhmm, "HH:mm", CultureInfo.InvariantCulture);

    public static DiaSemana DiaDe(DateTime fecha) => fecha.DayOfWeek switch
    {
        DayOfWeek.Monday => DiaSemana.Lunes,
        DayOfWeek.Tuesday => DiaSemana.Martes,
        DayOfWeek.Wednesday => DiaSemana.Miercoles,
        DayOfWeek.Thursday => DiaSemana.Jueves,
        DayOfWeek.Friday => DiaSemana.Viernes,
        DayOfWeek.Saturday => DiaSemana.Sabado,
        _ => DiaSemana.Domingo
    };

    /// <summary>Minutos desde las 00:00, para comparar bloques sin desbordar la medianoche.</summary>
    public static int Minutos(TimeOnly hora) => hora.Hour * 60 + hora.Minute;
}
