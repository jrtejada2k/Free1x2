// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

// ViewModel para FiltroPorcenJBPage.
// Porta el WinForms legacy Free1X2.UI.FiltroPorcenJB ("Separador Porcentajes Juan Bellas").
//
// CABLEADO REAL: la persistencia de rangos/límites (*.jb7) y la navegación de columnas
// ganadoras se cablean al sistema de ficheros con pickers de WinUI.
//
// CABLEADO DE MATRIZ: la rejilla de porcentajes (legacy UserControl 'valors', no portado)
// se sustituye por PorcentajesControl, cuya colección Porcentajes equivale a 'valors1.RetVals()'.
// RecuperaPantalla()/Valida()/Analizar()/ExporCols() se reproducen aquí desde FiltroPorcenJB.cs
// (RecuperaPantalla 224, Valida 465, Analizar 536, ExporCols 602).
public partial class FiltroPorcenJBViewModel : ObservableObject
{
    // Rejilla de porcentajes/valoraciones por partido (1/X/2). Sustituye al UserControl 'valors';
    // PorcentajesHelper.AMatriz(Porcentajes) equivale a valors1.RetVals() (double[14,3]).
    public ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    // Columnas ganadoras cargadas (legacy: string[3000] colgsR) y navegación (limcgsR / nrfCGR).
    private readonly List<string> _colgsR = new();
    private int _nrfCGR;

    private bool _salida;

    // --- Estado del motor reproducido del legacy (cps[14,7], scps[7], rangos rks, límites lims). ---
    private readonly int[,] _cps = new int[14, 7];
    private readonly string[] _scps = new string[7];
    private readonly int[] _lims = new int[6];
    private readonly int[,] _rks = new int[8, 2];

    // --- Limites de banda (legacy tblim1..tblim6, "<=" cut-offs). Defaults del Designer. ---
    [ObservableProperty]
    private double _limite1 = 15;

    [ObservableProperty]
    private double _limite2 = 22;

    [ObservableProperty]
    private double _limite3 = 29;

    [ObservableProperty]
    private double _limite4 = 36;

    [ObservableProperty]
    private double _limite5 = 48;

    [ObservableProperty]
    private double _limite6 = 61;

    // --- Rangos "min-max" por banda (legacy tbrank1..tbrank7), texto libre tipo "0-3". ---
    [ObservableProperty]
    private string _rango1 = "0-3";

    [ObservableProperty]
    private string _rango2 = "0-3";

    [ObservableProperty]
    private string _rango3 = "0-3";

    [ObservableProperty]
    private string _rango4 = "0-3";

    [ObservableProperty]
    private string _rango5 = "0-3";

    [ObservableProperty]
    private string _rango6 = "0-3";

    [ObservableProperty]
    private string _rango7 = "0-3";

    // Rango de recorrido max-min (legacy tbmgreco), "0-14".
    [ObservableProperty]
    private string _recorrido = "0-14";

    // --- Columna ganadora a analizar (legacy tbCG). ---
    [ObservableProperty]
    private string _columnaGanadora = "COL.GANADORA";

    // --- Etiquetas de resultado (legacy labels read-only). string para bindear a TextBlock.Text. ---
    [ObservableProperty]
    private string _columnasProcesadas = "procesadas";

    [ObservableProperty]
    private string _columnasAdmitidas = "admitidas";

    [ObservableProperty]
    private string _tiempo = "tiempo";

    [ObservableProperty]
    private string _ficheroResultado = "fichero";

    [ObservableProperty]
    private string _ficheroRangos = "fichero";

    [ObservableProperty]
    private string _ficheroGanadoras = "fichero ganadoras";

    [ObservableProperty]
    private string _contadorGanadoras = "-";

    // Resultado del analisis por banda (legacy lrk1..lrk7 + lreco).
    [ObservableProperty]
    private string _analisisBanda1 = "-";

    [ObservableProperty]
    private string _analisisBanda2 = "-";

    [ObservableProperty]
    private string _analisisBanda3 = "-";

    [ObservableProperty]
    private string _analisisBanda4 = "-";

    [ObservableProperty]
    private string _analisisBanda5 = "-";

    [ObservableProperty]
    private string _analisisBanda6 = "-";

    [ObservableProperty]
    private string _analisisBanda7 = "-";

    [ObservableProperty]
    private string _analisisRecorrido = "-";

    // --- Acciones ---

    // Columnas admitidas tras el último Calcular() (legacy: string[] validas, contador ctadm).
    private readonly List<string> _validas = new();

    [RelayCommand]
    private async Task CalcularAsync()
    {
        // Legacy: FiltroPorcenJB.Calcular() (línea 424) — RecuperaPantalla() (valors1.RetVals()),
        // luego filtra el fichero de entrada con Valida() y deja las admitidas en validas[].
        _salida = false;
        ColumnasProcesadas = " ";
        ColumnasAdmitidas = " ";
        Tiempo = " ";
        _validas.Clear();

        if (!RecuperaPantalla())
        {
            AppServices.MostrarError("error en datos de entrada");
            return;
        }

        // Legacy: OpenFileDialog del fichero de columnas de entrada (*.txt).
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        // Legacy: BitArray repes (4.782.969) para descartar columnas repetidas por índice s2n.
        var repes = new HashSet<int>();
        int ctini = 0, ctadm = 0;
        var time0 = DateTime.Now;
        try
        {
            using var sr = new StreamReader(file.Path);
            string? linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (_salida) break;
                string col = linea.Trim().ToUpper();
                if (col.Length == 0) continue;
                ctini++;
                if (col.Length < 14)
                {
                    AppServices.MostrarError("error longitud=" + col);
                    break;
                }
                int idx = S2n(col);
                if (repes.Add(idx))
                {
                    col = col.Replace('X', '4');
                    if (Valida(col)) { _validas.Add(col); ctadm++; }
                }
            }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de entrada: " + ex.Message);
            return;
        }

        ColumnasProcesadas = ctini.ToString();
        ColumnasAdmitidas = ctadm.ToString();
        Tiempo = (DateTime.Now - time0).ToString();
    }

    [RelayCommand]
    private async Task GrabarResultadoAsync()
    {
        // Legacy: FiltroPorcenJB.GrabaCols() (línea 398) — guarda validas[] a *.txt, volviendo '4'→'X'.
        if (_validas.Count == 0)
        {
            AppServices.MostrarInfo("No hay columnas admitidas que grabar. Ejecute primero Calcular.");
            return;
        }
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Resultados",
        };
        picker.FileTypeChoices.Add("Resultados", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        try
        {
            using var wr = new StreamWriter(file.Path);
            foreach (var v in _validas) wr.WriteLine(v.Replace('4', 'X'));
            FicheroResultado = Path.GetFileName(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido grabar el resultado: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Legacy: FiltroPorcenJB.BCancelarClick -> salida=true (aborta el bucle de Calcular).
        _salida = true;
    }

    [RelayCommand]
    private async Task SalvarRangosAsync()
    {
        // Legacy: FiltroPorcenJB.SalvarConds() — persiste 7 rangos + recorrido + 6 límites a *.jb7.
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "RangosJB",
        };
        picker.FileTypeChoices.Add("Condiciones", new List<string> { ".jb7" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        var inv = CultureInfo.InvariantCulture;
        try
        {
            using var sw = new StreamWriter(file.Path);
            sw.WriteLine(Rango1);
            sw.WriteLine(Rango2);
            sw.WriteLine(Rango3);
            sw.WriteLine(Rango4);
            sw.WriteLine(Rango5);
            sw.WriteLine(Rango6);
            sw.WriteLine(Rango7);
            sw.WriteLine(Recorrido);
            sw.WriteLine(string.Join(',', new[]
            {
                ((int)Limite1).ToString(inv), ((int)Limite2).ToString(inv), ((int)Limite3).ToString(inv),
                ((int)Limite4).ToString(inv), ((int)Limite5).ToString(inv), ((int)Limite6).ToString(inv),
            }));
            FicheroRangos = Path.GetFileName(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se han podido guardar los rangos: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task LeerRangosAsync()
    {
        // Legacy: FiltroPorcenJB.LeerConds() — carga 7 rangos + recorrido + 6 límites desde *.jb7.
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".jb7");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            string[] lineas = await File.ReadAllLinesAsync(file.Path);
            if (lineas.Length >= 1) Rango1 = lineas[0];
            if (lineas.Length >= 2) Rango2 = lineas[1];
            if (lineas.Length >= 3) Rango3 = lineas[2];
            if (lineas.Length >= 4) Rango4 = lineas[3];
            if (lineas.Length >= 5) Rango5 = lineas[4];
            if (lineas.Length >= 6) Rango6 = lineas[5];
            if (lineas.Length >= 7) Rango7 = lineas[6];
            if (lineas.Length >= 8) Recorrido = lineas[7];
            if (lineas.Length >= 9)
            {
                string[] lims = lineas[8].Split(',');
                if (lims.Length >= 1) Limite1 = ADouble(lims[0]);
                if (lims.Length >= 2) Limite2 = ADouble(lims[1]);
                if (lims.Length >= 3) Limite3 = ADouble(lims[2]);
                if (lims.Length >= 4) Limite4 = ADouble(lims[3]);
                if (lims.Length >= 5) Limite5 = ADouble(lims[4]);
                if (lims.Length >= 6) Limite6 = ADouble(lims[5]);
            }
            FicheroRangos = Path.GetFileName(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se han podido leer los rangos: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Analizar()
    {
        // Legacy: FiltroPorcenJB.Analizar() (línea 536) — cuenta coincidencias de ColumnaGanadora
        // por banda contra scps[], reportando recorrido = max - min.
        if (!RecuperaPantalla())
        {
            AppServices.MostrarError("error en datos de entrada");
            return;
        }
        string columna = ColumnaGanadora.Replace('x', '4').Replace('X', '4');
        if (columna.Length < 14)
        {
            AppServices.MostrarError("col.G. errónea=" + columna);
            return;
        }

        int min7 = 98, max7 = 0;
        var minmax = new int[7];
        for (int nr = 0; nr < 7; nr++)
        {
            int na = 0;
            string banda = _scps[nr];
            for (int nr2 = 0; nr2 < 14; nr2++)
            {
                char chp = banda[nr2];
                if (chp == 48) continue;       // '0'
                int ch3 = chp & columna[nr2];
                if (ch3 > 48) na++;
            }
            minmax[nr] = na;
            if (na > max7) max7 = na;
            if (na < min7) min7 = na;
        }

        AnalisisBanda1 = minmax[0].ToString();
        AnalisisBanda2 = minmax[1].ToString();
        AnalisisBanda3 = minmax[2].ToString();
        AnalisisBanda4 = minmax[3].ToString();
        AnalisisBanda5 = minmax[4].ToString();
        AnalisisBanda6 = minmax[5].ToString();
        AnalisisBanda7 = minmax[6].ToString();
        AnalisisRecorrido = (max7 - min7).ToString();
    }

    [RelayCommand]
    private async Task ExportarAsync()
    {
        // Legacy: FiltroPorcenJB.ExporCols() (línea 602) — exporta la rejilla de signos por banda
        // (cps -> Cambia) a CSV: 7 líneas (bandas) x 14 columnas (partidos).
        if (!RecuperaPantalla())
        {
            AppServices.MostrarError("error en datos de entrada");
            return;
        }
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "SignosPorBanda",
        };
        picker.FileTypeChoices.Add("Salida", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        try
        {
            using var sw = new StreamWriter(file.Path);
            for (int nr = 0; nr < 7; nr++)
            {
                string linea = Cambia(_cps[0, nr]);
                for (int np = 1; np < 14; np++) linea += "," + Cambia(_cps[np, nr]);
                sw.WriteLine(linea);
            }
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido exportar: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task CargarGanadorasAsync()
    {
        // Legacy: FiltroPorcenJB.EntraCGsR() — carga el fichero de ganadoras y selecciona la última.
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        try
        {
            _colgsR.Clear();
            using (var sr = new StreamReader(file.Path))
            {
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string tmp = linea.Trim().ToUpper();
                    if (tmp.Length < 14)
                    {
                        AppServices.MostrarError("col.G. errónea=" + tmp);
                        return;
                    }
                    _colgsR.Add(tmp);
                }
            }
            if (_colgsR.Count == 0)
            {
                AppServices.MostrarError("El fichero de ganadoras está vacío.");
                return;
            }
            _nrfCGR = _colgsR.Count;
            FicheroGanadoras = Path.GetFileName(file.Path);
            ContadorGanadoras = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido leer el fichero de ganadoras: " + ex.Message);
        }
    }

    [RelayCommand]
    private void GanadoraSiguiente()
    {
        // Legacy: FiltroPorcenJB.GRMas() — avanza a la siguiente columna ganadora.
        if (_nrfCGR < _colgsR.Count)
        {
            _nrfCGR++;
            ContadorGanadoras = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    [RelayCommand]
    private void GanadoraAnterior()
    {
        // Legacy: FiltroPorcenJB.GRMenos() — retrocede a la columna ganadora anterior.
        if (_nrfCGR > 1)
        {
            _nrfCGR--;
            ContadorGanadoras = _nrfCGR.ToString();
            ColumnaGanadora = _colgsR[_nrfCGR - 1];
        }
    }

    private static double ADouble(string? s) => double.TryParse(s, out double v) ? v : 0;

    // --- Lógica de dominio reproducida de FiltroPorcenJB.cs (sin tocar la lógica original). ---

    // Legacy: RecuperaPantalla() (línea 224). nvals = valors1.RetVals(); aquí AMatriz(Porcentajes).
    // Construye cps[14,7] clasificando cada porcentaje con valora() y deriva las cadenas scps[7].
    private bool RecuperaPantalla()
    {
        if (!RecuperaRangos()) return false;
        double[,] nvals = PorcentajesHelper.AMatriz(Porcentajes);
        for (int nr = 0; nr < 14; nr++)
            for (int nc = 0; nc < 7; nc++) _cps[nr, nc] = 0;
        for (int nr = 0; nr < 14; nr++)
            for (int nc = 0; nc < 3; nc++)
            {
                int val = (int)nvals[nr, nc];
                int[] rsl = Valora(val, nc);
                _cps[nr, rsl[0]] += rsl[1];
            }
        for (int nc = 0; nc < 7; nc++)
        {
            string tmp = "";
            for (int np = 0; np < 14; np++) tmp += _cps[np, nc];
            _scps[nc] = tmp;
        }
        return true;
    }

    // Legacy: RecuperaRangos() (línea 239). Parsea rangos "min-max" y los 6 límites; valida que
    // los límites sean crecientes.
    private bool RecuperaRangos()
    {
        try
        {
            ParseaRango(Rango1, 0);
            ParseaRango(Rango2, 1);
            ParseaRango(Rango3, 2);
            ParseaRango(Rango4, 3);
            ParseaRango(Rango5, 4);
            ParseaRango(Rango6, 5);
            ParseaRango(Rango7, 6);
            ParseaRango(Recorrido, 7);
        }
        catch
        {
            return false;
        }
        _lims[0] = (int)Limite1;
        _lims[1] = (int)Limite2;
        _lims[2] = (int)Limite3;
        _lims[3] = (int)Limite4;
        _lims[4] = (int)Limite5;
        _lims[5] = (int)Limite6;
        for (int nr = 1; nr < 6; nr++) if (_lims[nr] < _lims[nr - 1]) return false;
        return true;
    }

    private void ParseaRango(string texto, int fila)
    {
        string[] aux = texto.Split('-');
        _rks[fila, 0] = Convert.ToInt32(aux[0]);
        _rks[fila, 1] = Convert.ToInt32(aux[1]);
    }

    // Legacy: valora() (línea 265). Asigna banda según los límites y aporta el bit del signo
    // (1→1, 2→2, X→4) en esa banda.
    private int[] Valora(int percent, int ncol)
    {
        int ind;
        if (percent <= _lims[0]) ind = 0;
        else if (percent <= _lims[1]) ind = 1;
        else if (percent <= _lims[2]) ind = 2;
        else if (percent <= _lims[3]) ind = 3;
        else if (percent <= _lims[4]) ind = 4;
        else if (percent <= _lims[5]) ind = 5;
        else ind = 6;
        int val = (ncol == 0) ? 1 : ((ncol == 2) ? 2 : 4);
        return new[] { ind, val };
    }

    // Legacy: Cambia() (línea 386). Convierte el código de signos de una banda a su texto 1/X/2.
    private static string Cambia(int valor) => valor switch
    {
        0 => "-",
        1 => "1",
        2 => "2",
        3 => "12",
        4 => "X",
        5 => "1X",
        6 => "X2",
        _ => "1X2",
    };

    // Legacy: Valida() (línea 465). Acepta la columna si los aciertos por banda caen en sus rangos
    // y el recorrido (max-min) cae en su rango.
    private bool Valida(string columna)
    {
        int max7 = 0, min7 = 98;
        for (int nr = 0; nr < 7; nr++)
        {
            int na = 0;
            string banda = _scps[nr];
            for (int nr2 = 0; nr2 < 14; nr2++)
            {
                char chp = banda[nr2];
                if (chp == 48) continue;       // '0'
                int ch3 = chp & columna[nr2];
                if (ch3 > 48) na++;
            }
            if (na < _rks[nr, 0] || na > _rks[nr, 1]) return false;
            if (na > max7) max7 = na;
            if (na < min7) min7 = na;
        }
        int rec = max7 - min7;
        if (rec < _rks[7, 0] || rec > _rks[7, 1]) return false;
        return true;
    }

    // Legacy: s2n() (línea 629). Índice ternario de la columna para el control de repetidos.
    private static int S2n(string ax)
    {
        int nx = 0;
        for (int nr = 0; nr < 14; nr++)
        {
            nx *= 3;
            if (ax[nr] == '1') nx += 1;
            else if (ax[nr] == '2') nx += 2;
        }
        return nx;
    }
}
