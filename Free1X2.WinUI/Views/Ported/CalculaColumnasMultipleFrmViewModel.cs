using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del port WinUI 3 del WinForms <c>CalculaColumnasMultipleFrm</c>.
///
/// Propósito (legacy): procesar EN LOTE varios archivos de combinaciones
/// (*.comb / *.xml), aplicar el reductor/analizador a cada uno y volcar las columnas
/// resultantes en la carpeta "Columnas" como .txt. Durante el proceso muestra
/// estadísticas en vivo (columnas procesadas/aceptadas, coste, porcentaje, estimadas,
/// máximas) y tiempos de comienzo/fin.
///
/// Motor de dominio usado (idéntico al legacy BtnCalcularClick, Free1X2/UI/CalculaColumnasMultipleFrm.cs):
///   ArchivoCombinacion.AbrirArchivoCombinacion / CargaControladorGrupos / LeeFiltroColumnas / LeePronosticos,
///   Analizador.CtrlGrupos / ArchivoColumnasBase / Pronosticos / AnalizaCombinacion(string) / PararAnalisis /
///   ColsAnalizadas / ColsAceptadas, ArchivoColumnasTexto.ObtenNumCols, VariablesGlobales.PrecioApuesta/Moneda.
///
/// El bucle de cálculo corre en Task.Run y los contadores se refrescan en el hilo de UI
/// con un DispatcherQueueTimer (equivalente al Timer de 500 ms del form legacy).
/// </summary>
public partial class CalculaColumnasMultipleFrmViewModel : ObservableObject
{
    // Rutas completas de los archivos de combinaciones (legacy: ArrayList 'combinaciones').
    private readonly List<string> _combinaciones = new();

    // Analizador en curso (legacy: campo 'analizador', reasignado por cada archivo).
    private Analizador _analizador = new();

    // Columnas máximas del archivo en curso (legacy: campo 'colsMaximas').
    private long _colsMaximas;

    private CancellationTokenSource? _cts;

    /// <summary>
    /// Navegación a la pantalla de resultados a través del ContentFrame, inyectada por la Page
    /// (mismo patrón que ColGanadoraFrmViewModel.Navegar). El resumen viaja por el handoff
    /// estático ResultadosCalculoMultipleFrmViewModel.UltimosResultados, así que basta navegar
    /// al tipo de página. Equivale a form.ShowDialog() del legacy CalculaColumnasMultipleFrm.
    /// </summary>
    public Action<Type>? Navegar { get; set; }

    // Archivos de combinaciones seleccionados (ListBox listaFicheros del form legacy).
    public ObservableCollection<string> Ficheros { get; } = new();

    // Resultados del lote (legacy: filas de ResultadosCalculoMultipleFrm.listaResumen).
    // El host puede leer esta colección al terminar el cálculo para mostrar el resumen.
    public ObservableCollection<ResultadoCalculoMultipleItem> Resultados { get; } = new();

    // Índice seleccionado en la lista (listaFicheros.SelectedIndex).
    [ObservableProperty]
    private int _ficheroSeleccionado = -1;

    // true cuando hay al menos un fichero (btnCalcular.Enabled del legacy).
    public bool HayFicheros => Ficheros.Count > 0;

    // ===== Estadísticas en vivo (labels del form legacy) =====
    // Se exponen como string porque el dominio aporta long/double con formato
    // "#,##0" y "€ #,##0.00" (regla anti-crash: no bindear numérico directo a Text).

    // lblColsProcesadas / colProcesadaCoste
    [ObservableProperty] private string _columnasProcesadas = "0";
    [ObservableProperty] private string _costeProcesadas = "€ 0,00";

    // lblColsAdmitidas / colAceptadaCoste / lblPorcentaje
    [ObservableProperty] private string _columnasAceptadas = "0";
    [ObservableProperty] private string _costeAceptadas = "€ 0,00";
    [ObservableProperty] private string _porcentaje = "0,00 %";

    // lblColsEstimadas / colEstimadasCoste
    [ObservableProperty] private string _columnasEstimadas = "0";
    [ObservableProperty] private string _costeEstimadas = "€ 0,00";

    // lblColsMaximo / colMaximoCoste
    [ObservableProperty] private string _columnasMaximas = "0";
    [ObservableProperty] private string _costeMaximas = "€ 0,00";

    // horaComienzo / horaFinal / lblSeg
    [ObservableProperty] private string _horaComienzo = "00:00:00";
    [ObservableProperty] private string _horaFinal = "00:00:00";
    [ObservableProperty] private string _tiempoTotal = "0,0";

    // ===== Progreso (progressBar / progressBarArchivos del legacy) =====
    [ObservableProperty] private double _progresoColumnas;   // 0..100 (archivo en curso)
    [ObservableProperty] private double _progresoArchivos;   // 0..100 (lote completo)
    [ObservableProperty] private bool _progresoVisible;      // visibilidad de ambas barras

    // btnCalcular.Enabled: hay ficheros y no hay un proceso en marcha.
    [ObservableProperty] private bool _procesoEnMarcha;

    public bool PuedeCalcular => HayFicheros && !ProcesoEnMarcha;

    partial void OnProcesoEnMarchaChanged(bool value) => OnPropertyChanged(nameof(PuedeCalcular));

    [RelayCommand]
    private async Task SeleccionarArchivosAsync()
    {
        // Legacy btnSelArch_Click: OpenFileDialog (Multiselect) sobre "{StartupPath}/Combinaciones/",
        // filtro ".comb / .xml / *". Tras elegir: combinaciones.Sort() y rellenar el ListBox con
        // Path.GetFileName de cada ruta. Habilita Calcular (HayFicheros / PuedeCalcular).
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".comb");
        picker.FileTypeFilter.Add(".xml");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var files = await picker.PickMultipleFilesAsync();
        if (files == null || files.Count == 0) return;

        // Elimina la lista de ficheros anteriores (legacy: combinaciones.Clear / listaFicheros.Items.Clear).
        _combinaciones.Clear();
        Ficheros.Clear();
        foreach (var f in files)
        {
            _combinaciones.Add(f.Path);
        }
        // Ordena las combinaciones y las añade a la lista (legacy: combinaciones.Sort()).
        _combinaciones.Sort(StringComparer.Ordinal);
        foreach (var ruta in _combinaciones)
        {
            Ficheros.Add(Path.GetFileName(ruta));
        }

        OnPropertyChanged(nameof(HayFicheros));
        OnPropertyChanged(nameof(PuedeCalcular));
    }

    [RelayCommand]
    private async Task CalcularAsync()
    {
        // Legacy BtnCalcularClick. Carpeta destino = "{BaseDirectory}/Columnas/"; cada fichero
        // de salida es el nombre de origen con extensión .txt (.comb/.xml -> .txt).
        if (ProcesoEnMarcha || _combinaciones.Count == 0) return;

        // Carpeta destino: AppContext.BaseDirectory equivale a Application.StartupPath del legacy.
        string baseDir = AppContext.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string carpetaDestino = Path.Combine(baseDir, "Columnas");
        try
        {
            Directory.CreateDirectory(carpetaDestino);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo acceder a la carpeta de salida: " + ex.Message);
            return;
        }

        // Construye la lista de ficheros de salida (legacy: ficherosSalida) cambiando la extensión.
        var ficherosSalida = new List<string>();
        foreach (var origen in _combinaciones)
        {
            string nombre = Path.GetFileName(origen)
                .Replace(".comb", ".txt")
                .Replace(".xml", ".txt");
            ficherosSalida.Add(Path.Combine(carpetaDestino, nombre));
        }

        // Comprueba si ya existen y, si es así, pide confirmación de reemplazo (legacy: MessageBox YesNo).
        // En WinUI no se puede mostrar un diálogo modal por archivo sin acoplar la UI; replicamos la
        // semántica con una única confirmación cuando alguno de los destinos ya existe.
        bool algunoExiste = false;
        foreach (var destino in ficherosSalida)
        {
            if (File.Exists(destino)) { algunoExiste = true; break; }
        }
        if (algunoExiste && !await ConfirmarReemplazoAsync())
        {
            return; // El usuario decidió no reemplazar; se cancela todo el lote.
        }

        ProcesoEnMarcha = true;
        ProgresoVisible = true;
        ProgresoColumnas = 0;
        ProgresoArchivos = 0;
        HoraFinal = "";
        TiempoTotal = "";
        Resultados.Clear();
        _cts = new CancellationTokenSource();

        var dt1 = DateTime.Now;
        var dtTemp1 = DateTime.Now;
        HoraComienzo = dt1.ToLongTimeString();

        // Timer de refresco en el hilo de UI (equivale al Timer 500 ms + ActualizaDatosCalculo).
        var dispatcher = AppServices.UiDispatcher;
        var timer = dispatcher?.CreateTimer();
        if (timer != null)
        {
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (_, _) => ActualizaDatosCalculo();
            timer.Start();
        }

        try
        {
            for (int i = 0; i < ficherosSalida.Count; i++)
            {
                if (_cts.IsCancellationRequested) break;

                FicheroSeleccionado = i;
                ProgresoArchivos = (i + 1) * 100.0 / ficherosSalida.Count;

                string ficheroOrigen = _combinaciones[i];
                string ficheroDestino = ficherosSalida[i];

                // Replica EXACTA del legacy (líneas 217-227): abrir combinación, cargar grupos,
                // leer filtro/pronósticos, calcular columnas previstas y analizar.
                await Task.Run(() =>
                {
                    var archComb = new ArchivoCombinacion();
                    archComb.AbrirArchivoCombinacion(ficheroOrigen);
                    _analizador = new Analizador();
                    archComb.CargaControladorGrupos(_analizador.CtrlGrupos);
                    archComb.LeeFiltroColumnas();
                    _analizador.ArchivoColumnasBase = archComb.LeeFiltroColumnas();
                    _analizador.Pronosticos = archComb.LeePronosticos();
                    archComb.Pronosticos = _analizador.Pronosticos;
                    ActualizaColumnasPrevistas();
                    _analizador.AnalizaCombinacion(ficheroDestino);
                }, _cts.Token);

                var dtTemp2 = DateTime.Now;

                // Fila de resumen (legacy: ListViewItem añadido a form.listaResumen.Items).
                Resultados.Add(new ResultadoCalculoMultipleItem
                {
                    ArchivoCombinacion = Path.GetFileName(ficheroOrigen),
                    ArchivoColumnas = Path.GetFileName(ficheroDestino),
                    ColumnasAnalizadas = _analizador.ColsAnalizadas.ToString(),
                    ColumnasAceptadas = _analizador.ColsAceptadas.ToString(),
                    Tiempo = dtTemp2.Subtract(dtTemp1).ToString(),
                });
                dtTemp1 = DateTime.Now;
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelado por el usuario; se sale del bucle de forma limpia.
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error en el cálculo en lote: " + ex.Message);
        }
        finally
        {
            timer?.Stop();
            var dt2 = DateTime.Now;
            HoraFinal = dt2.ToLongTimeString();
            TiempoTotal = dt2.Subtract(dt1).ToString();
            // Refresco final con el último archivo procesado (legacy: activa procesoEnMarcha y refresca).
            ActualizaDatosCalculo();

            ProcesoEnMarcha = false;
            ProgresoVisible = false;
        }

        // Legacy: form.ShowDialog() abre ResultadosCalculoMultipleFrm con el resumen ya cargado
        // (Free1X2/UI/CalculaColumnasMultipleFrm.cs línea 255). Se deja el resumen en el handoff
        // estático y se navega a la pantalla de resultados, que lo consume al construirse.
        ResultadosCalculoMultipleFrmViewModel.UltimosResultados =
            new List<ResultadoCalculoMultipleItem>(Resultados);
        Navegar?.Invoke(typeof(ResultadosCalculoMultipleFrmPage));
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy BtnCancelarClick: analizador.PararAnalisis() detiene el GeneradorColumnas.
        _analizador.PararAnalisis();
        _cts?.Cancel();
        ProcesoEnMarcha = false;
        ProgresoVisible = false;
    }

    /// <summary>
    /// Calcula las columnas máximas previstas del archivo en curso.
    /// Replica EXACTA de actualizaColumnasPrevistas() del form legacy (línea 106):
    /// si hay filtro base, ObtenNumCols(); si no, 2^dobles * 3^triples.
    /// </summary>
    private void ActualizaColumnasPrevistas()
    {
        if (!string.IsNullOrEmpty(_analizador.ArchivoColumnasBase) && _analizador.ArchivoColumnasBase.Length > 0)
        {
            // Hay un filtro: si el archivo existe, lee su número de columnas; si no, calcula sin filtro.
            if (File.Exists(_analizador.ArchivoColumnasBase))
            {
                IArchivoColumnas f = new ArchivoColumnasTexto(_analizador.ArchivoColumnasBase);
                _colsMaximas = f.ObtenNumCols();
                f.Cerrar();
            }
            else
            {
                AppServices.MostrarInfo("No se ha encontrado el filtro " + _analizador.ArchivoColumnasBase +
                                        ". Se calculan las columnas sin filtro.");
                _analizador.ArchivoColumnasBase = "";
                CalcularCols();
            }
        }
        else
        {
            CalcularCols();
        }

        ColumnasMaximas = _colsMaximas.ToString("#,##0;0");
        double costeMaximo = _colsMaximas * Free1X2.VariablesGlobales.PrecioApuesta;
        CosteMaximas = costeMaximo.ToString(Free1X2.VariablesGlobales.Moneda + "#,##0.00;0.0");
    }

    // Replica EXACTA de calcularCols() del form legacy (línea 92): 2^dobles * 3^triples.
    private void CalcularCols()
    {
        int dobles = 0, triples = 0;
        var pron = _analizador.Pronosticos;
        if (pron != null)
        {
            for (int i = 0; i < pron.Length; i++)
            {
                string p = (pron[i] ?? "").Replace(",", "");
                if (p.Length == 2) dobles++;
                if (p.Length == 3) triples++;
            }
        }
        _colsMaximas = Convert.ToInt64(Math.Pow(2, dobles) * Math.Pow(3, triples));
    }

    // Replica de ActualizaDatosCalculo() del form legacy (línea 134): refresca contadores/costes/porcentaje.
    private void ActualizaDatosCalculo()
    {
        int procesadas = _analizador.ColsAnalizadas;
        int aceptadas = _analizador.ColsAceptadas;
        double precio = Free1X2.VariablesGlobales.PrecioApuesta;
        string moneda = Free1X2.VariablesGlobales.Moneda;

        ColumnasProcesadas = procesadas.ToString("#,##0;0");
        CosteProcesadas = (procesadas * precio).ToString(moneda + "#,##0.00;0.0");

        ProgresoColumnas = _colsMaximas > 0
            ? Math.Min(100.0, procesadas * 100.0 / _colsMaximas)
            : 0;

        ColumnasAceptadas = aceptadas.ToString("#,##0;0");
        CosteAceptadas = (aceptadas * precio).ToString(moneda + "#,##0.00;0.0");

        double porcentaje = procesadas > 0 ? aceptadas * 100.0 / procesadas : 0;
        Porcentaje = porcentaje.ToString("#,##0.00;0.00") + " %";

        double colsEstimadas = Math.Round(_colsMaximas * porcentaje / 100, 0);
        ColumnasEstimadas = colsEstimadas.ToString("#,##0;0");
        CosteEstimadas = (colsEstimadas * precio).ToString(moneda + "#,##0.00;0.0");
    }

    private static async Task<bool> ConfirmarReemplazoAsync()
    {
        var root = AppServices.MainWindow?.Content?.XamlRoot;
        if (root is null) return true; // headless: comportamiento por defecto = continuar.

        var dlg = new Microsoft.UI.Xaml.Controls.ContentDialog
        {
            Title = "Free1X2",
            Content = "Algunos archivos de columnas ya existen. ¿Deseas reemplazarlos?",
            PrimaryButtonText = "Reemplazar",
            CloseButtonText = "Cancelar",
            DefaultButton = Microsoft.UI.Xaml.Controls.ContentDialogButton.Primary,
            XamlRoot = root,
        };
        var resultado = await dlg.ShowAsync();
        return resultado == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary;
    }
}
