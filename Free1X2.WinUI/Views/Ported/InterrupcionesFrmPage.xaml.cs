using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>InterrupcionesFrm</c> (filtro "Interrupciones").
/// Una interrupción es cada cambio de signo a lo largo de la columna. La página
/// permite introducir, para Global / Var / 1 / X / 2, los valores admitidos (0–14)
/// tanto de interrupciones como de interrupciones seguidas. Recibe el Grupo a editar
/// vía AppState.GrupoEnEdicion y escribe los cambios de vuelta al <c>FiltroInterrupciones</c>
/// al Aceptar.
/// </summary>
public sealed partial class InterrupcionesFrmPage : Page
{
    public InterrupcionesFrmViewModel ViewModel { get; } = new();

    public InterrupcionesFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del visor de estadísticas (NavigationMode.Back) no recargamos desde el grupo
        // para no perder los valores en edición; en la entrada normal cargamos.
        if (e.NavigationMode != NavigationMode.Back)
        {
            ViewModel.CargarDesdeGrupo();
        }
    }
}
