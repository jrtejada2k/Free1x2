using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
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
    /// LIMITACIÓN DE DOMINIO: la generación de las 6 columnas (PreparaColumnas), el cálculo
    /// (Calcular), el análisis (Analizar) y la exportación (ExporCols) dependen de la matriz de
    /// valoraciones 'nvals[14,3]' que el form legacy obtiene de 'valors1.RetVals()' — el
    /// UserControl Free1X2.UI.Controls.valors, que NO está portado a WinUI (no existe en
    /// aidomnouPage.xaml). Por eso esas acciones quedan como TODO con la referencia exacta.
    /// </summary>
    public partial class aidomnouViewModel : ObservableObject
    {
        // Columnas ganadoras cargadas (legacy: string[3000] colgsR) y navegación (limcgsR / nrfCGR).
        private readonly List<string> _colgsR = new();
        private int _nrfCGR;

        private bool _salida;

        // 6 columnas generadas, una por línea (legacy: PintaPantalla rellena cps[14,6] -> Cambia()).
        // ItemsSource del ListView de resultado.
        public ObservableCollection<string> ColumnasGeneradas { get; } = new();

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

        // Legacy: bCalcular -> Calcular(). Recupera pantalla, prepara las 6 columnas,
        // abre el fichero de entrada y valida cada columna contra los límites.
        [RelayCommand]
        private void Calcular()
        {
            // TODO[dominio]: depende de la matriz de valoraciones nvals[14,3] del UserControl
            //   'valors' (valors1.RetVals()), NO portado a aidomnouPage.xaml. Sin esas valoraciones
            //   no se pueden generar las 6 columnas probables ni validar el fichero de entrada.
            //   Portar la rejilla 'valors' (Free1X2/UI/Controls/valors + Aidomnou.cs RecuperaPantalla
            //   línea 255, PreparaColumnas/Calcular).
            AppServices.MostrarInfo("El cálculo requiere la rejilla de valoraciones ('valors'), aún no portada.");
        }

        // Legacy: bGrabaCols -> GrabaCols(). Vuelca el BitArray 'validas' a un fichero de texto.
        [RelayCommand]
        private void GrabarResultado()
        {
            // TODO[dominio]: depende del resultado de Calcular() (BitArray 'validas'), que a su vez
            //   depende de la rejilla 'valors' no portada (Aidomnou.cs GrabaCols).
            AppServices.MostrarInfo("No hay resultado que grabar: el cálculo aún no está disponible (falta 'valors').");
        }

        // Legacy: bExporCols -> ExporCols(). Exporta las 6 columnas generadas (cps) a fichero.
        [RelayCommand]
        private void ExportarColumnas()
        {
            // TODO[dominio]: depende de la matriz 'cps' (6 columnas generadas) derivada de la
            //   rejilla 'valors' no portada (Aidomnou.cs ExporCols línea 702).
            AppServices.MostrarInfo("La exportación requiere la rejilla de valoraciones ('valors'), aún no portada.");
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

        // Legacy: bAnalizar -> Analizar(). Cuenta aciertos de la columna contra las 6 generadas.
        [RelayCommand]
        private void Analizar()
        {
            // TODO[dominio]: depende de las 6 columnas generadas (cps) derivadas de la rejilla
            //   'valors' no portada. Sin ellas no hay nada contra lo que contar aciertos
            //   (Aidomnou.cs Analizar / RecuperaPantalla línea 255).
            AppServices.MostrarInfo("El análisis requiere la rejilla de valoraciones ('valors'), aún no portada.");
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
