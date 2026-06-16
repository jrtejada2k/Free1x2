using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views;

/// <summary>
/// Pantalla principal de la app WinUI (réplica de <c>Free1X2.UI.MainForm</c>):
/// boleto base editable a la izquierda y, a la derecha (M3), la rejilla de condiciones,
/// la navegación de grupos, el filtro de columnas y las acciones (Calcular / Reducir /
/// Nueva / Abrir / Guardar combinación). Es la página de inicio del NavigationView.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel { get; } = new();

    public MainPage()
    {
        this.InitializeComponent();
    }
}
