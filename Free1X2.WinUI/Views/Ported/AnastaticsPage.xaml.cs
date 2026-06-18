// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>Anastatics</c> (ventana "Estadísticas 0.3.4").
/// Permite elegir un modo de análisis, seleccionar un fichero de columnas origen,
/// calcular estadísticas y mostrar los resultados. Al pulsar "Mostrar resultados" la VM
/// navega a la Page del modo (DibForm/DibRepFrm/StaInterFrm/StaSSForm) entregándole la
/// matriz calculada por su handoff estático, igual que dib.Show()/dibrep.Show() del legacy.
/// </summary>
public sealed partial class AnastaticsPage : Page
{
    public AnastaticsViewModel ViewModel { get; } = new();

    public AnastaticsPage()
    {
        this.InitializeComponent();

        // La VM navega a través del Frame de la página (no conoce la UI directamente),
        // mismo patrón que MainPage.ViewModel.Navegar.
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }
}
