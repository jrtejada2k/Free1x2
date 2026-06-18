using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>Coincidencias</c>.
///
/// Propósito: a partir de un fichero de columnas de entrada y dos columnas ganadoras de
/// referencia (anterior y reciente), calcula la "radiografía" de 108 características de cada
/// columna, cuenta coincidencias según grupos/rangos definidos por el usuario y clasifica las
/// columnas válidas, mostrando las distribuciones resultantes y permitiendo grabarlas.
///
/// La lógica de dominio (lectura de ficheros, cálculo de radiografía, validación, grabado y
/// exportación a Excel) está marcada como TODO en <see cref="CoincidenciasViewModel"/>,
/// citando los métodos de la clase legacy <c>Coincidencias</c>.
/// </summary>
public sealed partial class CoincidenciasPage : Page
{
    public CoincidenciasViewModel ViewModel { get; } = new();

    public CoincidenciasPage()
    {
        this.InitializeComponent();
    }
}
