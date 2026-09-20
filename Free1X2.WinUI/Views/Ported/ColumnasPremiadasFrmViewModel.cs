// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Windows.Storage;
using Windows.Storage.Pickers;

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
/// a fichero (todas / seleccionadas). La exportación replica el legacy: por cada fila
/// se escribe el texto de la columna (SubItems[2], aquí <see cref="ColumnaPremiadaItem.Columna"/>).
/// </summary>
public partial class ColumnasPremiadasFrmViewModel : ObservableObject
{
    /// <summary>
    /// Colección de columnas premiadas a mostrar en la rejilla
    /// (legacy: listaResumen.Items, alimentado por el formulario que abría este diálogo).
    /// </summary>
    /// <remarks>C-13: <see cref="ColeccionUi{T}"/> para volcar el resumen de una sola vez.</remarks>
    public ColeccionUi<ColumnaPremiadaItem> Columnas { get; } = new();

    /// <summary>
    /// Exporta TODAS las columnas del listado a un fichero de texto
    /// (legacy: btnGuardarTodas_Click).
    /// </summary>
    [RelayCommand]
    private async Task GuardarTodasAsync()
    {
        await GuardarAsync(Columnas);
    }

    /// <summary>
    /// Exporta únicamente las columnas seleccionadas en el ListView
    /// (legacy: btnGuardarSeleccionadas_Click). La selección se obtiene desde el
    /// code-behind porque vive en el control de UI.
    /// </summary>
    /// <remarks>
    /// C-27: devuelve <c>Task</c> (antes era <c>async void</c> sin ser un manejador de evento,
    /// así que una excepción no capturada habría llegado al manejador global sin contexto).
    /// El <c>async void</c> queda donde corresponde: el handler de la página, que hace await.
    /// </remarks>
    public Task GuardarSeleccionadasAsync(IReadOnlyList<ColumnaPremiadaItem> seleccionadas)
    {
        return GuardarAsync(seleccionadas);
    }

    /// <summary>
    /// Escribe el texto de cada columna a un fichero elegido con FileSavePicker.
    /// Legacy: SaveFileDialog filtro "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*",
    /// StreamWriter -> writer.WriteLine(item.SubItems[2].Text) por fila.
    /// </summary>
    private static async Task GuardarAsync(IReadOnlyList<ColumnaPremiadaItem> filas)
    {
        if (filas is null || filas.Count == 0)
        {
            Services.AppServices.MostrarInfo("No hay columnas que guardar.");
            return;
        }

        try
        {
            // Legacy: "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*".
            StorageFile? file = await Services.PickerHelper.GuardarAsync("columnas", ("Columnas", ".txt"));
            if (file is null)
            {
                return;
            }

            // Legacy: StreamWriter writer = new StreamWriter(nombre); por fila WriteLine(columna).
            await Task.Run(() =>
            {
                using var writer = new StreamWriter(file.Path);
                foreach (var fila in filas)
                {
                    writer.WriteLine(fila.Columna);
                }
            });

            Services.AppServices.MostrarInfo($"Guardadas {filas.Count} columna(s) en {file.Name}.");
        }
        catch (Exception ex)
        {
            Services.AppServices.MostrarError($"No se pudieron guardar las columnas: {ex.Message}");
        }
    }
}
