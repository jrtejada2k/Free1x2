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
    /// Colección de resultados a mostrar en la rejilla
    /// (legacy: listaResumen.Items, alimentado por el formulario que abría este diálogo
    /// tras ejecutar la generación múltiple).
    /// </summary>
    public ObservableCollection<ResultadoCalculoMultipleItem> Resultados { get; } = new();

    // TODO[dominio]: poblar 'Resultados' con las filas del proceso de generación múltiple.
    //   Legacy: el formulario que abría ResultadosCalculoMultipleFrm añadía cada fila vía
    //   listaResumen.Items.Add(new ListViewItem(new[]{ archCombinacion, archColumnas,
    //   colsAnalizadas, colsAceptadas, tiempo })).
    //   No implementar aquí el cálculo; solo enlazar el resultado ya calculado.
}
