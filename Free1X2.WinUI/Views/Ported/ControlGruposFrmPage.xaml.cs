// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>ControlGruposFrm</c> ("Control de Grupos").
/// Permite definir, sobre el <c>ControladorGrupos</c> de la combinación actual
/// (AppState.Instancia.Analizador.CtrlGrupos), una lista de controles de fallos
/// sobre grupos de partidos y otra sobre conjuntos de grupos. Cada sección se
/// recorre con Anterior/Siguiente, muestra un contador "actual/total" y permite
/// eliminar el control actual. Al Aceptar escribe las listas de vuelta al motor.
/// </summary>
public sealed partial class ControlGruposFrmPage : Page
{
    public ControlGruposFrmViewModel ViewModel { get; } = new();

    public ControlGruposFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.CargarDesdeMotor();
    }
}
