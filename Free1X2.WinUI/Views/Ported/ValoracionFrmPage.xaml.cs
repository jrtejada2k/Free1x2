// Free1X2 · WinUI 3 — WIN3
using Free1X2.WinUI.Services;
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
    public ValoracionFrmViewModel ViewModel { get; private set; } = new();

    public ValoracionFrmPage()
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
        if (EdicionPendiente.Recuperar<ValoracionFrmViewModel>(this, e) is { } pendiente)
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
