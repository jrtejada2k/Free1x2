using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila del resumen de columnas premiadas (legacy: cada ListViewItem de listaResumen,
/// con sus 6 SubItems). Todos los campos se exponen como string para enlazarlos
/// directamente a TextBlock.Text en el DataTemplate.
/// </summary>
public sealed class ColumnaPremiadaItem
{
    /// <summary>Fichero de columnas de origen (legacy: columnHeader1 "Arch.Columnas").</summary>
    public string ArchivoColumnas { get; set; } = string.Empty;

    /// <summary>Jornada (legacy: columnHeader2 "Jorn.").</summary>
    public string Jornada { get; set; } = string.Empty;

    /// <summary>Texto de la columna jugada (legacy: columnHeader3 "Columna", SubItems[2]).</summary>
    public string Columna { get; set; } = string.Empty;

    /// <summary>Categoría de premio obtenida (legacy: columnHeader4 "Premio").</summary>
    public string Premio { get; set; } = string.Empty;

    /// <summary>Número de columna dentro del boleto (legacy: columnHeader5 "Nº Col.").</summary>
    public string NumeroColumna { get; set; } = string.Empty;

    /// <summary>Número de boleto (legacy: columnHeader6 "Nº Boleto").</summary>
    public string NumeroBoleto { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel para la pantalla "Columnas Premiadas" (legacy: ColumnasPremiadasFrm).
/// Mantiene el listado de columnas premiadas y expone las acciones de exportación
/// a fichero (todas / seleccionadas).
/// </summary>
public partial class ColumnasPremiadasFrmViewModel : ObservableObject
{
    /// <summary>
    /// Colección de columnas premiadas a mostrar en la rejilla
    /// (legacy: listaResumen.Items, alimentado por el formulario que abría este diálogo).
    /// </summary>
    public ObservableCollection<ColumnaPremiadaItem> Columnas { get; } = new();

    /// <summary>
    /// Exporta TODAS las columnas del listado a un fichero de texto
    /// (legacy: btnGuardarTodas_Click).
    /// </summary>
    [RelayCommand]
    private void GuardarTodas()
    {
        // TODO[dominio]: exportar todas las filas a un fichero de texto.
        //   Legacy: ColumnasPremiadasFrm.btnGuardarTodas_Click
        //     - SaveFileDialog filtro "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*".
        //       En WinUI usar Windows.Storage.Pickers.FileSavePicker.
        //     - StreamWriter writer = new StreamWriter(nombre);
        //       for (i=0..listaResumen.Items.Count)
        //         writer.WriteLine(listaResumen.Items[i].SubItems[2].Text);  // columna (índice 2)
        //       writer.Close();
        //   Aquí equivale a escribir item.Columna por cada elemento de Columnas.
    }

    /// <summary>
    /// Exporta únicamente las columnas seleccionadas en el ListView
    /// (legacy: btnGuardarSeleccionadas_Click). La selección se obtiene desde el
    /// code-behind porque vive en el control de UI.
    /// </summary>
    public void GuardarSeleccionadas(IReadOnlyList<ColumnaPremiadaItem> seleccionadas)
    {
        // TODO[dominio]: exportar solo las filas seleccionadas a un fichero de texto.
        //   Legacy: ColumnasPremiadasFrm.btnGuardarSeleccionadas_Click
        //     - SaveFileDialog filtro "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*".
        //       En WinUI usar Windows.Storage.Pickers.FileSavePicker.
        //     - StreamWriter writer = new StreamWriter(nombre);
        //       for (i=0..Count) if (listaResumen.Items[i].Selected)
        //         writer.WriteLine(listaResumen.Items[i].SubItems[2].Text);  // columna (índice 2)
        //       writer.Close();
        //   Aquí 'seleccionadas' ya son los ColumnaPremiadaItem marcados; escribir item.Columna.
    }
}
