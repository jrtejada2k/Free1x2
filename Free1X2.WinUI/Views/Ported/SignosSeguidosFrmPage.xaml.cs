// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>SignosSeguidosFrm</c> (filtro "Signos Seguidos").
/// Permite introducir, para Var / 1 / X / 2, las cantidades (0–15) admitidas de cada
/// concepto SEGUIDO dentro de una combinación, y definir las "Figuras" asociadas a cada uno.
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta al
/// <c>FiltroSignosSeguidos</c> al Aceptar. La edición de figuras (navega a FigurasFiltrosFrmPage)
/// y la persistencia al filtro están implementadas en el ViewModel.
/// </summary>
public sealed partial class SignosSeguidosFrmPage : Page
{
    public SignosSeguidosFrmViewModel ViewModel { get; } = new();

    public SignosSeguidosFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del editor de figuras o del visor de estadísticas (NavigationMode.Back)
        // sólo refrescamos los indicadores de figuras; recargar desde el grupo borraría la edición.
        if (e.NavigationMode == NavigationMode.Back)
        {
            ViewModel.RefrescarFiguras();
        }
        else
        {
            ViewModel.CargarDesdeGrupo();
        }
    }
}
