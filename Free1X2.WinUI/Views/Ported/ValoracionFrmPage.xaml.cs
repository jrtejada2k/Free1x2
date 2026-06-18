// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>ValoracionFrm</c> (filtro "Valoración" /
/// <c>FiltroValoracionSignos</c>). Permite elegir el tipo (suma / productos x 3E7) y
/// acotar los rangos de valoración Global, de Unos, Equis y Doses. Recibe el Grupo a editar
/// vía AppState.GrupoEnEdicion y escribe tipo + rangos + matriz de porcentajes (de la
/// rejilla PorcentajesControl) de vuelta al filtro al Aceptar. La persistencia
/// (Guardar/Abrir/Copiar/Pegar/Estadísticas) está implementada en el ViewModel.
/// </summary>
public sealed partial class ValoracionFrmPage : Page
{
    public ValoracionFrmViewModel ViewModel { get; } = new();

    public ValoracionFrmPage()
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
