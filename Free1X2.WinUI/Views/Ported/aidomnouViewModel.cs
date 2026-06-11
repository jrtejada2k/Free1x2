using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada de "aidomnou" (WinForms legacy: Free1X2.UI.aidomnou).
    ///
    /// Propósito legacy: filtro "Filtro Aidomnou". A partir de las valoraciones de los 14
    /// partidos (control 'valors', double[14,3] = valor por signo 1/X/2), genera 6 columnas
    /// probables, cada una con una estrategia distinta:
    ///   - Columna 1 y 4: por puntos (PreCol1 / PreCol4, límites lims[0] / lims[4]).
    ///   - Columna 2 y 5: por cantidad de unos/equis/doses (PreCol2 / PreCol5, lims[1..3] / lims[5..7]).
    ///   - Columna 3: 14 dobles (PreCol3).
    ///   - Columna 6: 14 fijos (PreCol6).
    /// Después valida un fichero de columnas (Calcular -> Valida) contra los límites min-max de
    /// aciertos por columna, la suma y el recorrido (rks[8,2], desde tbmg1..tbmg6/suma/reco),
    /// cuenta procesadas/válidas y permite grabar el resultado (GrabaCols / BitArray validas),
    /// exportar las 6 columnas (ExporCols), salvar/recuperar los límites en ficheros .cnd
    /// (SalvaCondis / LeeCondis) y analizar columnas ganadoras (EntraCGsR / Analizar).
    ///
    /// Toda la lógica de dominio (generación de columnas, validación bit a bit, lectura/escritura
    /// de ficheros, configuración aidomnou.cfg, control 'valors') queda como TODO; aquí solo se
    /// replican los campos de entrada y las acciones de la UI.
    /// </summary>
    public partial class aidomnouViewModel : ObservableObject
    {
        // 6 columnas generadas, una por línea (legacy: PintaPantalla rellena cps[14,6] -> Cambia()).
        // ItemsSource del ListView de resultado.
        public IReadOnlyList<string> ColumnasGeneradas { get; } = new List<string>();

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

        // ================= Comandos (lógica de dominio = TODO) =================

        // Legacy: bCalcular -> Calcular(). Recupera pantalla, prepara las 6 columnas,
        // abre el fichero de entrada y valida cada columna contra los límites.
        [RelayCommand]
        private void Calcular()
        {
            // TODO: portar Free1X2.UI.aidomnou.Calcular()
            //       (RecuperaPantalla + PreparaColumnas + PintaPantalla + lectura del fichero + Valida).
        }

        // Legacy: bGrabaCols -> GrabaCols(). Vuelca el BitArray 'validas' a un fichero de texto.
        [RelayCommand]
        private void GrabarResultado()
        {
            // TODO: portar Free1X2.UI.aidomnou.GrabaCols() (recorre 'validas' y escribe n2s()).
        }

        // Legacy: bExporCols -> ExporCols(). Exporta las 6 columnas generadas (cps) a fichero.
        [RelayCommand]
        private void ExportarColumnas()
        {
            // TODO: portar Free1X2.UI.aidomnou.ExporCols() (escribe Cambia(cps[np,nr]) por línea).
        }

        // Legacy: bCancelar -> salida = true. Aborta el bucle de cálculo en curso.
        [RelayCommand]
        private void Cancelar()
        {
            // TODO: portar Free1X2.UI.aidomnou.BCancelarClick (pone salida=true para abortar Calcular()).
        }

        // Legacy: bSalvaCondis -> SalvaCondis(). Guarda los 8 rangos en un fichero .cnd.
        [RelayCommand]
        private void SalvarLimites()
        {
            // TODO: portar Free1X2.UI.aidomnou.SalvaCondis() (escribe tbmg1..tbmgreco en .cnd).
        }

        // Legacy: bLeeCondis -> LeeCondis(). Carga los 8 rangos desde un fichero .cnd.
        [RelayCommand]
        private void RecuperarLimites()
        {
            // TODO: portar Free1X2.UI.aidomnou.LeeCondis() (lee .cnd a tbmg1..tbmgreco).
        }

        // Legacy: bFG -> EntraCGsR(). Abre el fichero de columnas ganadoras y carga colgsR.
        [RelayCommand]
        private void AbrirGanadoras()
        {
            // TODO: portar Free1X2.UI.aidomnou.EntraCGsR() (lee fichero, VerColumna, llena colgsR).
        }

        // Legacy: bMenosR -> GRMenos(). Retrocede en el fichero de ganadoras.
        [RelayCommand]
        private void GanadoraAnterior()
        {
            // TODO: portar Free1X2.UI.aidomnou.GRMenos() (nrfCGR-- y muestra colgsR[nrfCGR-1] en tbColR).
        }

        // Legacy: bMasR -> GRMas(). Avanza en el fichero de ganadoras.
        [RelayCommand]
        private void GanadoraSiguiente()
        {
            // TODO: portar Free1X2.UI.aidomnou.GRMas() (nrfCGR++ y muestra colgsR[nrfCGR-1] en tbColR).
        }

        // Legacy: bAnalizar -> Analizar(). Cuenta aciertos de la columna contra las 6 generadas.
        [RelayCommand]
        private void Analizar()
        {
            // TODO: portar Free1X2.UI.aidomnou.Analizar()
            //       (RecuperaPantalla + PreparaColumnas + conteo bit a bit -> lrk1..lrk6, lsuma, lreco).
        }
    }
}
