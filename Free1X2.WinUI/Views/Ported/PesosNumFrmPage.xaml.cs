// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "PesosNumFrm".
/// Edita una condición de filtro de Pesos Numéricos: conjuntos de dígitos (0..9) para
/// Global, Variantes, 1, X y 2 (más sus tolerancias homónimas), las tolerancias numéricas
/// 0..5 y las figuras seleccionables (3-2, 3-1-1, 2-2-1, 2-1-1-1, 1-1-1-1-1).
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta al
/// <c>FiltroPesosNumericos</c> al Aceptar. La persistencia en disco está implementada.
/// </summary>
public sealed partial class PesosNumFrmPage : Page
{
    public PesosNumFrmViewModel ViewModel { get; private set; } = new();

    public PesosNumFrmPage()
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
        if (EdicionPendiente.Recuperar<PesosNumFrmViewModel>(this, e) is { } pendiente)
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
