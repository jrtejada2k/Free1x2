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
            ResultadoJornada resultado = await _servicio
                .ObtenerJornadaAsync(pais, CancellationToken.None)
                .ConfigureAwait(true); // continúa en el hilo de UI para tocar AppState/binding

            JornadaQuiniela jornada = resultado.Jornada;

            // Fuente compartida de nombres reales: boleto y "Grupos de Equipos" leen de aquí.
            AppState.Instancia.JornadaActual = jornada;

            // B-05: el mensaje distingue lo que ACABA de bajar de la red de lo que viene del
            // fichero local (guarda anti-doble-descarga). Antes ambos casos decían lo mismo,
            // así que una caché podía presentarse como recién actualizada.
            // C-05: la fecha la trae el propio servicio (marca de tiempo del fichero); ya no se
            // relee ni se re-parsea el JSON completo en el hilo de UI solo para el timestamp.
            string resumen = "Jornada " + jornada.Jornada + " · " + jornada.Partidos.Count +
                             " partidos cargados";
            Mensaje = resultado.DesdeCache
                ? resumen + " — mostrando la última guardada" + SufijoFecha(resultado.GuardadoUtc, " (", ")") + "."
                : resumen + SufijoFecha(resultado.GuardadoUtc, " · Última actualización: ", "");
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Fallo de red/HTTP/parseo (cualquier excepción que no sea cancelación): intentar
            // fallback a la jornada guardada antes de rendirse.
            if (JornadaCache.TryCargar(pais, out JornadaQuiniela? cacheada, out DateTime guardadoUtc) &&
                cacheada is not null)
            {
                Log.Warn("Descarga de jornada fallida (" + pais + "); se usa la caché de " +
                         FormatearFechaLocal(guardadoUtc) + ". Causa: " + ex.Message);
                AppState.Instancia.JornadaActual = cacheada;
                Mensaje = "Sin conexión: mostrando la jornada guardada (actualizada " +
                          FormatearFechaLocal(guardadoUtc) + ").";
            }
            else
            {
                // Sin caché tampoco: mensaje de error original + modo manual.
                Log.Error("DescargaBoletoFrmViewModel.Descargar(" + pais + ") sin caché de respaldo", ex);
                string detalle = ex is QuinielaOnlineException
                    ? ex.Message
                    : "No se pudo descargar la jornada: " + ex.Message;
                Mensaje = detalle + " El boleto sigue en modo manual.";
            }
        }
        finally
        {
            Descargando = false;
        }
    }

    /// <summary>
    /// Compone el fragmento de fecha del mensaje, o cadena vacía si el servicio no pudo
    /// determinar cuándo se guardó la copia local (nunca se inventa una fecha).
    /// </summary>
    private static string SufijoFecha(DateTime? guardadoUtc, string prefijo, string sufijo) =>
        guardadoUtc is DateTime f ? prefijo + FormatearFechaLocal(f) + sufijo : string.Empty;

    /// <summary>Formatea una marca UTC a hora local legible para el usuario.</summary>
    private static string FormatearFechaLocal(DateTime utc) =>
        utc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
}
