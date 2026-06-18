using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página de Configuración portada del WinForms <c>ConfiguracionFrm</c>.
/// Permite editar los parámetros del boleto, la puntuación en CPs, el separador
/// de porcentajes JB, los parámetros LAE y las preferencias generales del programa.
/// </summary>
public sealed partial class ConfiguracionFrmPage : Page
{
    public ConfiguracionFrmViewModel ViewModel { get; } = new();

    public ConfiguracionFrmPage()
    {
        this.InitializeComponent();
    }

    private void VolverButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (Frame?.CanGoBack == true)
        {
            Frame.GoBack();
        }
    }
}
