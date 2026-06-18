using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>CalculaColumnasMultipleFrm</c> (cálculo en lote de columnas).
///
/// Permite seleccionar varios archivos de combinaciones (.comb / .xml), procesarlos
/// secuencialmente con el reductor/analizador y generar sus columnas en la carpeta
/// "Columnas". Muestra estadísticas en vivo (procesadas, aceptadas, porcentaje,
/// estimadas y máximas con su coste), tiempos de comienzo/fin y dos barras de progreso.
///
/// La lógica de dominio (Analizador, ArchivoCombinacion, ArchivoColumnasTexto,
/// VariablesGlobales, ResultadosCalculoMultipleFrm) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class CalculaColumnasMultipleFrmPage : Page
{
    public CalculaColumnasMultipleFrmViewModel ViewModel { get; } = new();

    public CalculaColumnasMultipleFrmPage()
    {
        this.InitializeComponent();

        // Tras calcular el lote, la VM navega a la pantalla de resultados a través del
        // ContentFrame (mismo patrón que ColGanadoraFrmPage.Navegar). El resumen viaja por el
        // handoff estático ResultadosCalculoMultipleFrmViewModel.UltimosResultados.
        // Equivale a form.ShowDialog() del legacy CalculaColumnasMultipleFrm.
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
    }
}
