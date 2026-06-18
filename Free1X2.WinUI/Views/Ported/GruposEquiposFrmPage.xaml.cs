// Free1X2 · WinUI 3 — WIN3
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
        // La VM navega al visor de estadísticas a través del Frame (comando Estadísticas).
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del visor de estadísticas (NavigationMode.Back) NO recargamos: recargar desde el
        // grupo borraría la edición en curso (incluida una condición recién abierta de disco). Sólo en
        // la entrada normal (New / navegación hacia delante) cargamos los datos del grupo en edición.
        if (e.NavigationMode != NavigationMode.Back)
        {
            ViewModel.CargarDesdeGrupo();
        }
    }
}
