using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "VisorAnalisisColumnasAbdonFrm" (EstuCol - Visor de Informe).
/// Muestra el análisis ABDON de las columnas generadas frente a las ganadoras del archivo
/// (rango de aciertos, agrupaciones paso fijo/solapadas, escaleras, sandwichs y suma de aciertos),
/// con modos Global e Individual, y permite generar un fichero de condición.
/// </summary>
public sealed partial class VisorAnalisisColumnasAbdonFrmPage : Page
{
    public VisorAnalisisColumnasAbdonFrmViewModel ViewModel { get; } = new();

    public VisorAnalisisColumnasAbdonFrmPage()
    {
        InitializeComponent();
    }
}
