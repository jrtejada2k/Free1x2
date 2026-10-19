// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Free1X2.Escrutinio;

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

    /// <summary>
    /// Posición de la premiada en el escrutinio (#5): con las premiadas por páginas, la selección
    /// va por ella para que siga al cambiar de página.
    /// </summary>
    public int Posicion { get; set; }

    /// <summary>Fila de una premiada del escrutinio (legacy: listaResumen.Items.Add en EscrutiniosFrm).</summary>
    public static ColumnaPremiadaItem De(ColumnasPremiadas p, int posicion)
    {
        // Legacy: NoBoleto + " (" + orden + ")", con orden = NoColumna % 8 (8 si 0).
        int orden = p.NoColumna % 8;
        if (orden == 0) orden = 8;
        return new ColumnaPremiadaItem
        {
            ArchivoColumnas = Path.GetFileName(p.Fichero),
            Jornada = p.Jornada.ToString(),
            Columna = p.Columna,
            Premio = p.Premio.ToString(),
            NumeroColumna = p.NoColumna.ToString(),
            NumeroBoleto = p.NoBoleto + " (" + orden + ")",
            Posicion = posicion,
        };
    }
}

/// <summary>
/// Las premiadas de un escrutinio, leídas del disco por páginas de 1.000 (pueden ser millones, #5),
/// para la tarjeta de Escrutinios y para «Columnas Premiadas». Vuelca cada página en <c>filas</c>.
/// </summary>
public sealed partial class PaginasPremiadas : ObservableObject
{
    private readonly ColeccionUi<ColumnaPremiadaItem> _filas;
    private int _pagina;

    public PaginasPremiadas(ColeccionUi<ColumnaPremiadaItem> filas) => _filas = filas;

    /// <summary>Antes de cambiar de página: para guardar la selección de la que se deja.</summary>
    public event Action? Cambiando;

    /// <summary>Con la página nueva ya en las filas.</summary>
    public event Action? Cargada;

    public PremiadasEnDisco? Premiadas { get; private set; }

    public bool VariasPaginas => (Premiadas?.Paginas ?? 0) > 1;

    public string Texto => Premiadas is null ? string.Empty : $"Página {_pagina + 1} de {Premiadas.Paginas}";

    /// <summary>Muestra la primera página de <paramref name="premiadas"/> (nada con <c>null</c>).</summary>
    public void Cargar(PremiadasEnDisco? premiadas)
    {
        Premiadas = premiadas;
        OnPropertyChanged(nameof(VariasPaginas));
        Ir(0);
    }

    private bool HayAnterior() => _pagina > 0;

    private bool HaySiguiente() => Premiadas is not null && _pagina + 1 < Premiadas.Paginas;

    [RelayCommand(CanExecute = nameof(HayAnterior))]
    private void Anterior() => Ir(_pagina - 1);

    [RelayCommand(CanExecute = nameof(HaySiguiente))]
    private void Siguiente() => Ir(_pagina + 1);

    private void Ir(int pagina)
    {
        Cambiando?.Invoke();
        _pagina = pagina;
        OnPropertyChanged(nameof(Texto));
        AnteriorCommand.NotifyCanExecuteChanged();
        SiguienteCommand.NotifyCanExecuteChanged();
        var filas = new List<ColumnaPremiadaItem>();
        if (Premiadas is { } p)
        {
            try
            {
                int primera = pagina * PremiadasEnDisco.PorPagina;
                var premiadas = p.Pagina(pagina);
                for (int i = 0; i < premiadas.Count; i++) filas.Add(ColumnaPremiadaItem.De(premiadas[i], primera + i));
            }
            catch (Exception ex)
            {
                Services.Log.Error("PaginasPremiadas.Ir [" + p.Ruta + "]", ex);
                Services.AppServices.MostrarError("No se pudieron leer las premiadas: " + ex.Message);
            }
        }
        // C-13: un único Reset por página.
        _filas.ReemplazarTodo(filas);
        Cargada?.Invoke();
    }
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
    /// <summary>Las premiadas del escrutinio por páginas (#5); sin cargar si vienen en una lista.</summary>
    public PaginasPremiadas Paginas { get; }

    public ColumnasPremiadasFrmViewModel()
    {
        Paginas = new PaginasPremiadas(Columnas);
    }

    [RelayCommand]
    private async Task GuardarTodasAsync()
    {
        // #5: las del escrutinio se leen del disco página a página.
        if (Paginas.Premiadas is { } p)
            await GuardarAsync(p.Total, p.Columnas);
        else
            await GuardarAsync(Columnas.Count, () => Columnas.Select(f => f.Columna));
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
        return GuardarAsync(seleccionadas.Count, () => seleccionadas.Select(f => f.Columna));
    }

    /// <summary>
    /// #5: las seleccionadas de las premiadas en disco, por su posición en el escrutinio y en ese
    /// orden, leyendo solo sus páginas.
    /// </summary>
    public Task GuardarSeleccionadasAsync(PremiadasEnDisco premiadas, IEnumerable<int> posiciones)
    {
        var orden = posiciones.OrderBy(i => i).ToList();
        return GuardarAsync(orden.Count, () => Seleccionadas(premiadas, orden));
    }

    private static IEnumerable<string> Seleccionadas(PremiadasEnDisco premiadas, List<int> orden)
    {
        int cargada = -1;
        List<ColumnasPremiadas> filas = new();
        foreach (int i in orden)
        {
            int pagina = i / PremiadasEnDisco.PorPagina;
            if (pagina != cargada)
            {
                filas = premiadas.Pagina(pagina);
                cargada = pagina;
            }
            yield return filas[i % PremiadasEnDisco.PorPagina].Columna;
        }
    }

    /// <summary>
    /// Escribe el texto de cada columna a un fichero elegido con FileSavePicker.
    /// Legacy: SaveFileDialog filtro "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*",
    /// StreamWriter -> writer.WriteLine(item.SubItems[2].Text) por fila.
    /// </summary>
    private static async Task GuardarAsync(int cuantas, Func<IEnumerable<string>> columnas)
    {
        if (cuantas == 0)
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
                foreach (string columna in columnas())
                {
                    writer.WriteLine(columna);
                }
            });

            Services.AppServices.MostrarInfo($"Guardadas {cuantas} columna(s) en {file.Name}.");
        }
        catch (Exception ex)
        {
            Services.AppServices.MostrarError($"No se pudieron guardar las columnas: {ex.Message}");
        }
    }
}
