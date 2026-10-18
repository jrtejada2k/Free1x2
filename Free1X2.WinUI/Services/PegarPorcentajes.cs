// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Free1X2.Utils;
using Windows.ApplicationModel.DataTransfer;

namespace Free1X2.WinUI.Services;

/// <summary>
/// «Pegar desde Excel» (W15, DT-92) de las tablas de porcentajes 1 / X / 2: lee el texto del portapapeles con la regla
/// de <see cref="PorcentajesPegados"/> y avisa con los mismos textos que la app Mac. Lo usan PorcentajesControl (todas
/// las tablas comunes) y las rejillas propias de Pim, Generador de CPs y Modificador.
/// </summary>
public static class PegarPorcentajes
{
    public const string TextoBoton = "Pegar desde Excel";

    public const string Ayuda = "Copia en Excel las filas de porcentajes (una por partido: 1, X y 2; pueden llevar el " +
                                "partido o los equipos delante) y pulsa aquí.";

    /// <summary>
    /// Filas leídas del portapapeles (como mucho <paramref name="filas"/>), o null si no hay nada válido (ya avisado).
    /// Si llegan menos filas que partidos, avisa de que el resto queda como estaba.
    /// </summary>
    public static async Task<List<double[]>?> LeerAsync(int filas)
    {
        string texto = "";
        try
        {
            DataPackageView contenido = Clipboard.GetContent();
            if (contenido.Contains(StandardDataFormats.Text))
                texto = await contenido.GetTextAsync();
        }
        catch (Exception ex)
        {
            Log.Error("PegarPorcentajes.LeerAsync", ex);
        }

        if (string.IsNullOrWhiteSpace(texto))
        {
            AppServices.MostrarError(PorcentajesPegados.MensajeVacio);
            return null;
        }

        try
        {
            var r = PorcentajesPegados.Leer(texto, filas);
            if (r.Count < filas)
                AppServices.MostrarInfo("Se pegaron " + r.Count + " de " + filas + " filas; el resto queda como estaba.");
            return r;
        }
        catch (FormatException ex)
        {
            AppServices.MostrarError(ex.Message);
            return null;
        }
    }
}
