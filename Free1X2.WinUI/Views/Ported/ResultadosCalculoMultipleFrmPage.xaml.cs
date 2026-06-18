using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "ResultadosCalculoMultipleFrm"
/// (título: "Resultados de la Generación Múltiple").
/// Pantalla de solo lectura que muestra un resumen (rejilla de 5 columnas:
/// Arch. Combinación, Arch. Columnas, Cols. Analizadas, Cols. Aceptadas, Tiempo)
/// con el resultado de cada cálculo realizado en lote durante la generación múltiple.
/// </summary>
public sealed partial class ResultadosCalculoMultipleFrmPage : Page
{
    public ResultadosCalculoMultipleFrmViewModel ViewModel { get; } = new();

    public ResultadosCalculoMultipleFrmPage()
    {
        InitializeComponent();
    }
}
