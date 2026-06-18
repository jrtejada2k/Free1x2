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
    public static async System.Threading.Tasks.Task<bool> ConfirmarAsync(string mensaje, string titulo = "Free1X2")
    {
        var root = MainWindow?.Content?.XamlRoot;
        if (root is null) return false;

        var dlg = new ContentDialog
        {
            Title = titulo,
            Content = mensaje,
            PrimaryButtonText = "Sí",
            CloseButtonText = "No",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = root,
        };
        return await dlg.ShowAsync() == ContentDialogResult.Primary;
    }

    /// <summary>Muestra un mensaje de error (marshalado al hilo de UI).</summary>
    public static void MostrarError(string mensaje) => EncolarDialogo("Error", mensaje);

    /// <summary>Muestra un mensaje informativo (marshalado al hilo de UI).</summary>
    public static void MostrarInfo(string mensaje) => EncolarDialogo("Free1X2", mensaje);

    private static void EncolarDialogo(string titulo, string mensaje)
    {
        var disp = UiDispatcher;
        if (disp is null) return; // headless / aún sin ventana: silencioso por diseño

        disp.TryEnqueue(() =>
        {
            _colaDialogos.Enqueue((titulo, mensaje));
            ProcesarColaDialogos();
        });
    }

    private static async void ProcesarColaDialogos()
    {
        if (_dialogoAbierto) return;
        var root = MainWindow?.Content?.XamlRoot;
        if (root is null) return;

        _dialogoAbierto = true;
        try
        {
            while (_colaDialogos.Count > 0)
            {
                var (titulo, mensaje) = _colaDialogos.Dequeue();
                var dlg = new ContentDialog
                {
                    Title = titulo,
                    Content = mensaje,
                    CloseButtonText = "Aceptar",
                    XamlRoot = root,
                };
                await dlg.ShowAsync();
            }
        }
        finally
        {
            _dialogoAbierto = false;
        }
    }
}
