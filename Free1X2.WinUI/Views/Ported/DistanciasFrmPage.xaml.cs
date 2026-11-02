// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>DistanciasFrm</c> (filtro "Distancias").
/// Permite introducir, para Var / 1 / X / 2, las distancias máximas (0–15) admitidas
/// entre dos signos iguales. Recibe el Grupo a editar vía AppState.GrupoEnEdicion y
/// escribe los cambios de vuelta al FiltroDistancias al Aceptar.
/// </summary>
public sealed partial class DistanciasFrmPage : Page
{
    public DistanciasFrmViewModel ViewModel { get; private set; } = new();

    public DistanciasFrmPage()
    {
        this.InitializeComponent();
        // La VM cierra la página volviendo atrás en el Frame (equivale a CerrarVentana()).
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        // La VM navega al visor de estadísticas a través del Frame (mismo patrón que MainPage).
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver de una pantalla auxiliar (Estadísticas…) se recupera el ViewModel que había, con lo
        // editado y no aceptado: en el original esas pantallas eran diálogos encima del formulario.
        // En la entrada normal (o si no hay nada pendiente) se carga el grupo (MarcarValores legacy).
        if (EdicionPendiente.Recuperar<DistanciasFrmViewModel>(this, e) is { } pendiente)
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
