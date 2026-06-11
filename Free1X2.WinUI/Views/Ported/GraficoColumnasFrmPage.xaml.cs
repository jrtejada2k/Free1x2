using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "GraficoColumnasFrm".
/// Abre un fichero de columnas (.txt), calcula los límites (mínimo/máximo) de la combinación
/// y la representa gráficamente en 10 franjas. La "Guía" indica, para cada franja, qué fila
/// de la quiniela queda representada.
/// </summary>
public sealed partial class GraficoColumnasFrmPage : Page
{
    public GraficoColumnasFrmViewModel ViewModel { get; } = new();

    public GraficoColumnasFrmPage()
    {
        InitializeComponent();

        // TODO[dominio]: el dibujo real del gráfico (líneas verticales por apuesta sobre
        //   10 franjas de 959x25) se generaba en GraficoColumnasFrm_Paint con System.Drawing.
        //   En WinUI debe redibujarse sobre 'LienzoGrafico' (Canvas) o un CanvasControl (Win2D)
        //   tras leer las columnas con Free1X2.EntradaSalida.ArchivoColumnasTexto, aún no migrado.
    }
}
