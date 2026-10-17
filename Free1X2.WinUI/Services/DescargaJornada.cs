// Free1X2 · WinUI 3 — WIN3
using System;
using System.Threading;
using System.Threading.Tasks;
using Free1X2.Online;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Descarga EXPLÍCITA de la jornada actual (botón «Actualizar jornada» de Descarga de boleto y
/// «Jornada ▾» de Inicio, W6/DT-81): SIEMPRE intenta la red; en éxito publica la jornada en
/// <see cref="AppState.JornadaActual"/> (la deja cacheada el servicio). Si la red falla (sin
/// conexión / 429 / error), cae a la jornada CACHEADA si la hay, con un mensaje de modo offline;
/// si no hay caché, devuelve el error original y <c>Jornada = null</c>.
/// Antes vivía en <c>DescargaBoletoFrmViewModel.Descargar</c>; se comparte sin cambiar textos.
/// </summary>
public static class DescargaJornada
{
    public sealed record Resultado(JornadaQuiniela? Jornada, string Mensaje);

    public static async Task<Resultado> DescargarAsync(string pais)
    {
        try
        {
            ResultadoJornada resultado = await QuinielaOnlineService.Instancia
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
            string mensaje = resultado.DesdeCache
                ? resumen + " — mostrando la última guardada" + SufijoFecha(resultado.GuardadoUtc, " (", ")") + "."
                : resumen + SufijoFecha(resultado.GuardadoUtc, " · Última actualización: ", "");
            return new Resultado(jornada, mensaje);
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
                return new Resultado(cacheada,
                    "Sin conexión: mostrando la jornada guardada (actualizada " +
                    FormatearFechaLocal(guardadoUtc) + ").");
            }

            // Sin caché tampoco: mensaje de error original + modo manual.
            Log.Error("DescargaJornada.DescargarAsync(" + pais + ") sin caché de respaldo", ex);
            string detalle = ex is QuinielaOnlineException
                ? ex.Message
                : "No se pudo descargar la jornada: " + ex.Message;
            return new Resultado(null, detalle + " El boleto sigue en modo manual.");
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
