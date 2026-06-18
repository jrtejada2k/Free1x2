using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Utils;
using Free1X2.WinUI.Controls;
using Windows.UI;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para TramificarGraficasFrmPage.
    /// Legacy: Free1X2.UI.TramificarGraficasFrm — visor de gráficas de los
    /// resultados del análisis de tramos. Cada botón dibuja una curva distinta
    /// sobre los datos de la lista de Tramos (Tramo.ProbAcumulada, P14..P10,
    /// ColumnasPremiadas, TotalImportePremios, Balance) usando la clase Grafico.
    ///
    /// Aquí se cablea el CÁLCULO de los puntos (Puntos[]) de cada curva, que es la parte
    /// portable. El RENDER (clase Grafico, System.Drawing) sigue en WinForms; ver TODO en Dibujar.
    /// </summary>
    public partial class TramificarGraficasFrmViewModel : ObservableObject
    {
        /// <summary>
        /// Handoff de la lista de Tramos analizados desde la pantalla de tramificación
        /// (legacy: TramificarForm pasa el ArrayList de Tramos al ctor TramificarGraficasFrm(pTramos),
        /// ver Free1X2/UI/TramificarForm.cs línea 3461). Sigue el patrón de handoff estático de la app
        /// (cf. VisorAnalisisColumnasFrmViewModel.UltimoGrupo).
        /// </summary>
        public static IReadOnlyList<Tramo>? TramosAnalizados { get; set; }

        // Selección del tipo de curva a representar.
        // Legacy: cada case del toolBarGraficas_ButtonClick.
        [ObservableProperty]
        private string _curvaSeleccionada = "Probabilidad acumulada";

        // Estado descriptivo mostrado al usuario.
        [ObservableProperty]
        private string _estadoTexto = "Selecciona una curva para dibujarla sobre la gráfica.";

        // Tamaño del grid (legacy: grafica.DibujaGrid(50)).
        [ObservableProperty]
        private double _tamanoGrid = 50;

        // Puntos calculados de la última curva pedida (legacy: Point[] Puntos).
        // Se exponen como pares (X = NumeroDeTramo-1, Y = valor) listos para el render.
        private readonly List<(int X, int Y)> _puntos = new();

        /// <summary>Puntos (x,y) de la última curva calculada, para alimentar el render del lienzo.</summary>
        public IReadOnlyList<(int X, int Y)> Puntos => _puntos;

        /// <summary>
        /// Curvas ya dibujadas sobre el lienzo (las consume GraficoLineasControl). El legacy
        /// SUPERPONE curvas sobre el mismo PictureBox hasta pulsar "Limpiar"; aquí cada Dibujar
        /// añade (o reemplaza si se repite el tipo) su curva con el color del Pen original.
        /// </summary>
        public ObservableCollection<CurvaGrafico> Curvas { get; } = new();

        /// <summary>
        /// Se solicita copiar la imagen de la gráfica al portapapeles. Lo escucha el code-behind
        /// de la Page (que tiene acceso al GraficoLineasControl para hacer el RenderTargetBitmap).
        /// El VM no referencia controles de la vista (separación MVVM).
        /// </summary>
        public event EventHandler? CopiaImagenSolicitada;

        /// <summary>
        /// Color del trazo de cada curva, idéntico al Pen del legacy (Dibuja*() en
        /// Free1X2/UI/TramificarGraficasFrm.cs) y a la leyenda de panel2..panel10.
        /// </summary>
        private static Color ColorDeCurva(string curva) => curva switch
        {
            "Probabilidad acumulada" => Color.FromArgb(0xFF, 0x00, 0x00, 0x00),           // Black (DibujaProbabilidad)
            "14 aciertos" => Color.FromArgb(0xFF, 0x1E, 0x90, 0xFF),                       // DodgerBlue (panel3) / Pen Blue legacy DibujaP14
            "13 aciertos" => Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A),                       // Brown (DibujaP13)
            "12 aciertos" => Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF),                       // Cyan (DibujaP12)
            "11 aciertos" => Color.FromArgb(0xFF, 0x00, 0x80, 0x00),                       // Green (DibujaP11)
            "10 aciertos" => Color.FromArgb(0xFF, 0xFF, 0x00, 0x00),                       // Red (DibujaP10)
            "Nº de aciertos / columnas premiadas" => Color.FromArgb(0xFF, 0x5F, 0x9E, 0xA0), // CadetBlue (DibujaColumnasPremiadas)
            "Importe de premios" => Color.FromArgb(0xFF, 0x22, 0x8B, 0x22),               // ForestGreen (DibujaTotalImportePremios)
            "Balance (ingresos - gastos)" => Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00),      // Orange (DibujaBalance)
            _ => Color.FromArgb(0xFF, 0x00, 0x00, 0x00),
        };

        /// <summary>
        /// Tipos de curva disponibles (legacy: botones del toolBarGraficas).
        /// </summary>
        public IReadOnlyList<string> CurvasDisponibles { get; } = new List<string>
        {
            "Probabilidad acumulada", // toolBarButton1 "Pr"
            "14 aciertos",            // toolBarButton2 "14"
            "13 aciertos",            // toolBarButton3 "13"
            "12 aciertos",            // toolBarButton4 "12"
            "11 aciertos",            // toolBarButton5 "11"
            "10 aciertos",            // toolBarButton6 "10"
            "Nº de aciertos / columnas premiadas", // toolBarButton7 "Nº"
            "Importe de premios",     // toolBarButton8
            "Balance (ingresos - gastos)", // toolBarButton9 "+/-"
        };

        [RelayCommand]
        private void Dibujar()
        {
            var tramos = TramosAnalizados;
            if (tramos == null || tramos.Count == 0)
            {
                EstadoTexto = "No hay tramos analizados que representar.";
                return;
            }

            // Cálculo de los puntos de la curva seleccionada (legacy: cada método Dibuja*()).
            // El valor Y por tramo replica EXACTAMENTE la fórmula legacy de cada Dibuja*():
            //   Probabilidad acumulada -> 35 + ProbAcumulada (DibujaProbabilidad)
            //   14/13/12/11/10 aciertos -> P14/P13/P12/P11/P10
            //   columnas premiadas -> ColumnasPremiadas
            //   importe de premios -> TotalImportePremios
            //   balance -> Balance
            _puntos.Clear();
            var puntos = new List<(double X, double Y)>();
            foreach (var tr in tramos)
            {
                int x = tr.NumeroDeTramo - 1;
                int y = CurvaSeleccionada switch
                {
                    "Probabilidad acumulada" => (int)(35 + tr.ProbAcumulada),
                    "14 aciertos" => tr.P14,
                    "13 aciertos" => tr.P13,
                    "12 aciertos" => tr.P12,
                    "11 aciertos" => tr.P11,
                    "10 aciertos" => tr.P10,
                    "Nº de aciertos / columnas premiadas" => tr.ColumnasPremiadas,
                    "Importe de premios" => (int)tr.TotalImportePremios,
                    "Balance (ingresos - gastos)" => (int)tr.Balance,
                    _ => 0,
                };
                _puntos.Add((x, y));
                puntos.Add((x, y));
            }
            OnPropertyChanged(nameof(Puntos));

            // Render del lienzo (legacy: Grafico(Puntos, pictureBox1).DibujaCurva(new Pen(color))).
            //   El legacy SUPERPONE curvas hasta "Limpiar"; aquí se reemplaza la curva del mismo tipo
            //   (re-dibujar la misma curva no la duplica) y se añaden las nuevas. GraficoLineasControl
            //   calcula la escala común, los ejes y la cuadrícula (DibujarEjes/DibujaGrid) y traza una
            //   polilínea por curva con el color original del Pen.
            for (int i = Curvas.Count - 1; i >= 0; i--)
                if (Curvas[i].Nombre == CurvaSeleccionada) Curvas.RemoveAt(i);
            Curvas.Add(new CurvaGrafico(CurvaSeleccionada, ColorDeCurva(CurvaSeleccionada), puntos));

            EstadoTexto = $"Curva '{CurvaSeleccionada}': {_puntos.Count} puntos dibujados.";
        }

        [RelayCommand]
        private void Limpiar()
        {
            // Legacy LimpiarImagen(): limpia el lienzo, primeraVez=true, escalaX=escalaY=0.
            // Aquí se vacían los datos calculados y las curvas; GraficoLineasControl se redibuja
            // (queda el lienzo con sólo la cuadrícula, como el PictureBox Beige vacío del legacy).
            _puntos.Clear();
            Curvas.Clear();
            OnPropertyChanged(nameof(Puntos));
            EstadoTexto = "Imagen limpiada. Selecciona una curva para volver a dibujar.";
        }

        [RelayCommand]
        private void Copiar()
        {
            // Legacy: Free1X2/UI/TramificarGraficasFrm.cs CopiarImagenEnClipboard() (línea 481)
            //   hacía Clipboard.SetDataObject(pictureBox1.Image, true). Equivalente WinUI: capturar
            //   el GraficoLineasControl con RenderTargetBitmap y ponerlo en el portapapeles
            //   (DataPackage con bitmap). El RenderTargetBitmap necesita el UIElement, así que lo hace
            //   la Page; el VM sólo dispara el evento. Las curvas ya están dibujadas en el lienzo.
            if (Curvas.Count == 0)
            {
                EstadoTexto = "No hay ninguna curva dibujada que copiar.";
                return;
            }
            CopiaImagenSolicitada?.Invoke(this, EventArgs.Empty);
        }
    }
}
