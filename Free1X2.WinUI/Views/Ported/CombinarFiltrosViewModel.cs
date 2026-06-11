using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Representa una fila del grid "Filtros" del WinForms legacy CombinarFiltros.
/// Columnas legacy: N (orden), F (nombre archivo), FP (ruta completa),
/// A (activo/bool), C (cols. contadas tras el proceso).
/// </summary>
public partial class FiltroFilaViewModel : ObservableObject
{
    [ObservableProperty]
    private int _numero;

    // Regla anti-crash #2: no bindear int directo a Text.
    public string NumeroTexto => Numero.ToString();

    partial void OnNumeroChanged(int value) => OnPropertyChanged(nameof(NumeroTexto));

    [ObservableProperty]
    private string _nombre = string.Empty;

    // FP legacy: ruta completa del archivo de columnas (no se muestra en el grid legacy).
    [ObservableProperty]
    private string _ruta = string.Empty;

    [ObservableProperty]
    private bool _activo = true;

    [ObservableProperty]
    private int _columnas;

    // Regla anti-crash #2: no bindear int directo a Text. Exponemos string.
    public string ColumnasTexto => Columnas.ToString();

    partial void OnColumnasChanged(int value) => OnPropertyChanged(nameof(ColumnasTexto));
}

/// <summary>
/// ViewModel portado del WinForms CombinarFiltros ("Combinación de Filtros").
/// Combina varios archivos de columnas (.txt) contando en cuántos filtros
/// aparece cada columna, y permite grabar/sumar las que caen en un rango
/// [mínimo, máximo] de "filtros acertados".
/// Toda la lógica de dominio (lectura/escritura de archivos, conteo) está
/// marcada como TODO citando la clase legacy.
/// </summary>
public partial class CombinarFiltrosViewModel : ObservableObject
{
    public ObservableCollection<FiltroFilaViewModel> Filtros { get; } = new();

    // tbmin / tbmax legacy (TextBox numéricos, valor por defecto "1").
    // NumberBox.Value es double (regla anti-crash #7).
    [ObservableProperty]
    private double _minimo = 1;

    [ObservableProperty]
    private double _maximo = 1;

    // ckMD legacy: "Activa / Desactiva" todas las filas a la vez.
    [ObservableProperty]
    private bool _activarTodas = true;

    // lCols legacy: total de columnas resultantes.
    [ObservableProperty]
    private int _totalColumnas;

    public string TotalColumnasTexto => TotalColumnas.ToString();

    partial void OnTotalColumnasChanged(int value) => OnPropertyChanged(nameof(TotalColumnasTexto));

    // lfilout legacy: nombre del archivo de salida grabado.
    [ObservableProperty]
    private string _archivoSalida = string.Empty;

    // bIniciar legacy: solo procede si hay filtros cargados (limite > 0).
    [ObservableProperty]
    private bool _procesando;

    public bool HayFiltros => Filtros.Count > 0;

    public CombinarFiltrosViewModel()
    {
        Filtros.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HayFiltros));
    }

    // btnCargarFiltro -> CargarFiltro() legacy.
    [RelayCommand]
    private void CargarFiltro()
    {
        // TODO: portar CombinarFiltros.CargarFiltro():
        //   OpenFileDialog multiselección (*.txt), valida que todos los filtros
        //   tengan el mismo número de partidos (ArchivoColumnasTexto.ObtenNumSignos),
        //   añade una fila por archivo y llama AdaptarANumeroDePartidos().
    }

    // bCargaLista -> CargarLista() legacy.
    [RelayCommand]
    private void CargarLista()
    {
        // TODO: portar CombinarFiltros.CargarLista():
        //   limpia el grid, abre un .lst (lista de rutas), lee cada línea como
        //   un ArchivoColumnasTexto, valida número de partidos y reconstruye la lista.
    }

    // bSalvaLista -> SalvarLista() legacy.
    [RelayCommand]
    private void SalvarLista()
    {
        // TODO: portar CombinarFiltros.SalvarLista():
        //   SaveFileDialog (*.lst), escribe la ruta (FP) de cada fila en líneas.
    }

    // bIniciar -> IniciarProceso() legacy.
    [RelayCommand]
    private void Iniciar()
    {
        // TODO: portar CombinarFiltros.IniciarProceso():
        //   recorre cada filtro activo, lee sus columnas (ArchivoColumnasTexto),
        //   normaliza (Pral.Normaliza), convierte a índice (s2n) y cuenta en
        //   htCols[] cuántos filtros (sin repetir, BitArray repes) contienen
        //   cada columna; actualiza la columna C de cada fila.
    }

    // bCancelar -> salida = true en BCancelarClick legacy.
    [RelayCommand]
    private void Cancelar()
    {
        // TODO: portar CombinarFiltros: poner el flag 'salida' a true para
        //   abortar el bucle de IniciarProceso().
        Procesando = false;
    }

    // btnGrabarFiltro -> Grabar() legacy.
    [RelayCommand]
    private void GrabarColumnas()
    {
        // TODO: portar CombinarFiltros.Grabar():
        //   SaveFileDialog (*.txt), recorre htCols[], graba (n2s) las columnas
        //   cuyo conteo esté en [Minimo, Maximo] vía ArchivoColumnasTexto.GuardarCols;
        //   actualiza TotalColumnas y ArchivoSalida.
    }

    // bSumaCols -> SumaCols() legacy.
    [RelayCommand]
    private void SumarColumnas()
    {
        // TODO: portar CombinarFiltros.SumaCols():
        //   cuenta (sin grabar) cuántas columnas de htCols[] caen en [Minimo, Maximo]
        //   y lo refleja en TotalColumnas.
    }

    // btnReiniciaTodo -> btnReiniciaTodo_Click legacy.
    [RelayCommand]
    private void ReiniciarTodo()
    {
        Filtros.Clear();
        TotalColumnas = 0;
        ArchivoSalida = string.Empty;
        // TODO: portar CombinarFiltros.btnReiniciaTodo_Click():
        //   reinicia InicializaFuenteDatos(), partidosEnJuego = 0 y limite = 0.
    }

    // ckMD -> MarcaDesmarca() legacy.
    [RelayCommand]
    private void MarcarDesmarcar()
    {
        foreach (var fila in Filtros)
        {
            fila.Activo = ActivarTodas;
        }
        // TODO: equivalente a CombinarFiltros.MarcaDesmarca() sobre el DataSet legacy.
    }
}
