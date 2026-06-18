using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada de Free1X2.UI.Filtros.Formatos123Frm (WinForms).
/// Filtro de Formatos 123: lista editable de formatos con min/max aciertos,
/// líneas acertadas permitidas, opción "ignorar repeticiones" y traductor 1X2->123.
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los formatos y la matriz
/// de valoración (rejilla PorcentajesControl) de vuelta al <c>FiltroFormatos123</c> al Aceptar.
/// </summary>
public sealed partial class Formatos123FrmPage : Page
{
    public Formatos123FrmViewModel ViewModel { get; } = new();

    public Formatos123FrmPage()
    {
        InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del visor de estadísticas (NavigationMode.Back) no recargamos desde el grupo
        // para no perder los formatos en edición; en la entrada normal cargamos.
        if (e.NavigationMode != NavigationMode.Back)
        {
            ViewModel.CargarDesdeGrupo();
        }
    }
}
