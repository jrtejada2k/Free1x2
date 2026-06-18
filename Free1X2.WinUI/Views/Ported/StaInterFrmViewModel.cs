// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para StaInterFrmPage — port del WinForms legacy "StaInterFrm"
    /// (Free1X2/UI/Estadisticas/StaInterFrm.cs).
    ///
    /// Propósito original: mostrar la tabla de "interrupciones" de una reducción:
    /// una matriz de 5 filas x 14 columnas (partidos 0..13) donde cada fila es un
    /// agregado distinto:
    ///   fila 0 = "globales"   (rsl[0, c])
    ///   fila 1 = "cant. 1"    (rsl[1, c])
    ///   fila 2 = "cant. X"    (rsl[2, c])
    ///   fila 3 = "cant. 2"    (rsl[3, c])
    ///   fila 4 = "cant. V"    (rsl[4, c])
    /// El usuario alterna entre ver "porcentajes" (valor*10000/numcol/100) o
    /// "columnas" (el conteo crudo) mediante los RadioButton rbPercent/rbCols.
    ///
    /// El constructor legacy recibía: StaInterFrm(int[,] ofparent, int ncol).
    /// rsl = matriz de resultados; numcol = nº total de columnas analizadas.
    ///
    /// Los datos (rsl/numcol) los calcula AnastaticsViewModel (modo "Interrupciones")
    /// e inyecta vía <see cref="CargarDatos"/>.
    /// </summary>
    public partial class StaInterFrmViewModel : ObservableObject
    {
        public const int Filas = 5;
        public const int Columnas = 14;

        /// <summary>
        /// Matriz int[15,15] entregada por el productor (AnastaticsViewModel modo "Interrupciones")
        /// antes de navegar a StaInterFrmPage. Handoff estático de proceso (mismo patrón que
        /// VisorEstadisticas.UltimasEstadisticas / AppState.GrupoEnEdicion). El ctor la lee y, si
        /// existe, vuelca sus filas 0..4 / columnas 0..13 a la rejilla. Equivale al ctor legacy
        /// StaInterFrm(int[,] ofparent, int ncol).
        /// </summary>
        public static int[,]? MatrizEntrada { get; set; }

        /// <summary>Total de columnas analizadas (legacy: numcol). Handoff estático.</summary>
        public static int NumColEntrada { get; set; }

        // Encabezados de fila (label34, label85, label153, label119, label136 en el legacy).
        public IReadOnlyList<string> EncabezadosFila { get; } = new[]
        {
            "globales", "cant. 1", "cant. X", "cant. 2", "cant. V"
        };

        // Encabezados de columna: partidos 0..13 (label4=0 .. label16=13 en el legacy).
        public IReadOnlyList<string> EncabezadosColumna { get; }

        // Opciones del grupo "mostrar" (groupBox con rbPercent / rbCols).
        public IReadOnlyList<string> OpcionesMostrar { get; } = new[]
        {
            "porcentajes", "columnas"
        };

        // Matriz de celdas como strings (las celdas legacy lt{f}{cc} son Label.Text).
        // Regla anti-crash 2: nunca bindeamos int/double directo a TextBlock.Text;
        // exponemos strings ya formateados.
        public string[][] Celdas { get; }

        // Datos crudos en memoria (equivalente a rsl[,] del legacy). Se rellenan por dominio.
        private readonly int[,] _rsl = new int[Filas, Columnas];

        [ObservableProperty]
        private int _numColumnas;

        // Texto del nº de columnas (lncol.Text en el legacy).
        public string NumColumnasTexto => NumColumnas.ToString();

        // Índice seleccionado en "mostrar": 0 = porcentajes, 1 = columnas.
        [ObservableProperty]
        private int _modoMostrarIndex;

        public StaInterFrmViewModel()
        {
            var colHeaders = new string[Columnas];
            for (int c = 0; c < Columnas; c++) colHeaders[c] = c.ToString();
            EncabezadosColumna = colHeaders;

            Celdas = new string[Filas][];
            for (int f = 0; f < Filas; f++)
            {
                Celdas[f] = new string[Columnas];
                for (int c = 0; c < Columnas; c++) Celdas[f][c] = "-";
            }

            // Si el productor dejó una matriz en el handoff estático, vuélcala (ctor legacy
            // StaInterFrm(int[,] ofparent, int ncol)). Si no, queda a 0 (rejilla vacía).
            if (MatrizEntrada != null)
                CargarDatos(MatrizEntrada, NumColEntrada);
            else
                Repintar();
        }

        partial void OnModoMostrarIndexChanged(int value) => Repintar();

        partial void OnNumColumnasChanged(int value) => OnPropertyChanged(nameof(NumColumnasTexto));

        /// <summary>
        /// Carga la matriz calculada por el motor (equivale al ctor legacy
        /// StaInterFrm(int[,] ofparent, int ncol): rsl = ofparent; numcol = ncol; PintaPantalla()).
        /// Sólo se usan las columnas 0..13 (los partidos) de la matriz [15,15] del cálculo.
        /// </summary>
        public void CargarDatos(int[,] rsl, int numcol)
        {
            for (int f = 0; f < Filas; f++)
                for (int c = 0; c < Columnas; c++)
                    _rsl[f, c] = rsl[f, c];

            NumColumnas = numcol;
            Repintar();
        }

        /// <summary>
        /// Equivalente a PintaPantalla() del legacy: según el modo, formatea cada
        /// celda como porcentaje (Porcentajes()) o como conteo crudo (PintaColumnas()).
        /// </summary>
        [RelayCommand]
        private void Repintar()
        {
            bool porcentajes = ModoMostrarIndex == 0;
            int divisor = NumColumnas == 0 ? 1 : NumColumnas; // evita /0 (el legacy asumía numcol>0)

            for (int f = 0; f < Filas; f++)
            {
                for (int c = 0; c < Columnas; c++)
                {
                    if (porcentajes)
                    {
                        // Legacy: (rsl[f,c]*10000/numcol)/1E2
                        double pct = (_rsl[f, c] * 10000 / divisor) / 1E2;
                        Celdas[f][c] = pct.ToString();
                    }
                    else
                    {
                        // Legacy: rsl[f,c]
                        Celdas[f][c] = _rsl[f, c].ToString();
                    }
                }
            }

            OnPropertyChanged(nameof(Celdas));
        }
    }
}
