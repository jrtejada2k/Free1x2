using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "EscrutarCombinacionesFrm".
/// Escruta (puntúa) ficheros de combinaciones contra columna(s) ganadora(s) en tres modos:
/// escrutinio simple (columna manual de 14 signos), contra un fichero de referencia y contra
/// las jornadas de una/varias temporadas (plantilla de nombre + dígitos de temporada/jornada).
/// El original tenía un TabControl (tpSimple/tbFichero/tbTemporada), una lista de ficheros, el
/// rango de aciertos, el grid de resultados (dgResultados) y botones de acción.
/// El motor (EscrutadorComb), la lectura del histórico de jornadas, la grabación de columnas
/// y la lista de premiadas están cableados en el ViewModel.
/// </summary>
public sealed partial class EscrutarCombinacionesFrmPage : Page
{
    public EscrutarCombinacionesFrmViewModel ViewModel { get; } = new();

    public EscrutarCombinacionesFrmPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Propaga al ViewModel las temporadas marcadas (legacy: lstTemporadas.SelectedIndices,
    /// MultiExtended). La selección múltiple vive en el control de UI.
    /// </summary>
    private void OnTemporadasSeleccionadas(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.TemporadasSeleccionadas.Clear();
        foreach (var item in ListaTemporadas.SelectedItems)
        {
            if (item is string s && int.TryParse(s, out int temp))
            {
                ViewModel.TemporadasSeleccionadas.Add(temp);
            }
        }
    }
}
