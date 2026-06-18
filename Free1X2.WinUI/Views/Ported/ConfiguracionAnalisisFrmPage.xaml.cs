// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "ConfiguracionAnalisisFrm" (Configurar Análisis).
/// Permite activar/desactivar los tipos de análisis estadístico de la quiniela.
/// </summary>
public sealed partial class ConfiguracionAnalisisFrmPage : Page
{
    public ConfiguracionAnalisisFrmViewModel ViewModel { get; } = new();

    public ConfiguracionAnalisisFrmPage()
    {
        this.InitializeComponent();
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        // El guardado real lo hace ViewModel.GuardarCommand (TODO dominio).
        // Tras guardar, el form legacy cerraba la ventana; aquí se navega hacia atrás si es posible.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }

    private void BtnSalir_Click(object sender, RoutedEventArgs e)
    {
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
