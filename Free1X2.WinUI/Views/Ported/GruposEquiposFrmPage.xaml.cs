// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
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
    public GruposEquiposFrmViewModel ViewModel { get; private set; } = new();

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
        // Al volver de una pantalla auxiliar (Estadísticas…) se recupera el ViewModel que había, con lo
        // editado y no aceptado: en el original esas pantallas eran diálogos encima del formulario.
        // En la entrada normal (o si no hay nada pendiente) se carga el grupo (MarcarValores legacy).
        if (EdicionPendiente.Recuperar<GruposEquiposFrmViewModel>(this, e) is { } pendiente)
        {
            pendiente.Volver = ViewModel.Volver;
            pendiente.Navegar = ViewModel.Navegar;
            ViewModel = pendiente;
            Bindings.Update();
        }
        else
        {
            ViewModel.CargarDesdeGrupo();
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        EdicionPendiente.Guardar(this, ViewModel, e);
    }
}
