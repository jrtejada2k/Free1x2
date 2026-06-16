using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>GruposEquiposFrm</c> ("Grupos de Equipos").
/// Define agrupaciones de equipos para el filtro <c>FiltroGruposEquipos</c>:
/// en la pestaña "Grupos Equipos" se marcan los equipos (casa/fuera) de los 14
/// partidos y se fijan victorias / empates / derrotas / suma de puntos; en
/// "Relaciones" se combinan grupos por índice con sus sumas. El grupo a editar
/// llega vía <c>AppState.GrupoEnEdicion</c> y los cambios se escriben de vuelta al
/// <c>FiltroGruposEquipos</c> al Aceptar.
/// </summary>
public sealed partial class GruposEquiposFrmPage : Page
{
    public GruposEquiposFrmViewModel ViewModel { get; } = new();

    public GruposEquiposFrmPage()
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
