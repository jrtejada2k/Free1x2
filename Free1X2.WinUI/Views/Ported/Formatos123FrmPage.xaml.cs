using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada de Free1X2.UI.Filtros.Formatos123Frm (WinForms).
/// Filtro de Formatos 123: lista editable de formatos con min/max aciertos,
/// líneas acertadas permitidas, opción "ignorar repeticiones" y traductor 1X2->123.
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los formatos de vuelta
/// al <c>FiltroFormatos123</c> al Aceptar (la matriz de valoración queda como TODO).
/// </summary>
public sealed partial class Formatos123FrmPage : Page
{
    public Formatos123FrmViewModel ViewModel { get; } = new();

    public Formatos123FrmPage()
    {
        InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.CargarDesdeGrupo();
    }
}
