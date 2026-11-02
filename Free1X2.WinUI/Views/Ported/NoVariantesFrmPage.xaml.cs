// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
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
    public NoVariantesFrmViewModel ViewModel { get; private set; } = new();

    public NoVariantesFrmPage()
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
        if (EdicionPendiente.Recuperar<NoVariantesFrmViewModel>(this, e) is { } pendiente)
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
