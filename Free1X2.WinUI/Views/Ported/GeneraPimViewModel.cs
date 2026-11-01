// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada del WinForms "Filtro Pim" (clase legacy GeneraPim, en
    /// Free1X2/UI/FiltroPim.cs). Con los valores (1/X/2) de los 14 partidos forma 7 rankings de 6
    /// casillas y filtra las columnas por sus aciertos con cada ranking (R1..R7) y su recorrido
    /// (max-min). La lógica está en <see cref="FiltroPim"/> (Free1X2.Domain). Antes este ViewModel
    /// llevaba el algoritmo de Aidomnou (6 columnas y suma), que no es el del Pim.
    ///
    /// La rejilla <see cref="Partidos"/> sustituye al UserControl legacy "valors" (valors1):
    /// se lee desde ella en RecuperaPantalla en vez de valors1.RetVals().
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

        // W15 (DT-92): «Pegar desde Excel» rellena los valores 1 / X / 2 de cada partido.
        [RelayCommand]
        private async Task PegarDesdeExcel()
        {
            var filas = await PegarPorcentajes.LeerAsync(Partidos.Count);
            if (filas is null) return;
            for (int i = 0; i < filas.Count && i < Partidos.Count; i++)
            {
                Partidos[i].Valor1 = filas[i][0];
                Partidos[i].ValorX = filas[i][1];
                Partidos[i].Valor2 = filas[i][2];
            }
        }

        // ===== Estado interno del motor (campos del form GeneraPim) =====
        private readonly double[,] _nvals = new double[14, 3];
        private readonly int[,] _rks = new int[8, 2];
        // [partido, ranking]: signos de cada uno de los 7 rankings (FiltroPim.CalculaRankings).
        private int[,] _rankings = new int[14, FiltroPim.Rankings];
        private string[] _colgsR = new string[3000];
        private int _limcgsR;
        private int _nrfCGR;
        // Columnas admitidas, en el orden del fichero de entrada (legacy: validas[0..ctadm)).
        private List<string>? _validas;

        public GeneraPimViewModel()
        {
            for (int i = 1; i <= 14; i++)
            {
                Partidos.Add(new PartidoValores { Numero = $"P{i:00}" });
            }
        }

        /// <summary>
        /// Legacy GeneraPim.Calcular(): lee un fichero de columnas de entrada, descarta las repetidas y
        /// valida las demás con los rangos por ranking y el recorrido, en el orden del fichero.
        /// </summary>
        [RelayCommand]
        private Task Calcular() => CalcularCon(null);

        // W10 (DT-85): «Calcular con columnas de Inicio» (las del boleto, sin guardar antes un fichero).
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextoCalcularInicio))]
        [NotifyCanExecuteChangedFor(nameof(CalcularConColumnasInicioCommand))]
        private bool _generandoInicio;

        public string TextoCalcularInicio =>
            GenerandoInicio ? ColumnasInicioServicio.TextoGenerando : "Calcular con columnas de Inicio";

        private bool PuedeUsarColumnasInicio() => !GenerandoInicio;

        [RelayCommand(CanExecute = nameof(PuedeUsarColumnasInicio))]
        private async Task CalcularConColumnasInicio()
        {
            GenerandoInicio = true;
            try
            {
                string? ruta = await ColumnasInicioServicio.GenerarAsync();
                if (ruta != null) await CalcularCon(ruta);
            }
            finally
            {
                GenerandoInicio = false;
            }
        }

        // origen null ⇒ se pide el fichero de entrada (comportamiento legacy).
        private async Task CalcularCon(string? origen)
        {
            // Recoge la pantalla y forma los rankings ANTES de elegir el fichero (igual que el legacy).
            RecuperaPantalla();
            int[,] rankings = _rankings;
            int[,] rangos = (int[,])_rks.Clone();

            string ruta;
            if (origen != null)
            {
                ruta = origen;
            }
            else
            {
                StorageFile? archivo = await PickerHelper.AbrirAsync(".txt", "*");
                if (archivo is null)
                {
                    return;
                }
                ruta = archivo.Path;
            }
            DateTime time0 = DateTime.Now;
            ColumnasProcesadas = "...";
            ColumnasAdmitidas = "...";
            Tiempo = "...";

            int ctini = 0;
            bool errorLongitud = false;
            var validas = new List<string>();
            var repes = new BitArray(4782969);

            // B-12: sin try/catch, cualquier IOException (fichero bloqueado, unidad de red caída,
            // disco lleno) escapaba al manejador global y los tres indicadores se quedaban
            // COLGADOS en "..." para siempre. Ahora el error se registra y se explica, y el
            // finally deja siempre los contadores en un valor coherente. Estas asignaciones
            // están tras el await, es decir, ya en el hilo de UI (son propiedades enlazadas).
            try
            {
                await Task.Run(() =>
                {
                    using var sr = new StreamReader(ruta);
                    while (sr.Peek() > 0)
                    {
                        // FiltroPim.cs:431-440: mayúsculas, las repetidas no se validan otra vez y las
                        // admitidas quedan en el orden del fichero.
                        string tmp = sr.ReadLine()!.Trim().ToUpperInvariant();
                        ctini++;
                        if (tmp.Length < 14)
                        {
                            errorLongitud = true;
                            break;
                        }
                        int idx = S2n(tmp, 14);
                        if (repes[idx]) continue;
                        repes[idx] = true;
                        if (FiltroPim.Admite(FiltroPim.Aciertos(rankings, tmp), rangos))
                        {
                            validas.Add(tmp);
                        }
                    }
                });

                _validas = validas;

                if (errorLongitud)
                {
                    AppServices.MostrarError("error de longitud en una columna de entrada");
                }
            }
            catch (Exception ex)
            {
                Log.Error("GeneraPim.Calcular [" + ruta + "]", ex);
                AppServices.MostrarError(
                    "No se pudo leer el fichero de columnas:\n" + ruta + "\n\n" + ex.Message);
            }
            finally
            {
                ColumnasProcesadas = ctini.ToString();
                ColumnasAdmitidas = validas.Count.ToString();
                string t = (DateTime.Now - time0).ToString() + "0000000000";
                Tiempo = t.Substring(0, 10);
            }
        }

        /// <summary>
        /// Legacy GeneraPim.GrabaCols(): vuelca al fichero las columnas admitidas, en el orden del fichero
        /// de entrada.
        /// </summary>
        [RelayCommand]
        private async Task GrabarResultado()
        {
            if (_validas is null)
            {
                AppServices.MostrarError("No hay resultado calculado. Pulsa Calcular primero.");
                return;
            }

            StorageFile? archivo = await PickerHelper.GuardarConExtensionPorDefectoAsync(
                "resultado", ".txt", ("Resultados", ".txt"), ("Todos los archivos", "."));
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            List<string> validas = _validas;

            // B-12: escritura de hasta 4.782.969 líneas; un fallo de E/S a mitad dejaba el
            // fichero a medias y el error sin explicar (ni el nombre en pantalla se actualizaba).
            try
            {
                await Task.Run(() =>
                {
                    using var wr = new StreamWriter(ruta);
                    foreach (string columna in validas)
                    {
                        wr.WriteLine(columna);
                    }
                });

                FicheroResultado = Path.GetFileName(ruta);
            }
            catch (Exception ex)
            {
                Log.Error("GeneraPim.GrabarResultado [" + ruta + "]", ex);
                AppServices.MostrarError(
                    "No se pudo grabar el fichero de resultado:\n" + ruta + "\n\n" + ex.Message);
            }
        }

        /// <summary>
        /// Legacy aidomnou.SalvaCondis(): escribe los 7 rangos + recorrido, línea a línea, a un fichero.
        /// </summary>
        [RelayCommand]
        private async Task SalvarRangos()
        {
            StorageFile? archivo = await PickerHelper.GuardarConExtensionPorDefectoAsync(
                "rangos", ".cnd", ("Rangos", ".cnd"), ("Todos los archivos", "."));
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            string[] lineas = { Rango1, Rango2, Rango3, Rango4, Rango5, Rango6, Rango7, RangoRecorrido };

            // B-12: sin catch, un fallo al escribir los rangos llegaba al manejador global.
            try
            {
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
            catch (Exception ex)
            {
                Log.Error("GeneraPim.SalvarRangos [" + ruta + "]", ex);
                AppServices.MostrarError(
                    "No se pudieron grabar los rangos en:\n" + ruta + "\n\n" + ex.Message);
            }
        }

        /// <summary>
        /// Legacy aidomnou.LeeCondis(): lee 8 líneas (7 rangos + recorrido) de un fichero a los campos.
        /// </summary>
        [RelayCommand]
        private async Task RecuperarRangos()
        {
            StorageFile? archivo = await PickerHelper.AbrirAsync(".cnd", "*");
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;

            // B-12: fichero inexistente, corrupto o inaccesible -> error explicado, sin tocar
            // los rangos actuales (se sale antes de asignar nada).
            string[] lineas;
            try
            {
                lineas = await Task.Run(() => File.ReadAllLines(ruta));
            }
            catch (Exception ex)
            {
                Log.Error("GeneraPim.RecuperarRangos [" + ruta + "]", ex);
                AppServices.MostrarError(
                    "No se pudo leer el fichero de rangos:\n" + ruta + "\n\n" + ex.Message);
                return;
            }

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
        /// Legacy GeneraPim.ExporCols(): exporta los 7 rankings, un partido tras otro separados por comas.
        /// </summary>
        [RelayCommand]
        private async Task ExportarColumnas()
        {
            // Recoge la pantalla y forma los rankings (igual que Analizar/Calcular del legacy).
            RecuperaPantalla();

            StorageFile? archivo = await PickerHelper.GuardarConExtensionPorDefectoAsync(
                "columnas", ".txt", ("F.Salida", ".txt"), ("Todos los archivos", "."));
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            int[,] rankings = _rankings;

            // B-12: sin catch, un fallo de escritura llegaba al manejador global sin decir
            // siquiera qué fichero falló.
            try
            {
                await Task.Run(() =>
                {
                    using var sw = new StreamWriter(ruta);
                    for (int nr = 0; nr < FiltroPim.Rankings; nr++)
                    {
                        string tmp = FiltroPim.Signos(rankings[0, nr]);
                        for (int np = 1; np < 14; np++)
                        {
                            tmp += "," + FiltroPim.Signos(rankings[np, nr]);
                        }
                        sw.WriteLine(tmp);
                    }
                });

                FicheroResultado = Path.GetFileName(ruta);
            }
            catch (Exception ex)
            {
                Log.Error("GeneraPim.ExportarColumnas [" + ruta + "]", ex);
                AppServices.MostrarError(
                    "No se pudieron exportar las columnas a:\n" + ruta + "\n\n" + ex.Message);
            }
        }

        /// <summary>
        /// Legacy aidomnou.EntraCGsR(): carga un fichero de columnas ganadoras, validándolas con
        /// VerColumna, y fija la última como columna en pantalla.
        /// </summary>
        [RelayCommand]
        private async Task CargarGanadoras()
        {
            StorageFile? archivo = await PickerHelper.AbrirAsync(".txt", "*");
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            var colgsR = new string[3000];
            int limcgsR = 0;
            bool columnaErronea = false;

            // B-12: sin catch, un fichero inaccesible (o con más de 3000 columnas ->
            // IndexOutOfRange) llegaba al manejador global y las ganadoras quedaban a medias.
            try
            {
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
            }
            catch (Exception ex)
            {
                Log.Error("GeneraPim.CargarGanadoras [" + ruta + "]", ex);
                AppServices.MostrarError(
                    "No se pudo leer el fichero de columnas ganadoras:\n" + ruta + "\n\n" + ex.Message);
                return; // No se toca el estado actual: las ganadoras previas siguen válidas.
            }

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
        /// Legacy GeneraPim.Analizar(): recoge la pantalla, forma los rankings y cuenta los aciertos
        /// de la columna ganadora con cada uno, volcando a Analisis1..7 + recorrido.
        /// </summary>
        [RelayCommand]
        private void Analizar()
        {
            string columna = ColumnaGanadora ?? "";
            if (columna.Length < 14)
            {
                AppServices.MostrarError("Carga antes una columna ganadora de 14 signos.");
                return;
            }

            RecuperaPantalla();
            int[] aciertos = FiltroPim.Aciertos(_rankings, columna);

            Analisis1 = aciertos[0].ToString();
            Analisis2 = aciertos[1].ToString();
            Analisis3 = aciertos[2].ToString();
            Analisis4 = aciertos[3].ToString();
            Analisis5 = aciertos[4].ToString();
            Analisis6 = aciertos[5].ToString();
            Analisis7 = aciertos[6].ToString();
            AnalisisRecorrido = FiltroPim.Recorrido(aciertos).ToString();
        }

        /// <summary>
        /// Legacy RecuperaPantalla(): vuelca los valores de la rejilla (sustituto de valors1.RetVals())
        /// a _nvals, parsea los 8 rangos (formato "min-max") a _rks y forma los 7 rankings.
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
            _rankings = FiltroPim.CalculaRankings(_nvals);
        }

        private void ParseRango(string texto, int fila)
        {
            string[] aux = (texto ?? "").Split('-');
            _rks[fila, 0] = Convert.ToInt32(aux[0]);
            _rks[fila, 1] = Convert.ToInt32(aux[1]);
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
