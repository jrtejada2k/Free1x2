using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>SimetriasFrm</c> (filtro "Simetrías").
/// Permite definir simetrías —grupos de partidos que deben compartir el mismo signo—
/// y un campo de "Aciertos". Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe
/// las simetrías de vuelta al <c>FiltroSimetrias</c> al Aceptar. La persistencia en disco
/// queda como TODO en el ViewModel.
/// </summary>
public sealed partial class SimetriasFrmPage : Page
{
    public SimetriasFrmViewModel ViewModel { get; } = new();

    public SimetriasFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.CargarDesdeGrupo();
    }
}
