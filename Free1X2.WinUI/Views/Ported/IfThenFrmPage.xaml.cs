using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "IfThenFrm" — "Condiciones relacionadas (if-then)".
///
/// Permite definir relaciones del tipo "si se produce una condición, entonces debe
/// cumplirse otra", en dos pestañas:
///  - Condiciones sencillas: relaciona condiciones de filtro (genérica + específica + valor + negación).
///  - Grupos: relaciona grupos de partidos entre sí.
///
/// Opera sobre el Analizador compartido (AppState.Instancia.Analizador): Aceptar
/// construye un ControladorIfThen y lo asigna a analizador.IfThen; Guardar/Abrir/Copiar/Pegar
/// usan ArchivoCondiciones (.if/.xml).
/// </summary>
public sealed partial class IfThenFrmPage : Page
{
    public IfThenFrmViewModel ViewModel { get; } = new();

    public IfThenFrmPage()
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
