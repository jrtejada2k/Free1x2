// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
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
    public InterrupcionesFrmViewModel ViewModel { get; private set; } = new();

    public InterrupcionesFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver de una pantalla auxiliar (Estadísticas…) se recupera el ViewModel que había, con lo
        // editado y no aceptado: en el original esas pantallas eran diálogos encima del formulario.
        // En la entrada normal (o si no hay nada pendiente) se carga el grupo (MarcarValores legacy).
        if (EdicionPendiente.Recuperar<InterrupcionesFrmViewModel>(this, e) is { } pendiente)
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
