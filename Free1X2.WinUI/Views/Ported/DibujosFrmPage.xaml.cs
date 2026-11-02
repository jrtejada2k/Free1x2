// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>DibujosFrm</c> (categoría Filtros).
/// Filtro de "dibujos X+2": el usuario selecciona en una malla las figuras (NumX + Num2)
/// que deben cumplir las columnas. Recibe el Grupo a editar vía AppState.GrupoEnEdicion y
/// escribe los dibujos seleccionados de vuelta al <c>FiltroDibujos</c> al Aceptar.
/// </summary>
public sealed partial class DibujosFrmPage : Page
{
    public DibujosFrmViewModel ViewModel { get; private set; } = new();

    public DibujosFrmPage()
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
        if (EdicionPendiente.Recuperar<DibujosFrmViewModel>(this, e) is { } pendiente)
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
