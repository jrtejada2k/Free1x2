using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada del WinForms "Filtro Pim" (clase legacy Free1X2.UI.GeneraPim).
    /// Genera/reduce columnas probables a partir de los valores (1/X/2) de los 14 partidos,
    /// aplica rangos de aciertos por "ranking" y un rango de recorrido (max-min).
    /// La lógica de dominio NO se implementa aquí: queda marcada como TODO citando la clase legacy.
    /// </summary>
    public partial class GeneraPimViewModel : ObservableObject
    {
        // ---- Rangos de aciertos por ranking (legacy: tbrank1..tbrank7, formato "min-max") ----
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

        // ---- Columna ganadora actual para análisis (legacy: ltColR) ----
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

        // ---- Resultado del análisis por ranking (legacy: lrk1..lrk7 + lreco) ----
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

        public GeneraPimViewModel()
        {
            for (int i = 1; i <= 14; i++)
            {
                Partidos.Add(new PartidoValores { Numero = $"P{i:00}" });
            }
        }

        [RelayCommand]
        private void Calcular()
        {
            // TODO (legacy GeneraPim.Calcular): abrir OpenFileDialog de columnas de entrada,
            // recorrer cada columna, descartar repetidas (BitArray repes / s2n) y validar con
            // GeneraPim.Valida usando los rangos por ranking y el recorrido. Actualizar
            // ColumnasProcesadas / ColumnasAdmitidas / Tiempo (veureelmeu).
        }

        [RelayCommand]
        private void GrabarResultado()
        {
            // TODO (legacy GeneraPim.GrabaCols): SaveFileDialog y volcar las columnas válidas
            // (validas[0..ctadm]) a fichero .txt, sustituyendo '4' por 'X'.
        }

        [RelayCommand]
        private void SalvarRangos()
        {
            // TODO (legacy GeneraPim.SalvarConds): SaveFileDialog *.jb7 y escribir los 7 rangos
            // + rango de recorrido (tbrank1..7, tbmgreco) línea a línea.
        }

        [RelayCommand]
        private void RecuperarRangos()
        {
            // TODO (legacy GeneraPim.LeerConds): OpenFileDialog *.jb7, leer 8 líneas a los rangos,
            // luego RecuperaPantalla + PintaPantalla.
        }

        [RelayCommand]
        private void ExportarColumnas()
        {
            // TODO (legacy GeneraPim.ExporCols): SaveFileDialog *.txt y exportar las 7 columnas
            // probables (cps) traducidas con Cambia().
        }

        [RelayCommand]
        private void CargarGanadoras()
        {
            // TODO (legacy GeneraPim.EntraCGsR): OpenFileDialog *.txt de columnas ganadoras,
            // cargar colgsR[], fijar IndiceColGanadora y ColumnaGanadora a la última.
        }

        [RelayCommand]
        private void GanadoraSiguiente()
        {
            // TODO (legacy GeneraPim.GRMas): avanzar nrfCGR y refrescar ColumnaGanadora / IndiceColGanadora.
        }

        [RelayCommand]
        private void GanadoraAnterior()
        {
            // TODO (legacy GeneraPim.GRMenos): retroceder nrfCGR y refrescar ColumnaGanadora / IndiceColGanadora.
        }

        [RelayCommand]
        private void Analizar()
        {
            // TODO (legacy GeneraPim.Analizar): RecuperaPantalla + PintaPantalla, contar aciertos por
            // ranking contra ColumnaGanadora y volcar a Analisis1..7 + AnalisisRecorrido (max-min).
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
