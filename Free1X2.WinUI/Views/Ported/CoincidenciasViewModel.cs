// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Microsoft.UI.Dispatching;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la rejilla de Condiciones del form legacy <c>Coincidencias</c> (DataGrid <c>tabcond</c>).
/// "Grupos" = lista de números de característica (1-108, p.ej. "1-9,33,40") cuya coincidencia se cuenta;
/// "Rango" = números de coincidencias admitidos para ese grupo (p.ej. "0-3"); "Rsl" = resultado calculado.
/// </summary>
public partial class CoincidenciaCondicionViewModel : ObservableObject
{
    public CoincidenciaCondicionViewModel(string grupos = "", string rango = "")
    {
        _grupos = grupos;
        _rango = rango;
    }

    // Columna "G" del DataGrid legacy (lista de características separadas por comas / rangos con guion).
    [ObservableProperty]
    private string _grupos = string.Empty;

    // Columna "M" del DataGrid legacy (rango de coincidencias admitidas).
    [ObservableProperty]
    private string _rango = string.Empty;

    // Columna "R" del DataGrid legacy (resultado tras Analizar/Calcular). Solo lectura en pantalla.
    [ObservableProperty]
    private int _resultado;

    // Bindeable a TextBlock (regla anti-crash: no bindear int directo a Text).
    public string ResultadoTexto => Resultado.ToString();

    partial void OnResultadoChanged(int value) => OnPropertyChanged(nameof(ResultadoTexto));
}

/// <summary>
/// Una fila de la "Radiografía" (DataGrid <c>tabrad</c>): código de característica + descripción + valor.
/// Las 108 descripciones provienen de <c>InitTexs()</c> del form legacy.
/// </summary>
public partial class RadiografiaFilaViewModel : ObservableObject
{
    public RadiografiaFilaViewModel(int codigo, string descripcion, int valor)
    {
        Codigo = codigo;
        Descripcion = descripcion;
        _valor = valor;
    }

    public int Codigo { get; }
    public string CodigoTexto => Codigo.ToString();
    public string Descripcion { get; }

    [ObservableProperty]
    private int _valor;

    public string ValorTexto => Valor.ToString();

    partial void OnValorChanged(int value) => OnPropertyChanged(nameof(ValorTexto));
}

/// <summary>
/// Una fila de la "Distribución de Aceptadas" (DataGrid <c>tabout</c>): nº de coincidencias (0-108),
/// un check (S) para incluirla al grabar, y los recuentos s/AC, s/GR y AMC del índice seleccionado.
/// </summary>
public partial class DistribucionFilaViewModel : ObservableObject
{
    public DistribucionFilaViewModel(int numero)
    {
        Numero = numero;
        _seleccionada = true;
    }

    public int Numero { get; }
    public string NumeroTexto => Numero.ToString();

    // Columna "S" (DataGridBoolColumn legacy): incluir esta fila al grabar resultados.
    [ObservableProperty]
    private bool _seleccionada;

    // Columna "Q" (s/AC: distribución por nº de aceptadas).
    [ObservableProperty]
    private int _porAceptadas;

    public string PorAceptadasTexto => PorAceptadas.ToString();

    // Columna "G" (s/GR: distribución por grupo/característica).
    [ObservableProperty]
    private int _porGrupo;

    public string PorGrupoTexto => PorGrupo.ToString();

    // Columna "M" (AMC del índice de grupo seleccionado).
    [ObservableProperty]
    private int _amc;

    public string AmcTexto => Amc.ToString();

    partial void OnPorAceptadasChanged(int value) => OnPropertyChanged(nameof(PorAceptadasTexto));
    partial void OnPorGrupoChanged(int value) => OnPropertyChanged(nameof(PorGrupoTexto));
    partial void OnAmcChanged(int value) => OnPropertyChanged(nameof(AmcTexto));
}

/// <summary>
/// ViewModel de la pantalla "Coincidencias" (port del WinForms <c>Coincidencias</c>).
///
/// Propósito del form legacy: a partir de un fichero de columnas de entrada y dos columnas
/// ganadoras de referencia (una "anterior" y una "reciente"), calcula la "radiografía" de 108
/// características de cada columna, cuenta las coincidencias por grupos definidos por el usuario
/// y filtra/clasifica las columnas válidas, mostrando distribuciones y permitiendo grabarlas.
///
/// Toda la lógica de dominio (lectura de ficheros, cálculo de radiografía, validación y grabado)
/// está marcada como TODO citando los métodos de la clase legacy <c>Coincidencias</c>.
/// </summary>
public partial class CoincidenciasViewModel : ObservableObject
{
    public CoincidenciasViewModel()
    {
        Condiciones = new ObservableCollection<CoincidenciaCondicionViewModel>
        {
            new("1-9", "0-9"),
        };

        Radiografia = new ObservableCollection<RadiografiaFilaViewModel>();
        for (int i = 0; i < Descripciones.Length; i++)
        {
            Radiografia.Add(new RadiografiaFilaViewModel(i + 1, Descripciones[i], 0));
        }

        Distribucion = new ObservableCollection<DistribucionFilaViewModel>();
        for (int n = 0; n < 109; n++)
        {
            Distribucion.Add(new DistribucionFilaViewModel(n));
        }

        // Índices AMC disponibles (uno por cada condición). ComboBox legacy lbAMC + botones +/-.
        IndicesAmc = new[] { "1" };
    }

    // ===== Ficheros y contadores (etiquetas lfilein/lcolsin/lfileout/lcolsout del form legacy) =====

    // Nombre del fichero de entrada seleccionado (label lfilein).
    [ObservableProperty]
    private string _ficheroEntrada = "(ningún fichero)";

    // Columnas leídas del fichero de entrada (label lcolsin).
    [ObservableProperty]
    private string _columnasLeidas = "0";

    // Nombre del fichero de condiciones (.cnd) cargado/guardado (label lfileconds).
    [ObservableProperty]
    private string _ficheroCondiciones = "(ninguno)";

    // Nombre del fichero de resultados grabado (label lfileout).
    [ObservableProperty]
    private string _ficheroSalida = "(sin grabar)";

    // Columnas grabadas en el resultado (label lcolsout).
    [ObservableProperty]
    private string _columnasGrabadas = "0";

    // ===== Estado de proceso (labels lColsProc/lColsAdm/lTime del form legacy) =====

    [ObservableProperty]
    private string _columnasProcesadas = "0";

    [ObservableProperty]
    private string _columnasValidas = "0";

    [ObservableProperty]
    private string _tiempo = "00:00:00.0";

    // ===== Columna ganadora ANTERIOR (grupo bFGA / bMas / bMenos / lbCGA / ltColA) =====

    // Fichero de ganadoras anteriores (label lFGA).
    [ObservableProperty]
    private string _ficheroGanadorasAnterior = "(sin fichero)";

    // Índice / total de la ganadora anterior seleccionada (label lbCGA).
    [ObservableProperty]
    private string _indiceGanadoraAnterior = "0";

    // Columna ganadora anterior actual (label ltColA).
    [ObservableProperty]
    private string _columnaGanadoraAnterior = "··············";

    // ===== Columna ganadora RECIENTE (grupo bFGR / bMasR / bMenosR / lbCGR / ltColR) =====

    // Fichero de ganadoras recientes (label lFGR).
    [ObservableProperty]
    private string _ficheroGanadorasReciente = "(sin fichero)";

    // Índice / total de la ganadora reciente seleccionada (label lbCGR).
    [ObservableProperty]
    private string _indiceGanadoraReciente = "0";

    // Columna ganadora reciente actual (label ltColR).
    [ObservableProperty]
    private string _columnaGanadoraReciente = "··············";

    // ===== Selector AMC (label lbAMC + botones bAMC1/bAMC2) =====

    public IReadOnlyList<string> IndicesAmc { get; }

    // Índice AMC seleccionado para la columna "AMC" de la distribución.
    [ObservableProperty]
    private string _indiceAmcSeleccionado = "1";

    // ===== Colecciones bindeadas a las tres rejillas =====

    public ObservableCollection<CoincidenciaCondicionViewModel> Condiciones { get; }
    public ObservableCollection<RadiografiaFilaViewModel> Radiografia { get; }
    public ObservableCollection<DistribucionFilaViewModel> Distribucion { get; }

    // ===== Comandos: condiciones =====

    [RelayCommand]
    private void AgregarCondicion()
    {
        // Equivale a añadir una fila al DataGrid tabcond del form legacy.
        Condiciones.Add(new CoincidenciaCondicionViewModel());
    }

    [RelayCommand]
    private void QuitarCondicion()
    {
        if (Condiciones.Count > 0)
        {
            Condiciones.RemoveAt(Condiciones.Count - 1);
        }
    }

    // ===== Comandos: ficheros de entrada / ganadoras =====

    [RelayCommand]
    private async Task SeleccionarFicheroEntrada()
    {
        // Equivale a Coincidencias.LeerFileIn(): lee cada columna y marca entrada[s2n(col)] = true.
        var file = await ElegirFicheroAbrir(".txt");
        if (file == null) return;
        try
        {
            _entrada.SetAll(false);
            int ctproc = 0;
            string rutaArchivo = file;
            await Task.Run(() =>
            {
                using var sr = new StreamReader(rutaArchivo);
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string columna = linea.Trim().ToUpper();
                    if (columna.Length < 14) throw new InvalidOperationException("Error longitud=" + columna);
                    ctproc++;
                    _entrada[S2N(columna)] = true;
                }
            });
            _ctproc = ctproc;
            FicheroEntrada = Path.GetFileName(file);
            ColumnasLeidas = ctproc.ToString();
            _xfil = 1;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo leer el fichero de entrada: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task SeleccionarGanadorasAnterior()
    {
        // Equivale a Coincidencias.EntraCGsA(): carga colgsA[], limcgsA, nrfCGA.
        var file = await ElegirFicheroAbrir(".txt");
        if (file == null) return;
        try
        {
            _limcgsA = 0;
            using (var sr = new StreamReader(file))
            {
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string tmp = linea.Trim().ToUpper();
                    if (tmp.Length < 14) { AppServices.MostrarError("col.G. errónea=" + tmp); return; }
                    _colgsA[_limcgsA] = tmp.Substring(0, 14);
                    _limcgsA++;
                }
            }
            _nrfCGA = _limcgsA;
            FicheroGanadorasAnterior = Path.GetFileName(file);
            IndiceGanadoraAnterior = _nrfCGA.ToString();
            ColumnaGanadoraAnterior = _nrfCGA > 0 ? _colgsA[_nrfCGA - 1] : "··············";
            _xgan1 = 1;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo leer el fichero de ganadoras anteriores: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task SeleccionarGanadorasReciente()
    {
        // Equivale a Coincidencias.EntraCGsR(): carga colgsR[], limcgsR, nrfCGR.
        var file = await ElegirFicheroAbrir(".txt");
        if (file == null) return;
        try
        {
            _limcgsR = 0;
            using (var sr = new StreamReader(file))
            {
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string tmp = linea.Trim().ToUpper();
                    if (tmp.Length < 14) { AppServices.MostrarError("col.G. errónea=" + tmp); return; }
                    _colgsR[_limcgsR] = tmp.Substring(0, 14);
                    _limcgsR++;
                }
            }
            _nrfCGR = _limcgsR;
            FicheroGanadorasReciente = Path.GetFileName(file);
            IndiceGanadoraReciente = _nrfCGR.ToString();
            ColumnaGanadoraReciente = _nrfCGR > 0 ? _colgsR[_nrfCGR - 1] : "··············";
            _xgan2 = 1;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo leer el fichero de ganadoras recientes: " + ex.Message);
        }
    }

    [RelayCommand]
    private void GanadoraAnteriorSiguiente()
    {
        // Equivale a Coincidencias.GAMas() (nrfCGA++ si nrfCGA<limcgsA).
        if (_nrfCGA < _limcgsA)
        {
            _nrfCGA++;
            IndiceGanadoraAnterior = _nrfCGA.ToString();
            ColumnaGanadoraAnterior = _colgsA[_nrfCGA - 1];
        }
    }

    [RelayCommand]
    private void GanadoraAnteriorAnterior()
    {
        // Equivale a Coincidencias.GAMenos() (nrfCGA-- si nrfCGA>1).
        if (_nrfCGA > 1)
        {
            _nrfCGA--;
            IndiceGanadoraAnterior = _nrfCGA.ToString();
            ColumnaGanadoraAnterior = _colgsA[_nrfCGA - 1];
        }
    }

    [RelayCommand]
    private void GanadoraRecienteSiguiente()
    {
        // Equivale a Coincidencias.GRMas() (nrfCGR++ si nrfCGR<limcgsR).
        if (_nrfCGR < _limcgsR)
        {
            _nrfCGR++;
            IndiceGanadoraReciente = _nrfCGR.ToString();
            ColumnaGanadoraReciente = _colgsR[_nrfCGR - 1];
        }
    }

    [RelayCommand]
    private void GanadoraRecienteAnterior()
    {
        // Equivale a Coincidencias.GRMenos() (nrfCGR-- si nrfCGR>1).
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            IndiceGanadoraReciente = _nrfCGR.ToString();
            ColumnaGanadoraReciente = _colgsR[_nrfCGR - 1];
        }
    }

    // ===== Comandos: condiciones (fichero .cnd) =====

    [RelayCommand]
    private async Task LeerCondiciones()
    {
        // Equivale a Coincidencias.LeeCondis(): lee "G;M" por línea y rellena la rejilla.
        var file = await ElegirFicheroAbrir(".cnd");
        if (file == null) return;
        try
        {
            Condiciones.Clear();
            using (var sr = new StreamReader(file))
            {
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string[] aux = linea.Split(';');
                    string grupos = aux.Length > 0 ? aux[0] : "";
                    string rango = aux.Length > 1 ? aux[1] : "";
                    Condiciones.Add(new CoincidenciaCondicionViewModel(grupos, rango));
                }
            }
            FicheroCondiciones = Path.GetFileName(file);
            _xcon = 1;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron leer las condiciones: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task GuardarCondiciones()
    {
        // Equivale a Coincidencias.SalvaCondis(): escribe "G;M" por cada fila de Condiciones.
        var file = await ElegirFicheroGuardar("Condiciones", ".cnd");
        if (file == null) return;
        try
        {
            using (var sw = new StreamWriter(file))
            {
                foreach (var c in Condiciones)
                {
                    sw.WriteLine(c.Grupos + ";" + c.Rango);
                }
            }
            FicheroCondiciones = Path.GetFileName(file);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron guardar las condiciones: " + ex.Message);
        }
    }

    // ===== Comandos: cálculo y análisis =====

    [RelayCommand]
    private void CalcularRadiografia()
    {
        // Equivale a Coincidencias.CalRadioB(): radiografía de la ganadora anterior actual.
        if (_nrfCGA < 1)
        {
            AppServices.MostrarError("Faltan ganadoras anteriores.");
            return;
        }
        string columna = _colgsA[_nrfCGA - 1].Replace('x', 'X');
        Calcondis(columna);
        for (int nr = 0; nr < 108; nr++) _condis[nr] = _rsls[nr];
        VolcarRadiografia();
    }

    [RelayCommand]
    private async Task Calcular()
    {
        // Equivale a Coincidencias.Calcular(): valida cada columna de entrada contra las condiciones.
        if (!LeerCondicionesGrid())
        {
            AppServices.MostrarError("Error en condiciones");
            return;
        }
        _xcon = _limGrN > 0 ? 1 : 0;
        int xpro = _xfil * _xgan1 * _xcon;
        if (xpro == 0)
        {
            AppServices.MostrarError("Faltan entradas (fichero, ganadora anterior y condiciones).");
            return;
        }
        if (_nrfCGA < 1)
        {
            AppServices.MostrarError("Faltan ganadoras anteriores.");
            return;
        }

        _salida = false;
        DateTime dt0 = DateTime.Now;
        DispatcherQueue dispatcher = AppServices.UiDispatcher!;

        if (!InitConds())
        {
            AppServices.MostrarError("Error en condiciones");
            return;
        }

        // Radiografía de referencia (ganadora anterior).
        string columnaRef = _colgsA[_nrfCGA - 1].Replace('x', 'X');
        Calcondis(columnaRef);
        for (int nr = 0; nr < 108; nr++) _condis[nr] = _rsls[nr];
        VolcarRadiografia();

        try
        {
            await Task.Run(() => EjecutarCalcular(dispatcher, dt0));
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al calcular: " + ex.Message);
            return;
        }

        VolcarDistribucion();
        ColumnasProcesadas = _ctproc.ToString();
        ColumnasValidas = _ctadm.ToString();
        Tiempo = FormatearTiempo(DateTime.Now - dt0);
    }

    // Núcleo del bucle de Calcular (legacy), fuera del hilo de UI.
    private void EjecutarCalcular(DispatcherQueue dispatcher, DateTime dt0)
    {
        _validas.SetAll(false);
        for (int nr = 0; nr < 109; nr++) { _agrabarAC[nr] = 0; _agrabarGR[nr] = 0; }
        for (int nr = 0; nr < 100; nr++)
            for (int nr2 = 0; nr2 < 109; nr2++) _agrabarAMC[nr, nr2] = 0;
        Array.Clear(_agrabarCOL, 0, _agrabarCOL.Length);

        _ctadm = 0;
        int procesadas = 0;
        for (int idx = 0; idx < TotalCombinaciones; idx++)
        {
            if (_salida) break;
            if (_entrada[idx])
            {
                string columna = N2S(idx);
                procesadas++;
                Valida(columna, idx);

                if ((procesadas & 0xFFF) == 0)
                {
                    int procActual = procesadas;
                    int admActual = _ctadm;
                    DateTime ahora = DateTime.Now;
                    dispatcher.TryEnqueue(() =>
                    {
                        ColumnasProcesadas = procActual.ToString();
                        ColumnasValidas = admActual.ToString();
                        Tiempo = FormatearTiempo(ahora - dt0);
                    });
                }
            }
        }
        _ctproc = procesadas;
    }

    // legacy: Valida(string columna)
    private void Valida(string columna, int idx)
    {
        int nc = 0;
        Calcondis(columna);
        _coin.SetAll(false);
        for (int nr = 0; nr < 108; nr++) if (_condis[nr] == _rsls[nr]) _coin[nr] = true;
        for (int ng = 0; ng < _limGrN; ng++)
        {
            nc = 0;
            for (int nr = 0; nr < 108; nr++)
            {
                if (_grN[ng, nr] && _coin[nr]) nc++;
            }
            if (!_rks[ng, nc]) return;
        }
        _validas[idx] = true; _ctadm++;
        _agrabarCOL[idx] = nc;
        _agrabarAC[nc]++;
        for (int nr = 0; nr < 108; nr++) if (_coin[nr]) _agrabarGR[nr + 1]++;
        for (int ng = 0; ng < _limGrN; ng++)
        {
            nc = 0;
            for (int nr = 0; nr < 109; nr++)
            {
                if (nr < 108 && _grN[ng, nr] && _coin[nr]) nc++;
            }
            _agrabarAMC[ng, nc]++;
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a Coincidencias.salida = true (interrumpe el bucle de Calcular).
        _salida = true;
    }

    [RelayCommand]
    private void Analizar()
    {
        // Equivale a Coincidencias.Analizar(): coincidencias por grupo entre ganadora anterior y reciente.
        int xana = _xfil * _xgan1 * _xcon * _xgan2;
        if (!LeerCondicionesGrid()) { AppServices.MostrarError("Error en condiciones"); return; }
        if (_limcgsA == 0 || _limcgsR == 0) { AppServices.MostrarError("Faltan ganadoras"); return; }
        if (!InitConds()) { AppServices.MostrarError("Error en condiciones"); return; }

        string columna = _colgsA[_nrfCGA - 1].Replace('x', 'X');
        Calcondis(columna);
        for (int nr = 0; nr < 108; nr++) _condis[nr] = _rsls[nr];
        columna = _colgsR[_nrfCGR - 1].Replace('x', 'X');
        Calcondis(columna);
        _coin.SetAll(false);
        for (int nr = 0; nr < 108; nr++) if (_condis[nr] == _rsls[nr]) _coin[nr] = true;
        for (int nr = 0; nr < _limGrN && nr < Condiciones.Count; nr++)
        {
            int na = 0;
            for (int nr2 = 0; nr2 < 108; nr2++)
            {
                if (_grN[nr, nr2] && _coin[nr2]) na++;
            }
            Condiciones[nr].Resultado = na;
        }
    }

    // ===== Comandos: grabado de resultados =====

    [RelayCommand]
    private async Task GrabarResultado()
    {
        // Equivale a Coincidencias.Grabar(): graba las columnas válidas cuya fila de distribución
        // (por nº de coincidencias agrabarCOL) está marcada como Seleccionada.
        var file = await ElegirFicheroGuardar("Resultados", ".txt");
        if (file == null) return;
        try
        {
            int ctgrab = 0;
            string ruta = file;
            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                for (int nr = 0; nr < TotalCombinaciones; nr++)
                {
                    if (_validas[nr])
                    {
                        int nq = _agrabarCOL[nr];
                        if (nq >= 0 && nq < Distribucion.Count && Distribucion[nq].Seleccionada)
                        {
                            sw.WriteLine(N2S(nr));
                            ctgrab++;
                        }
                    }
                }
            });
            FicheroSalida = Path.GetFileName(file);
            ColumnasGrabadas = ctgrab.ToString();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron grabar los resultados: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task ImprimirRadiografia()
    {
        // Equivale a Coincidencias.PrintRadio(): escribe radiografia.txt con las 108 características.
        var file = await ElegirFicheroGuardar("radiografia", ".txt");
        if (file == null) return;
        try
        {
            using (var sw = new StreamWriter(file))
            {
                sw.WriteLine("Radiografia de la columna = " + IndiceGanadoraAnterior);
                for (int nr = 0; nr < 108; nr++)
                {
                    sw.WriteLine(Descripciones[nr] + " = " + _condis[nr]);
                }
            }
            AppServices.MostrarInfo("Radiografía guardada en " + Path.GetFileName(file));
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo imprimir la radiografía: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task GrabarExcel()
    {
        // Equivale a Coincidencias.GrabExcel(): vuelca agrabarAC/agrabarGR/agrabarAMC tabulado.
        var file = await ElegirFicheroGuardar("Resultados", ".xls");
        if (file == null) return;
        try
        {
            char tab = (char)9;
            using (var sw = new StreamWriter(file))
            {
                string tmp = "N" + tab + "s/AC" + tab + "N" + tab + "s/GR";
                for (int nr = 0; nr < _limGrN; nr++) tmp += ("" + tab + "N" + tab + "AMC" + (nr + 1));
                sw.WriteLine(tmp);
                for (int nr = 0; nr < 109; nr++)
                {
                    tmp = "" + nr + tab + _agrabarAC[nr] + tab + nr + tab + _agrabarGR[nr];
                    for (int ng = 0; ng < _limGrN; ng++)
                        tmp += ("" + tab + nr + tab + _agrabarAMC[ng, nr]);
                    sw.WriteLine(tmp);
                }
            }
            AppServices.MostrarInfo("Distribuciones guardadas en " + Path.GetFileName(file));
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo exportar a Excel: " + ex.Message);
        }
    }

    // ===== Algoritmo autocontenido portado 1:1 de Coincidencias =====

    private const int TotalCombinaciones = 4782969; // 3^14

    // Estado interno (legacy: campos de Coincidencias).
    private readonly BitArray _entrada = new BitArray(TotalCombinaciones);
    private readonly BitArray _validas = new BitArray(TotalCombinaciones);
    private readonly BitArray _coin = new BitArray(108);
    private readonly bool[,] _grN = new bool[100, 109];
    private readonly bool[,] _rks = new bool[100, 109];
    private int _limGrN;
    private readonly int[] _condis = new int[108];
    private readonly int[] _rsls = new int[108];
    private readonly int[] _agrabarAC = new int[109];
    private readonly int[] _agrabarGR = new int[109];
    private readonly int[,] _agrabarAMC = new int[100, 109];
    private readonly int[] _agrabarCOL = new int[TotalCombinaciones];
    private int _ctproc, _ctadm;
    private int _limcgsA, _limcgsR, _nrfCGA, _nrfCGR;
    private readonly string[] _colgsA = new string[3000];
    private readonly string[] _colgsR = new string[3000];
    private int _xfil, _xgan1, _xcon, _xgan2;
    private volatile bool _salida;

    // Vuelca _condis -> Radiografia (legacy: InitDsRadio).
    private void VolcarRadiografia()
    {
        for (int i = 0; i < Radiografia.Count && i < 108; i++)
        {
            Radiografia[i].Valor = _condis[i];
        }
    }

    // Vuelca agrabarAC/agrabarGR/agrabarAMC -> Distribucion (legacy: InitDsOut, índice AMC seleccionado).
    private void VolcarDistribucion()
    {
        int nAMC = 0;
        if (int.TryParse(IndiceAmcSeleccionado, out int amc) && amc >= 1) nAMC = amc - 1;
        for (int nr = 0; nr < Distribucion.Count && nr < 109; nr++)
        {
            Distribucion[nr].PorAceptadas = _agrabarAC[nr];
            Distribucion[nr].PorGrupo = _agrabarGR[nr];
            Distribucion[nr].Amc = _agrabarAMC[nAMC, nr];
        }
    }

    // Lee el grid de condiciones a la cuenta limGrN (legacy: bucle inicial de Calcular/InitConds).
    private bool LeerCondicionesGrid()
    {
        _limGrN = Condiciones.Count;
        return true;
    }

    // legacy: InitConds() — traduce las condiciones (grupos/rango) a las matrices GrN[]/rks[].
    private bool InitConds()
    {
        _limGrN = 0;
        for (int nr = 0; nr < 100; nr++)
            for (int nr2 = 0; nr2 < 109; nr2++) { _grN[nr, nr2] = false; _rks[nr, nr2] = false; }

        _limGrN = Condiciones.Count;
        if (_limGrN == 0) return false;

        try
        {
            for (int nl = 0; nl < _limGrN; nl++)
            {
                string[] aux1 = Condiciones[nl].Grupos.Split(',');
                for (int nr2 = 0; nr2 < aux1.Length; nr2++)
                {
                    string[] aux2 = aux1[nr2].Split('-');
                    if (aux2.Length == 1)
                    {
                        int n1 = Convert.ToInt32(aux2[0]);
                        _grN[nl, n1 - 1] = true;
                    }
                    else if (aux2.Length == 2)
                    {
                        int n1 = Convert.ToInt32(aux2[0]);
                        int n2 = Convert.ToInt32(aux2[1]);
                        for (int nr3 = n1 - 1; nr3 < n2; nr3++) _grN[nl, nr3] = true;
                    }
                    else return false;
                }
                string[] aux1b = Condiciones[nl].Rango.Split(',');
                for (int nl2 = 0; nl2 < aux1b.Length; nl2++)
                {
                    string[] aux2 = aux1b[nl2].Split('-');
                    if (aux2.Length == 1)
                    {
                        int n1 = Convert.ToInt32(aux2[0]);
                        _rks[nl, n1] = true;
                    }
                    else if (aux2.Length == 2)
                    {
                        int n1 = Convert.ToInt32(aux2[0]);
                        int n2 = Convert.ToInt32(aux2[1]);
                        for (int nr3 = n1; nr3 <= n2; nr3++) _rks[nl, nr3] = true;
                    }
                    else return false;
                }
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    // legacy: Calcondis(string col) -> rsls[0..107]
    private void Calcondis(string col)
    {
        col = col.Substring(0, 14);
        for (int nr = 0; nr < 108; nr++) _rsls[nr] = 0;
        CalVX2(col); CalSS(col); CalDist(col); CalContac(col); CalPN(col); CalInterr(col);
    }

    private void CalVX2(string columna)
    {
        int nv7p, nx7p, n27p, nv7u, nx7u, n27u;
        char ch;
        nv7p = nv7u = nx7p = nx7u = n27p = n27u = 0;
        for (int nr = 0; nr < 7; nr++)
        {
            ch = columna[nr];
            if (ch == '1') { }
            else if (ch == '2') { nv7p++; n27p++; }
            else { nv7p++; nx7p++; }
        }
        for (int nr = 7; nr < 14; nr++)
        {
            ch = columna[nr];
            if (ch == '1') { }
            else if (ch == '2') { nv7u++; n27u++; }
            else { nv7u++; nx7u++; }
        }
        _rsls[0] = nv7p + nv7u; _rsls[1] = nx7p + nx7u; _rsls[2] = n27p + n27u;
        _rsls[3] = nv7p; _rsls[4] = nx7p; _rsls[5] = n27p;
        _rsls[6] = nv7u; _rsls[7] = nx7u; _rsls[8] = n27u;
    }

    private void CalSS(string columna)
    {
        for (int pas = 0; pas < 3; pas++)
        {
            string tmp = pas == 0 ? columna : pas == 1 ? columna.Substring(0, 7) : columna.Substring(7);
            int nv = 0, nu = 0, nx = 0, nd = 0, mv = 0, mu = 0, mx = 0, md = 0;
            for (int nr = 0; nr < tmp.Length; nr++)
            {
                char ch = tmp[nr];
                if (ch != '1') { nv++; if (nv > mv) mv = nv; } else nv = 0;
                if (ch == '1') { nu++; if (nu > mu) mu = nu; } else nu = 0;
                if (ch == 'X') { nx++; if (nx > mx) mx = nx; } else nx = 0;
                if (ch == '2') { nd++; if (nd > md) md = nd; } else nd = 0;
            }
            if (pas == 0) { _rsls[9] = mv; _rsls[10] = mu; _rsls[11] = mx; _rsls[12] = md; }
            else if (pas == 1) { _rsls[13] = mv; _rsls[14] = mu; _rsls[15] = mx; _rsls[16] = md; }
            else { _rsls[17] = mv; _rsls[18] = mu; _rsls[19] = mx; _rsls[20] = md; }
        }
    }

    private void CalDist(string columna)
    {
        for (int pas = 0; pas < 3; pas++)
        {
            string tmp = pas == 0 ? columna : pas == 1 ? columna.Substring(0, 7) : columna.Substring(7);
            int dv = 0, du = 0, dx = 0, dd = 0;
            int act, ant;
            ant = tmp.IndexOf('1');
            do { act = tmp.IndexOf('1', ant + 1); if (act >= 0) { du = Math.Max(act - ant, du); ant = act; } } while (act >= 0);
            ant = tmp.IndexOf('X');
            do { act = tmp.IndexOf('X', ant + 1); if (act >= 0) { dx = Math.Max(act - ant, dx); ant = act; } } while (act >= 0);
            ant = tmp.IndexOf('2');
            do { act = tmp.IndexOf('2', ant + 1); if (act >= 0) { dd = Math.Max(act - ant, dd); ant = act; } } while (act >= 0);
            tmp = tmp.Replace('X', '2');
            ant = tmp.IndexOf('2');
            do { act = tmp.IndexOf('2', ant + 1); if (act >= 0) { dv = Math.Max(act - ant, dv); ant = act; } } while (act >= 0);
            if (pas == 0) { _rsls[21] = dv; _rsls[22] = du; _rsls[23] = dx; _rsls[24] = dd; }
            else if (pas == 1) { _rsls[25] = dv; _rsls[26] = du; _rsls[27] = dx; _rsls[28] = dd; }
            else { _rsls[29] = dv; _rsls[30] = du; _rsls[31] = dx; _rsls[32] = dd; }
        }
    }

    private void CalContac(string columna)
    {
        for (int pas = 0; pas < 3; pas++)
        {
            string tmp = pas == 0 ? columna.Substring(0, 14) : pas == 1 ? columna.Substring(0, 7) : columna.Substring(7, 7);
            int n1x = 0, n12 = 0, nx2 = 0, n11 = 0, nxx = 0, n22 = 0, n1v = 0, nxv = 0, n2v = 0, nvv = 0;
            for (int nr = 0; nr < (tmp.Length - 1); nr++)
            {
                switch (tmp.Substring(nr, 2))
                {
                    case "1X":
                    case "X1": n1x++; n1v++; break;
                    case "12":
                    case "21": n12++; n1v++; break;
                    case "X2":
                    case "2X": nx2++; nvv++; nxv++; n2v++; break;
                    case "11": n11++; break;
                    case "XX": nxx++; nvv++; nxv++; break;
                    case "22": n22++; nvv++; n2v++; break;
                }
            }
            if (pas == 0)
            {
                _rsls[33] = n1x; _rsls[34] = n12; _rsls[35] = nx2; _rsls[36] = n11; _rsls[37] = nxx;
                _rsls[38] = n22; _rsls[39] = n1v; _rsls[40] = nxv; _rsls[41] = n2v; _rsls[42] = nvv;
            }
            else if (pas == 1)
            {
                _rsls[43] = n1x; _rsls[44] = n12; _rsls[45] = nx2; _rsls[46] = n11; _rsls[47] = nxx;
                _rsls[48] = n22; _rsls[49] = n1v; _rsls[50] = nxv; _rsls[51] = n2v; _rsls[52] = nvv;
            }
            else
            {
                _rsls[53] = n1x; _rsls[54] = n12; _rsls[55] = nx2; _rsls[56] = n11; _rsls[57] = nxx;
                _rsls[58] = n22; _rsls[59] = n1v; _rsls[60] = nxv; _rsls[61] = n2v; _rsls[62] = nvv;
            }
        }
    }

    private void CalPN(string columna)
    {
        int[] indicesUnos = new[] { 7, 5, 3, 1, 8, 6, 4, 2, 9, 7, 5, 3, 1, 8 };
        int[] indicesEquis = new[] { 5, 1, 6, 2, 7, 3, 8, 4, 9, 5, 1, 6, 2, 7 };
        int[] indicesDoses = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5 };
        for (int pas = 0; pas < 3; pas++)
        {
            string tmp = pas == 0 ? columna.Substring(0, 14) : pas == 1 ? columna.Substring(0, 7) : columna.Substring(7, 7);
            int nSuma1 = 0, nSumaX = 0, nSuma2 = 0;
            for (int i = 0; i < tmp.Length; i++)
            {
                switch (tmp[i])
                {
                    case '1': nSuma1 += indicesUnos[i]; break;
                    case 'X': nSumaX += indicesEquis[i]; break;
                    case '2': nSuma2 += indicesDoses[i]; break;
                }
            }
            int pesoUnos = CalculaPeso(nSuma1);
            int pesoEquis = CalculaPeso(nSumaX);
            int pesoDoses = CalculaPeso(nSuma2);
            int pesoVar = CalculaPeso(pesoEquis + pesoDoses);
            int pesoGlobal = CalculaPeso(pesoVar + pesoUnos);
            if (pas == 0) { _rsls[63] = pesoGlobal; _rsls[64] = pesoVar; _rsls[65] = pesoUnos; _rsls[66] = pesoEquis; _rsls[67] = pesoDoses; }
            else if (pas == 1) { _rsls[68] = pesoGlobal; _rsls[69] = pesoVar; _rsls[70] = pesoUnos; _rsls[71] = pesoEquis; _rsls[72] = pesoDoses; }
            else { _rsls[73] = pesoGlobal; _rsls[74] = pesoVar; _rsls[75] = pesoUnos; _rsls[76] = pesoEquis; _rsls[77] = pesoDoses; }
        }
    }

    private static int CalculaPeso(int valor)
    {
        int peso = valor;
        while (peso > 9)
        {
            peso = (peso / 10) + (peso % 10);
        }
        return peso;
    }

    private void CalInterr(string columna)
    {
        for (int pas = 0; pas < 3; pas++)
        {
            string tmp = pas == 0 ? columna : pas == 1 ? columna.Substring(0, 7) : columna.Substring(7);
            int noIntGlobal = 0, noIntVar = 0, noInt1 = 0, noIntX = 0, noInt2 = 0;
            int noIntGlobalSeg = 0, noIntVarSeg = 0, noInt1Seg = 0, noIntXSeg = 0, noInt2Seg = 0;
            int ngs = 0, nvs = 0, nus = 0, nxs = 0, nds = 0;
            char ant = tmp[0];
            for (int i = 1; i < tmp.Length; i++)
            {
                char act = tmp[i];
                if (act == ant)
                {
                    if (ngs > noIntGlobalSeg) noIntGlobalSeg = ngs;
                    if (nus > noInt1Seg) noInt1Seg = nus;
                    if (nds > noInt2Seg) noInt2Seg = nds;
                    if (nxs > noIntXSeg) noIntXSeg = nxs;
                    if (nvs > noIntVarSeg) noIntVarSeg = nvs;
                    ngs = nus = nds = nxs = nvs = 0;
                }
                else
                {
                    noIntGlobal++; ngs++;
                    if (ant == '1')
                    {
                        if (act == '2') { noInt1++; nus++; if (nxs > noIntXSeg) noIntXSeg = nxs; nxs = 0; }
                        else { noInt1++; nus++; if (nds > noInt2Seg) noInt2Seg = nds; nds = 0; }
                    }
                    else if (ant == '2')
                    {
                        if (act == '1') { noInt2++; noIntVar++; nds++; nvs++; if (nxs > noIntXSeg) noIntXSeg = nxs; nxs = 0; }
                        else { noInt2++; nds++; if (nus > noInt1Seg) noInt1Seg = nus; if (nvs > noIntVarSeg) noIntVarSeg = nvs; nus = nvs = 0; }
                    }
                    else
                    {
                        if (act == '1') { noIntX++; noIntVar++; nxs++; nvs++; if (nds > noInt2Seg) noInt2Seg = nds; nds = 0; }
                        else { noIntX++; nxs++; if (nus > noInt1Seg) noInt1Seg = nus; if (nvs > noIntVarSeg) noIntVarSeg = nvs; nus = nvs = 0; }
                    }
                }
                ant = act;
            }
            if (ngs > noIntGlobalSeg) noIntGlobalSeg = ngs;
            if (nus > noInt1Seg) noInt1Seg = nus;
            if (nds > noInt2Seg) noInt2Seg = nds;
            if (nxs > noIntXSeg) noIntXSeg = nxs;
            if (nvs > noIntVarSeg) noIntVarSeg = nvs;
            if (pas == 0)
            {
                _rsls[78] = noIntGlobal; _rsls[93] = noIntGlobalSeg;
                _rsls[79] = noIntVar; _rsls[94] = noIntVarSeg;
                _rsls[80] = noInt1; _rsls[95] = noInt1Seg;
                _rsls[81] = noIntX; _rsls[96] = noIntXSeg;
                _rsls[82] = noInt2; _rsls[97] = noInt2Seg;
            }
            else if (pas == 1)
            {
                _rsls[83] = noIntGlobal; _rsls[98] = noIntGlobalSeg;
                _rsls[84] = noIntVar; _rsls[99] = noIntVarSeg;
                _rsls[85] = noInt1; _rsls[100] = noInt1Seg;
                _rsls[86] = noIntX; _rsls[101] = noIntXSeg;
                _rsls[87] = noInt2; _rsls[102] = noInt2Seg;
            }
            else
            {
                _rsls[88] = noIntGlobal; _rsls[103] = noIntGlobalSeg;
                _rsls[89] = noIntVar; _rsls[104] = noIntVarSeg;
                _rsls[90] = noInt1; _rsls[105] = noInt1Seg;
                _rsls[91] = noIntX; _rsls[106] = noIntXSeg;
                _rsls[92] = noInt2; _rsls[107] = noInt2Seg;
            }
        }
    }

    // legacy: s2n(string)
    private static int S2N(string ax)
    {
        int nx = 0;
        for (int nr = 0; nr < 14; nr++)
        {
            nx *= 3;
            char ch = ax[nr];
            if (ch == '1') nx += 1;
            else if (ch == '2') nx += 2;
        }
        return nx;
    }

    // legacy: n2s(int)
    private static string N2S(int nx)
    {
        string ax = "";
        for (int nr = 0; nr < 14; nr++)
        {
            int nx2 = nx % 3; nx /= 3;
            if (nx2 == 1) ax = "1" + ax;
            else if (nx2 == 2) ax = "2" + ax;
            else ax = "X" + ax;
        }
        return ax;
    }

    private static string FormatearTiempo(TimeSpan transcurrido)
    {
        string temp = transcurrido + "0000000000";
        return temp.Substring(0, 10);
    }

    // Selector genérico de fichero a abrir (filtro por extensión).
    private static async Task<string?> ElegirFicheroAbrir(string extension)
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(extension);
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        var file = await picker.PickSingleFileAsync();
        return file?.Path;
    }

    // Selector genérico de fichero a guardar.
    private static async Task<string?> ElegirFicheroGuardar(string nombreSugerido, string extension)
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = nombreSugerido,
        };
        picker.FileTypeChoices.Add("Archivo", new List<string> { extension });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        var file = await picker.PickSaveFileAsync();
        return file?.Path;
    }

    // 108 descripciones de características de la radiografía (Coincidencias.InitTexs()).
    private static readonly string[] Descripciones =
    {
        "V14","X14","D14","V7P","X7P","D7P","V7U","X7U","D7U","SSV14",
        "SSU14","SSX14","SSD14","SSV7P","SSU7P","SSX7P","SSD7P","SSV7U","SSU7U","SSX7U",
        "SSD7U","disV14","disU14","disX14","disD14","disV7P","disU7P","disX7P","disD7P","disV7U",
        "disU7U","disX7U","disD7U","con(1x)14","con(12)14","con(x2)14","con(11)14","con(xx)14","con(22)14","con(1v)14",
        "con(xv)14","con(2v)14","con(vv)14","con(1x)7P","con(12)7P","con(x2)7P","con(11)7P","con(xx)7P","con(22)7P","con(1v)7P",
        "con(xv)7P","con(2v)7P","con(vv)7P","con(1x)7U","con(12)7U","con(x2)7U","con(11)7U","con(xx)7U","con(22)7U","con(1v)7U",
        "con(xv)7U","con(2v)7U","con(vv)7U","PNG14","PNV14","PNU14","PNX14","PND14","PNG7P","PNV7P",
        "PNU7P","PNX7P","PND7P","PNG7U","PNV7U","PNU7U","PNX7U","PND7U","IG14","IV14",
        "IU14","IX14","ID14","IG7P","IV7P","IU7P","IX7P","ID7P","IG7U","IV7U",
        "IU7U","IX7U","ID7U","ISG14","ISV14","ISU14","ISX14","ISD14","ISG7P","ISV7P",
        "ISU7P","ISX7P","ISD7P","ISG7U","ISV7U","ISU7U","ISX7U","ISD7U",
    };
}
