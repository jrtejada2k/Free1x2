// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Servicios a nivel de app para que el dominio (motor de cálculo) pueda
/// interactuar con la UI de WinUI sin referenciar tipos de WinUI: handle de
/// ventana (para FileOpenPicker/FileSavePicker), DispatcherQueue del hilo de UI
/// y diálogos marshalados al hilo de UI.
/// </summary>
public static class AppServices
{
    public static Window? MainWindow { get; private set; }
    public static DispatcherQueue? UiDispatcher { get; private set; }

    /// <summary>HWND de la ventana principal, para WinRT.Interop.InitializeWithWindow.</summary>
    public static IntPtr WindowHandle =>
        MainWindow is null ? IntPtr.Zero : WinRT.Interop.WindowNative.GetWindowHandle(MainWindow);

    // Cola de diálogos para evitar el InvalidOperationException de ContentDialog
    // cuando ya hay uno abierto (solo puede haber uno a la vez por XamlRoot).
    private static readonly Queue<(string titulo, string mensaje)> _colaDialogos = new();

    /// <summary>
    /// Puerta ÚNICA por la que pasa TODO <c>ContentDialog.ShowAsync</c> de este servicio
    /// (la cola de mensajes y <see cref="ConfirmarAsync"/>). WinUI solo admite un ContentDialog
    /// a la vez por XamlRoot: antes, un <c>ConfirmarAsync</c> abierto hacía que el ShowAsync de
    /// la cola lanzara <c>InvalidOperationException</c> desde un <c>async void</c> → manejador
    /// global → MostrarError → re-encola → vuelve a lanzar (bucle) y el mensaje original,
    /// ya des-encolado, se perdía. Serializando aquí, ese escenario no puede darse.
    /// </summary>
    private static readonly System.Threading.SemaphoreSlim _puertaDialogos = new(1, 1);

    /// <summary>
    /// True mientras hay un diálogo de este servicio en pantalla. Evita que la bomba de la cola
    /// se ponga a esperar en la puerta mientras un <see cref="ConfirmarAsync"/> está abierto:
    /// la cola se reanuda sola al cerrarse el diálogo (ver el <c>finally</c> de ConfirmarAsync).
    /// </summary>
    private static bool _dialogoAbierto;

    public static void Inicializar(Window mainWindow)
    {
        MainWindow = mainWindow;
        UiDispatcher = mainWindow.DispatcherQueue;
    }

    /// <summary>
    /// Pregunta Sí/No al usuario (réplica de los <c>MessageBox.Show(..., YesNo, Question)</c> del
    /// MainForm original: Nueva/Abrir combinación, Borrar temporales, Borrar informes). Devuelve
    /// true si el usuario confirma. Debe invocarse desde el hilo de UI. En modo headless (sin
    /// XamlRoot) devuelve false para no realizar acciones destructivas sin confirmación.
    /// </summary>
    public static System.Threading.Tasks.Task<bool> ConfirmarAsync(string mensaje, string titulo = "Free1X2")
        => ConfirmarAsync(mensaje, titulo, ContentDialogButton.Close);

    /// <summary>
    /// Igual que <see cref="ConfirmarAsync(string,string)"/>, pero permite elegir qué botón queda
    /// por defecto (el que responde a Intro). Necesario porque algunas pantallas portadas tenían
    /// su propio ContentDialog con "Sí" por defecto: al unificarlas aquí (C-22) pasan a compartir
    /// la puerta de diálogos (B-01) sin cambiar a qué botón responde el Intro.
    /// </summary>
    public static async System.Threading.Tasks.Task<bool> ConfirmarAsync(
        string mensaje, string titulo, ContentDialogButton botonPorDefecto)
    {
        if (MainWindow?.Content?.XamlRoot is null) return false;

        // Pasa por la MISMA puerta que la cola de mensajes (B-01): si ya hay un diálogo en
        // pantalla, este espera su turno en vez de lanzar InvalidOperationException.
        await _puertaDialogos.WaitAsync().ConfigureAwait(true);
        _dialogoAbierto = true;
        try
        {
            // Se relee el XamlRoot tras esperar: la ventana pudo cerrarse mientras tanto.
            var root = MainWindow?.Content?.XamlRoot;
            if (root is null) return false;

            var dlg = new ContentDialog
            {
                Title = titulo,
                Content = mensaje,
                PrimaryButtonText = "Sí",
                CloseButtonText = "No",
                DefaultButton = botonPorDefecto,
                XamlRoot = root,
                // U-01: un ContentDialog vive en un popup del XamlRoot, no dentro del árbol de
                // la ventana, así que NO hereda el RequestedTheme de la raíz. Se le pasa el tema
                // elegido (con "Sistema" es Default = seguir a Windows, igual que antes).
                RequestedTheme = TemaApp.TemaDeElemento,
            };
            return await dlg.ShowAsync() == ContentDialogResult.Primary;
        }
        catch (Exception ex)
        {
            // Si el diálogo no se puede mostrar, se mantiene el contrato "sin confirmación
            // explícita no se hace nada destructivo" (mismo valor que el caso headless).
            Log.Error("AppServices.ConfirmarAsync", ex);
            return false;
        }
        finally
        {
            _dialogoAbierto = false;
            _puertaDialogos.Release();

            // Mensajes que se encolaron mientras la confirmación estaba abierta: la bomba se
            // cortó a sí misma (_dialogoAbierto), así que hay que reanudarla aquí o quedarían
            // esperando indefinidamente.
            if (_colaDialogos.Count > 0) ProcesarColaDialogos();
        }
    }

    /// <summary>
    /// Ejecuta una acción asíncrona "dispara y olvida" OBSERVANDO su resultado: si falla, se
    /// registra y se avisa al usuario (B-06). Sustituye a los <c>_ = MetodoAsync();</c>, donde
    /// la Task descartada se llevaba la excepción y la acción fallaba en silencio.
    /// Recibe una <c>Func&lt;Task&gt;</c> (no una Task ya iniciada) para cubrir también la parte
    /// SÍNCRONA del método, que de otro modo lanzaría en el llamador.
    /// </summary>
    public static async void EjecutarObservandoErrores(
        Func<System.Threading.Tasks.Task> accion, string contexto)
    {
        try
        {
            await accion().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Log.Error(contexto, ex);
            MostrarError("No se pudo completar la acción:\n\n" + ex.Message);
        }
    }

    /// <summary>Muestra un mensaje de error (marshalado al hilo de UI).</summary>
    public static void MostrarError(string mensaje) => EncolarDialogo("Error", mensaje);

    /// <summary>Muestra un mensaje informativo (marshalado al hilo de UI).</summary>
    public static void MostrarInfo(string mensaje) => EncolarDialogo("Free1X2", mensaje);

    private static void EncolarDialogo(string titulo, string mensaje)
    {
        var disp = UiDispatcher;
        if (disp is null)
        {
            // Headless / aún sin ventana: sigue siendo silencioso por diseño (p. ej. errores
            // antes de AppServices.Inicializar), pero al menos queda escrito en el log.
            Log.Warn("Diálogo no mostrado (sin hilo de UI todavía) — " + titulo + ": " + mensaje);
            return;
        }

        if (!disp.TryEnqueue(() =>
        {
            _colaDialogos.Enqueue((titulo, mensaje));
            ProcesarColaDialogos();
        }))
        {
            Log.Warn("Diálogo no mostrado (cola de UI cerrada) — " + titulo + ": " + mensaje);
        }
    }

    private static async void ProcesarColaDialogos()
    {
        // Ya hay un diálogo de este servicio en pantalla: la bomba se reanudará cuando se
        // cierre (ConfirmarAsync lo hace en su finally, y el while de aquí sigue drenando).
        if (_dialogoAbierto) return;

        if (MainWindow?.Content?.XamlRoot is null)
        {
            // Sin XamlRoot todavía: los mensajes se quedan en la cola (no se pierden) y se
            // mostrarán en la siguiente llamada, cuando la ventana ya tenga contenido.
            Log.Warn("Cola de diálogos en espera: la ventana aún no tiene XamlRoot.");
            return;
        }

        await _puertaDialogos.WaitAsync().ConfigureAwait(true);
        _dialogoAbierto = true;
        try
        {
            while (_colaDialogos.Count > 0)
            {
                var root = MainWindow?.Content?.XamlRoot;
                if (root is null) return; // ventana cerrándose: se deja lo que quede en la cola

                var (titulo, mensaje) = _colaDialogos.Dequeue();
                var dlg = new ContentDialog
                {
                    Title = titulo,
                    Content = mensaje,
                    CloseButtonText = "Aceptar",
                    XamlRoot = root,
                    // U-01: mismo motivo que en ConfirmarAsync (el popup no hereda el tema).
                    RequestedTheme = TemaApp.TemaDeElemento,
                };

                try
                {
                    await dlg.ShowAsync();
                }
                catch (Exception ex)
                {
                    // Clave del B-01: el fallo de UN diálogo NO puede escapar de este async void
                    // (llegaría al manejador global → MostrarError → re-encola → bucle). Se
                    // registra el mensaje que no se pudo mostrar y se sigue con el siguiente.
                    Log.Error("AppServices.ProcesarColaDialogos [" + titulo + "] " + mensaje, ex);
                }
            }
        }
        finally
        {
            _dialogoAbierto = false;
            _puertaDialogos.Release();
        }
    }
}
