// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del WinForms <c>SelecJM</c> ("Selección 5+5+4 de JuanM", archivo UI/SelectorJM.cs).
/// Reparte los 14 partidos en 3 grupos (gr.1 = 5 partidos, gr.2 = 5 partidos,
/// gr.3 = 4 partidos), genera las tablas ordenadas de productos de cada grupo
/// (243 / 243 / 81), aplica límites por grupo y por total, y obtiene las
/// columnas válidas — bien por corte de rangos, bien por valor exacto.
/// También analiza columnas ganadoras leídas de fichero para ubicar su rango.
/// Los selectores de ficheros (.cnd / ganadoras) están cableados, y el cálculo está
/// portado: la matriz de valoraciones procede de la rejilla PorcentajesControl
/// (reemplaza el UserControl 'valors'): PorcentajesHelper.AMatriz(Porcentajes) == valors1.RetVals().
/// </summary>
public partial class SelecJMViewModel : ObservableObject
{
    // Rejilla de valoraciones 1/X/2 por partido (reemplaza el UserControl WinForms 'valors'
    // / valors1). PorcentajesHelper.AMatriz(Porcentajes) == valors1.RetVals() (double[14,3]).
    public ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(VariablesGlobales.NumeroPartidos);

    // Fichero de ganadoras navegable (legacy: colgsR / nrfCGR / limcgsR).
    private readonly List<string> _colgsR = new();
    private int _nrfCGR;

    // ===== Estado interno del cálculo (campos de instancia del Form legacy SelecJM) =====
    private bool _salida;                                  // legacy salida (cancelación)
    private double[,] _nvals = new double[14, 3];          // legacy nvals (= valors1.RetVals())
    private readonly double[,] _wvals = new double[5, 3];  // legacy wvals
    private readonly int[] _ngrups = new int[14];          // legacy ngrups
    private readonly double[] _pg1 = new double[243];
    private readonly int[] _id1 = new int[243];
    private readonly BitArray _tablim1 = new(243);
    private readonly double[] _pg2 = new double[243];
    private readonly int[] _id2 = new int[243];
    private readonly BitArray _tablim2 = new(243);
    private readonly double[] _pg3 = new double[81];
    private readonly int[] _id3 = new int[81];
    private readonly BitArray _tablim3 = new(81);
    private readonly BitArray _tablimtt = new(567);
    private BitArray _validasBits = new(4782969);
    private int _ctproc, _ctval;

    // ===== Grupos (g01..g14): a qué grupo 1/2/3 pertenece cada uno de los 14 partidos =====
    [ObservableProperty] private string _grupo01 = "1";
    [ObservableProperty] private string _grupo02 = "1";
    [ObservableProperty] private string _grupo03 = "1";
    [ObservableProperty] private string _grupo04 = "1";
    [ObservableProperty] private string _grupo05 = "1";
    [ObservableProperty] private string _grupo06 = "2";
    [ObservableProperty] private string _grupo07 = "2";
    [ObservableProperty] private string _grupo08 = "2";
    [ObservableProperty] private string _grupo09 = "2";
    [ObservableProperty] private string _grupo10 = "2";
    [ObservableProperty] private string _grupo11 = "3";
    [ObservableProperty] private string _grupo12 = "3";
    [ObservableProperty] private string _grupo13 = "3";
    [ObservableProperty] private string _grupo14 = "3";

    // ===== Límites (GroupBox "Límites") =====
    [ObservableProperty] private string _limiteGrupo1 = "1-243"; // legacy tbgr1
    [ObservableProperty] private string _limiteGrupo2 = "1-243"; // legacy tbgr2
    [ObservableProperty] private string _limiteGrupo3 = "1-81";  // legacy tbgr3
    [ObservableProperty] private string _limiteTotal = "1-567";  // legacy tbtot

    // ===== Resultados de % por grupo (readonly: lpc1/lpc2/lpc3) =====
    [ObservableProperty] private string _porcentaje1 = "%1";
    [ObservableProperty] private string _porcentaje2 = "%2";
    [ObservableProperty] private string _porcentaje3 = "%3";

    // ===== Modo de examen (GroupBox "Examinar": rbcorte / rbvalor) =====
    public IReadOnlyList<string> ModosExamen { get; } = new[] { "Por corte", "Por valor" };

    [ObservableProperty] private string _modoExamen = "Por corte";

    // ===== Fichero de condiciones (lfcond) =====
    [ObservableProperty] private string _ficheroCondiciones = "(ninguno)";

    // ===== Análisis de resultados (GroupBox "Análisis Resultados") =====
    [ObservableProperty] private string _ficheroGanadoras = "Fichero ganadoras"; // legacy lFGR
    [ObservableProperty] private string _columnaGanadora = string.Empty;          // legacy tbCG (max 14)
    [ObservableProperty] private string _indiceGanadora = string.Empty;           // legacy lbCGR

    // Rangos calculados de la columna analizada dentro de cada grupo (ang1/ang2/ang3/angt).
    [ObservableProperty] private string _rangoGrupo1 = "-";
    [ObservableProperty] private string _rangoGrupo2 = "-";
    [ObservableProperty] private string _rangoGrupo3 = "-";
    [ObservableProperty] private string _rangoTotal = "-";

    // ===== Estado del proceso (lproc / lvalidas / ltime / lfile) =====
    [ObservableProperty] private string _procesadas = "Procesadas";
    [ObservableProperty] private string _validas = "Válidas";
    [ObservableProperty] private string _tiempo = "Tiempo";
    [ObservableProperty] private string _ficheroSalida = "(ninguno)";

    [ObservableProperty] private bool _puedeAnalizar;

    // ===== Acciones =====

    [RelayCommand]
    private async Task Calcular()
    {
        // Equivale a SelecJM.Calcular() (Free1X2/UI/SelectorJM.cs líneas 168-187):
        //   RecuperaPantalla() (lee valors1.RetVals() + grupos + límites), Calgr1/2/3() y
        //   BuscaCorte()/BuscaValor() según el modo. La matriz de valoraciones procede de la
        //   rejilla de porcentajes (PorcentajesHelper.AMatriz(Porcentajes) == valors1.RetVals()).
        _salida = false;
        Procesadas = Validas = Tiempo = " ";
        FicheroSalida = " ";

        // La rejilla se lee en el hilo de UI; el cálculo pesado (hasta 3^14 columnas) en background.
        double[,] nvalsUi = PorcentajesHelper.AMatriz(Porcentajes);  // == valors1.RetVals()
        string g1 = LimiteGrupo1, g2 = LimiteGrupo2, g3 = LimiteGrupo3, gt = LimiteTotal;
        int[] grupos = LeerGrupos();
        bool porCorte = ModoExamen == "Por corte";

        DateTime dt0 = DateTime.Now;
        try
        {
            var res = await Task.Run(() =>
            {
                string? error = RecuperaPantalla(nvalsUi, grupos, g1, g2, g3, gt);
                if (error != null) return (Error: error, P1: "", P2: "", P3: "", Proc: 0, Val: 0);
                string p1 = Calgr1();
                string p2 = Calgr2();
                string p3 = Calgr3();
                if (porCorte) BuscaCorte(); else BuscaValor();
                return (Error: (string?)null, P1: p1, P2: p2, P3: p3, Proc: _ctproc, Val: _ctval);
            });

            if (res.Error != null)
            {
                AppServices.MostrarError(res.Error);
                return;
            }

            // Equivale a veureelmeu() (líneas 188-194): vuelca %, procesadas, válidas y tiempo.
            Porcentaje1 = res.P1;
            Porcentaje2 = res.P2;
            Porcentaje3 = res.P3;
            Validas = res.Val.ToString();
            Procesadas = res.Proc.ToString();
            string temp = (DateTime.Now - dt0).ToString() + "0000000000";
            Tiempo = temp.Length >= 10 ? temp.Substring(0, 10) : temp;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError(ex.Message);
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy BCancelarClick: salida = true (aborta el bucle de cálculo).
        _salida = true;
    }

    [RelayCommand]
    private async Task GrabarColumnas()
    {
        // Equivale a SelecJM.GrabaCols() (Free1X2/UI/SelectorJM.cs líneas 195-245): graba la
        // tabla de productos por grupo y, en el fichero elegido, las columnas válidas (validas[]).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ResultadosJM",
        };
        picker.FileTypeChoices.Add("Resultados", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        try
        {
            string ruta = file.Path;
            await Task.Run(() =>
            {
                using var wr = new StreamWriter(ruta);
                for (int nr = 0; nr < 4782969; nr++)
                {
                    if (_validasBits[nr]) wr.WriteLine(n2s(nr, 14));
                }
            });
            FicheroSalida = Path.GetFileName(ruta);
        }
        catch (Exception ex) { AppServices.MostrarError(ex.Message); }
    }

    [RelayCommand]
    private async Task LeerCondiciones()
    {
        // LeeCondis() legacy: lee un .cnd de 6 líneas (grupos, límites gr1/gr2/gr3, total, modo).
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".cnd");
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            var buff = await Task.Run(() => File.ReadAllLines(file.Path));
            if (buff.Length < 6) { AppServices.MostrarError("fichero de condiciones erróneo"); return; }

            string grupos = buff[0];
            if (grupos.Length < 14) { AppServices.MostrarError("fichero de condiciones erróneo"); return; }
            Grupo01 = grupos[0].ToString(); Grupo02 = grupos[1].ToString();
            Grupo03 = grupos[2].ToString(); Grupo04 = grupos[3].ToString();
            Grupo05 = grupos[4].ToString(); Grupo06 = grupos[5].ToString();
            Grupo07 = grupos[6].ToString(); Grupo08 = grupos[7].ToString();
            Grupo09 = grupos[8].ToString(); Grupo10 = grupos[9].ToString();
            Grupo11 = grupos[10].ToString(); Grupo12 = grupos[11].ToString();
            Grupo13 = grupos[12].ToString(); Grupo14 = grupos[13].ToString();
            LimiteGrupo1 = buff[1];
            LimiteGrupo2 = buff[2];
            LimiteGrupo3 = buff[3];
            LimiteTotal = buff[4];
            ModoExamen = buff[5] == "0" ? "Por valor" : "Por corte";
            FicheroCondiciones = Path.GetFileName(file.Path);
        }
        catch (Exception ex) { AppServices.MostrarError(ex.Message); }
    }

    [RelayCommand]
    private async Task SalvarCondiciones()
    {
        // SalvaCondis() legacy: graba grupos + límites + modo a un .cnd (6 líneas).
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Condiciones",
        };
        picker.FileTypeChoices.Add("F.Condiciones", new List<string> { ".cnd" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string grupos = Grupo01 + Grupo02 + Grupo03 + Grupo04 + Grupo05 + Grupo06 + Grupo07 +
            Grupo08 + Grupo09 + Grupo10 + Grupo11 + Grupo12 + Grupo13 + Grupo14;
        var lineas = new[]
        {
            grupos,
            LimiteGrupo1,
            LimiteGrupo2,
            LimiteGrupo3,
            LimiteTotal,
            ModoExamen == "Por corte" ? "1" : "0",
        };
        try
        {
            await Task.Run(() => File.WriteAllLines(file.Path, lineas));
            FicheroCondiciones = Path.GetFileName(file.Path);
        }
        catch (Exception ex) { AppServices.MostrarError(ex.Message); }
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
                if (tmp.Length < 14) { AppServices.MostrarError("col.G. errónea=" + (_colgsR.Count + 1)); break; }
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
    private void Analizar()
    {
        // Equivale a SelecJM.Analizar() (Free1X2/UI/SelectorJM.cs líneas 599-602):
        //   por corte -> AnaCorte(), por valor -> AnaValor(). Ambos hacen RecuperaPantalla()
        //   + Calgr1/2/3() y ubican la columna ganadora en cada grupo. La valoración procede
        //   de la rejilla de porcentajes (== valors1.RetVals()).
        string columna = (ColumnaGanadora ?? "").ToUpper();
        if (columna.Length < 14) { AppServices.MostrarError("col.G. errónea"); return; }

        double[,] nvalsUi = PorcentajesHelper.AMatriz(Porcentajes);  // == valors1.RetVals()
        int[] grupos = LeerGrupos();
        string? error = RecuperaPantalla(nvalsUi, grupos, LimiteGrupo1, LimiteGrupo2, LimiteGrupo3, LimiteTotal);
        if (error != null) { AppServices.MostrarError(error); return; }
        Porcentaje1 = Calgr1();
        Porcentaje2 = Calgr2();
        Porcentaje3 = Calgr3();

        if (ModoExamen == "Por corte") AnaCorte(columna);
        else AnaValor(columna);
    }

    [RelayCommand]
    private void GanadoraSiguiente()
    {
        // GRMas() legacy: avanza nrfCGR en colgsR[].
        if (_nrfCGR < _colgsR.Count)
        {
            _nrfCGR++;
            IndiceGanadora = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    [RelayCommand]
    private void GanadoraAnterior()
    {
        // GRMenos() legacy: retrocede nrfCGR en colgsR[].
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            IndiceGanadora = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    // ==================== Lógica portada de SelecJM (sin tocar el motor) ====================

    // Lee los 14 grupos de pantalla a un int[] (g01..g14). Convert.ToInt32 igual que el legacy.
    private int[] LeerGrupos()
    {
        var g = new[]
        {
            Grupo01, Grupo02, Grupo03, Grupo04, Grupo05, Grupo06, Grupo07,
            Grupo08, Grupo09, Grupo10, Grupo11, Grupo12, Grupo13, Grupo14,
        };
        var n = new int[14];
        for (int i = 0; i < 14; i++) int.TryParse((g[i] ?? "").Trim(), out n[i]);
        return n;
    }

    // Equivale a SelecJM.RecuperaPantalla() (líneas 246-383): copia nvals (sustituyendo ceros por
    // 1E-1), valida que los grupos sean 5/5/4 y construye las tablas de límites (BitArrays).
    // Devuelve null si todo correcto o el mensaje de error legacy en caso contrario.
    private string? RecuperaPantalla(double[,] nvals, int[] grupos, string lim1, string lim2, string lim3, string limt)
    {
        _nvals = (double[,])nvals.Clone();
        for (int nr1 = 0; nr1 < 14; nr1++)
            for (int nr2 = 0; nr2 < 3; nr2++)
                if (_nvals[nr1, nr2] == 0) _nvals[nr1, nr2] = 1E-1;

        for (int nr = 0; nr < 14; nr++) _ngrups[nr] = grupos[nr];

        int n1 = 0, n2 = 0, n3 = 0;
        for (int nr = 0; nr < 14; nr++)
        {
            switch (_ngrups[nr])
            {
                case 1: n1++; break;
                case 2: n2++; break;
                case 3: n3++; break;
            }
        }
        if (n1 != 5 || n2 != 5 || n3 != 4) return "error en grupos";

        bool nerr = false;
        nerr |= !LlenarTablim(_tablim1, lim1, 243);
        nerr |= !LlenarTablim(_tablim2, lim2, 243);
        nerr |= !LlenarTablim(_tablim3, lim3, 81);
        nerr |= !LlenarTablim(_tablimtt, limt, 567);
        return nerr ? "error en limites" : null;
    }

    // Réplica del parseo de límites "a,b-c,..." del legacy (RecuperaPantalla, líneas 282-378).
    private static bool LlenarTablim(BitArray tabla, string texto, int max)
    {
        tabla.SetAll(false);
        string[] aux1 = (texto ?? "").Split(',');
        if (aux1.Length == 0) return false;
        foreach (string parte in aux1)
        {
            string[] aux2 = parte.Split('-');
            if (aux2.Length == 0) continue;
            if (aux2.Length == 1)
            {
                if (!int.TryParse(aux2[0], out int ni)) return false;
                if (ni < 1) ni = 1; if (ni > max) ni = max;
                tabla[ni - 1] = true;
            }
            else if (aux2.Length == 2)
            {
                if (!int.TryParse(aux2[0], out int ni)) return false;
                if (!int.TryParse(aux2[1], out int ns)) return false;
                if (ni > ns) { int nx = ni; ni = ns; ns = nx; }
                if (ni < 1) ni = 1; if (ni > max) ni = max;
                if (ns < 1) ns = 1; if (ns > max) ns = max;
                for (int nr = ni - 1; nr < ns; nr++) tabla[nr] = true;
            }
            else return false;
        }
        return true;
    }

    // Equivale a SelecJM.mxsort() (líneas 384-396): selección descendente sobre (ls1, ls2).
    private static void mxsort(double[] ls1, int[] ls2, int inf, int sup)
    {
        for (int i = inf; i < sup; i++)
        {
            double refer = ls1[i], tr1 = ls1[i];
            int s = i;
            for (int nr = i + 1; nr <= sup; nr++)
            {
                if (ls1[nr] > refer) { s = nr; refer = ls1[s]; }
            }
            ls1[i] = refer; ls1[s] = tr1;
            int t = ls2[i]; ls2[i] = ls2[s]; ls2[s] = t;
        }
    }

    // Equivale a SelecJM.s2n()/n2s() (líneas 499-518): base 3 con X=0, 1=1, 2=2.
    private static int s2n(string ax, int lim)
    {
        int nx = 0;
        for (int nr = 0; nr < lim; nr++)
        {
            nx *= 3;
            char ch = ax[nr];
            if (ch == '1') nx += 1;
            else if (ch == '2') nx += 2;
        }
        return nx;
    }

    private static string n2s(int nx, int lim)
    {
        string ax = "";
        for (int nr = 0; nr < lim; nr++)
        {
            int nx2 = nx % 3; nx /= 3;
            if (nx2 == 1) ax = "1" + ax;
            else if (nx2 == 2) ax = "2" + ax;
            else ax = "X" + ax;
        }
        return ax;
    }

    // Equivale a SelecJM.Calgr1/2/3() (líneas 397-498): genera la tabla de productos de cada
    // grupo, la ordena y devuelve el % acumulado de los límites (texto, como lpc1/2/3.Text).
    private string Calgr1() => CalgrCinco(1, _pg1, _id1, _tablim1, 1e8);
    private string Calgr2() => CalgrCinco(2, _pg2, _id2, _tablim2, 1e8);
    private string Calgr3() => CalgrCuatro(_pg3, _id3, _tablim3);

    private string CalgrCinco(int grupo, double[] pg, int[] id, BitArray tablim, double divisor)
    {
        for (int nr = 0, nx = 0; nr < 14; nr++)
        {
            if (_ngrups[nr] == grupo)
            {
                _wvals[nx, 1] = _nvals[nr, 0];
                _wvals[nx, 0] = _nvals[nr, 1];
                _wvals[nx, 2] = _nvals[nr, 2];
                nx++;
            }
        }
        int idx = 0;
        for (int a = 0; a < 3; a++)
        {
            double vl1 = _wvals[0, a];
            for (int b = 0; b < 3; b++)
            {
                double vl2 = vl1 * _wvals[1, b];
                for (int c = 0; c < 3; c++)
                {
                    double vl3 = vl2 * _wvals[2, c];
                    for (int d = 0; d < 3; d++)
                    {
                        double vl4 = vl3 * _wvals[3, d];
                        for (int e = 0; e < 3; e++)
                        {
                            pg[idx] = vl4 * _wvals[4, e];
                            id[idx] = idx;
                            idx++;
                        }
                    }
                }
            }
        }
        mxsort(pg, id, 0, 242);
        double pc = 0;
        for (int nr = 0; nr < 243; nr++) if (tablim[nr]) pc += pg[nr];
        return string.Format(CultureInfo.CurrentCulture, "{0:f2}", pc / divisor);
    }

    private string CalgrCuatro(double[] pg, int[] id, BitArray tablim)
    {
        for (int nr = 0, nx = 0; nr < 14; nr++)
        {
            if (_ngrups[nr] == 3)
            {
                _wvals[nx, 1] = _nvals[nr, 0];
                _wvals[nx, 0] = _nvals[nr, 1];
                _wvals[nx, 2] = _nvals[nr, 2];
                nx++;
            }
        }
        int idx = 0;
        for (int a = 0; a < 3; a++)
        {
            double vl1 = _wvals[0, a];
            for (int b = 0; b < 3; b++)
            {
                double vl2 = vl1 * _wvals[1, b];
                for (int c = 0; c < 3; c++)
                {
                    double vl3 = vl2 * _wvals[2, c];
                    for (int d = 0; d < 3; d++)
                    {
                        pg[idx] = vl3 * _wvals[3, d];
                        id[idx] = idx;
                        idx++;
                    }
                }
            }
        }
        mxsort(pg, id, 0, 80);
        double pc = 0;
        for (int nr = 0; nr < 81; nr++) if (tablim[nr]) pc += pg[nr];
        return string.Format(CultureInfo.CurrentCulture, "{0:f2}", pc / 1e6);
    }

    // Equivale a SelecJM.BuscaCorte() (líneas 519-558): por intersección de los rangos de cada
    // grupo y del total, marca las columnas válidas (validas[]).
    private void BuscaCorte()
    {
        _validasBits = new BitArray(4782969);
        _ctval = _ctproc = 0;
        for (int nr1 = 0; nr1 < 243; nr1++)
        {
            if (_salida) break;
            if (_tablim1[nr1])
            {
                string col1 = n2s(_id1[nr1], 5);
                for (int nr2 = 0; nr2 < 243; nr2++)
                {
                    if (_tablim2[nr2])
                    {
                        string col2 = n2s(_id2[nr2], 5);
                        for (int nr3 = 0; nr3 < 81; nr3++)
                        {
                            if (_tablim3[nr3] && _tablimtt[nr1 + nr2 + nr3 + 2])
                            {
                                string col3 = n2s(_id3[nr3], 4);
                                string tmp14 = "", tmp1 = col1, tmp2 = col2, tmp3 = col3;
                                for (int nr4 = 0; nr4 < 14; nr4++)
                                {
                                    if (_ngrups[nr4] == 1) { tmp14 += tmp1.Substring(0, 1); tmp1 = tmp1.Substring(1); }
                                    else if (_ngrups[nr4] == 2) { tmp14 += tmp2.Substring(0, 1); tmp2 = tmp2.Substring(1); }
                                    else { tmp14 += tmp3.Substring(0, 1); tmp3 = tmp3.Substring(1); }
                                }
                                _validasBits[s2n(tmp14, 14)] = true; _ctval++;
                            }
                            _ctproc++;
                        }
                    }
                    else _ctproc += 81;
                }
            }
            else _ctproc += 19683;
        }
    }

    // Equivale a SelecJM.BuscaValor() (líneas 559-598): recorre las 3^14 columnas y valida las
    // que coinciden con un producto presente en cada grupo dentro de límites.
    private void BuscaValor()
    {
        _validasBits = new BitArray(4782969);
        _ctval = _ctproc = 0;
        for (int nr0 = 0; nr0 < 4782969; nr0++)
        {
            if (_salida) break;
            string tmp1 = n2s(nr0, 14);
            double n1 = 1, n2 = 1, n3 = 1;
            for (int nr = 0; nr < 14; nr++)
            {
                char ch = tmp1[nr];
                double nv;
                if (ch == '1') nv = _nvals[nr, 0];
                else if (ch == '2') nv = _nvals[nr, 2];
                else nv = _nvals[nr, 1];
                if (_ngrups[nr] == 1) n1 *= nv;
                else if (_ngrups[nr] == 2) n2 *= nv;
                else n3 *= nv;
            }
            for (int nr1 = 0; nr1 < 243; nr1++)
            {
                if (_pg1[nr1] == n1 && _tablim1[nr1])
                {
                    for (int nr2 = 0; nr2 < 243; nr2++)
                    {
                        if (_pg2[nr2] == n2 && _tablim2[nr2])
                        {
                            for (int nr3 = 0; nr3 < 81; nr3++)
                            {
                                if (_pg3[nr3] == n3 && _tablim3[nr3])
                                {
                                    if (_tablimtt[nr1 + nr2 + nr3 + 2])
                                    {
                                        if (!_validasBits[nr0]) { _validasBits[nr0] = true; _ctval++; }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            _ctproc++;
        }
    }

    // Equivale a SelecJM.AnaCorte() (líneas 603-631): ubica la columna ganadora por índice en cada grupo.
    private void AnaCorte(string columna)
    {
        string col1 = "", col2 = "", col3 = "";
        for (int nr = 0; nr < 14; nr++)
        {
            char ch = columna[nr];
            int id = _ngrups[nr];
            if (id == 1) col1 += ch;
            else if (id == 2) col2 += ch;
            else col3 += ch;
        }
        int i1 = s2n(col1, 5), i2 = s2n(col2, 5), i3 = s2n(col3, 4);
        for (int nr = 0; nr < 243; nr++) if (_id1[nr] == i1) { i1 = nr; break; }
        for (int nr = 0; nr < 243; nr++) if (_id2[nr] == i2) { i2 = nr; break; }
        for (int nr = 0; nr < 81; nr++) if (_id3[nr] == i3) { i3 = nr; break; }
        RangoGrupo1 = "" + (i1 + 1);
        RangoGrupo2 = "" + (i2 + 1);
        RangoGrupo3 = "" + (i3 + 1);
        RangoTotal = "" + (i1 + i2 + i3 + 3);
    }

    // Equivale a SelecJM.AnaValor() (líneas 632-665): ubica la columna ganadora por producto en cada grupo.
    private void AnaValor(string columna)
    {
        double n1 = 1, n2 = 1, n3 = 1;
        for (int nr = 0; nr < 14; nr++)
        {
            char ch = columna[nr];
            double nv;
            if (ch == '1') nv = _nvals[nr, 0];
            else if (ch == '2') nv = _nvals[nr, 2];
            else nv = _nvals[nr, 1];
            if (nv == 0) nv = 1;
            if (_ngrups[nr] == 1) n1 *= nv;
            else if (_ngrups[nr] == 2) n2 *= nv;
            else n3 *= nv;
        }
        int i1 = 0, i2 = 0, i3 = 0;
        for (int nr = 0; nr < 243; nr++) if (_pg1[nr] == n1) { i1 = nr; break; }
        for (int nr = 0; nr < 243; nr++) if (_pg2[nr] == n2) { i2 = nr; break; }
        for (int nr = 0; nr < 81; nr++) if (_pg3[nr] == n3) { i3 = nr; break; }
        RangoGrupo1 = "" + (i1 + 1);
        RangoGrupo2 = "" + (i2 + 1);
        RangoGrupo3 = "" + (i3 + 1);
        RangoTotal = "" + (i1 + i2 + i3 + 3);
    }
}
