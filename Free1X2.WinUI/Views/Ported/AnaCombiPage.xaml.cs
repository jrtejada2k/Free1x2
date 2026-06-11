using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "AnaCombi" (Análisis de grupos en combinación).
/// Define un grupo/patrón sobre los 14 partidos, carga un fichero de columnas, cuenta
/// cuántas encajan en cada combinación de grupo y permite grabar la selección.
/// </summary>
public sealed partial class AnaCombiPage : Page
{
    public AnaCombiViewModel ViewModel { get; } = new();

    public AnaCombiPage()
    {
        InitializeComponent();
    }
}
