// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>NoVariantesFrm</c> (filtro "Número de Variantes").
/// Permite introducir, para Var / X / 2, las cantidades (0–15) admitidas de cada
/// concepto dentro de una combinación. Recibe el Grupo a editar vía AppState.GrupoEnEdicion
/// y escribe los cambios de vuelta al FiltroNoVariantes al Aceptar.
/// </summary>
public sealed partial class NoVariantesFrmPage : Page
{
    public NoVariantesFrmViewModel ViewModel { get; } = new();

    public NoVariantesFrmPage()
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
