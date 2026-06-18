// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada del WinForms "Filtro Pim" (clase legacy
    /// Free1X2.UI.aidomnou, en Free1X2/UI/Aidomnou.cs). Genera/reduce columnas probables a
    /// partir de los valores (1/X/2) de los 14 partidos, aplica rangos de aciertos por
    /// "ranking" y un rango de recorrido (max-min).
    ///
    /// La rejilla <see cref="Partidos"/> sustituye al UserControl legacy "valors" (valors1):
    /// se lee desde ella en RecuperaPantalla en vez de valors1.RetVals(). Los algoritmos
    /// (PreCol1..6, Valida, s2n/n2s, Cambia) se transcriben literalmente del legacy. La carga
    /// del fichero de configuración aidomnou.cfg (GetConfig/SetConfig) no se porta: se usan los
    /// límites por defecto del legacy.
    /// </summary>
    public partial class GeneraPimViewModel : ObservableObject
    {
        // ---- Rangos de aciertos por ranking (legacy: tbmg1..tbmg6, tbmgsuma; formato "min-max") ----
        [ObservableProperty]
        private string _rango1 = "3-6";

        [ObservableProperty]
        private string _rango2 = "2-6";

        [ObservableProperty]
        private string _rango3 = "2-6";

        [ObservableProperty]
        private string _rango4 = "2-6";

        [ObservableProperty]
        private string _rango5 = "2-6";

        [ObservableProperty]
        private string _rango6 = "0-1";

        [ObservableProperty]
        private string _rango7 = "0-1";

        // ---- Rango de recorrido máximo-mínimo (legacy: tbmgreco, "0-14") ----
        [ObservableProperty]
        private string _rangoRecorrido = "0-14";

        // ---- Columna ganadora actual para análisis (legacy: tbColR) ----
        [ObservableProperty]
        private string _columnaGanadora = "COL.GANADORA";

        // ---- Ficheros / etiquetas de estado (legacy: lrangos, lfile, lFGR) ----
        [ObservableProperty]
        private string _ficheroRangos = "Fichero";

        [ObservableProperty]
        private string _ficheroResultado = "Fichero";

        [ObservableProperty]
        private string _ficheroGanadoras = "Fichero Ganadoras";

        // ---- Contadores de salida (legacy: lColsIni, lColsAdm, lTime, lbCGR) ----
        // Reglas anti-crash: TextBlock.Text se bindea SOLO a string, nunca a int directo.
        [ObservableProperty]
        private string _columnasProcesadas = "-";

        [ObservableProperty]
        private string _columnasAdmitidas = "-";

        [ObservableProperty]
        private string _tiempo = "-";

        [ObservableProperty]
        private string _indiceColGanadora = "-";

        // ---- Resultado del análisis por ranking (legacy: lrk1..lrk6, lsuma + lreco) ----
        [ObservableProperty]
        private string _analisis1 = "-";

        [ObservableProperty]
        private string _analisis2 = "-";

        [ObservableProperty]
        private string _analisis3 = "-";

        [ObservableProperty]
        private string _analisis4 = "-";

        [ObservableProperty]
        private string _analisis5 = "-";

        [ObservableProperty]
        private string _analisis6 = "-";

        [ObservableProperty]
        private string _analisis7 = "-";

        [ObservableProperty]
        private string _analisisRecorrido = "-";

        /// <summary>
        /// Valores de probabilidad por partido (14 filas) y signo (1/X/2).
        /// Legacy: control Free1X2.UI.Controls.valors (valors1), matriz double[14,3].
        /// </summary>
        public ObservableCollection<PartidoValores> Partidos { get; } = new();

        // ===== Estado interno del motor (campos del form aidomnou) =====
        private readonly double[,] _nvals = new double[14, 3];
        // Límites por defecto del legacy (aidomnou.cfg no se porta; ver GetConfig/SetConfig).
        private readonly int[] _lims = { 145, 11, 9, 8, 65, 9, 6, 5 };
        private readonly int[,] _rks = new int[8, 2];
        private readonly string[] _scps = new string[6];
        private readonly int[,] _cps = new int[14, 6];
        private string[] _colgsR = new string[3000];
        private int _limcgsR;
        private int _nrfCGR;
        // BitArray de 3^14 columnas válidas (legacy: validas).
        private BitArray? _validas;
        private int _ctadm;

        public GeneraPimViewModel()
        {
            for (int i = 1; i <= 14; i++)
            {
                Partidos.Add(new PartidoValores { Numero = $"P{i:00}" });
            }
        }

        /// <summary>
        /// Legacy aidomnou.Calcular(): lee un fichero de columnas de entrada, valida cada una con
        /// los rangos por ranking y el recorrido, y marca las válidas en el BitArray.
        /// </summary>
        [RelayCommand]
        private async Task Calcular()
        {
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            // Recoge la pantalla y prepara las 6 columnas ANTES de elegir el fichero (igual que el legacy).
            RecuperaPantalla();
            PreparaColumnas();

            StorageFile? archivo = await picker.PickSingleFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            DateTime time0 = DateTime.Now;
            ColumnasProcesadas = "...";
            ColumnasAdmitidas = "...";
            Tiempo = "...";

            int ctini = 0;
            int ctadm = 0;
            bool errorLongitud = false;
            var validas = new BitArray(4782969);

            await Task.Run(() =>
            {
                using var sr = new StreamReader(ruta);
                while (sr.Peek() > 0)
                {
                    string tmp = sr.ReadLine()!.Trim();
                    ctini++;
                    if (tmp.Length < 14)
                    {
                        errorLongitud = true;
                        break;
                    }
                    tmp = tmp.Replace('x', '4');
                    tmp = tmp.Replace('X', '4');
                    if (Valida(tmp))
                    {
                        int idx = S2n(tmp, 14);
                        if (validas[idx] == false)
                        {
                            validas[idx] = true;
                            ctadm++;
                        }
                    }
                }
            });

            _validas = validas;
            _ctadm = ctadm;

            if (errorLongitud)
            {
                AppServices.MostrarError("error de longitud en una columna de entrada");
            }

            ColumnasProcesadas = ctini.ToString();
            ColumnasAdmitidas = ctadm.ToString();
            string t = (DateTime.Now - time0).ToString() + "0000000000";
            Tiempo = t.Substring(0, 10);
        }

        /// <summary>
        /// Legacy aidomnou.GrabaCols(): vuelca al fichero las columnas válidas (validas) traducidas
        /// con n2s, sustituyendo internamente '4' por 'X'.
        /// </summary>
        [RelayCommand]
        private async Task GrabarResultado()
        {
            if (_validas is null)
            {
                AppServices.MostrarError("No hay resultado calculado. Pulsa Calcular primero.");
                return;
            }

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = ".txt",
                SuggestedFileName = "resultado",
            };
            picker.FileTypeChoices.Add("Resultados", new System.Collections.Generic.List<string> { ".txt" });
            picker.FileTypeChoices.Add("Todos los archivos", new System.Collections.Generic.List<string> { "." });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSaveFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            BitArray validas = _validas;

            await Task.Run(() =>
            {
                using var wr = new StreamWriter(ruta);
                for (int nr = 0; nr < 4782969; nr++)
                {
                    if (validas[nr])
                    {
                        wr.WriteLine(N2s(nr, 14));
                    }
                }
            });

            FicheroResultado = Path.GetFileName(ruta);
        }

        /// <summary>
        /// Legacy aidomnou.SalvaCondis(): escribe los 7 rangos + recorrido, línea a línea, a un fichero.
        /// </summary>
        [RelayCommand]
        private async Task SalvarRangos()
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = ".cnd",
                SuggestedFileName = "rangos",
            };
            picker.FileTypeChoices.Add("Rangos", new System.Collections.Generic.List<string> { ".cnd" });
            picker.FileTypeChoices.Add("Todos los archivos", new System.Collections.Generic.List<string> { "." });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSaveFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            string[] lineas = { Rango1, Rango2, Rango3, Rango4, Rango5, Rango6, Rango7, RangoRecorrido };

            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                foreach (string l in lineas)
                {
                    sw.WriteLine(l);
                }
            });

            FicheroRangos = Path.GetFileName(ruta);
        }

        /// <summary>
        /// Legacy aidomnou.LeeCondis(): lee 8 líneas (7 rangos + recorrido) de un fichero a los campos.
        /// </summary>
        [RelayCommand]
        private async Task RecuperarRangos()
        {
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeFilter.Add(".cnd");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSingleFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            string[] lineas = await Task.Run(() => File.ReadAllLines(ruta));

            // Asigna en el mismo orden que el legacy (tbmg1..6, tbmgsuma, tbmgreco).
            if (lineas.Length > 0) Rango1 = lineas[0];
            if (lineas.Length > 1) Rango2 = lineas[1];
            if (lineas.Length > 2) Rango3 = lineas[2];
            if (lineas.Length > 3) Rango4 = lineas[3];
            if (lineas.Length > 4) Rango5 = lineas[4];
            if (lineas.Length > 5) Rango6 = lineas[5];
            if (lineas.Length > 6) Rango7 = lineas[6];
            if (lineas.Length > 7) RangoRecorrido = lineas[7];

            FicheroRangos = Path.GetFileName(ruta);
        }

        /// <summary>
        /// Legacy aidomnou.ExporCols(): exporta las 6 columnas probables (cps) traducidas con Cambia().
        /// </summary>
        [RelayCommand]
        private async Task ExportarColumnas()
        {
            // Recoge la pantalla y prepara las columnas (igual que Analizar/Calcular del legacy
            // hacen antes de usar cps).
            RecuperaPantalla();
            PreparaColumnas();

            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = ".txt",
                SuggestedFileName = "columnas",
            };
            picker.FileTypeChoices.Add("F.Salida", new System.Collections.Generic.List<string> { ".txt" });
            picker.FileTypeChoices.Add("Todos los archivos", new System.Collections.Generic.List<string> { "." });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSaveFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            int[,] cps = (int[,])_cps.Clone();

            await Task.Run(() =>
            {
                using var sw = new StreamWriter(ruta);
                for (int nr = 0; nr < 6; nr++)
                {
                    string tmp = Cambia(cps[0, nr]);
                    for (int np = 1; np < 14; np++)
                    {
                        tmp += "," + Cambia(cps[np, nr]);
                    }
                    sw.WriteLine(tmp);
                }
            });

            FicheroResultado = Path.GetFileName(ruta);
        }

        /// <summary>
        /// Legacy aidomnou.EntraCGsR(): carga un fichero de columnas ganadoras, validándolas con
        /// VerColumna, y fija la última como columna en pantalla.
        /// </summary>
        [RelayCommand]
        private async Task CargarGanadoras()
        {
            var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSingleFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            var colgsR = new string[3000];
            int limcgsR = 0;
            bool columnaErronea = false;

            await Task.Run(() =>
            {
                using var sr = new StreamReader(ruta);
                while (sr.Peek() > 0)
                {
                    string tmp = VerColumna(sr.ReadLine() ?? "");
                    if (tmp.Length == 0)
                    {
                        columnaErronea = true;
                        return;
                    }
                    colgsR[limcgsR] = tmp;
                    limcgsR++;
                }
            });

            if (columnaErronea)
            {
                AppServices.MostrarError("col.G. errónea");
                return;
            }

            _colgsR = colgsR;
            _limcgsR = limcgsR;
            _nrfCGR = limcgsR;
            FicheroGanadoras = Path.GetFileName(ruta);
            IndiceColGanadora = _nrfCGR.ToString();
            if (_nrfCGR > 0)
            {
                ColumnaGanadora = _colgsR[_nrfCGR - 1];
            }
        }

        /// <summary>Legacy aidomnou.GRMas(): avanza al siguiente fichero/columna ganadora.</summary>
        [RelayCommand]
        private void GanadoraSiguiente()
        {
            if (_nrfCGR < _limcgsR)
            {
                _nrfCGR++;
                IndiceColGanadora = _nrfCGR.ToString();
                ColumnaGanadora = _colgsR[_nrfCGR - 1];
            }
        }

        /// <summary>Legacy aidomnou.GRMenos(): retrocede al fichero/columna ganadora anterior.</summary>
        [RelayCommand]
        private void GanadoraAnterior()
        {
            if (_nrfCGR > 1)
            {
                _nrfCGR--;
                IndiceColGanadora = _nrfCGR.ToString();
                ColumnaGanadora = _colgsR[_nrfCGR - 1];
            }
        }

        /// <summary>
        /// Legacy aidomnou.Analizar(): recoge la pantalla, prepara las columnas y cuenta los
        /// aciertos por ranking de la columna ganadora, volcando a Analisis1..7 + recorrido.
        /// </summary>
        [RelayCommand]
        private void Analizar()
        {
            int na, min6, max6, sum6;
            int[] minmax = new int[6];
            string columna = (ColumnaGanadora ?? "").Replace('x', '4');
            columna = columna.Replace('X', '4');
            min6 = 99; max6 = sum6 = 0;

            RecuperaPantalla();
            PreparaColumnas();

            for (int nr = 0; nr < 6; nr++)
            {
                na = 0;
                string tmp = _scps[nr];
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    char chp = tmp[nr2];
                    if (chp == 48) continue;
                    int ch3 = chp & columna[nr2];
                    if (ch3 > 48) na++;
                }
                minmax[nr] = na;
                if (na < min6) min6 = na;
                if (na > max6) max6 = na;
                sum6 += na;
            }

            Analisis1 = minmax[0].ToString();
            Analisis2 = minmax[1].ToString();
            Analisis3 = minmax[2].ToString();
            Analisis4 = minmax[3].ToString();
            Analisis5 = minmax[4].ToString();
            Analisis6 = minmax[5].ToString();
            Analisis7 = sum6.ToString();
            AnalisisRecorrido = (max6 - min6).ToString();
        }

        // ===== Métodos del motor transcritos literalmente de aidomnou =====

        /// <summary>
        /// Legacy RecuperaPantalla(): vuelca los valores de la rejilla (sustituto de valors1.RetVals())
        /// a _nvals y parsea los 8 rangos (formato "min-max") a _rks.
        /// </summary>
        private void RecuperaPantalla()
        {
            for (int i = 0; i < 14 && i < Partidos.Count; i++)
            {
                _nvals[i, 0] = Partidos[i].Valor1;
                _nvals[i, 1] = Partidos[i].ValorX;
                _nvals[i, 2] = Partidos[i].Valor2;
            }

            ParseRango(Rango1, 0);
            ParseRango(Rango2, 1);
            ParseRango(Rango3, 2);
            ParseRango(Rango4, 3);
            ParseRango(Rango5, 4);
            ParseRango(Rango6, 5);
            ParseRango(Rango7, 6);
            ParseRango(RangoRecorrido, 7);
        }

        private void ParseRango(string texto, int fila)
        {
            string[] aux = (texto ?? "").Split('-');
            _rks[fila, 0] = Convert.ToInt32(aux[0]);
            _rks[fila, 1] = Convert.ToInt32(aux[1]);
        }

        // Legacy PreparaColumnas()
        private void PreparaColumnas()
        {
            for (int nr = 0; nr < 14; nr++) for (int nc = 0; nc < 6; nc++) _cps[nr, nc] = 0;
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

        // Legacy PreCol1()
        private void PreCol1()
        {
            int nq1, nq2; double nv;
            double[,] pvals = new double[14, 3];
            for (int nr1 = 0; nr1 < 14; nr1++)
                for (int nr2 = 0; nr2 < 3; nr2++) pvals[nr1, nr2] = _nvals[nr1, nr2];
            for (int nt = 0; nt < 42; nt++)
            {
                nq1 = nq2 = 0; nv = 0;
                for (int nr1 = 0; nr1 < 14; nr1++)
                {
                    for (int nr2 = 0; nr2 < 3; nr2++)
                    {
                        if (pvals[nr1, nr2] >= nv)
                        {
                            nv = pvals[nr1, nr2]; nq1 = nr1; nq2 = nr2;
                        }
                    }
                }
                _cps[nq1, 0] += (nq2 == 0 ? 1 : nq2 == 1 ? 4 : 2);
                pvals[nq1, nq2] = (-1);
                nv = 0;
                for (int nr = 0; nr < 14; nr++)
                {
                    switch (_cps[nr, 0])
                    {
                        case 3:
                        case 5:
                        case 6: nv += 10; break;
                        case 7: nv += 15; break;
                    }
                }
                if (nv >= _lims[0]) break;
            }
        }

        // Legacy PreCol4()
        private void PreCol4()
        {
            int nq1, nq2; double nv;
            double[,] pvals = new double[14, 3];
            for (int nr1 = 0; nr1 < 14; nr1++)
                for (int nr2 = 0; nr2 < 3; nr2++) pvals[nr1, nr2] = _nvals[nr1, nr2];
            while (true)
            {
                nq1 = nq2 = 0; nv = 0;
                for (int nr1 = 0; nr1 < 14; nr1++)
                {
                    for (int nr2 = 0; nr2 < 3; nr2++)
                    {
                        if (pvals[nr1, nr2] >= nv)
                        {
                            nv = pvals[nr1, nr2]; nq1 = nr1; nq2 = nr2;
                        }
                    }
                }
                _cps[nq1, 3] += (nq2 == 0 ? 1 : nq2 == 1 ? 4 : 2);
                pvals[nq1, nq2] = (-1);
                nv = 0;
                for (int nr = 0; nr < 14; nr++)
                {
                    switch (_cps[nr, 3])
                    {
                        case 3:
                        case 5:
                        case 6: nv += 10; break;
                        case 7: nv += 15; break;
                    }
                }
                if (nv >= _lims[4]) break;
            }
        }

        // Legacy PreCol2()
        private void PreCol2()
        {
            int nq; double nv;
            for (int nr1 = 0; nr1 < _lims[1]; nr1++)
            {
                nq = 0; nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    if (_cps[nr2, 1] < 1 && _nvals[nr2, 0] >= nv)
                    {
                        nv = _nvals[nr2, 0]; nq = nr2;
                    }
                }
                _cps[nq, 1] += 1;
            }
            for (int nr1 = 0; nr1 < _lims[3]; nr1++)
            {
                nq = 0; nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    if (_cps[nr2, 1] < 2 && _nvals[nr2, 2] > nv)
                    {
                        nv = _nvals[nr2, 2]; nq = nr2;
                    }
                }
                _cps[nq, 1] += 2;
            }
            for (int nr1 = 0; nr1 < _lims[2]; nr1++)
            {
                nq = 0; nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    if (_cps[nr2, 1] < 4 && _nvals[nr2, 1] > nv)
                    {
                        nv = _nvals[nr2, 1]; nq = nr2;
                    }
                }
                _cps[nq, 1] += 4;
            }
        }

        // Legacy PreCol5()
        private void PreCol5()
        {
            int nq; double nv;
            for (int nr1 = 0; nr1 < _lims[5]; nr1++)
            {
                nq = 0; nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    if (_cps[nr2, 4] < 1 && _nvals[nr2, 0] >= nv)
                    {
                        nv = _nvals[nr2, 0]; nq = nr2;
                    }
                }
                _cps[nq, 4] += 1;
            }
            for (int nr1 = 0; nr1 < _lims[7]; nr1++)
            {
                nq = 0; nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    if (_cps[nr2, 4] < 2 && _nvals[nr2, 2] > nv)
                    {
                        nv = _nvals[nr2, 2]; nq = nr2;
                    }
                }
                _cps[nq, 4] += 2;
            }
            for (int nr1 = 0; nr1 < _lims[6]; nr1++)
            {
                nq = 0; nv = 0;
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    if (_cps[nr2, 4] < 4 && _nvals[nr2, 1] > nv)
                    {
                        nv = _nvals[nr2, 1]; nq = nr2;
                    }
                }
                _cps[nq, 4] += 4;
            }
        }

        // Legacy PreCol6() — 14 fijos
        private void PreCol6()
        {
            for (int nr1 = 0; nr1 < 14; nr1++)
            {
                int nq = 0; double nv = 0;
                for (int nr2 = 0; nr2 < 3; nr2++)
                {
                    if (_nvals[nr1, nr2] > nv) { nv = _nvals[nr1, nr2]; nq = nr2; }
                }
                _cps[nr1, 5] = (nq == 0 ? 1 : (nq == 1 ? 4 : 2));
            }
        }

        // Legacy PreCol3() — 14 dobles
        private void PreCol3()
        {
            for (int nr1 = 0; nr1 < 14; nr1++)
            {
                int nq = 0; double nv = 100;
                for (int nr2 = 0; nr2 < 3; nr2++)
                {
                    if (_nvals[nr1, nr2] < nv) { nv = _nvals[nr1, nr2]; nq = nr2; }
                }
                _cps[nr1, 2] = (nq == 0 ? 6 : (nq == 1 ? 3 : 5));
            }
        }

        // Legacy Cambia()
        private static string Cambia(int valor)
        {
            string rsl;
            if (valor == 0) rsl = "-";
            else if (valor == 1) rsl = "1";
            else if (valor == 2) rsl = "2";
            else if (valor == 3) rsl = "12";
            else if (valor == 4) rsl = "X";
            else if (valor == 5) rsl = "1X";
            else if (valor == 6) rsl = "X2";
            else rsl = "1X2";
            return rsl;
        }

        // Legacy Valida()
        private bool Valida(string columna)
        {
            int na, sum6, min6, max6, reco6; string temp;
            sum6 = max6 = 0; min6 = 84;
            for (int nr = 0; nr < 6; nr++)
            {
                na = 0;
                temp = _scps[nr];
                for (int nr2 = 0; nr2 < 14; nr2++)
                {
                    char chp = temp[nr2];
                    if (chp == 48) continue;
                    int ch3 = chp & columna[nr2];
                    if (ch3 > 48) na++;
                }
                if (na < _rks[nr, 0] || na > _rks[nr, 1]) return false;
                sum6 += na; if (na < min6) min6 = na; if (na > max6) max6 = na;
            }
            if (sum6 < _rks[6, 0] || sum6 > _rks[6, 1]) return false;
            reco6 = max6 - min6;
            if (reco6 < _rks[7, 0] || reco6 > _rks[7, 1]) return false;
            return true;
        }

        // Legacy VerColumna()
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

        // Legacy s2n()
        private static int S2n(string ax, int lim)
        {
            int nx = 0;
            for (int nr = 0; nr < lim; nr++)
            {
                nx *= 3;
                string ch = ax.Substring(nr, 1);
                switch (ch)
                {
                    case "1":
                        nx += 1;
                        break;
                    case "2":
                        nx += 2;
                        break;
                }
            }
            return nx;
        }

        // Legacy n2s()
        private static string N2s(int nx, int lim)
        {
            string ax = "";
            for (int nr = 0; nr < lim; nr++)
            {
                int nx2 = nx % 3; nx /= 3;
                switch (nx2)
                {
                    case 1:
                        ax = "1" + ax;
                        break;
                    case 2:
                        ax = "2" + ax;
                        break;
                    default:
                        ax = "X" + ax;
                        break;
                }
            }
            return ax;
        }
    }

    /// <summary>
    /// Fila de la rejilla de valores: probabilidades 1 / X / 2 de un partido.
    /// Legacy: control valors (matriz double[14,3]).
    /// </summary>
    public partial class PartidoValores : ObservableObject
    {
        [ObservableProperty]
        private string _numero = "";

        // NumberBox.Value es double -> las propiedades de entrada son double (regla anti-crash 7).
        [ObservableProperty]
        private double _valor1;

        [ObservableProperty]
        private double _valorX;

        [ObservableProperty]
        private double _valor2;
    }
}
