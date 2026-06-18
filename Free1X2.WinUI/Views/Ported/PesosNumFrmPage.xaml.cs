// Free1X2 · WinUI 3 — WIN3
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
    public PesosNumFrmViewModel ViewModel { get; } = new();

    public PesosNumFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del visor de estadísticas (NavigationMode.Back) no recargamos desde el grupo
        // para no perder los valores en edición; en la entrada normal cargamos.
        if (e.NavigationMode != NavigationMode.Back)
        {
            ViewModel.CargarDesdeGrupo();
        }
    }
}
