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
    public DibujosFrmViewModel ViewModel { get; } = new();

    public DibujosFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Al volver del visor de estadísticas (NavigationMode.Back) no recargamos desde el grupo
        // para no perder la selección en edición; en la entrada normal cargamos (MarcarValores legacy).
        if (e.NavigationMode != NavigationMode.Back)
        {
            ViewModel.CargarDesdeGrupo();
        }
    }
}
