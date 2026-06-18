// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del port WinUI 3 del WinForms <c>SelectorMS</c> ("Selector MarioSan").
///
/// Lee un fichero de columnas (.txt) y calcula, para cada columna, cuántas columnas del
/// propio fichero están a "aspecto" 13, 12 u 11 (distancia de signos), agrupándolas en una
/// tabla de distribución. Permite analizar la distribución contra una columna ganadora
/// (premios P14..P10), sumar las columnas del grupo seleccionado y grabarlas a fichero.
/// Lógica portada de Free1X2/UI/SelectorMS.cs (s2n/n2s/neq/calcular11/12/13/Comparador2).
/// </summary>
public partial class SelectorMSViewModel : ObservableObject
{
    // pot[] legacy: potencias de 3 para extraer el signo de cada partido.
    private static readonly int[] Pot =
    {
        1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323,
    };

    // Estado del cálculo (legacy: arrays tabla/existe/lbtab/resuls + conta + limite).
    private int[,] _tabla = new int[0, 0]; // [columna, equivalentes]
    private int _conta;
    private int _limite = 13;
    private int _limsh;
    private int[] _lbtab = Array.Empty<int>();
    private int[,] _resuls = new int[0, 0];

    // comtab[] de Comparador2 (coincidencias entre dos mitades de 7 signos), perezoso.
    private int[,]? _comtab;

    // Fichero de ganadoras navegable (legacy: colgsR / nrfCGR / limcgsR).
    private readonly List<string> _colgsR = new();
    private int _nrfCGR;

    private CancellationTokenSource? _cts;

    // ===== Aspecto a calcular (lbasp + bMas/bMenos, rango 11..13) =====
    public IReadOnlyList<string> OpcionesAspecto { get; } = new[] { "11", "12", "13" };

    [ObservableProperty]
    private string _aspectoSeleccionado = "13";

    // ===== Fichero de columnas de entrada (lFileIn) =====
    [ObservableProperty] private string _ficheroEntrada = "";

    // ===== Tabla de distribución (dataGrid1 "Distribución") =====
    public ObservableCollection<FilaDistribucion> Distribucion { get; } = new();

    [ObservableProperty]
    private int _filaSeleccionada = -1;

    // ===== Estadísticas (labels del form) =====
    [ObservableProperty] private string _totalColumnas = "0";   // lCol
    [ObservableProperty] private string _tiempo = " ";          // lTime
    [ObservableProperty] private string _sumaSeleccion = "0";   // lSumSel
    [ObservableProperty] private string _ficheroSalida = "";    // lFileOut

    // ===== Sección "Análisis Resultados" (groupBox3) =====
    [ObservableProperty] private string _ficheroGanadoras = "Fichero Ganadoras"; // lFGR
    [ObservableProperty] private string _columnaGanadora = "";                   // tbColR (max 14)
    [ObservableProperty] private string _indiceGanadora = "0";                   // lbCGR

    [ObservableProperty]
    private bool _puedeAnalizar;

    [RelayCommand]
    private void AspectoMas()
    {
        if (int.TryParse(AspectoSeleccionado, out var n) && n < 13)
            AspectoSeleccionado = (n + 1).ToString();
    }

    [RelayCommand]
    private void AspectoMenos()
    {
        if (int.TryParse(AspectoSeleccionado, out var n) && n > 11)
            AspectoSeleccionado = (n - 1).ToString();
    }

    [RelayCommand]
    private async Task Iniciar()
    {
        // Iniciar() legacy: elige fichero, lee columnas, calcula vecinos y agrupa la distribución.
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        FicheroEntrada = Path.GetFileName(file.Path);
        _limite = Convert.ToInt32(AspectoSeleccionado);
        string ruta = file.Path;
        int limite = _limite;

        _cts = new CancellationTokenSource();
        CancellationToken token = _cts.Token;
        Tiempo = " ";
        var time0 = DateTime.Now;
        try
        {
            var (tabla, conta, lbtab) = await Task.Run(() =>
                LeerYCalcular(ruta, limite, token), token);
            _tabla = tabla;
            _conta = conta;
            _lbtab = lbtab;
            _limsh = LimSh(limite);
            _resuls = new int[_limsh, 5];
            RepoblarDistribucion();
            TotalColumnas = conta.ToString();
        }
        catch (OperationCanceledException) { /* cancelado por el usuario */ }
        catch (Exception ex) { AppServices.MostrarError("Error: " + ex.Message); }
        finally
        {
            Tiempo = (DateTime.Now - time0).ToString();
            _cts?.Dispose();
            _cts = null;
        }
    }

    [RelayCommand]
    private void Cancelar() => _cts?.Cancel();

    [RelayCommand]
    private void Sumar()
    {
        // SumSel() legacy: suma la columna C de las filas seleccionadas (aquí, la fila simple).
        if (FilaSeleccionada < 0 || FilaSeleccionada >= Distribucion.Count)
        {
            SumaSeleccion = "0";
            return;
        }
        SumaSeleccion = int.TryParse(Distribucion[FilaSeleccionada].C, out int c) ? c.ToString() : "0";
    }

    [RelayCommand]
    private async Task Grabar()
    {
        // Grabar() legacy: graba las columnas cuyo grupo (equivalentes) está seleccionado.
        if (_conta == 0 || FilaSeleccionada < 0) return;

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "SeleccionMS",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        int grupo = FilaSeleccionada; // índice Q seleccionado
        var tabla = _tabla;
        int conta = _conta;
        try
        {
            await Task.Run(() =>
            {
                IArchivoColumnas w = new ArchivoColumnasTexto(ruta);
                for (int nr = 0; nr < conta; nr++)
                {
                    if (tabla[nr, 1] == grupo) w.GuardarCols(N2S(tabla[nr, 0]));
                }
                w.Cerrar();
            });
            FicheroSalida = Path.GetFileName(ruta);
        }
        catch (Exception ex) { AppServices.MostrarError("Error al grabar: " + ex.Message); }
    }

    [RelayCommand]
    private async Task CargarGanadoras()
    {
        // EntraCGsR() legacy: lee columnas ganadoras (>=14 chars) a colgsR[].
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        _colgsR.Clear();
        try
        {
            foreach (var linea in await Task.Run(() => File.ReadAllLines(file.Path)))
            {
                string tmp = linea.Trim().ToUpper();
                if (tmp.Length == 0) continue;
                if (tmp.Length < 14) { AppServices.MostrarError("col.G. errónea=" + tmp); return; }
                _colgsR.Add(tmp);
            }
        }
        catch (Exception ex) { AppServices.MostrarError(ex.Message); return; }

        if (_colgsR.Count == 0) return;
        _nrfCGR = _colgsR.Count;
        FicheroGanadoras = Path.GetFileName(file.Path);
        IndiceGanadora = _nrfCGR.ToString();
        ColumnaGanadora = _colgsR[_nrfCGR - 1];
        PuedeAnalizar = true;
    }

    [RelayCommand]
    private void GanadoraMas()
    {
        // GRMas() legacy: avanza la posición en colgsR[].
        if (_nrfCGR < _colgsR.Count)
        {
            _nrfCGR++;
            IndiceGanadora = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    [RelayCommand]
    private void GanadoraMenos()
    {
        // GRMenos() legacy: retrocede la posición en colgsR[].
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            IndiceGanadora = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    [RelayCommand]
    private void Analizar()
    {
        // Analizar() legacy: cuenta premios P14..P10 por grupo frente a la columna ganadora.
        if (_conta == 0 || ColumnaGanadora.Length < 14) return;
        _limsh = LimSh(_limite);
        _resuls = new int[_limsh, 5];
        _comtab ??= Comparador2();

        int ncg = S2N(ColumnaGanadora);
        int c7p = ncg / 2187, c7u = ncg % 2187;
        for (int nr = 0; nr < _conta; nr++)
        {
            int nx = _tabla[nr, 0];
            int nref = _tabla[nr, 1];
            int na = Neq(nx, c7p, c7u);
            if (na > 9) _resuls[nref, 14 - na]++;
        }
        RepoblarDistribucion();
    }

    // ---- Lógica portada de SelectorMS ----

    private static int LimSh(int limite) => limite == 13 ? 29 : limite == 12 ? 365 : 2913;

    // Iniciar(): lee columnas a 'tabla', calcula vecinos según aspecto y agrupa en lbtab[].
    private static (int[,] tabla, int conta, int[] lbtab) LeerYCalcular(
        string ruta, int limite, CancellationToken token)
    {
        var existe = new BitArray(4782969, false);
        var tabla = new int[4782969, 2];
        int conta = 0;

        IArchivoColumnas sr = new ArchivoColumnasTexto(ruta);
        while (sr.SiguienteColumna())
        {
            if (token.IsCancellationRequested) { sr.Cerrar(); throw new OperationCanceledException(token); }
            int nx = S2N(sr.LeeColumnaSinComas());
            if (!existe[nx])
            {
                tabla[conta, 0] = nx;
                tabla[conta, 1] = 0;
                existe[nx] = true;
                conta++;
            }
        }
        sr.Cerrar();

        if (limite == 13) Calcular13(tabla, conta, existe, token);
        else if (limite == 12) Calcular12(tabla, conta, existe, token);
        else Calcular11(tabla, conta, existe, token);

        var lbtab = new int[2913];
        for (int nr = 0; nr < conta; nr++) lbtab[tabla[nr, 1]]++;

        return (tabla, conta, lbtab);
    }

    private static void Calcular13(int[,] tabla, int conta, BitArray existe, CancellationToken token)
    {
        for (int ncalc = 0; ncalc < conta; ncalc++)
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException(token);
            int ncg = tabla[ncalc, 0];
            for (int nr = 0; nr < 14; nr++)
            {
                int sign1 = (ncg / Pot[nr]) % 3;
                for (int z1 = 0; z1 < 3; z1++)
                {
                    if (z1 == sign1) continue;
                    int col1 = ncg + Pot[nr] * (z1 - sign1);
                    if (existe[col1]) tabla[ncalc, 1]++;
                }
            }
        }
    }

    private static void Calcular12(int[,] tabla, int conta, BitArray existe, CancellationToken token)
    {
        var repes = new BitArray(4782969, false);
        for (int ncalc = 0; ncalc < conta; ncalc++)
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException(token);
            repes.SetAll(false);
            int ncg = tabla[ncalc, 0];
            repes[ncg] = true;
            for (int nr = 0; nr < 14; nr++)
            {
                int sign1 = (ncg / Pot[nr]) % 3;
                for (int z1 = 0; z1 < 3; z1++)
                {
                    if (z1 == sign1) continue;
                    repes[ncg + Pot[nr] * (z1 - sign1)] = true;
                }
            }
            for (int nr = 0; nr < 14; nr++)
            {
                int sign1 = (ncg / Pot[nr]) % 3;
                for (int z1 = 0; z1 < 3; z1++)
                {
                    if (z1 == sign1) continue;
                    int col1 = ncg + Pot[nr] * (z1 - sign1);
                    for (int nr2 = 0; nr2 < 14; nr2++)
                    {
                        int sign2 = (col1 / Pot[nr2]) % 3;
                        for (int z2 = 0; z2 < 3; z2++)
                        {
                            if (z2 == sign2) continue;
                            int col2 = col1 + Pot[nr2] * (z2 - sign2);
                            if (existe[col2] && !repes[col2]) { repes[col2] = true; tabla[ncalc, 1]++; }
                        }
                    }
                }
            }
        }
    }

    private static void Calcular11(int[,] tabla, int conta, BitArray existe, CancellationToken token)
    {
        var repes = new BitArray(4782969, false);
        for (int ncalc = 0; ncalc < conta; ncalc++)
        {
            if (token.IsCancellationRequested) throw new OperationCanceledException(token);
            repes.SetAll(false);
            int ncg = tabla[ncalc, 0];
            repes[ncg] = true;
            for (int nr = 0; nr < 14; nr++)
            {
                int sign1 = (ncg / Pot[nr]) % 3;
                for (int z1 = 0; z1 < 3; z1++)
                {
                    if (z1 == sign1) continue;
                    int col1 = ncg + Pot[nr] * (z1 - sign1);
                    repes[col1] = true;
                    for (int nr2 = 0; nr2 < 14; nr2++)
                    {
                        int sign2 = (col1 / Pot[nr2]) % 3;
                        for (int z2 = 0; z2 < 3; z2++)
                        {
                            if (z2 == sign2) continue;
                            repes[col1 + Pot[nr2] * (z2 - sign2)] = true;
                        }
                    }
                }
            }
            for (int nr = 0; nr < 14; nr++)
            {
                int sign1 = (ncg / Pot[nr]) % 3;
                for (int z1 = 0; z1 < 3; z1++)
                {
                    if (z1 == sign1) continue;
                    int col1 = ncg + Pot[nr] * (z1 - sign1);
                    for (int nr2 = 0; nr2 < 14; nr2++)
                    {
                        int sign2 = (col1 / Pot[nr2]) % 3;
                        for (int z2 = 0; z2 < 3; z2++)
                        {
                            if (z2 == sign2) continue;
                            int col2 = col1 + Pot[nr2] * (z2 - sign2);
                            for (int nr3 = 0; nr3 < 14; nr3++)
                            {
                                int sign3 = (col2 / Pot[nr3]) % 3;
                                for (int z3 = 0; z3 < 3; z3++)
                                {
                                    if (z3 == sign3) continue;
                                    int col3 = col2 + Pot[nr3] * (z3 - sign3);
                                    if (existe[col3] && !repes[col3]) { repes[col3] = true; tabla[ncalc, 1]++; }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Comparador2() legacy: tabla de coincidencias entre todas las mitades de 7 signos.
    private static int[,] Comparador2()
    {
        var comtab = new int[2187, 2187];
        for (int nr = 0; nr < 2187; nr++)
        {
            string ax1 = N7S(nr);
            comtab[nr, nr] = 7;
            for (int nr2 = nr + 1; nr2 < 2187; nr2++)
            {
                int na = Neq7(ax1, N7S(nr2));
                comtab[nr, nr2] = na;
                comtab[nr2, nr] = na;
            }
        }
        return comtab;
    }

    private int Neq(int colnum, int c7p, int c7u)
    {
        int n7p = colnum / 2187, n7u = colnum % 2187;
        return _comtab![c7p, n7p] + _comtab[c7u, n7u];
    }

    private static int Neq7(string ax1, string ax2)
    {
        int na = 0;
        for (int nr = 0; nr < 7; nr++) if (ax1[nr] == ax2[nr]) na++;
        return na;
    }

    private static string N7S(int nx)
    {
        string ax = "";
        for (int nr = 0; nr < 7; nr++)
        {
            int nx2 = nx % 3; nx /= 3;
            ax = (nx2 == 1 ? "1" : nx2 == 2 ? "2" : "X") + ax;
        }
        return ax;
    }

    private static string N2S(int nx)
    {
        string ax = "";
        for (int nr = 0; nr < 14; nr++)
        {
            int nx2 = nx % 3; nx /= 3;
            ax = (nx2 == 1 ? "1" : nx2 == 2 ? "2" : "X") + ax;
        }
        return ax;
    }

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

    // InicializaFuenteDatos() legacy: vuelca lbtab/resuls a la tabla de distribución.
    private void RepoblarDistribucion()
    {
        Distribucion.Clear();
        for (int nr = 0; nr < _limsh; nr++)
        {
            Distribucion.Add(new FilaDistribucion
            {
                Q = nr.ToString(),
                C = (nr < _lbtab.Length ? _lbtab[nr] : 0).ToString(),
                P14 = _resuls[nr, 0].ToString(),
                P13 = _resuls[nr, 1].ToString(),
                P12 = _resuls[nr, 2].ToString(),
                P11 = _resuls[nr, 3].ToString(),
                P10 = _resuls[nr, 4].ToString(),
            });
        }
    }
}

/// <summary>
/// Fila de la tabla de distribución (dataGrid1 "Resultados"/"Distribución" del legacy).
/// Q = índice de grupo; C = nº de columnas; P14..P10 = premios tras analizar.
/// Regla anti-crash 2: todos los valores se exponen como string para enlazar a TextBlock.Text.
/// </summary>
public sealed class FilaDistribucion
{
    public string Q { get; init; } = "";
    public string C { get; init; } = "";
    public string P14 { get; init; } = "";
    public string P13 { get; init; } = "";
    public string P12 { get; init; } = "";
    public string P11 { get; init; } = "";
    public string P10 { get; init; } = "";
}
