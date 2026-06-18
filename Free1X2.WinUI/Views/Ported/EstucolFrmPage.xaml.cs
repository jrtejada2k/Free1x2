// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "EstucolFrm"
/// (EstuCol - Generador / Analizador de Columnas Probables).
/// Permite seleccionar los archivos de columnas reducidas y ganadoras, elegir el modo de
/// agrupación/emparejamiento de columnas y generar el informe del escrutinio.
/// </summary>
public sealed partial class EstucolFrmPage : Page
{
    public EstucolFrmViewModel ViewModel { get; } = new();

    public EstucolFrmPage()
    {
        InitializeComponent();

        // Tras generar el informe, la VM abre el visor ABDON a través del ContentFrame
        // (mismo patrón que ColGanadoraFrmPage.Navegar). El informe viaja por el handoff
        // estático EstucolFrmViewModel.UltimoInforme, que VisorAnalisisColumnasAbdonFrmViewModel
        // consume al construirse. Equivale a new VisorAnalisisColumnasAbdonFrm(...).Show() del legacy.
        ViewModel.AbrirVisor = () => Frame?.Navigate(typeof(VisorAnalisisColumnasAbdonFrmPage));
    }
}
