// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Free1X2.WinUI.Views.Ported
{
    // ViewModel para DibRepFrmPage — portado del WinForms "DibRepFrm" (coincidencias).
    //
    // El form legacy DibRepFrm recibe en su constructor:
    //   public DibRepFrm(int[,] ofparent, int ncol)
    // donde 'ofparent' (rsl) es una matriz int[5,15] con los conteos de coincidencias
    // (5 filas de categorias x 15 posiciones/partidos 0..14) y 'ncol' (numcol) el total de columnas.
    //
    // El form muestra cada celda como:
    //   - porcentaje:  (rsl[f,c] * 10000 / numcol) / 1E2   (rbPercent.Checked)
    //   - conteo bruto: rsl[f,c]                            (rbCols.Checked)
    // y un GroupBox "mostrar" con radios "porcentajes" / "columnas" que alterna el modo.
    //
    // Es un sub-diálogo de SÓLO PRESENTACIÓN (sin E/S de fichero ni motor): formatea la matriz
    // que entrega su form padre. En WinUI esa matriz llega por el handoff estático
    // MatrizEntrada / NumColEntrada (igual patrón que AppState.GrupoEnEdicion). Mientras el
    // productor (form padre) no lo rellene, la tabla se muestra a 0 (TODO documentado).
    public partial class DibRepFrmViewModel : ObservableObject
    {
        /// <summary>
        /// Matriz int[5,15] entregada por el form padre (legacy: ctor DibRepFrm(int[,] ofparent, int ncol)).
        /// Handoff estático de proceso, análogo a AppState.GrupoEnEdicion.
        /// TODO[dominio]: el productor (pantalla padre) debe asignar esta matriz y NumColEntrada
        ///   antes de navegar a DibRepFrmPage (Free1X2/UI/Estadisticas/DibRepFrm.cs, constructor).
        /// </summary>
        public static int[,]? MatrizEntrada { get; set; }

        /// <summary>Total de columnas analizadas (legacy: numcol). Handoff estático.</summary>
        public static int NumColEntrada { get; set; }

        private readonly int[,] _rsl;
        private readonly int _numcol;

        // Numero de columnas total (numcol del form legacy). Expuesto como string para binding seguro.
        [ObservableProperty]
        private string _numColumnasTexto = "0";

        // Modo de presentacion: true = porcentajes (default en legacy rbPercent.Checked),
        // false = conteo de columnas (rbCols).
        [ObservableProperty]
        private bool _mostrarPorcentajes = true;

        // Encabezados de columna: "0".."14" (15 posiciones de la quiniela).
        public IReadOnlyList<string> EncabezadosColumnas { get; } = new List<string>
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14"
        };

        // Etiquetas de fila del form legacy, en el orden visual (por coordenada Y) de los
        // labels del DibRepFrm: label34 "posiciones" (rsl[0]), label85 "cant. 1" (rsl[1]),
        // label153 "cant. X" (rsl[2]), label119 "cant. 2" (rsl[3]), label136 "cant. V" (rsl[4]).
        // Coincide con Proceso() modo "Sus coincidencias": rsl[0]=ind[0] (posiciones de la
        // 1ª variante), rsl[1]=cant.1 repetida, rsl[2]=cant.X repetida, rsl[3]=cant.2 repetida,
        // rsl[4]=cant.V repetida (statistics.cs líneas 84-96).
        public IReadOnlyList<string> EtiquetasFilas { get; } = new List<string>
        {
            "posiciones", "cant. 1", "cant. X", "cant. 2", "cant. V"
        };

        // Filas de la tabla. Cada fila contiene 15 celdas string ya formateadas.
        public ObservableCollection<FilaCoincidencias> Filas { get; } = new();

        public DibRepFrmViewModel()
        {
            // Toma el handoff del form padre (legacy: rsl = ofparent; numcol = ncol).
            _rsl = MatrizEntrada ?? new int[5, 15];
            _numcol = NumColEntrada;
            Repintar();
        }

        // Reacciona al cambio del selector "mostrar" (rbCols.CheckedChanged -> PintaPantalla legacy).
        partial void OnMostrarPorcentajesChanged(bool value)
        {
            Repintar();
        }

        /// <summary>
        /// Equivale a DibRepFrm.PintaPantalla() del legacy: rellena cada celda con el porcentaje
        /// (rsl[f,c]*10000/numcol)/1E2 o el conteo bruto rsl[f,c] según el modo.
        /// </summary>
        private void Repintar()
        {
            NumColumnasTexto = _numcol.ToString();

            Filas.Clear();
            int filas = _rsl.GetLength(0);
            int cols = _rsl.GetLength(1);
            int divisor = _numcol == 0 ? 1 : _numcol;
            var inv = CultureInfo.InvariantCulture;

            for (int f = 0; f < filas; f++)
            {
                string etiqueta = f < EtiquetasFilas.Count ? EtiquetasFilas[f] : f.ToString();
                var fila = new FilaCoincidencias(etiqueta);
                fila.Celdas.Clear();
                for (int c = 0; c < cols; c++)
                {
                    int valor = _rsl[f, c];
                    string texto = MostrarPorcentajes
                        ? ((valor * 10000 / divisor) / 1E2).ToString(inv)
                        : valor.ToString(inv);
                    fila.Celdas.Add(texto);
                }
                Filas.Add(fila);
            }
        }
    }

    // Representa una fila de la tabla de coincidencias: una etiqueta + 15 celdas formateadas.
    public partial class FilaCoincidencias : ObservableObject
    {
        [ObservableProperty]
        private string _etiqueta = string.Empty;

        // 15 celdas (posiciones 0..14) como strings ya formateados.
        public ObservableCollection<string> Celdas { get; } = new();

        public FilaCoincidencias(string etiqueta)
        {
            Etiqueta = etiqueta;
            for (int c = 0; c < 15; c++)
            {
                Celdas.Add("-"); // Valor por defecto; Repintar() lo sustituye por rsl[fila, c].
            }
        }
    }
}
