// Free1X2 · WinUI 3 — WIN3
namespace Free1X2.WinUI.Services;

/// <summary>Precio de la apuesta, % al 14, recaudación y moneda de un juego (L.A.E. o Progol).</summary>
public sealed record ParametrosLAE(double Precio, double Porcentaje14, double Recaudacion, string Moneda);

/// <summary>
/// Parámetros de Configuración ▸ Parámetros LAE que usan los cálculos (W14–W16), en lugar de las constantes del
/// original (0,50 € y 14.000.000 €). <see cref="Espana"/> lee <c>parametros.free1x2</c> (VariablesGlobales, que se
/// recarga al guardar Configuración).
/// </summary>
public static class ParametrosLAEActuales
{
    /// <summary>Los de España (L.A.E.): lo que lee o escribe <c>Jornadas/InfoJornadasLAE.txt</c> usa siempre estos.</summary>
    public static ParametrosLAE Espana() => new(
        Free1X2.VariablesGlobales.PrecioApuesta,
        Free1X2.VariablesGlobales.Porcentaje14,
        Free1X2.VariablesGlobales.Recaudacion,
        Free1X2.VariablesGlobales.Moneda);

    /// <summary>Los de los cálculos de la combinación (Esperanza, Rentabilidad, Tramificar, Banco de pruebas…).</summary>
    public static ParametrosLAE Calculo() => Espana();
}
