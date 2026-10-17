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
    // Instancia única compartida del cliente HTTP (C-08): todo su estado ya era estático.
    private readonly QuinielaOnlineService _servicio = QuinielaOnlineService.Instancia;

    /// <summary>Países disponibles para el selector (España / México). Catálogo compartido (C-10).</summary>
    public IReadOnlyList<OpcionPais> Paises => PaisesOnline.Todos;

    [ObservableProperty]
    private OpcionPais _paisSeleccionado;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    /// <summary>True mientras se descarga (deshabilita el botón en la vista).</summary>
    [ObservableProperty]
    private bool _descargando;

    public DescargaBoletoFrmViewModel()
    {
        // País inicial: el de la última jornada cacheada (el que se sembró al arrancar), para que
        // el selector y el seed estén alineados; si no hay caché, España por defecto (la quiniela
        // "clásica" de la app). Es una lectura local; no hay red en el constructor.
        _paisSeleccionado = ResolverPaisInicial();
    }

    private static OpcionPais ResolverPaisInicial()
    {
        try
        {
            return PaisesOnline.PorCodigo(JornadaCache.PaisMasReciente());
        }
        catch (Exception ex)
        {
            // Sin caché o error de E/S: se cae al valor por defecto (España).
            Log.Error("DescargaBoletoFrmViewModel.ResolverPaisInicial", ex);
            return PaisesOnline.PorDefecto;
        }
    }

    /// <summary>
    /// Acción EXPLÍCITA del usuario ("Actualizar jornada"): SIEMPRE intenta la red para traer lo
    /// último. En éxito publica la jornada en <see cref="AppState.JornadaActual"/> (fuente
    /// compartida de nombres reales), la deja cacheada (lo hace el servicio) y muestra el resumen
    /// con la fecha. Si la red falla (sin conexión / 429 / error), cae a la jornada CACHEADA si la
    /// hay, con un mensaje claro de modo offline; si no hay caché, muestra el error original.
    /// La descarga ocurre fuera del hilo de UI (await sobre HttpClient); sin DoEvents.
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
            DescargaJornada.Resultado r = await DescargaJornada.DescargarAsync(pais);
            Mensaje = r.Mensaje;

            // W6 (DT-81): los nombres de la jornada pasan también al boleto de Inicio, como dice
            // el texto de esta pantalla.
            if (r.Jornada is not null)
                AppState.Instancia.PonerJornadaEnInicio(r.Jornada);
        }
        finally
        {
            Descargando = false;
        }
    }
}
