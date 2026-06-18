// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>SelectorMS</c> ("Selector MarioSan").
///
/// Lee un fichero de columnas (.txt) y calcula, por columna, cuántas columnas del
/// fichero quedan a aspecto 13/12/11 signos (calcular13/12/11). Agrupa el resultado en
/// una tabla de distribución (Q / cols. / P14..P10), permite seleccionar grupos,
/// sumarlos (SumSel) y grabar sus columnas (Grabar). La sección "Análisis de Resultados"
/// carga un fichero de columnas ganadoras navegable y analiza la distribución frente a
/// una columna ganadora (Analizar + Comparador2/neq).
///
/// La lógica de dominio (s2n/n2s, calcular11/12/13, Comparador2, neq, ArchivoColumnasTexto,
/// diálogos de archivo y el refresco de la distribución) está implementada en el ViewModel.
/// </summary>
public sealed partial class SelectorMSPage : Page
{
    public SelectorMSViewModel ViewModel { get; } = new();

    public SelectorMSPage()
    {
        this.InitializeComponent();
    }
}
