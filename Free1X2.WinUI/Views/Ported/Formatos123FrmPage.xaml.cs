// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
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
    public Formatos123FrmViewModel ViewModel { get; private set; } = new();

    public Formatos123FrmPage()
    {
        InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver de una pantalla auxiliar (Estadísticas…) se recupera el ViewModel que había, con lo
        // editado y no aceptado: en el original esas pantallas eran diálogos encima del formulario.
        // En la entrada normal (o si no hay nada pendiente) se carga el grupo (MarcarValores legacy).
        if (EdicionPendiente.Recuperar<Formatos123FrmViewModel>(this, e) is { } pendiente)
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
