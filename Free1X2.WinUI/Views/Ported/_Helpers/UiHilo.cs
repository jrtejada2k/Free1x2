// Free1X2 · WinUI 3 — WIN3
using System;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Marshalado al hilo de UI para las páginas portadas, con las dos comprobaciones que
/// antes se omitían una y otra vez (C-25, C-26) y que causaban fallos reales:
///
///   · <c>AppServices.UiDispatcher</c> puede ser <c>null</c> (modo headless / smoke / tests:
///     nunca se llamó a <c>AppServices.Inicializar</c>). El patrón <c>UiDispatcher!</c> producía
///     un <c>NullReferenceException</c> DENTRO de un <c>Task.Run</c>, que el catch del llamador
///     reportaba como «Error al calcular: Object reference not set…» — un mensaje engañoso.
///     Aquí, sin hilo de UI, la acción se ejecuta directamente en el hilo actual: es el mismo
///     fallback que ya usaba <c>OrdenarPorProbabilidadFrmViewModel.ActualizarEstadoMotor</c>.
///
///   · <c>DispatcherQueue.TryEnqueue</c> devuelve <c>false</c> cuando la cola ya está cerrada
///     (la app se está cerrando). Ignorar ese <c>false</c> hacía que la acción se perdiera en
///     silencio. Aquí queda registrada en el log; NO se ejecuta en el hilo actual, porque eso
///     sería justo el acceso desde hilo equivocado que se quiere evitar.
///
/// No cambia la lógica de negocio: solo decide en qué hilo corre la actualización de la UI.
/// </summary>
internal static class UiHilo
{
    /// <summary>
    /// Ejecuta <paramref name="accion"/> en el hilo de UI. Si no hay hilo de UI (headless), la
    /// ejecuta de inmediato en el hilo actual. Si la cola de UI está cerrada, lo registra.
    /// </summary>
    /// <param name="contexto">Identificador para el log cuando la cola está cerrada.</param>
    public static void Ejecutar(Action accion, string contexto)
    {
        if (accion is null) return;

        var disp = AppServices.UiDispatcher;
        if (disp is null)
        {
            // Headless: no hay hilo de UI al que marshalar, así que el hilo actual ES el correcto.
            accion();
            return;
        }

        if (!disp.TryEnqueue(() => accion()))
        {
            Log.Warn("Actualización de UI descartada (cola de UI cerrada) — " + contexto);
        }
    }
}
