using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>SignosSeguidosFrm</c> (filtro "Signos Seguidos").
/// Permite introducir, para Var / 1 / X / 2, las cantidades (0–15) admitidas de cada
/// concepto SEGUIDO dentro de una combinación, y definir las "Figuras" asociadas a cada uno.
/// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta al
/// <c>FiltroSignosSeguidos</c> al Aceptar. La edición de figuras y la persistencia quedan como TODO.
/// </summary>
public sealed partial class SignosSeguidosFrmPage : Page
{
    public SignosSeguidosFrmViewModel ViewModel { get; } = new();

    public SignosSeguidosFrmPage()
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
