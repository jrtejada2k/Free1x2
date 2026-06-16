using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila editable de la rejilla de "Niveles" del WinForms <c>TriosFrm</c>. Cada fila tiene los
/// 3 índices del trío (Pos1/Pos2/Pos3, 1-based: qué tres partidos forman el trío) y los 27
/// niveles por combinación de signos del trío (V0..V26 en el orden 111, 11X, 112, 1X1, 1XX,
/// 1X2, 121, 12X, 122, X11, X1X, X12, XX1, XXX, XX2, X21, X2X, X22, 211, 21X, 212, 2X1, 2XX,
/// 2X2, 221, 22X, 222). Mapea 1:1 a la matriz nivells[fila, 0..29] de RecuperarPantalla()
/// (Free1X2/UI/TriosFrm.cs línea 772 en adelante). Los valores son string para editarlos en
/// TextBox (regla anti-crash: no se bindean ints directos a Text).
/// </summary>
public partial class NivelRowItem : ObservableObject
{
    [ObservableProperty] private string _pos1 = "0";
    [ObservableProperty] private string _pos2 = "0";
    [ObservableProperty] private string _pos3 = "0";
    [ObservableProperty] private string _v0 = "0";
    [ObservableProperty] private string _v1 = "0";
    [ObservableProperty] private string _v2 = "0";
    [ObservableProperty] private string _v3 = "0";
    [ObservableProperty] private string _v4 = "0";
    [ObservableProperty] private string _v5 = "0";
    [ObservableProperty] private string _v6 = "0";
    [ObservableProperty] private string _v7 = "0";
    [ObservableProperty] private string _v8 = "0";
    [ObservableProperty] private string _v9 = "0";
    [ObservableProperty] private string _v10 = "0";
    [ObservableProperty] private string _v11 = "0";
    [ObservableProperty] private string _v12 = "0";
    [ObservableProperty] private string _v13 = "0";
    [ObservableProperty] private string _v14 = "0";
    [ObservableProperty] private string _v15 = "0";
    [ObservableProperty] private string _v16 = "0";
    [ObservableProperty] private string _v17 = "0";
    [ObservableProperty] private string _v18 = "0";
    [ObservableProperty] private string _v19 = "0";
    [ObservableProperty] private string _v20 = "0";
    [ObservableProperty] private string _v21 = "0";
    [ObservableProperty] private string _v22 = "0";
    [ObservableProperty] private string _v23 = "0";
    [ObservableProperty] private string _v24 = "0";
    [ObservableProperty] private string _v25 = "0";
    [ObservableProperty] private string _v26 = "0";

    /// <summary>Número de fila (1-based) para la cabecera. Legacy: grupos p0x/p1x..pbx.</summary>
    public string Etiqueta { get; init; } = string.Empty;

    /// <summary>Devuelve la celda c (0..29) como entero (0 si no parsea). Para volcar a nivells[fila, c].</summary>
    public int Celda(int c) => c switch
    {
        0 => AInt(Pos1), 1 => AInt(Pos2), 2 => AInt(Pos3),
        3 => AInt(V0), 4 => AInt(V1), 5 => AInt(V2), 6 => AInt(V3), 7 => AInt(V4),
        8 => AInt(V5), 9 => AInt(V6), 10 => AInt(V7), 11 => AInt(V8), 12 => AInt(V9),
        13 => AInt(V10), 14 => AInt(V11), 15 => AInt(V12), 16 => AInt(V13), 17 => AInt(V14),
        18 => AInt(V15), 19 => AInt(V16), 20 => AInt(V17), 21 => AInt(V18), 22 => AInt(V19),
        23 => AInt(V20), 24 => AInt(V21), 25 => AInt(V22), 26 => AInt(V23), 27 => AInt(V24),
        28 => AInt(V25), 29 => AInt(V26),
        _ => 0,
    };

    /// <summary>Asigna la celda c (0..29) desde un entero (round-trip de Abrir Condiciones).</summary>
    public void FijarCelda(int c, int valor)
    {
        string s = valor.ToString();
        switch (c)
        {
            case 0: Pos1 = s; break; case 1: Pos2 = s; break; case 2: Pos3 = s; break;
            case 3: V0 = s; break; case 4: V1 = s; break; case 5: V2 = s; break;
            case 6: V3 = s; break; case 7: V4 = s; break; case 8: V5 = s; break;
            case 9: V6 = s; break; case 10: V7 = s; break; case 11: V8 = s; break;
            case 12: V9 = s; break; case 13: V10 = s; break; case 14: V11 = s; break;
            case 15: V12 = s; break; case 16: V13 = s; break; case 17: V14 = s; break;
            case 18: V15 = s; break; case 19: V16 = s; break; case 20: V17 = s; break;
            case 21: V18 = s; break; case 22: V19 = s; break; case 23: V20 = s; break;
            case 24: V21 = s; break; case 25: V22 = s; break; case 26: V23 = s; break;
            case 27: V24 = s; break; case 28: V25 = s; break; case 29: V26 = s; break;
        }
    }

    private static int AInt(string? s) => int.TryParse(s, out int v) ? v : 0;
}

/// <summary>
/// ViewModel del WinForms <c>TriosFrm</c> ("Tríos").
/// El formulario analiza un fichero de columnas de entrada validando cada columna
/// contra una tabla de "Límites" de 5 niveles (mínimo / máximo, campos legacy
/// c11/c12 ... c51/c52) y una gran matriz de "Niveles" (campos v###/p##/va##/vb##).
/// Cuenta columnas procesadas / válidas, muestra el tiempo y un resultado por nivel
/// (r1..r5). Permite abrir/salvar condiciones, calcular, grabar el resultado y analizar.
///
/// Herramienta autónoma fichero→fichero (no edita un Grupo del motor). La lógica
/// (Valida/s1n/n1s) se replica 1:1 del formulario legacy. Las matrices de dominio son
/// int[12,30] nivells (3 posiciones de trío + 27 niveles por combinación) e int[5,3]
/// rks (min, max, recuento por nivel).
///
/// La rejilla de "Niveles" (matriz nivells[12,30]) está portada en TriosFrmPage.xaml como un
/// ItemsControl editable sobre la colección Niveles (12 filas de NivelRowItem). RecuperarPantalla()
/// vuelca esa colección a la matriz nivells para que Valida() contabilice los tríos, igual que el
/// formulario legacy. Aquí se cablea todo (Límites, Niveles, ficheros, bucle de cálculo, grabar,
/// analizar). Las matrices de dominio son int[12,30] nivells y int[5,3] rks (min, max, recuento).
/// </summary>
public partial class TriosFrmViewModel : ObservableObject
{
    public TriosFrmViewModel()
    {
        // 12 filas de Niveles (legacy: grupos p0x/p1x..p7x + va/vb), editables en la Page.
        Niveles = new ObservableCollection<NivelRowItem>();
        for (int r = 0; r < 12; r++)
        {
            Niveles.Add(new NivelRowItem { Etiqueta = (r + 1).ToString() });
        }
    }

    // Matriz de validación de columnas (legacy: BitArray validas de 4.782.969 = 3^14).
    private readonly BitArray _validasBits = new(4782969);

    // Matriz de niveles de trío (legacy: int[12,30] nivells). Se rellena en RecuperarPantalla()
    // desde la colección Niveles (rejilla portada en TriosFrmPage.xaml).
    private int[,] _nivells = new int[12, 30];

    /// <summary>
    /// Rejilla de "Niveles": 12 filas con los 3 índices del trío + 27 niveles por combinación de
    /// signos. Equivale a los cientos de TextBox v###/p##/va##/vb## del WinForms TriosFrm.
    /// </summary>
    public ObservableCollection<NivelRowItem> Niveles { get; }

    // Límites por nivel (legacy: int[5,3] rks -> {0=min,1=max,2=recuento}).
    private int[,] _rks = new int[5, 3];

    private bool _salida;

    // ===== Tabla de Límites (5 niveles) =====
    // Cada nivel tiene un mínimo y un máximo de aciertos admitidos. En el form legacy
    // son los TextBox c11/c12 (nivel 1) .. c51/c52 (nivel 5); por defecto min=0, max=270.
    // RecuperarPantalla() los vuelca en la matriz rks[nivel, {0=min,1=max}].
    // NumberBox.Value es double -> las propiedades son double.

    [ObservableProperty]
    private double _nivel1Min = 0;
    [ObservableProperty]
    private double _nivel1Max = 270;

    [ObservableProperty]
    private double _nivel2Min = 0;
    [ObservableProperty]
    private double _nivel2Max = 270;

    [ObservableProperty]
    private double _nivel3Min = 0;
    [ObservableProperty]
    private double _nivel3Max = 270;

    [ObservableProperty]
    private double _nivel4Min = 0;
    [ObservableProperty]
    private double _nivel4Max = 270;

    [ObservableProperty]
    private double _nivel5Min = 0;
    [ObservableProperty]
    private double _nivel5Max = 270;

    // ===== Resultado por nivel (etiquetas r1..r5 del form legacy) =====
    // Se rellenan tras Analizar() con rks[i,2]. Strings para bindear a TextBlock.

    [ObservableProperty]
    private string _resultadoNivel1 = "-";
    [ObservableProperty]
    private string _resultadoNivel2 = "-";
    [ObservableProperty]
    private string _resultadoNivel3 = "-";
    [ObservableProperty]
    private string _resultadoNivel4 = "-";
    [ObservableProperty]
    private string _resultadoNivel5 = "-";

    // ===== Estado del proceso (etiquetas lproc / lval / ltime del form legacy) =====
    // string para bindear directamente a TextBlock.Text (regla anti-crash #2).

    // Columnas procesadas (contador ctproc).
    [ObservableProperty]
    private string _columnasProcesadas = "0";

    // Columnas válidas (contador ctval).
    [ObservableProperty]
    private string _columnasValidas = "0";

    // Tiempo empleado (etiqueta ltime).
    [ObservableProperty]
    private string _tiempo = "0";

    // Columna ganadora a analizar (legacy: tCG).
    [ObservableProperty]
    private string _columnaGanadora = "111X11X222X111";

    // ===== Acciones (botones del form legacy) =====

    [RelayCommand]
    private async Task CalcularAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _salida = false;
        int ctproc = 0, ctval = 0;
        ColumnasProcesadas = "0"; ColumnasValidas = "0"; Tiempo = "0";
        var time0 = DateTime.Now;

        RecuperarPantalla();

        string ruta = file.Path;
        try
        {
            await Task.Run(() =>
            {
                _validasBits.SetAll(false);
                using var sr = new StreamReader(ruta);
                string? columna;
                while ((columna = sr.ReadLine()) != null)
                {
                    if (_salida) break;
                    ctproc++;
                    if (Valida(columna))
                    {
                        int idx = S1n(columna);
                        _validasBits[idx] = true;
                        ctval++;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al procesar el fichero de columnas: " + ex.Message);
            return;
        }

        ColumnasProcesadas = ctproc.ToString();
        ColumnasValidas = ctval.ToString();
        Tiempo = (DateTime.Now - time0).ToString();
    }

    [RelayCommand]
    private void Analizar()
    {
        // Legacy: TriosFrm.Analizar() — RecuperarPantalla() + Valida(tCG.Text); r1..r5 = rks[*,2].
        RecuperarPantalla();
        Valida(ColumnaGanadora ?? string.Empty);
        ResultadoNivel1 = _rks[0, 2].ToString();
        ResultadoNivel2 = _rks[1, 2].ToString();
        ResultadoNivel3 = _rks[2, 2].ToString();
        ResultadoNivel4 = _rks[3, 2].ToString();
        ResultadoNivel5 = _rks[4, 2].ToString();
    }

    [RelayCommand]
    private async Task GrabarAsync()
    {
        // Legacy: TriosFrm.GrabarCols() — recorre el BitArray validas y escribe n1s(idx).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Resultados",
        };
        picker.FileTypeChoices.Add("Resultados", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        try
        {
            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                for (int nr = 0; nr < 4782969; nr++)
                {
                    if (_validasBits[nr]) sw.WriteLine(N1s(nr));
                }
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al grabar el resultado: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task AbrirCondicionesAsync()
    {
        // Legacy: TriosFrm.LeeCondis() (líneas 473-602) — 12 líneas de Niveles (30 valores
        // c/u: Pos1,Pos2,Pos3 + 27 niveles) seguidas de 5 líneas de Límites (min,max).
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".tri");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            string[] lineas = await File.ReadAllLinesAsync(file.Path);
            // Estructura legacy: 12 líneas de Niveles (30 valores) + 5 líneas de Límites (min,max).
            // Las 12 primeras se vuelcan a la colección Niveles (rejilla portada); las 5 últimas a Límites.
            for (int r = 0; r < 12 && r < lineas.Length && r < Niveles.Count; r++)
            {
                string[] aux = lineas[r].Split(',');
                for (int c = 0; c < 30 && c < aux.Length; c++)
                {
                    Niveles[r].FijarCelda(c, int.TryParse(aux[c].Trim(), out int v) ? v : 0);
                }
            }

            int totalLimites = 5;
            int inicioLimites = lineas.Length - totalLimites;
            if (inicioLimites < 0) inicioLimites = 0;
            var lims = new (double min, double max)[totalLimites];
            for (int i = 0; i < totalLimites; i++)
            {
                int li = inicioLimites + i;
                if (li < 0 || li >= lineas.Length) continue;
                string[] aux = lineas[li].Split(',');
                if (aux.Length < 2) continue;
                lims[i] = (ADouble(aux[0]), ADouble(aux[1]));
            }
            Nivel1Min = lims[0].min; Nivel1Max = lims[0].max;
            Nivel2Min = lims[1].min; Nivel2Max = lims[1].max;
            Nivel3Min = lims[2].min; Nivel3Max = lims[2].max;
            Nivel4Min = lims[3].min; Nivel4Max = lims[3].max;
            Nivel5Min = lims[4].min; Nivel5Max = lims[4].max;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de condiciones: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task SalvarCondicionesAsync()
    {
        // Legacy: TriosFrm.SalvaCondis() (líneas 603+) — persiste la matriz de Niveles (12 líneas
        // de 30 valores) y los Límites (5 líneas min,max). RecuperarPantalla() llena _nivells desde
        // la rejilla portada, por lo que las 12 líneas reflejan los valores editados por el usuario.
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Trios",
        };
        picker.FileTypeChoices.Add("Condiciones", new List<string> { ".tri" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        RecuperarPantalla();
        try
        {
            using var sw = new StreamWriter(file.Path);
            // 12 líneas de Niveles (30 valores por línea). _nivells a 0 mientras no haya rejilla.
            for (int r = 0; r < 12; r++)
            {
                var celdas = new string[30];
                for (int c = 0; c < 30; c++) celdas[c] = _nivells[r, c].ToString();
                sw.WriteLine(string.Join(',', celdas));
            }
            // 5 líneas de Límites (min,max).
            for (int n = 0; n < 5; n++)
            {
                sw.WriteLine(_rks[n, 0] + "," + _rks[n, 1]);
            }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido guardar el fichero de condiciones: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy: TriosFrm.BCancelarClick -> salida = true (aborta el bucle de Calcular()).
        _salida = true;
    }

    // ===== Lógica de dominio replicada 1:1 del WinForms TriosFrm =====

    /// <summary>
    /// Vuelca los Límites (rks[5,3]) y la rejilla de Niveles (nivells[12,30]) de la pantalla a las
    /// matrices de dominio. Legacy: TriosFrm.RecuperarPantalla() (líneas 761-1132): rks desde
    /// c11/c12..c51/c52 y nivells desde p##/v###/va##/vb##.
    /// </summary>
    private void RecuperarPantalla()
    {
        _rks = new int[5, 3];
        _rks[0, 0] = (int)Nivel1Min; _rks[0, 1] = (int)Nivel1Max;
        _rks[1, 0] = (int)Nivel2Min; _rks[1, 1] = (int)Nivel2Max;
        _rks[2, 0] = (int)Nivel3Min; _rks[2, 1] = (int)Nivel3Max;
        _rks[3, 0] = (int)Nivel4Min; _rks[3, 1] = (int)Nivel4Max;
        _rks[4, 0] = (int)Nivel5Min; _rks[4, 1] = (int)Nivel5Max;

        // nivells desde la rejilla de Niveles (12 filas × 30 celdas: 3 posiciones de trío + 27 niveles).
        _nivells = new int[12, 30];
        for (int r = 0; r < 12 && r < Niveles.Count; r++)
        {
            for (int c = 0; c < 30; c++)
            {
                _nivells[r, c] = Niveles[r].Celda(c);
            }
        }
    }

    /// <summary>Valida una columna contra los tríos/condiciones. Legacy: TriosFrm.Valida (1133-1176).</summary>
    private bool Valida(string columna)
    {
        int nv = 0;
        for (int nr = 0; nr < 5; nr++) _rks[nr, 2] = 0;
        columna = (columna ?? string.Empty).ToUpper();
        for (int nr = 0; nr < 12; nr++)
        {
            int n1 = _nivells[nr, 0] - 1, n2 = _nivells[nr, 1] - 1, n3 = _nivells[nr, 2] - 1;
            if (n1 < 0 || n2 < 0 || n3 < 0) continue;
            if (n1 >= columna.Length || n2 >= columna.Length || n3 >= columna.Length) continue;
            string trio = columna.Substring(n1, 1) + columna.Substring(n2, 1) + columna.Substring(n3, 1);
            switch (trio)
            {
                case "111": nv = _nivells[nr, 3]; break;
                case "11X": nv = _nivells[nr, 4]; break;
                case "112": nv = _nivells[nr, 5]; break;
                case "1X1": nv = _nivells[nr, 6]; break;
                case "1XX": nv = _nivells[nr, 7]; break;
                case "1X2": nv = _nivells[nr, 8]; break;
                case "121": nv = _nivells[nr, 9]; break;
                case "12X": nv = _nivells[nr, 10]; break;
                case "122": nv = _nivells[nr, 11]; break;
                case "X11": nv = _nivells[nr, 12]; break;
                case "X1X": nv = _nivells[nr, 13]; break;
                case "X12": nv = _nivells[nr, 14]; break;
                case "XX1": nv = _nivells[nr, 15]; break;
                case "XXX": nv = _nivells[nr, 16]; break;
                case "XX2": nv = _nivells[nr, 17]; break;
                case "X21": nv = _nivells[nr, 18]; break;
                case "X2X": nv = _nivells[nr, 19]; break;
                case "X22": nv = _nivells[nr, 20]; break;
                case "211": nv = _nivells[nr, 21]; break;
                case "21X": nv = _nivells[nr, 22]; break;
                case "212": nv = _nivells[nr, 23]; break;
                case "2X1": nv = _nivells[nr, 24]; break;
                case "2XX": nv = _nivells[nr, 25]; break;
                case "2X2": nv = _nivells[nr, 26]; break;
                case "221": nv = _nivells[nr, 27]; break;
                case "22X": nv = _nivells[nr, 28]; break;
                case "222": nv = _nivells[nr, 29]; break;
                default: nv = 0; break;
            }
            if (nv > 0) _rks[nv - 1, 2]++;
        }
        for (int nr = 0; nr < 5; nr++)
        {
            if (_rks[nr, 2] < _rks[nr, 0] || _rks[nr, 2] > _rks[nr, 1]) return false;
        }
        return true;
    }

    /// <summary>Índice -> columna de 14 signos. Legacy: TriosFrm.n1s (1177-1186).</summary>
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

    /// <summary>Columna de 14 signos -> índice. Legacy: TriosFrm.s1n (1187-1196).</summary>
    private static int S1n(string ax)
    {
        int nx = 0;
        for (int nr = 0; nr < 14; nr++)
        {
            nx *= 3;
            string ch = ax.Substring(nr, 1);
            if (ch == "1") nx += 1;
            else if (ch == "2") nx += 2;
        }
        return nx;
    }

    private static double ADouble(string? s) => double.TryParse(s, out double v) ? v : 0;
}
