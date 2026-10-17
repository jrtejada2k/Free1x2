// Free1X2 · WinUI 3 — WIN3
using System;
using System.IO;
using System.Threading.Tasks;
using Free1X2.MotorCalculo;

namespace Free1X2.WinUI.Services;

/// <summary>
/// «Columnas de Inicio» (W10, DT-85): desarrolla el boleto de Inicio con sus condiciones (y el
/// filtro de columnas si existe) en <c>Temp/ColumnasInicio.txt</c> para que el Reductor y los
/// filtros lo usen de entrada sin guardar antes un fichero. Mismos textos que la app Mac
/// (<c>Sesion.columnasDeInicio</c>).
/// </summary>
public static class ColumnasInicioServicio
{
    public const string TextoBoton = "Columnas de Inicio";
    public const string TextoGenerando = "Generando columnas…";
    public const string Ayuda = "Usar las columnas del boleto de Inicio, con sus condiciones, sin guardar antes un fichero";

    public static string CarpetaTemp => Path.Combine(AppContext.BaseDirectory, "Temp");

    /// <summary>
    /// Ruta del fichero generado, o null si el usuario no confirma (más de 1 000 000 columnas) o
    /// hay un error (se avisa).
    /// </summary>
    public static async Task<string?> GenerarAsync()
    {
        var estado = AppState.Instancia;
        var analizador = estado.Analizador;

        long total = ColumnasInicio.ColumnasSinCondiciones(analizador.Pronosticos ?? Array.Empty<string>());
        if (total > ColumnasInicio.LimiteSinAviso &&
            !await AppServices.ConfirmarAsync("El boleto desarrolla " + total +
                " columnas antes de aplicar las condiciones y puede tardar. ¿Continuar?"))
            return null;

        // Como «Calcular»: con filtro de columnas, las columnas salen de su fichero.
        string filtro = estado.ArchivoFiltroCols;
        analizador.ArchivoColumnasBase = filtro.Length > 0 && File.Exists(filtro) ? filtro : "";

        try
        {
            string ruta = "";
            await Task.Run(() => ColumnasInicio.Generar(analizador, CarpetaTemp, out ruta));
            return ruta;
        }
        catch (Exception ex)
        {
            Log.Error("ColumnasInicioServicio.GenerarAsync", ex);
            AppServices.MostrarError("No se pudieron generar las columnas de Inicio: " + ex.Message);
            return null;
        }
    }
}
