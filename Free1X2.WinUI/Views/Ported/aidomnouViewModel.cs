using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Una fila de la rejilla de "Columnas Probables" (legacy: l0PR, P=partido 1..14, R=columna 1..6).
    /// <see cref="Partido"/> es la cabecera de fila (nº de partido) y <see cref="Signos"/> son las 6
    /// celdas de solo lectura con el signo generado (-/1/2/12/X/1X/X2/1X2) por cada columna.
    /// </summary>
    public sealed class FilaColumnasProbables
    {
        public string Partido { get; init; } = string.Empty;
        public IReadOnlyList<string> Signos { get; init; } = System.Array.Empty<string>();
    }

    /// <summary>
    /// ViewModel para la Page portada de "aidomnou" (WinForms legacy: Free1X2.UI.aidomnou).
    ///
    /// Propósito legacy: filtro "Filtro Aidomnou". A partir de las valoraciones de los 14
    /// partidos (control 'valors', double[14,3] = valor por signo 1/X/2), genera 6 columnas
    /// probables, cada una con una estrategia distinta, y valida un fichero de columnas contra
    /// los límites min-max de aciertos por columna, la suma y el recorrido (rks[8,2]).
    ///
    /// CABLEADO REAL: la persistencia de límites (*.cnd) y la navegación de columnas ganadoras
    /// se cablean al sistema de ficheros con pickers de WinUI.
    ///
    /// CABLEADO DE MATRIZ: la rejilla de valoraciones (legacy UserControl 'valors') se sustituye
    /// por PorcentajesControl; PorcentajesHelper.AMatriz(Valoraciones) equivale a valors1.RetVals()
    /// (double[14,3]). La generación de las 6 columnas (PreparaColumnas / PreCol1..6), Calcular,
    /// Analizar y ExporCols se reproducen aquí desde Aidomnou.cs (RecuperaPantalla 255, Calcular 559,
    /// Valida 602, Analizar 622, ExporCols 702).
    /// </summary>
    public partial class aidomnouViewModel : ObservableObject
    {
        // Rejilla de valoraciones por partido (1/X/2). Sustituye al UserControl 'valors';
        // PorcentajesHelper.AMatriz(Valoraciones) equivale a valors1.RetVals() (double[14,3]).
        public ObservableCollection<FilaPorcentaje> Valoraciones { get; } =
            PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

        // Columnas ganadoras cargadas (legacy: string[3000] colgsR) y navegación (limcgsR / nrfCGR).
        private readonly List<string> _colgsR = new();
        private int _nrfCGR;

        private bool _salida;

        // 6 columnas generadas, una por línea (legacy: PintaPantalla rellena cps[14,6] -> Cambia()).
        // ItemsSource del ListView de resultado.
        public ObservableCollection<string> ColumnasGeneradas { get; } = new();

        // Rejilla 14x6 de "Columnas Probables" (legacy: labels l0PR, fila=partido, columna=1..6).
        // Se rellena en PintaPantalla con Cambia(cps[partido, columna]) para mostrarla de solo lectura.
        public ObservableCollection<FilaColumnasProbables> ColumnasProbables { get; } = new();

        // --- Estado del motor reproducido del legacy. ---
        // lims por defecto del WinForms (Aidomnou.cs línea 168), coincide con aidomnou.cfg.
        private readonly int[] _lims = { 145, 11, 9, 8, 65, 9, 6, 5 };
        private readonly int[,] _rks = new int[8, 2];
        private readonly int[,] _cps = new int[14, 6];
        private readonly string[] _scps = new string[6];
        private double[,] _nvals = new double[14, 3];

        // Columnas admitidas tras el último Calcular() (legacy: BitArray validas, índices s2n).
        private readonly SortedSet<int> _validas = new();

        // ---------------- Límites de aciertos (rango "min-max") ----------------
        // Legacy: tbmg1..tbmg6 (por columna), tbmgsuma, tbmgreco. Strings "a-b".

        [ObservableProperty]
        private string _limColumna1 = "0-14";

        [ObservableProperty]
        private string _limColumna2 = "0-14";

        [ObservableProperty]
        private string _limColumna3 = "0-14";

        [ObservableProperty]
        private string _limColumna4 = "0-14";

        [ObservableProperty]
        private string _limColumna5 = "0-14";

        [ObservableProperty]
        private string _limColumna6 = "0-14";

        [ObservableProperty]
        private string _limSuma = "0-84";

        [ObservableProperty]
        private string _limRecorrido = "0-14";

        // Fichero de límites actual (legacy: lrangos).
        [ObservableProperty]
        private string _ficheroRangos = "Fichero";

        // ---------------- Salida del cálculo ----------------
        // Strings para no bindear int a TextBlock.Text (regla anti-crash 2).

        // Legacy: lColsIni (columnas procesadas del fichero de entrada).
        [ObservableProperty]
        private string _colsProcesadas = "-";

        // Legacy: lColsAdm (columnas admitidas/válidas).
        [ObservableProperty]
        private string _colsValidas = "-";

        // Legacy: lTime (tiempo de proceso).
        [ObservableProperty]
        private string _tiempo = "-";

        // Legacy: lfile (fichero de resultado grabado).
        [ObservableProperty]
        private string _ficheroResultado = "Fichero";

        // ---------------- Análisis de resultados ----------------

        // Columna a analizar (14 signos). Legacy: tbColR.
        [ObservableProperty]
        private string _columnaResultado = "2222XXXX222XXX";

        // Fichero de columnas ganadoras (legacy: lFGR).
        [ObservableProperty]
        private string _ficheroGanadoras = "Fichero Ganadoras";

        // Posición dentro del fichero de ganadoras (legacy: lbCGR "n"). String.
        [ObservableProperty]
        private string _paginacionGanadoras = "-";

        // Resultado del análisis por columna (legacy: lrk1..lrk6). Strings.
        [ObservableProperty]
        private string _rk1 = "-";

        [ObservableProperty]
        private string _rk2 = "-";

        [ObservableProperty]
        private string _rk3 = "-";

        [ObservableProperty]
        private string _rk4 = "-";

        [ObservableProperty]
        private string _rk5 = "-";

        [ObservableProperty]
        private string _rk6 = "-";

        // Legacy: lsuma (suma de aciertos de las 6 columnas).
        [ObservableProperty]
        private string _sumaAnalisis = "-";

        // Legacy: lreco (recorrido = max - min).
        [ObservableProperty]
        private string _recorridoAnalisis = "-";

        // ================= Comandos =================

        // Legacy: bCalcular -> Calcular() (línea 559). Recupera pantalla, prepara las 6 columnas,
        // abre el fichero de entrada y valida cada columna contra los límites.
        [RelayCommand]
        private async Task CalcularAsync()
        {
            _salida = false;
            ColsProcesadas = " ";
            ColsValidas = " ";
            Tiempo = " ";
            _validas.Clear();

            if (!RecuperaPantalla())
            {
                AppServices.MostrarError("error en datos de entrada");
                return;
            }
            PreparaColumnas();
            PintaPantalla();

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

            int ctini = 0, ctadm = 0;
            var time0 = DateTime.Now;
            try
            {
                using var sr = new StreamReader(file.Path);
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    if (_salida) break;
                    string col = linea.Trim();
                    if (col.Length == 0) continue;
                    ctini++;
                    if (col.Length < 14)
                    {
                        AppServices.MostrarError("error de longitud=" + col);
                        break;
                    }
                    col = col.Replace('x', '4').Replace('X', '4');
                    if (Valida(col))
                    {
                        int idx = S2n(col, 14);
                        if (_validas.Add(idx)) ctadm++;
                    }
                }
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se ha podido leer el fichero de entrada: " + ex.Message);
                return;
            }

            ColsProcesadas = ctini.ToString();
            ColsValidas = ctadm.ToString();
            Tiempo = (DateTime.Now - time0).ToString();
        }

        // Legacy: bGrabaCols -> GrabaCols() (línea 526). Vuelca las columnas válidas (índices s2n)
        // a un fichero de texto convirtiéndolas con n2s.
        [RelayCommand]
        private async Task GrabarResultadoAsync()
        {
            if (_validas.Count == 0)
            {
                AppServices.MostrarInfo("No hay columnas válidas que grabar. Ejecute primero Calcular.");
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
                foreach (int idx in _validas) wr.WriteLine(N2s(idx, 14));
                FicheroResultado = Path.GetFileName(file.Path);
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se ha podido grabar el resultado: " + ex.Message);
            }
        }

        // Legacy: bExporCols -> ExporCols() (línea 702). Exporta las 6 columnas generadas (cps) a CSV.
        [RelayCommand]
        private async Task ExportarColumnasAsync()
        {
            if (!RecuperaPantalla())
            {
                AppServices.MostrarError("error en datos de entrada");
                return;
            }
            PreparaColumnas();
            PintaPantalla();

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "ColumnasProbables",
            };
            picker.FileTypeChoices.Add("Salida", new List<string> { ".txt" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;
            try
            {
                using var sw = new StreamWriter(file.Path);
                for (int nr = 0; nr < 6; nr++)
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

        // Legacy: bCancelar -> salida = true. Aborta el bucle de cálculo en curso.
        [RelayCommand]
        private void Cancelar()
        {
            // Legacy: aidomnou.BCancelarClick -> salida = true.
            _salida = true;
        }

        // Legacy: bSalvaCondis -> SalvaCondis(). Guarda los 8 rangos en un fichero .cnd.
        [RelayCommand]
        private async Task SalvarLimitesAsync()
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "Aidomnou",
            };
            picker.FileTypeChoices.Add("Rangos", new List<string> { ".cnd" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            try
            {
                using var sw = new StreamWriter(file.Path);
                sw.WriteLine(LimColumna1);
                sw.WriteLine(LimColumna2);
                sw.WriteLine(LimColumna3);
                sw.WriteLine(LimColumna4);
                sw.WriteLine(LimColumna5);
                sw.WriteLine(LimColumna6);
                sw.WriteLine(LimSuma);
                sw.WriteLine(LimRecorrido);
                FicheroRangos = Path.GetFileName(file.Path);
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se han podido guardar los límites: " + ex.Message);
            }
        }

        // Legacy: bLeeCondis -> LeeCondis(). Carga los 8 rangos desde un fichero .cnd.
        [RelayCommand]
        private async Task RecuperarLimitesAsync()
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".cnd");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSingleFileAsync();
            if (file == null) return;

            try
            {
                string[] lineas = await File.ReadAllLinesAsync(file.Path);
                if (lineas.Length >= 1) LimColumna1 = lineas[0];
                if (lineas.Length >= 2) LimColumna2 = lineas[1];
                if (lineas.Length >= 3) LimColumna3 = lineas[2];
                if (lineas.Length >= 4) LimColumna4 = lineas[3];
                if (lineas.Length >= 5) LimColumna5 = lineas[4];
                if (lineas.Length >= 6) LimColumna6 = lineas[5];
                if (lineas.Length >= 7) LimSuma = lineas[6];
                if (lineas.Length >= 8) LimRecorrido = lineas[7];
                FicheroRangos = Path.GetFileName(file.Path);
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se han podido leer los límites: " + ex.Message);
            }
        }

        // Legacy: bFG -> EntraCGsR(). Abre el fichero de columnas ganadoras y carga colgsR.
        [RelayCommand]
        private async Task AbrirGanadorasAsync()
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

            try
            {
                _colgsR.Clear();
                using (var sr = new StreamReader(file.Path))
                {
                    string? linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        string tmp = VerColumna(linea);
                        if (tmp.Length == 0)
                        {
                            AppServices.MostrarError("col.G. errónea");
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
                PaginacionGanadoras = _nrfCGR.ToString();
                ColumnaResultado = _colgsR[_nrfCGR - 1];
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se ha podido leer el fichero de ganadoras: " + ex.Message);
            }
        }

        // Legacy: bMenosR -> GRMenos(). Retrocede en el fichero de ganadoras.
        [RelayCommand]
        private void GanadoraAnterior()
        {
            if (_nrfCGR > 1)
            {
                _nrfCGR--;
                PaginacionGanadoras = _nrfCGR.ToString();
                ColumnaResultado = _colgsR[_nrfCGR - 1];
            }
        }

        // Legacy: bMasR -> GRMas(). Avanza en el fichero de ganadoras.
        [RelayCommand]
        private void GanadoraSiguiente()
        {
            if (_nrfCGR < _colgsR.Count)
            {
                _nrfCGR++;
                PaginacionGanadoras = _nrfCGR.ToString();
                ColumnaResultado = _colgsR[_nrfCGR - 1];
            }
        }

        // Legacy: bAnalizar -> Analizar() (línea 622). Cuenta aciertos de la columna contra las 6
        // generadas (scps), reportando suma y recorrido = max - min.
        [RelayCommand]
        private void Analizar()
        {
            if (!RecuperaPantalla())
            {
                AppServices.MostrarError("error en datos de entrada");
                return;
            }
            PreparaColumnas();
            PintaPantalla();

            string columna = ColumnaResultado.Replace('x', '4').Replace('X', '4');
            if (columna.Length < 14)
            {
                AppServices.MostrarError("col. errónea=" + columna);
                return;
            }

            int min6 = 99, max6 = 0, sum6 = 0;
            var minmax = new int[6];
            for (int nr = 0; nr < 6; nr++)
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
                if (na < min6) min6 = na;
                if (na > max6) max6 = na;
                sum6 += na;
            }

            Rk1 = minmax[0].ToString();
            Rk2 = minmax[1].ToString();
            Rk3 = minmax[2].ToString();
            Rk4 = minmax[3].ToString();
            Rk5 = minmax[4].ToString();
            Rk6 = minmax[5].ToString();
            SumaAnalisis = sum6.ToString();
            RecorridoAnalisis = (max6 - min6).ToString();
        }

        // ===== Lógica de dominio reproducida de Aidomnou.cs (sin tocar la lógica original). =====

        // Legacy: RecuperaPantalla() (línea 255). nvals = valors1.RetVals(); aquí AMatriz(Valoraciones).
        // Parsea los 8 rangos "min-max" (6 columnas + suma + recorrido).
        private bool RecuperaPantalla()
        {
            _nvals = PorcentajesHelper.AMatriz(Valoraciones);
            try
            {
                ParseaRango(LimColumna1, 0);
                ParseaRango(LimColumna2, 1);
                ParseaRango(LimColumna3, 2);
                ParseaRango(LimColumna4, 3);
                ParseaRango(LimColumna5, 4);
                ParseaRango(LimColumna6, 5);
                ParseaRango(LimSuma, 6);
                ParseaRango(LimRecorrido, 7);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void ParseaRango(string texto, int fila)
        {
            string[] aux = texto.Split('-');
            _rks[fila, 0] = Convert.ToInt32(aux[0]);
            _rks[fila, 1] = Convert.ToInt32(aux[1]);
        }

        // Legacy: PreparaColumnas() (línea 267). Genera las 6 columnas en cps[14,6] y deriva scps[6].
        private void PreparaColumnas()
        {
            for (int nr = 0; nr < 14; nr++)
                for (int nc = 0; nc < 6; nc++) _cps[nr, nc] = 0;
            PreCol1(); PreCol2();
            PreCol3(); PreCol4();
            PreCol5(); PreCol6();
            for (int nr1 = 0; nr1 < 6; nr1++)
            {
                string tmp = "";
                for (int nr2 = 0; nr2 < 14; nr2++) tmp += _cps[nr2, nr1];
                _scps[nr1] = tmp;
            }
        }

        // Legacy: PreCol1() (línea 279). Columna-1: signos más valorados hasta llegar a lims[0] puntos.
        private void PreCol1()
        {
            var pvals = new double[14, 3];
            for (int nr1 = 0; nr1 < 14; nr1++)
                for (int nr2 = 0; nr2 < 3; nr2++) pvals[nr1, nr2] = _nvals[nr1, nr2];
            for (int nt = 0; nt < 42; nt++)
            {
                int nq1 = 0, nq2 = 0; double nv = 0;
                for (int nr1 = 0; nr1 < 14; nr1++)
                    for (int nr2 = 0; nr2 < 3; nr2++)
                        if (pvals[nr1, nr2] >= nv) { nv = pvals[nr1, nr2]; nq1 = nr1; nq2 = nr2; }
                _cps[nq1, 0] += (nq2 == 0 ? 1 : nq2 == 1 ? 4 : 2);
                pvals[nq1, nq2] = -1;
                nv = 0;
                for (int nr = 0; nr < 14; nr++)
                    switch (_cps[nr, 0]) { case 3: case 5: case 6: nv += 10; break; case 7: nv += 15; break; }
                if (nv >= _lims[0]) break;
            }
        }

        // Legacy: PreCol4() (línea 307). Columna-4: idéntica a Col1 pero con tope lims[4].
        private void PreCol4()
        {
            var pvals = new double[14, 3];
            for (int nr1 = 0; nr1 < 14; nr1++)
                for (int nr2 = 0; nr2 < 3; nr2++) pvals[nr1, nr2] = _nvals[nr1, nr2];
            while (true)
            {
                int nq1 = 0, nq2 = 0; double nv = 0;
                for (int nr1 = 0; nr1 < 14; nr1++)
                    for (int nr2 = 0; nr2 < 3; nr2++)
                        if (pvals[nr1, nr2] >= nv) { nv = pvals[nr1, nr2]; nq1 = nr1; nq2 = nr2; }
                _cps[nq1, 3] += (nq2 == 0 ? 1 : nq2 == 1 ? 4 : 2);
                pvals[nq1, nq2] = -1;
                nv = 0;
                for (int nr = 0; nr < 14; nr++)
                    switch (_cps[nr, 3]) { case 3: case 5: case 6: nv += 10; break; case 7: nv += 15; break; }
                if (nv >= _lims[4]) break;
            }
        }

        // Legacy: PreCol2() (línea 335). Columna-2: lims[1] unos, lims[3] doses, lims[2] equis.
        private void PreCol2()
        {
            for (int nr1 = 0; nr1 < _lims[1]; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                    if (_cps[nr2, 1] < 1 && _nvals[nr2, 0] >= nv) { nv = _nvals[nr2, 0]; nq = nr2; }
                _cps[nq, 1] += 1;
            }
            for (int nr1 = 0; nr1 < _lims[3]; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                    if (_cps[nr2, 1] < 2 && _nvals[nr2, 2] > nv) { nv = _nvals[nr2, 2]; nq = nr2; }
                _cps[nq, 1] += 2;
            }
            for (int nr1 = 0; nr1 < _lims[2]; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                    if (_cps[nr2, 1] < 4 && _nvals[nr2, 1] > nv) { nv = _nvals[nr2, 1]; nq = nr2; }
                _cps[nq, 1] += 4;
            }
        }

        // Legacy: PreCol5() (línea 365). Columna-5: lims[5] unos, lims[7] doses, lims[6] equis.
        private void PreCol5()
        {
            for (int nr1 = 0; nr1 < _lims[5]; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                    if (_cps[nr2, 4] < 1 && _nvals[nr2, 0] >= nv) { nv = _nvals[nr2, 0]; nq = nr2; }
                _cps[nq, 4] += 1;
            }
            for (int nr1 = 0; nr1 < _lims[7]; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                    if (_cps[nr2, 4] < 2 && _nvals[nr2, 2] > nv) { nv = _nvals[nr2, 2]; nq = nr2; }
                _cps[nq, 4] += 2;
            }
            for (int nr1 = 0; nr1 < _lims[6]; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                    if (_cps[nr2, 4] < 4 && _nvals[nr2, 1] > nv) { nv = _nvals[nr2, 1]; nq = nr2; }
                _cps[nq, 4] += 4;
            }
        }

        // Legacy: PreCol6() (línea 395). Columna-6: 14 fijos (signo más valorado por partido).
        private void PreCol6()
        {
            for (int nr1 = 0; nr1 < 14; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 3; nr2++)
                    if (_nvals[nr1, nr2] > nv) { nv = _nvals[nr1, nr2]; nq = nr2; }
                _cps[nr1, 5] = (nq == 0 ? 1 : (nq == 1 ? 4 : 2));
            }
        }

        // Legacy: PreCol3() (línea 404). Columna-3: 14 dobles (excluye el signo menos valorado).
        private void PreCol3()
        {
            for (int nr1 = 0; nr1 < 14; nr1++)
            {
                int nq = 0; double nv = 100;
                for (int nr2 = 0; nr2 < 3; nr2++)
                    if (_nvals[nr1, nr2] < nv) { nv = _nvals[nr1, nr2]; nq = nr2; }
                _cps[nr1, 2] = (nq == 0 ? 6 : (nq == 1 ? 3 : 5));
            }
        }

        // Legacy: PintaPantalla() (línea 413). Vuelca cps[14,6] -> 6 cadenas de signos (Cambia).
        private void PintaPantalla()
        {
            ColumnasGeneradas.Clear();
            for (int nc = 0; nc < 6; nc++)
            {
                string col = "";
                for (int nr = 0; nr < 14; nr++)
                {
                    if (nr > 0) col += " ";
                    col += Cambia(_cps[nr, nc]);
                }
                ColumnasGeneradas.Add($"Col {nc + 1}: {col}");
            }

            // Rejilla de solo lectura (legacy: labels l0PR). Fila = partido (1..14), columna = 1..6,
            // celda = Cambia(cps[partido, columna]).
            ColumnasProbables.Clear();
            for (int nr = 0; nr < 14; nr++)
            {
                var signos = new string[6];
                for (int nc = 0; nc < 6; nc++) signos[nc] = Cambia(_cps[nr, nc]);
                ColumnasProbables.Add(new FilaColumnasProbables
                {
                    Partido = (nr + 1).ToString("00"),
                    Signos = signos,
                });
            }
        }

        // Legacy: Cambia() (línea 514). Código de signos -> texto 1/X/2.
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

        // Legacy: Valida() (línea 602). Acepta la columna si los aciertos por columna caen en rango,
        // y la suma y el recorrido (max-min) caen en sus rangos.
        private bool Valida(string columna)
        {
            int sum6 = 0, max6 = 0, min6 = 84;
            for (int nr = 0; nr < 6; nr++)
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
                sum6 += na;
                if (na < min6) min6 = na;
                if (na > max6) max6 = na;
            }
            if (sum6 < _rks[6, 0] || sum6 > _rks[6, 1]) return false;
            int reco6 = max6 - min6;
            if (reco6 < _rks[7, 0] || reco6 > _rks[7, 1]) return false;
            return true;
        }

        // Legacy: s2n() (línea 758). Índice ternario de la columna.
        private static int S2n(string ax, int lim)
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

        // Legacy: n2s() (línea 775). Índice ternario -> columna de 14 signos (1/2/X).
        private static string N2s(int nx, int lim)
        {
            string ax = "";
            for (int nr = 0; nr < lim; nr++)
            {
                int nx2 = nx % 3; nx /= 3;
                ax = (nx2 == 1 ? "1" : nx2 == 2 ? "2" : "X") + ax;
            }
            return ax;
        }

        // ===== Utilidad replicada 1:1 del WinForms aidomnou =====

        /// <summary>Valida una columna ganadora de 14 signos. Legacy: aidomnou.VerColumna (657-667).</summary>
        private static string VerColumna(string columna)
        {
            const string chval = "12xX";
            string xcol = (columna ?? string.Empty).Trim();
            if (xcol.Length < 14) return "";
            xcol = xcol.Substring(0, 14);
            for (int nr = 0; nr < 14; nr++)
            {
                if (chval.IndexOf(xcol[nr]) < 0) return "";
            }
            return xcol;
        }
    }
}
