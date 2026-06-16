using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Utils;

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
            }
            OnPropertyChanged(nameof(Puntos));

            // TODO: render del lienzo — Free1X2/UI/TramificarGraficasFrm.cs (Dibuja*() + clase Grafico).
            //   El dibujo usa System.Drawing (Grafico(Puntos, pictureBox1).DibujaCurva(new Pen(color)))
            //   sobre un PictureBox, y en la primera vez DibujaGrid(TamanoGrid) + DibujarEjes() y guarda
            //   EscalaX/EscalaY. La clase Grafico permanece en WinForms; el render WinUI (p. ej. con
            //   Microsoft.UI.Xaml.Shapes.Polyline o un CanvasControl de Win2D) está pendiente.
            //   Los datos (Puntos) ya están calculados y disponibles aquí.
            EstadoTexto = $"Curva '{CurvaSeleccionada}': {_puntos.Count} puntos calculados (render pendiente).";
        }

        [RelayCommand]
        private void Limpiar()
        {
            // Legacy LimpiarImagen(): limpia el lienzo, primeraVez=true, escalaX=escalaY=0.
            // Aquí se limpian los datos calculados; el borrado del lienzo es parte del render (TODO Dibujar).
            _puntos.Clear();
            OnPropertyChanged(nameof(Puntos));
            EstadoTexto = "Imagen limpiada. Selecciona una curva para volver a dibujar.";
        }

        [RelayCommand]
        private void Copiar()
        {
            // TODO: portapapeles — Free1X2/UI/TramificarGraficasFrm.cs CopiarImagenEnClipboard():
            //   Clipboard.SetDataObject(pictureBox1.Image, true). Depende del render del lienzo
            //   (System.Drawing), aún pendiente; en WinUI usar Windows.ApplicationModel.DataTransfer
            //   con el bitmap resultante del render.
            EstadoTexto = "(Pendiente) Copiaría la imagen al portapapeles tras portar el render.";
        }
    }
}
