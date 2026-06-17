using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila del resumen de la generación múltiple (legacy: cada ListViewItem de
/// listaResumen, con sus 5 SubItems). Todos los campos se exponen como string
/// para enlazarlos directamente a TextBlock.Text en el DataTemplate (regla
/// anti-crash: no bindear int/double a TextBlock.Text).
/// </summary>
public sealed class ResultadoCalculoMultipleItem
{
    /// <summary>Fichero de combinación analizado (legacy: columnHeader1 "Arch.Combinación").</summary>
    public string ArchivoCombinacion { get; set; } = string.Empty;

    /// <summary>Fichero de columnas generado (legacy: columnHeader2 "Arch.Columnas").</summary>
    public string ArchivoColumnas { get; set; } = string.Empty;

    /// <summary>Número de columnas analizadas (legacy: columnHeader3 "Cols.Analizadas").</summary>
    public string ColumnasAnalizadas { get; set; } = string.Empty;

    /// <summary>Número de columnas aceptadas (legacy: columnHeader4 "Cols.Aceptadas").</summary>
    public string ColumnasAceptadas { get; set; } = string.Empty;

    /// <summary>Tiempo invertido en el proceso (legacy: columnHeader5 "Tiempo").</summary>
    public string Tiempo { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel para la pantalla "Resultados de la Generación Múltiple"
/// (legacy: ResultadosCalculoMultipleFrm). Es una pantalla de solo lectura:
/// mantiene el listado de resultados de cada cálculo realizado en lote.
/// </summary>
public partial class ResultadosCalculoMultipleFrmViewModel : ObservableObject
{
    /// <summary>
    /// Handoff estático con las filas de resumen calculadas por el lote. Equivale a los
    /// ListViewItem que CalculaColumnasMultipleFrm volcaba en form.listaResumen antes de
    /// form.ShowDialog() (Free1X2/UI/CalculaColumnasMultipleFrm.cs líneas 240-255). El productor
    /// (CalculaColumnasMultipleFrmViewModel) lo fija y navega aquí; la página lo consume al
    /// construirse (mismo patrón que AnalizarCombinacionFrmViewModel.UltimoAnalisis /
    /// EstucolFrmViewModel.UltimoInforme).
    /// </summary>
    public static IReadOnlyList<ResultadoCalculoMultipleItem>? UltimosResultados { get; set; }

    /// <summary>
    /// Colección de resultados a mostrar en la rejilla
    /// (legacy: listaResumen.Items, alimentado por el formulario que abría este diálogo
    /// tras ejecutar la generación múltiple).
    /// </summary>
    public ObservableCollection<ResultadoCalculoMultipleItem> Resultados { get; } = new();

    public ResultadosCalculoMultipleFrmViewModel()
    {
        // Consume y limpia el handoff dejado por el productor (la página se reconstruye en cada
        // navegación, así que basta leerlo aquí). Sin handoff, la rejilla queda vacía (apertura
        // directa desde el menú sin haber ejecutado un cálculo en lote).
        var resultados = UltimosResultados;
        UltimosResultados = null;
        if (resultados != null)
        {
            Cargar(resultados);
        }
    }

    /// <summary>
    /// Carga las filas ya calculadas del proceso de generación múltiple.
    /// Equivale a cómo el formulario padre legacy poblaba listaResumen.Items antes de
    /// ResultadosCalculoMultipleFrm.ShowDialog() (Free1X2/UI/CalculaColumnasMultipleFrm.cs
    /// línea 240). Aquí solo se enlaza el resultado ya calculado: no se ejecuta el motor.
    /// </summary>
    public void Cargar(IEnumerable<ResultadoCalculoMultipleItem> resultados)
    {
        Resultados.Clear();
        foreach (var fila in resultados)
        {
            Resultados.Add(fila);
        }
    }
}
