using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "EscrutarCombinacionesFrm".
/// Escruta (puntúa) ficheros de combinaciones contra columna(s) ganadora(s) en tres modos:
/// escrutinio simple (columna manual de 14 signos), contra un fichero de referencia y contra
/// las jornadas de una/varias temporadas (plantilla de nombre + dígitos de temporada/jornada).
/// El original tenía un TabControl (tpSimple/tbFichero/tbTemporada), una lista de ficheros, el
/// rango de aciertos, el grid de resultados (dgResultados) y botones de acción.
/// Toda la lógica de dominio (EscrutadorComb, lectura de ficheros, cálculo de premios) queda
/// marcada como TODO en el ViewModel; aún no portada a Free1X2.Domain.
/// </summary>
public sealed partial class EscrutarCombinacionesFrmPage : Page
{
    public EscrutarCombinacionesFrmViewModel ViewModel { get; } = new();

    public EscrutarCombinacionesFrmPage()
    {
        this.InitializeComponent();
    }
}
