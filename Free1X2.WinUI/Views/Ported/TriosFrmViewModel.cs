using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

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
/// IMPORTANTE: la rejilla de "Niveles" (matriz nivells) NO está portada en
/// TriosFrmPage.xaml — está marcada como TODO de dominio en esa Page. Sin esos datos
/// la matriz nivells queda a 0 y Valida() no contabiliza tríos. Aquí se cablea todo lo
/// que sí está expuesto (Límites, ficheros, bucle de cálculo, grabar, analizar). Cuando
/// se porte la rejilla de Niveles, rellenar la matriz en RecuperarPantalla().
/// </summary>
public partial class TriosFrmViewModel : ObservableObject
{
    // Matriz de validación de columnas (legacy: BitArray validas de 4.782.969 = 3^14).
    private readonly BitArray _validasBits = new(4782969);

    // Matriz de niveles de trío (legacy: int[12,30] nivells). De momento a 0: la rejilla de
    // "Niveles" no está portada en la Page (ver TODO de TriosFrmPage.xaml, Free1X2/UI/TriosFrm.cs línea 761).
    private int[,] _nivells = new int[12, 30];

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
        // Legacy: TriosFrm.LeeCondis() — carga desde fichero la tabla de Límites y la matriz
        // de Niveles. Aquí sólo se pueden cargar los Límites (5 niveles min/max): la primera
        // sección del fichero son las 12 líneas de la matriz de Niveles, que no tiene UI portada.
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
            // Estructura legacy: 12 líneas de Niveles + 5 líneas de Límites (min,max).
            // Sólo se vuelcan los Límites; la matriz de Niveles requiere la rejilla aún no portada.
            // TODO[dominio]: volcar las 12 primeras líneas a _nivells cuando se porte la rejilla de
            //   "Niveles" (Free1X2/UI/TriosFrm.cs LeeCondis, línea 473).
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
        // Legacy: TriosFrm.SalvaCondis() — persiste la matriz de Niveles (12 líneas) y los
        // Límites (5 líneas min,max). Aquí se guardan los Límites; las 12 líneas de Niveles se
        // emiten desde _nivells (a 0 mientras la rejilla no esté portada) para mantener el formato.
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
    /// Vuelca los Límites de la pantalla a rks[5,3]. La matriz nivells[12,30] queda a 0
    /// mientras la rejilla de "Niveles" no esté portada (TODO en TriosFrmPage.xaml).
    /// Legacy: TriosFrm.RecuperarPantalla() (línea 761).
    /// </summary>
    private void RecuperarPantalla()
    {
        _rks = new int[5, 3];
        _rks[0, 0] = (int)Nivel1Min; _rks[0, 1] = (int)Nivel1Max;
        _rks[1, 0] = (int)Nivel2Min; _rks[1, 1] = (int)Nivel2Max;
        _rks[2, 0] = (int)Nivel3Min; _rks[2, 1] = (int)Nivel3Max;
        _rks[3, 0] = (int)Nivel4Min; _rks[3, 1] = (int)Nivel4Max;
        _rks[4, 0] = (int)Nivel5Min; _rks[4, 1] = (int)Nivel5Max;
        // _nivells: la rejilla de Niveles no está portada en la Page. Permanece a 0.
        // TODO[dominio]: volcar la rejilla de "Niveles" a _nivells[12,30] cuando se porte
        //   (Free1X2/UI/TriosFrm.cs RecuperarPantalla, líneas 772-1131).
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
