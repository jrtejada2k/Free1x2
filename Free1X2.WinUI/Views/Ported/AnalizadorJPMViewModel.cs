using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Representa una "casilla de suma" (bucket 0..35) del analizador.
/// Cada casilla puede marcarse para seleccionar las columnas cuya puntuación
/// coincide con ese valor, y muestra cuántas columnas cayeron en él.
/// Las propiedades expuestas a la UI son string (regla anti-crash: no bindear
/// int/bool directo a TextBlock.Text).
/// </summary>
public partial class CasillaSumaViewModel : ObservableObject
{
    public int Indice { get; }

    /// <summary>Etiqueta del bucket (0..35) como texto para el TextBlock.</summary>
    public string IndiceTexto { get; }

    [ObservableProperty]
    private bool _seleccionada;

    [ObservableProperty]
    private string _conteoTexto = "0";

    public CasillaSumaViewModel(int indice)
    {
        Indice = indice;
        IndiceTexto = indice.ToString();
    }
}

/// <summary>
/// ViewModel para la pantalla "Sumas Pares Naturales (JPM)" (legacy: AnalizadorJPM / AnalizadorJPMFrm).
///
/// Propósito legacy: asignar una puntuación a cada par de signos de una columna
/// (11, 1X, 12, X1, XX, X2, 21, 2X, 22) y clasificar todas las columnas de un
/// fichero en 36 "casillas de suma" (0..35). El usuario marca casillas, suma sus
/// conteos, graba las columnas seleccionadas y analiza premios (10..14) de una
/// columna ganadora navegable.
///
/// Cableado al motor real: el algoritmo (toda la lógica reside en el form legacy, sin clase de
/// motor) se replica íntegramente aquí — Puntuar/s1n/n1s/BuscaPremios — y la E/S usa
/// System.IO + FileOpenPicker/FileSavePicker. Las operaciones intensivas corren en Task.Run.
/// </summary>
public partial class AnalizadorJPMViewModel : ObservableObject
{
    // Constantes del legacy: 3^14 combinaciones posibles.
    private const int TotalCombinaciones = 4782969;
    private static readonly int[] Pot = { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };

    // Estado de cálculo (equivalentes a los campos del form legacy).
    private int[]? _validas;           // validas[]: bucket por cada combinación (o -1)
    private readonly int[] _lbtab = new int[36];
    private readonly bool[] _marcas = new bool[36];
    private readonly int[] _vals = new int[9];
    private int _conta;                // nº de columnas únicas leídas
    private volatile bool _salida;     // señal de cancelación

    // Análisis de premios.
    private readonly int[] _nprs = new int[5];

    // Columnas ganadoras de referencia (legacy: colgsR[], limcgsR, nrfCGR).
    private readonly List<string> _colgsR = new();
    private int _nrfCGR;

    // --- Valores a usar (max. 5): puntuación por tipo de par de signos ---
    // Legacy: tbv0..tbv8 -> vals[0..8] (11,1X,12,X1,XX,X2,21,2X,22), recortados a [0,5].
    [ObservableProperty]
    private double _valor11;

    [ObservableProperty]
    private double _valor1X;

    [ObservableProperty]
    private double _valor12;

    [ObservableProperty]
    private double _valorX1;

    [ObservableProperty]
    private double _valorXX;

    [ObservableProperty]
    private double _valorX2;

    [ObservableProperty]
    private double _valor21;

    [ObservableProperty]
    private double _valor2X;

    [ObservableProperty]
    private double _valor22;

    // --- Casillas de suma (0..35) ---
    public ObservableCollection<CasillaSumaViewModel> Casillas { get; } = new();

    // --- Estado de proceso / ficheros ---
    [ObservableProperty]
    private string _ficheroEntrada = "-";       // legacy: lFileIn

    [ObservableProperty]
    private string _ficheroSalida = "-";         // legacy: lFileOut

    [ObservableProperty]
    private string _totalColumnasTexto = "0";    // legacy: lCol

    [ObservableProperty]
    private string _tiempoTexto = "-";           // legacy: lTime

    [ObservableProperty]
    private string _sumaSeleccionTexto = "0";    // legacy: lSumSel

    // --- Columna ganadora navegable (groupBox4 "analisis resultados") ---
    [ObservableProperty]
    private string _columnaGanadora = "COL.GANADORA"; // legacy: tbCG

    [ObservableProperty]
    private string _ficheroGanadorasTexto = "-";      // legacy: lFGR

    [ObservableProperty]
    private string _indiceGanadoraTexto = "0";        // legacy: lbCGR (nrfCGR)

    [ObservableProperty]
    private string _puntuacionGanadoraTexto = "-";    // legacy: lbCG (nx2)

    // --- Resultados del Análisis (premios 10..14) ---
    [ObservableProperty]
    private string _premios14Texto = "0";    // legacy: lpr14

    [ObservableProperty]
    private string _premios13Texto = "0";    // legacy: lpr13

    [ObservableProperty]
    private string _premios12Texto = "0";    // legacy: lpr12

    [ObservableProperty]
    private string _premios11Texto = "0";    // legacy: lpr11

    [ObservableProperty]
    private string _premios10Texto = "0";    // legacy: lpr10

    public AnalizadorJPMViewModel()
    {
        // Valores por defecto del WinForms original (tbv0..tbv8).
        Valor11 = 0; Valor1X = 1; Valor12 = 2;
        ValorX1 = 1; ValorXX = 3; ValorX2 = 4;
        Valor21 = 2; Valor2X = 4; Valor22 = 5;

        for (int i = 0; i < 36; i++)
            Casillas.Add(new CasillaSumaViewModel(i));
    }

    /// <summary>
    /// Legacy: Iniciar() -> abre OpenFileDialog, lee columnas, las puntúa y clasifica
    /// en lbtab[0..35], actualiza conteos, total y tiempo.
    /// </summary>
    [RelayCommand]
    private async Task IniciarAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        FicheroEntrada = file.Name;
        _salida = false;
        RecuperaValores(); // vals[] + marcas[] (lee Valor* y Casillas).
        string ruta = file.Path;

        var time0 = DateTime.Now;
        try
        {
            // Lectura + puntuación + clasificación (fuera del hilo de UI).
            string? errorLongitud = await Task.Run(() =>
            {
                _validas = new int[TotalCombinaciones];
                for (int nr = 0; nr < TotalCombinaciones; nr++) _validas[nr] = -1;
                for (int nr = 0; nr < 36; nr++) _lbtab[nr] = 0;
                var repes = new BitArray(TotalCombinaciones);
                _conta = 0;

                using var sr = new StreamReader(ruta);
                while (sr.Peek() > 0)
                {
                    if (_salida) break;
                    string columna = (sr.ReadLine() ?? "").Trim();
                    if (columna.Length < 14)
                    {
                        return columna; // error de longitud
                    }
                    int nx = S1n(columna);
                    if (!repes[nx])
                    {
                        int nx2 = Puntuar(columna);
                        _validas[nx] = nx2;
                        _lbtab[nx2]++;
                        _conta++;
                        repes[nx] = true;
                    }
                }
                return null;
            });

            if (errorLongitud != null)
            {
                AppServices.MostrarError("error de longitud=" + errorLongitud);
            }

            // Asignar(): vuelca lbtab[i] a las casillas.
            for (int i = 0; i < 36; i++)
                Casillas[i].ConteoTexto = _lbtab[i].ToString();

            var time9 = DateTime.Now;
            string tmp = (time9 - time0).ToString() + "00000000000";
            TiempoTexto = tmp.Substring(0, 11);
            TotalColumnasTexto = _conta.ToString();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al procesar el fichero: " + ex.Message);
        }
    }

    /// <summary>
    /// Legacy: Grabar() -> SaveFileDialog, recorre validas[] y escribe las columnas
    /// cuyo bucket está marcado (ck00..ck35).
    /// </summary>
    [RelayCommand]
    private async Task GrabarAsync()
    {
        if (_validas == null)
        {
            AppServices.MostrarError("Inicia primero el proceso de columnas.");
            return;
        }

        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ColumnasSalida",
        };
        picker.FileTypeChoices.Add("ColumnasSalida", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        // Captura de las casillas marcadas (legacy: ck00..ck35 -> marcas implícitas en el switch).
        var marcadas = new bool[36];
        for (int i = 0; i < 36; i++) marcadas[i] = Casillas[i].Seleccionada;

        string ruta = file.Path;
        int[] validas = _validas;

        try
        {
            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                for (int nr = 0; nr < TotalCombinaciones; nr++)
                {
                    int nx = validas[nr];
                    if (nx >= 0 && nx < 36 && marcadas[nx])
                    {
                        sw.WriteLine(N1s(nr)); // Grab1Col -> n1s(nr)
                    }
                }
            });
            FicheroSalida = file.Name;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al grabar: " + ex.Message);
        }
    }

    /// <summary>
    /// Legacy: SumSel() -> suma lbtab[i] de las casillas marcadas en lSumSel.
    /// </summary>
    [RelayCommand]
    private void Sumar()
    {
        int suma = 0;
        for (int nr = 0; nr < 36; nr++)
        {
            if (Casillas[nr].Seleccionada) suma += _lbtab[nr];
        }
        SumaSeleccionTexto = suma.ToString();
    }

    /// <summary>
    /// Legacy: BCancelarClick -> salida = true (aborta el bucle de proceso).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        _salida = true;
    }

    /// <summary>
    /// Legacy: EntraCGsR() / BFGClick -> abre fichero de columnas ganadoras y carga la lista navegable.
    /// </summary>
    [RelayCommand]
    private async Task CargarGanadorasAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        try
        {
            var (cols, errorEn) = await Task.Run(() =>
            {
                var lista = new List<string>();
                int idxError = -1;
                using var sr = new StreamReader(ruta);
                while (sr.Peek() > 0)
                {
                    string tmp = VerColumna(sr.ReadLine() ?? "");
                    if (tmp.Length == 0) { idxError = lista.Count; break; }
                    lista.Add(tmp);
                }
                return (lista, idxError);
            });

            if (errorEn >= 0)
            {
                AppServices.MostrarError("col.G. errónea");
                return;
            }

            _colgsR.Clear();
            _colgsR.AddRange(cols);
            _nrfCGR = _colgsR.Count;
            FicheroGanadorasTexto = file.Name;
            IndiceGanadoraTexto = _nrfCGR.ToString();
            if (_nrfCGR > 0) ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al leer ganadoras: " + ex.Message);
        }
    }

    /// <summary>
    /// Legacy: GRMas() / BMasRClick -> avanza a la siguiente columna ganadora.
    /// </summary>
    [RelayCommand]
    private void SiguienteGanadora()
    {
        if (_nrfCGR < _colgsR.Count)
        {
            _nrfCGR++;
            IndiceGanadoraTexto = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    /// <summary>
    /// Legacy: GRMenos() / BMenosRClick -> retrocede a la columna ganadora anterior.
    /// </summary>
    [RelayCommand]
    private void AnteriorGanadora()
    {
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            IndiceGanadoraTexto = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    /// <summary>
    /// Legacy: Analizar() -> puntúa la columna ganadora y cuenta premios 14/13/12/11/10
    /// recorriendo variaciones (BuscaPremios recursivo).
    /// </summary>
    [RelayCommand]
    private async Task AnalizarAsync()
    {
        _salida = false;
        RecuperaValores();
        string columna = ColumnaGanadora.Trim().ToUpper();
        if (columna.Length < 14)
        {
            AppServices.MostrarError("error de longitud=" + columna);
            return;
        }

        try
        {
            // colprs[] es enorme (3^14); se reserva aquí para no mantenerlo entre análisis.
            int puntuacion = await Task.Run(() =>
            {
                var colprs = new int[TotalCombinaciones];
                for (int nr = 0; nr < 5; nr++) _nprs[nr] = 0;

                int ncol = S1n(columna);
                int nx2 = Puntuar(columna);
                if (_marcas[nx2]) { colprs[ncol] = 14; _nprs[0]++; }
                BuscaPremios(colprs, ncol, 13);
                return nx2;
            });

            PuntuacionGanadoraTexto = puntuacion.ToString();
            Premios14Texto = _nprs[0].ToString();
            Premios13Texto = _nprs[1].ToString();
            Premios12Texto = (_nprs[2] - _nprs[1]).ToString();
            Premios11Texto = (_nprs[3] - _nprs[2]).ToString();
            Premios10Texto = (_nprs[4] - _nprs[3]).ToString();
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al analizar: " + ex.Message);
        }
    }

    // ---- Algoritmos puros del form legacy (sin dependencia de UI) ----

    /// <summary>Puntúa una columna sumando vals[] por cada par de signos (legacy Puntuar).</summary>
    private int Puntuar(string col)
    {
        int rsl = 0;
        for (int nr = 0; nr < 14; nr += 2)
        {
            string tmp = col.Substring(nr, 2);
            switch (tmp)
            {
                case "11": rsl += _vals[0]; break;
                case "1X": rsl += _vals[1]; break;
                case "12": rsl += _vals[2]; break;
                case "X1": rsl += _vals[3]; break;
                case "XX": rsl += _vals[4]; break;
                case "X2": rsl += _vals[5]; break;
                case "21": rsl += _vals[6]; break;
                case "2X": rsl += _vals[7]; break;
                case "22": rsl += _vals[8]; break;
            }
        }
        return rsl;
    }

    /// <summary>Convierte un índice base-3 a su columna 1/X/2 (legacy n1s).</summary>
    private static string N1s(int nx)
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

    /// <summary>Convierte una columna 1/X/2 a su índice base-3 (legacy s1n).</summary>
    private static int S1n(string ax)
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

    /// <summary>Recorre variaciones de la columna contando premios por profundidad (legacy BuscaPremios).</summary>
    private void BuscaPremios(int[] colprs, int col0, int nprof)
    {
        if (_salida) return;
        for (int nr0 = 0; nr0 < 14; nr0++)
        {
            int x1 = Pot[nr0];
            int sign1 = (col0 / x1) % 3;
            for (int z1 = 0; z1 < 3; z1++)
            {
                int col1 = col0 + x1 * (z1 - sign1);
                if (colprs[col1] < nprof)
                {
                    colprs[col1] = nprof;
                    int pos = Puntuar(N1s(col1));         // estas dos líneas
                    if (_marcas[pos]) _nprs[14 - nprof]++; // dependen de la condición
                }
                if (nprof > 10) BuscaPremios(colprs, col1, nprof - 1);
            }
        }
    }

    /// <summary>Lee Valor* (clamp [0,5]) a vals[] y las Casillas marcadas a marcas[] (legacy RecuperaValores).</summary>
    private void RecuperaValores()
    {
        _vals[0] = Convert.ToInt32(Valor11);
        _vals[1] = Convert.ToInt32(Valor1X);
        _vals[2] = Convert.ToInt32(Valor12);
        _vals[3] = Convert.ToInt32(ValorX1);
        _vals[4] = Convert.ToInt32(ValorXX);
        _vals[5] = Convert.ToInt32(ValorX2);
        _vals[6] = Convert.ToInt32(Valor21);
        _vals[7] = Convert.ToInt32(Valor2X);
        _vals[8] = Convert.ToInt32(Valor22);
        for (int nr = 0; nr < 9; nr++)
        {
            if (_vals[nr] > 5) _vals[nr] = 5;
            if (_vals[nr] < 0) _vals[nr] = 0;
        }
        for (int nr = 0; nr < 36; nr++) _marcas[nr] = Casillas[nr].Seleccionada;
    }

    /// <summary>Valida y normaliza una columna (14 signos {1,2,x,X}); "" si es errónea (legacy VerColumna).</summary>
    private static string VerColumna(string columna)
    {
        string chval = "12xX";
        string xcol = columna.Trim();
        if (xcol.Length < 14) return "";
        xcol = xcol.Substring(0, 14);
        for (int nr = 0; nr < 14; nr++)
        {
            char ch = xcol[nr];
            if (chval.IndexOf(ch) < 0) return "";
        }
        return xcol;
    }
}
