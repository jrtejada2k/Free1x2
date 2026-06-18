// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Online;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Descarga de boleto" (legacy: DescargaBoletoFrm), reactivada como
/// la integración ONLINE OPCIONAL con clubprogol.com (docs/API_CLUBPROGOL.md):
///
///   1. El usuario elige país (España / México) — único control nuevo (diseño aprobado).
///   2. La acción "Actualizar / Descargar" llama a
///      <see cref="QuinielaOnlineService.ObtenerJornadaAsync(string, CancellationToken)"/>,
///      que descarga y parsea la jornada vigente fuera del hilo de UI.
///   3. En caso de éxito se guarda en <see cref="AppState.JornadaActual"/> (fuente compartida
///      de los nombres reales) y se rellenan los 14 partidos del boleto; se muestra el resumen
///      "Jornada N · 14 partidos cargados".
///   4. Sin conexión / dato inválido → mensaje claro y modo manual (fallback offline; no rompe nada).
///
/// El legacy no tenía servicio (era un stub que devolvía "" y siempre mostraba "no disponible");
/// esta versión es funcional contra el JSON de muestra y, cuando exista el backend, solo cambia
/// la base URL (config o variable de entorno FREE1X2_API_BASE).
/// </summary>
public partial class DescargaBoletoFrmViewModel : ObservableObject
{
    private readonly QuinielaOnlineService _servicio = new();

    /// <summary>Opción de país para el selector (texto visible + código "es"/"mx").</summary>
    public sealed record OpcionPais(string Nombre, string Codigo)
    {
        public override string ToString() => Nombre;
    }

    /// <summary>Países disponibles para el selector (España / México). Diseño aprobado.</summary>
    public IReadOnlyList<OpcionPais> Paises { get; } = new List<OpcionPais>
    {
        new OpcionPais("España", "es"),
        new OpcionPais("México", "mx"),
    };

    [ObservableProperty]
    private OpcionPais _paisSeleccionado;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    /// <summary>True mientras se descarga (deshabilita el botón en la vista).</summary>
    [ObservableProperty]
    private bool _descargando;

    public DescargaBoletoFrmViewModel()
    {
        // España por defecto (la quiniela "clásica" de la app).
        _paisSeleccionado = Paises[0];
    }

    /// <summary>
    /// Descarga la jornada vigente del país seleccionado, la publica en
    /// <see cref="AppState.JornadaActual"/> y rellena el boleto. La descarga ocurre fuera del
    /// hilo de UI (await sobre HttpClient); sin DoEvents. Cualquier fallo cae al mensaje
    /// offline amigable sin romper la pantalla.
    /// </summary>
    [RelayCommand]
    private async Task Descargar()
    {
        if (Descargando) return;

        string pais = PaisSeleccionado?.Codigo ?? "es";
        Mensaje = "Conectando con el servicio online…";
        Descargando = true;
        try
        {
            JornadaQuiniela jornada = await _servicio
                .ObtenerJornadaAsync(pais, CancellationToken.None)
                .ConfigureAwait(true); // continúa en el hilo de UI para tocar AppState/binding

            // Fuente compartida de nombres reales: boleto y "Grupos de Equipos" leen de aquí.
            AppState.Instancia.JornadaActual = jornada;

            Mensaje = "Jornada " + jornada.Jornada + " · " + jornada.Partidos.Count + " partidos cargados";
        }
        catch (QuinielaOnlineException ex)
        {
            // Fallback offline: mensaje claro, modo manual; no se inventan datos.
            Mensaje = ex.Message + " El boleto sigue en modo manual.";
        }
        catch (Exception ex)
        {
            Mensaje = "No se pudo descargar la jornada: " + ex.Message + " El boleto sigue en modo manual.";
        }
        finally
        {
            Descargando = false;
        }
    }
}
