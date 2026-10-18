// Free1X2 · WinUI 3 — WIN3
using System;
using Free1X2.EntradaSalida;

namespace Free1X2.WinUI.Services;

/// <summary>Precio de la apuesta, % al 14, recaudación y moneda de un juego (L.A.E. o Progol).</summary>
public sealed record ParametrosLAE(double Precio, double Porcentaje14, double Recaudacion, string Moneda);

/// <summary>
/// Parámetros de Configuración que usan los cálculos (W14–W16, W21), en lugar de las constantes del original
/// (0,50 € y 14.000.000 €). <see cref="Espana"/> = Parámetros LAE (VariablesGlobales, que se recarga al guardar
/// Configuración); <see cref="Mexico"/> = Parámetros Progol; <see cref="Calculo"/> = los del país de la jornada cargada.
/// </summary>
public static class ParametrosLAEActuales
{
    /// <summary>Los de España (L.A.E.): lo que lee o escribe <c>Jornadas/InfoJornadasLAE.txt</c> usa siempre estos.</summary>
    public static ParametrosLAE Espana() => new(
        Free1X2.VariablesGlobales.PrecioApuesta,
        Free1X2.VariablesGlobales.Porcentaje14,
        Free1X2.VariablesGlobales.Recaudacion,
        Free1X2.VariablesGlobales.Moneda);

    /// <summary>W21 (DT-97): los del Progol (México), de <c>parametros.free1x2</c>; sin el nodo, los de fábrica.</summary>
    public static ParametrosLAE Mexico()
    {
        try
        {
            new AConfiguracion().ObtenValoresProgol(out double precio, out double pct, out double rec, out string moneda);
            return new ParametrosLAE(precio, pct, rec, moneda);
        }
        catch (Exception ex)
        {
            Log.Error("ParametrosLAEActuales.Mexico", ex);
            return new ParametrosLAE(AConfiguracion.ProgolPrecioFabrica, AConfiguracion.ProgolPorcentaje14Fabrica,
                AConfiguracion.ProgolRecaudacionFabrica, AConfiguracion.ProgolMonedaFabrica);
        }
    }

    /// <summary>True si la jornada cargada (Inicio ▸ Jornada, o la del arranque) es de México.</summary>
    public static bool JornadaDeMexico =>
        string.Equals(AppState.Instancia.JornadaActual?.Pais, "mx", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// W21 (DT-97): los de los cálculos de la combinación (Calcular, Rentabilidad, Esperanza, Ordenar por probabilidad,
    /// Tramificar, Banco de pruebas): los del país de la jornada cargada; sin jornada, España.
    /// </summary>
    public static ParametrosLAE Calculo() => JornadaDeMexico ? Mexico() : Espana();
}
