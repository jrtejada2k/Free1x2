using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "PesosNumFrm".
/// Edita una condición de filtro de Pesos Numéricos: conjuntos de dígitos (0..9) para
/// Global, Variantes, 1, X y 2 (más sus tolerancias homónimas), las tolerancias numéricas
/// 0..5 y las figuras seleccionables (3-2, 3-1-1, 2-2-1, 2-1-1-1, 1-1-1-1-1).
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta al
/// <c>FiltroPesosNumericos</c> al Aceptar. La persistencia en disco queda como TODO.
/// </summary>
public sealed partial class PesosNumFrmPage : Page
{
    public PesosNumFrmViewModel ViewModel { get; } = new();

    public PesosNumFrmPage()
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
