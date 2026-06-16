using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views;

/// <summary>
/// Pantalla principal de la app WinUI (réplica de <c>Free1X2.UI.MainForm</c>):
/// boleto base editable a la izquierda y, a la derecha, la rejilla de condiciones
/// (con semáforos), la navegación de grupos, el filtro de columnas y las acciones
/// (Calcular / Reducir / Nueva / Abrir / Guardar combinación). Página de inicio del NavigationView.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageViewModel ViewModel { get; } = new();

    public MainPage()
    {
        this.InitializeComponent();

        // El boleto crea su propio ViewModel; lo enlazamos al de la página para coordinar
        // (volcado de pronósticos, recarga al cambiar de grupo, etc.).
        ViewModel.Boleto = Boleto.ViewModel;

        // La VM navega a través del Frame de la página (no conoce la UI directamente).
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver de una página de filtro, refresca los semáforos de condiciones
        // (equivale a MainForm.MainFormActivated → ActualizaGrupoPantalla).
        ViewModel.RefrescarPantalla();
    }
}
