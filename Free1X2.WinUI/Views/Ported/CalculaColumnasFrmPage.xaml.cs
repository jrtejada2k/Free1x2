// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "CalculaColumnasFrm" (Calcular columnas).
/// Lanza el cálculo/generación de columnas de la combinación actual en tres modos
/// (Calcular / Calcular y analizar / Grabar archivo) y muestra las estadísticas del proceso.
/// </summary>
public sealed partial class CalculaColumnasFrmPage : Page
{
    public CalculaColumnasFrmViewModel ViewModel { get; } = new();

    public CalculaColumnasFrmPage()
    {
        this.InitializeComponent();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // El form legacy cerraba la ventana tras PararAnalisis(); aquí se navega hacia atrás.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
