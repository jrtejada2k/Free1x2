using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para TramificarGraficasFrmPage.
    /// Legacy: Free1X2.UI.TramificarGraficasFrm — visor de gráficas de los
    /// resultados del análisis de tramos. Cada botón dibuja una curva distinta
    /// sobre los datos de la lista de Tramos (Tramo.ProbAcumulada, P14..P10,
    /// ColumnasPremiadas, TotalImportePremios, Balance) usando la clase Grafico.
    /// </summary>
    public partial class TramificarGraficasFrmViewModel : ObservableObject
    {
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
            // TODO: lógica de dominio — Free1X2.UI.TramificarGraficasFrm.
            // Según CurvaSeleccionada invocar el método legacy correspondiente:
            //   "Probabilidad acumulada"            -> DibujaProbabilidad()
            //   "14 aciertos"                       -> DibujaP14()
            //   "13 aciertos"                       -> DibujaP13()
            //   "12 aciertos"                       -> DibujaP12()
            //   "11 aciertos"                       -> DibujaP11()
            //   "10 aciertos"                       -> DibujaP10()
            //   "Nº de aciertos / columnas premiadas" -> DibujaColumnasPremiadas()
            //   "Importe de premios"                -> DibujaTotalImportePremios()
            //   "Balance (ingresos - gastos)"       -> DibujaBalance()
            // En la primera vez: Grafico.DibujaGrid(TamanoGrid) + DibujarEjes()
            // y guardar EscalaX/EscalaY. Cada curva crea new Grafico(Puntos, lienzo)
            // y DibujaCurva(new Pen(colorDeLaCurva)).
            EstadoTexto = $"(Pendiente) Dibujaría la curva: {CurvaSeleccionada}.";
        }

        [RelayCommand]
        private void Limpiar()
        {
            // TODO: lógica de dominio — TramificarGraficasFrm.LimpiarImagen():
            // limpia el lienzo a color de fondo, primeraVez=true, escalaX=escalaY=0.
            EstadoTexto = "(Pendiente) Limpiaría la imagen del lienzo.";
        }

        [RelayCommand]
        private void Copiar()
        {
            // TODO: lógica de dominio — TramificarGraficasFrm.CopiarImagenEnClipboard():
            // Clipboard.SetDataObject(pictureBox1.Image, true).
            EstadoTexto = "(Pendiente) Copiaría la imagen al portapapeles.";
        }
    }
}
