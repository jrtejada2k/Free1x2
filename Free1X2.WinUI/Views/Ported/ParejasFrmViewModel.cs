// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Pares" (legacy: Free1X2.UI.ParejasFrm).
/// Filtra columnas de un fichero de entrada contando, para cada uno de los 14
/// partidos, el resultado de la "pareja" formada por otros dos partidos
/// (combinaciones 11, 1x, 12, x1, xx, x2, 21, 2x, 22). Cada combinación asigna
/// un nivel (1..7); luego, por nivel, se exige un número de aciertos dentro de
/// un rango [min, max] (las "Condiciones"). Las columnas que cumplen se guardan.
///
/// Es una herramienta autónoma fichero→fichero (no edita un Grupo del motor): toda
/// la lógica (Valida/s1n/n1s, lectura/escritura *.par y *.txt) vive en el propio
/// formulario legacy y se replica aquí 1:1. Las matrices de dominio son
/// int[14,11] nivells (P1, P2 + 9 valores de pareja por fila) e int[7,3] rks
/// (min, max, recuento por nivel).
/// </summary>
public partial class ParejasFrmViewModel : ObservableObject
{
    // Matriz de validación de columnas (legacy: BitArray validas de 4.782.969 = 3^14).
    private readonly BitArray _columnasValidasBits = new(4782969);
    private int[,] _nivells = new int[14, 11];
    private int[,] _rks = new int[7, 3];
    private bool _salida;

    public ParejasFrmViewModel()
    {
        // 14 partidos (filas de "Niveles"). Legacy: nivells[0..13, 0..10].
        for (int i = 0; i < 14; i++)
        {
            Niveles.Add(new NivelPareja(i + 1));
        }

        // 7 niveles de "Condiciones" con sus min/max por defecto del legacy.
        // Legacy InitializeComponent: c11=0/c12=10 ... c71=0/c72=10
        // (cMin por defecto 0; cMax por defecto 10; c11/c21 etc. = 0).
        for (int i = 0; i < 7; i++)
        {
            Condiciones.Add(new CondicionNivel(i + 1) { Min = "0", Max = "10" });
        }
    }

    /// <summary>14 filas de niveles de pareja (legacy: matriz nivells).</summary>
    public ObservableCollection<NivelPareja> Niveles { get; } = new();

    /// <summary>7 filas de condiciones por nivel (legacy: matriz rks min/max).</summary>
    public ObservableCollection<CondicionNivel> Condiciones { get; } = new();

    // Columna ganadora de prueba para "Analizar" (legacy: tCG, default "111X11X222X111").
    [ObservableProperty]
    private string _columnaGanadora = "111X11X222X111";

    // Contador de columnas procesadas (legacy: lproc, ctproc). String por regla anti-crash.
    [ObservableProperty]
    private string _procesadas = "0";

    // Contador de columnas válidas (legacy: lval, ctval). String por regla anti-crash.
    [ObservableProperty]
    private string _validas = "0";

    // Tiempo transcurrido (legacy: ltime). String por regla anti-crash.
    [ObservableProperty]
    private string _tiempo = "0";

    // Resultados del análisis de la columna ganadora, recuento por nivel 1..7
    // (legacy: r1..r7 = rks[0..6,2]). String "-" inicial por regla anti-crash.
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

    [ObservableProperty]
    private string _resultadoNivel6 = "-";

    [ObservableProperty]
    private string _resultadoNivel7 = "-";

    // Habilita/deshabilita el botón Calcular durante el cálculo (legacy: bCalcular.Enabled).
    [ObservableProperty]
    private bool _puedeCalcular = true;

    // Muestra el botón "Grabar resultado" tras un cálculo (legacy: bGrabar.Visible).
    [ObservableProperty]
    private bool _puedeGrabar;

    /// <summary>
    /// Carga las condiciones desde un fichero *.par.
    /// Legacy: ParejasFrm.LeeCondis() — OpenFileDialog (*.par), lee 14 líneas de
    /// niveles (P1,P2 + 9 valores) y 7 líneas de condiciones (min,max).
    /// </summary>
    [RelayCommand]
    private async Task LeerAsync()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".par");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            using var sr = new StreamReader(file.Path);
            // 14 líneas de niveles: P1, P2 + 9 valores de pareja.
            for (int r = 0; r < 14 && r < Niveles.Count; r++)
            {
                string? linea = sr.ReadLine();
                if (linea == null) break;
                string[] aux = linea.Split(',');
                if (aux.Length < 11) continue;
                var fila = Niveles[r];
                fila.P1 = aux[0]; fila.P2 = aux[1];
                fila.V11 = aux[2]; fila.V1X = aux[3]; fila.V12 = aux[4];
                fila.VX1 = aux[5]; fila.VXX = aux[6]; fila.VX2 = aux[7];
                fila.V21 = aux[8]; fila.V2X = aux[9]; fila.V22 = aux[10];
            }
            // 7 líneas de condiciones: min, max.
            for (int n = 0; n < 7 && n < Condiciones.Count; n++)
            {
                string? linea = sr.ReadLine();
                if (linea == null) break;
                string[] aux = linea.Split(',');
                if (aux.Length < 2) continue;
                Condiciones[n].Min = aux[0];
                Condiciones[n].Max = aux[1];
            }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de condiciones: " + ex.Message);
        }
    }

    /// <summary>
    /// Guarda las condiciones actuales a un fichero *.par.
    /// Legacy: ParejasFrm.SalvaCondis() — SaveFileDialog (*.par).
    /// </summary>
    [RelayCommand]
    private async Task SalvarCondicionesAsync()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Pares",
        };
        picker.FileTypeChoices.Add("Condiciones", new List<string> { ".par" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            using var sw = new StreamWriter(file.Path);
            // 14 líneas de niveles (legacy: P1,P2 + 9 valores por línea).
            foreach (var fila in Niveles)
            {
                sw.WriteLine(string.Join(',', new[]
                {
                    fila.P1, fila.P2, fila.V11, fila.V1X, fila.V12,
                    fila.VX1, fila.VXX, fila.VX2, fila.V21, fila.V2X, fila.V22,
                }));
            }
            // 7 líneas de condiciones (min,max).
            foreach (var cond in Condiciones)
            {
                sw.WriteLine(cond.Min + "," + cond.Max);
            }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido guardar el fichero de condiciones: " + ex.Message);
        }
    }

    /// <summary>
    /// Lee un fichero de columnas de entrada (*.txt) y filtra las que cumplen las
    /// condiciones de parejas. Legacy: ParejasFrm.Calcular() + RecuperarPantalla() + Valida().
    /// </summary>
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

        PuedeCalcular = false;
        PuedeGrabar = false;
        _salida = false;
        int ctproc = 0, ctval = 0;
        Procesadas = "0"; Validas = "0"; Tiempo = "0";
        var time0 = DateTime.Now;

        // Legacy: RecuperarPantalla() vuelca la pantalla a nivells[14,11] y rks[7,3].
        RecuperarPantalla();

        string ruta = file.Path;
        try
        {
            await Task.Run(() =>
            {
                _columnasValidasBits.SetAll(false);
                using var sr = new StreamReader(ruta);
                string? columna;
                while ((columna = sr.ReadLine()) != null)
                {
                    if (_salida) break;
                    ctproc++;
                    if (Valida(columna))
                    {
                        int idx = S1n(columna);
                        _columnasValidasBits[idx] = true;
                        ctval++;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al procesar el fichero de columnas: " + ex.Message);
            PuedeCalcular = true;
            return;
        }

        Procesadas = ctproc.ToString();
        Validas = ctval.ToString();
        Tiempo = (DateTime.Now - time0).ToString();
        PuedeCalcular = true;
        PuedeGrabar = true;
    }

    /// <summary>
    /// Guarda a un *.txt las columnas que pasaron el filtro.
    /// Legacy: ParejasFrm.GrabarCols() — recorre el BitArray validas (4.782.969) y
    /// escribe n1s(idx) por cada bit activo.
    /// </summary>
    [RelayCommand]
    private async Task GrabarResultadoAsync()
    {
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
                    if (_columnasValidasBits[nr]) sw.WriteLine(N1s(nr));
                }
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al grabar el resultado: " + ex.Message);
        }
    }

    /// <summary>
    /// Analiza una única "Columna Ganadora" mostrando el recuento por nivel 1..7.
    /// Legacy: ParejasFrm.Analizar() — RecuperarPantalla() + Valida(tCG.Text) y r1..r7 = rks[*,2].
    /// </summary>
    [RelayCommand]
    private void Analizar()
    {
        RecuperarPantalla();
        Valida(ColumnaGanadora ?? string.Empty);
        ResultadoNivel1 = _rks[0, 2].ToString();
        ResultadoNivel2 = _rks[1, 2].ToString();
        ResultadoNivel3 = _rks[2, 2].ToString();
        ResultadoNivel4 = _rks[3, 2].ToString();
        ResultadoNivel5 = _rks[4, 2].ToString();
        ResultadoNivel6 = _rks[5, 2].ToString();
        ResultadoNivel7 = _rks[6, 2].ToString();
    }

    /// <summary>
    /// Cierra/regresa sin guardar. Legacy: bCancelar -> Close(). Aquí aborta el bucle de cálculo.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // Aborta el bucle de Calcular() en curso (legacy: salida = true).
        _salida = true;
    }

    // ===== Lógica de dominio replicada 1:1 del WinForms ParejasFrm =====

    /// <summary>
    /// Vuelca las propiedades de la pantalla a las matrices nivells[14,11] y rks[7,3].
    /// Legacy: ParejasFrm.RecuperarPantalla() (líneas 463-632). Tolerante a celdas vacías.
    /// </summary>
    private void RecuperarPantalla()
    {
        _nivells = new int[14, 11];
        _rks = new int[7, 3];
        for (int r = 0; r < 14 && r < Niveles.Count; r++)
        {
            var fila = Niveles[r];
            _nivells[r, 0] = AInt(fila.P1);
            _nivells[r, 1] = AInt(fila.P2);
            _nivells[r, 2] = AInt(fila.V11);
            _nivells[r, 3] = AInt(fila.V1X);
            _nivells[r, 4] = AInt(fila.V12);
            _nivells[r, 5] = AInt(fila.VX1);
            _nivells[r, 6] = AInt(fila.VXX);
            _nivells[r, 7] = AInt(fila.VX2);
            _nivells[r, 8] = AInt(fila.V21);
            _nivells[r, 9] = AInt(fila.V2X);
            _nivells[r, 10] = AInt(fila.V22);
        }
        for (int n = 0; n < 7 && n < Condiciones.Count; n++)
        {
            _rks[n, 0] = AInt(Condiciones[n].Min);
            _rks[n, 1] = AInt(Condiciones[n].Max);
        }
    }

    /// <summary>Valida una columna contra las parejas/condiciones. Legacy: ParejasFrm.Valida (633-658).</summary>
    private bool Valida(string columna)
    {
        int nv = 0;
        for (int nr = 0; nr < 7; nr++) _rks[nr, 2] = 0;
        columna = (columna ?? string.Empty).ToUpper();
        for (int nr = 0; nr < 14; nr++)
        {
            int n1 = _nivells[nr, 0] - 1, n2 = _nivells[nr, 1] - 1;
            if (n1 < 0 || n2 < 0) continue;
            if (n1 >= columna.Length || n2 >= columna.Length) continue;
            string par = columna.Substring(n1, 1) + columna.Substring(n2, 1);
            switch (par)
            {
                case "11": nv = _nivells[nr, 2]; break;
                case "1X": nv = _nivells[nr, 3]; break;
                case "12": nv = _nivells[nr, 4]; break;
                case "X1": nv = _nivells[nr, 5]; break;
                case "XX": nv = _nivells[nr, 6]; break;
                case "X2": nv = _nivells[nr, 7]; break;
                case "21": nv = _nivells[nr, 8]; break;
                case "2X": nv = _nivells[nr, 9]; break;
                case "22": nv = _nivells[nr, 10]; break;
                default: nv = 0; break;
            }
            if (nv > 0) _rks[nv - 1, 2]++;
        }
        for (int nr = 0; nr < 7; nr++)
        {
            if (_rks[nr, 2] < _rks[nr, 0] || _rks[nr, 2] > _rks[nr, 1]) return false;
        }
        return true;
    }

    /// <summary>Índice (0..3^14-1) -> columna de 14 signos. Legacy: ParejasFrm.n1s (659-668).</summary>
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

    /// <summary>Columna de 14 signos -> índice. Legacy: ParejasFrm.s1n (669-678).</summary>
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

    // Convierte texto a entero de forma tolerante (legacy: Convert.ToInt32 sobre TextBox).
    private static int AInt(string? s) => int.TryParse(s, out int v) ? v : 0;
}

/// <summary>
/// Una fila de "Niveles": un partido (1..14) con sus posiciones de pareja (P1, P2)
/// y el nivel asignado a cada una de las 9 combinaciones de pareja.
/// Legacy: fila de nivells[r, 0..10]. Todas las propiedades son string por la
/// regla anti-crash (NumberBox.Value es double; aquí se usan TextBox numéricos).
/// </summary>
public partial class NivelPareja : ObservableObject
{
    public NivelPareja(int numero)
    {
        Numero = numero;
        NumeroTexto = numero.ToString();
    }

    /// <summary>Índice 1..14 del partido (solo lectura, para la cabecera de fila).</summary>
    public int Numero { get; }

    /// <summary>Etiqueta de fila ya formateada (legacy int->Text, expuesta como string).</summary>
    public string NumeroTexto { get; }

    // P1 / P2: posiciones (1..14) de los dos partidos que forman la pareja. Legacy: nivells[r,0], [r,1].
    [ObservableProperty]
    private string _p1 = "0";

    [ObservableProperty]
    private string _p2 = "0";

    // Nivel (1..7) asignado a cada combinación de pareja. Legacy: nivells[r,2..10].
    [ObservableProperty]
    private string _v11 = "0";

    [ObservableProperty]
    private string _v1X = "0";

    [ObservableProperty]
    private string _v12 = "0";

    [ObservableProperty]
    private string _vX1 = "0";

    [ObservableProperty]
    private string _vXX = "0";

    [ObservableProperty]
    private string _vX2 = "0";

    [ObservableProperty]
    private string _v21 = "0";

    [ObservableProperty]
    private string _v2X = "0";

    [ObservableProperty]
    private string _v22 = "0";
}

/// <summary>
/// Una fila de "Condiciones": para un nivel (1..7) define el rango [Min, Max] de
/// aciertos permitidos. Legacy: rks[n, 0] = min, rks[n, 1] = max.
/// </summary>
public partial class CondicionNivel : ObservableObject
{
    public CondicionNivel(int nivel)
    {
        Nivel = nivel;
        NivelTexto = nivel.ToString();
    }

    /// <summary>Índice 1..7 del nivel (solo lectura).</summary>
    public int Nivel { get; }

    /// <summary>Etiqueta de nivel formateada (legacy int->Text, expuesta como string).</summary>
    public string NivelTexto { get; }

    // Mínimo de aciertos para este nivel. Legacy: rks[n,0] (cX1).
    [ObservableProperty]
    private string _min = "0";

    // Máximo de aciertos para este nivel. Legacy: rks[n,1] (cX2).
    [ObservableProperty]
    private string _max = "10";
}
