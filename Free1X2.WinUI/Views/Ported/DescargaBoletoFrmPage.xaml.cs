// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "DescargaBoletoFrm", reactivada como la integración online
/// opcional con clubprogol.com: el usuario elige país y "Actualizar jornada" descarga la jornada
/// vigente, la guarda en AppState.JornadaActual y rellena el boleto con los equipos reales.
/// </summary>
public sealed partial class DescargaBoletoFrmPage : Page
{
    public DescargaBoletoFrmViewModel ViewModel { get; } = new();

    public DescargaBoletoFrmPage()
    {
        InitializeComponent();
    }

    private async void OnDescargarClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // La descarga es asíncrona y fuera del hilo de UI (HttpClient async); el botón se
        // deshabilita mientras dura para evitar dobles clics (sin DoEvents). Al terminar,
        // la jornada queda en AppState.JornadaActual y el boleto/Grupos de Equipos muestran
        // los nombres reales automáticamente (suscripción a AppState.JornadaCambiada).
        BtnDescargar.IsEnabled = false;
        try
        {
            await ViewModel.DescargarCommand.ExecuteAsync(null);
        }
        finally
        {
            BtnDescargar.IsEnabled = true;
        }
    }
}
