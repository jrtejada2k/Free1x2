using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    // NOTA: la matriz de datos y numcol provienen del dominio (clase legacy DibRepFrm /
    // el form padre que la construye). Aqui se exponen propiedades string ya formateadas
    // para no violar las reglas anti-crash del XamlCompiler (no bind de int/double a TextBlock.Text).

    public partial class DibRepFrmViewModel : ObservableObject
    {
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

        // Etiquetas de fila del form legacy (label85/label153/label119/label136 + posiciones).
        // El orden refleja las 5 filas de la matriz rsl[0..4, *].
        public IReadOnlyList<string> EtiquetasFilas { get; } = new List<string>
        {
            "cant. 1", "cant. X", "cant. 2", "cant. V", "posiciones"
        };

        // Filas de la tabla. Cada fila contiene 15 celdas string ya formateadas.
        public ObservableCollection<FilaCoincidencias> Filas { get; } = new();

        public DibRepFrmViewModel()
        {
            // TODO (dominio): poblar 'Filas', 'NumColumnasTexto' y aplicar el formato porcentaje/conteo
            // a partir de la matriz int[5,15] (rsl) y numcol que entrega el form padre legacy
            // (constructor DibRepFrm(int[,] ofparent, int ncol) -> PintaPantalla()/Porcentajes()/PintaColumnas()).
            // Cuando MostrarPorcentajes cambie hay que reformatear las celdas:
            //   porcentaje  = (rsl[f,c] * 10000 / numcol) / 1E2
            //   conteo      = rsl[f,c]
            // Por ahora se dejan placeholders para que la Page renderice sin datos de dominio.
            for (int f = 0; f < EtiquetasFilas.Count; f++)
            {
                var fila = new FilaCoincidencias(EtiquetasFilas[f]);
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
                Celdas.Add("-"); // TODO (dominio): valor real desde rsl[fila, c].
            }
        }
    }
}
