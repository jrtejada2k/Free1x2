// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Free1X2.Mascaras;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Máscaras y boleto de Inicio (W9, DT-84): guardar los signos del boleto base como máscara y
/// aplicar una guardada. Lo usan el menú «Máscaras ▾» de Inicio y la pantalla «Máscaras». Los
/// ficheros van en la carpeta de datos (junto a Equipos/), en <c>Mascaras/&lt;nombre&gt;.mas</c>.
/// </summary>
public static class MascarasInicio
{
    /// <summary>Carpeta de datos del programa (la de Equipos/, Temp/, Ganadoras/…).</summary>
    public static string CarpetaDatos => AppContext.BaseDirectory;

    public static int NumeroPartidos => Free1X2.VariablesGlobales.NumeroPartidos;

    public static List<Mascara> Listar() => CasosMascaras.Listar(NumeroPartidos, CarpetaDatos);

    /// <summary>Los signos del boleto base (los del Analizador compartido) como máscara.</summary>
    public static Mascara DelBoleto(string nombre)
    {
        string[]? pronosticos = AppState.Instancia.Analizador.Pronosticos;
        var p = new string[NumeroPartidos];
        for (int i = 0; i < p.Length; i++)
        {
            string? pr = pronosticos != null && i < pronosticos.Length ? pronosticos[i] : null;
            p[i] = string.IsNullOrEmpty(pr) ? "1,X,2" : pr;
        }
        return new Mascara(nombre, p);
    }

    /// <summary>Pone los signos de la máscara en el boleto y deja los equipos como estén.</summary>
    public static void Aplicar(Mascara m)
    {
        var analizador = AppState.Instancia.Analizador;
        for (int i = 0; i < m.Pronosticos.Count && i < NumeroPartidos; i++)
            analizador.SetPronostico(i, m.Pronosticos[i]);
        AppState.Instancia.NotificarCambio();
    }

    /// <summary>
    /// «Guardar boleto como máscara…»: pide el nombre, confirma si ya existe y guarda. Devuelve si
    /// se guardó.
    /// </summary>
    public static async Task<bool> GuardarBoletoComoMascaraAsync()
    {
        string? texto = await AppServices.PedirTextoAsync(
            "Guardar boleto como máscara",
            "Se guardan los signos de los " + NumeroPartidos + " partidos, sin los equipos.",
            "Nombre", "Guardar");
        if (texto is null) return false;

        string nombre = CasosMascaras.NombreValido(texto) ?? texto;
        if (CasosMascaras.Existe(nombre, CarpetaDatos) &&
            !await AppServices.ConfirmarAsync("Ya hay una máscara «" + nombre + "». ¿Reemplazarla?"))
            return false;

        try
        {
            CasosMascaras.Guardar(DelBoleto(nombre), CarpetaDatos);
            AppServices.MostrarInfo("Máscara «" + nombre + "» guardada.");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error("MascarasInicio.GuardarBoletoComoMascaraAsync", ex);
            AppServices.MostrarError("No se pudo guardar la máscara: " + ex.Message);
            return false;
        }
    }

    /// <summary>Texto de la entrada del menú: «nombre  ·  N col.» (dos espacios a cada lado del punto).</summary>
    public static string TextoMenu(Mascara m) => m.Nombre + "  ·  " + m.Columnas + " col.";
}
