using Microsoft.UI.Xaml;
using Free1X2.WinUI.Services;
using Free1X2.WinUI.Views.Ported;

namespace Free1X2.WinUI;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        AppServices.Inicializar(MainWindow);
        CablearHooksDominio();
        MainWindow.Activate();
    }

    /// <summary>
    /// Conecta los shims del dominio (Free1X2.Abstractions) con la UI de WinUI.
    /// Equivalente a Program.WireDomainHooks() del exe WinForms, pero usando
    /// DispatcherQueue/ContentDialog en lugar de Application.DoEvents/MessageBox.
    /// </summary>
    private static void CablearHooksDominio()
    {
        // El motor corre en hilos de fondo (Task.Run) en WinUI; no se bombea el
        // dispatcher desde un hilo de trabajo. No-op intencional.
        Free1X2.Abstractions.UiPump.Pump = static () => { };

        Free1X2.Abstractions.UserDialogs.ShowError = AppServices.MostrarError;
        Free1X2.Abstractions.UserDialogs.ShowInfo = AppServices.MostrarInfo;

        // Visor de análisis de columnas: por ahora se guarda el payload de forma
        // estática y se navega a la página del visor en el hilo de UI. El cableado
        // completo del visor (poblar el árbol desde el contenedor) queda pendiente.
        // TODO(visor): pasar (contenedorAnalisisGlobal, grupo) al ViewModel del
        // visor para reconstruir el árbol como hace VisorAnalisisColumnasFrm en WinForms.
        Free1X2.Abstractions.AnalisisUi.MostrarVisor = static (contenedor, grupo) =>
        {
            VisorAnalisisColumnasFrmViewModel.UltimoContenedor = contenedor;
            VisorAnalisisColumnasFrmViewModel.UltimoGrupo = grupo;
            AppServices.UiDispatcher?.TryEnqueue(static () =>
            {
                if (MainWindow?.Content is FrameworkElement fe &&
                    fe.FindName("ContentFrame") is Microsoft.UI.Xaml.Controls.Frame frame)
                {
                    frame.Navigate(typeof(VisorAnalisisColumnasFrmPage));
                }
            });
        };
    }
}
