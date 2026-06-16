using System.Collections.Specialized;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "GraficoColumnasFrm".
/// Abre un fichero de columnas (.txt), calcula los límites (mínimo/máximo) de la combinación
/// y la representa gráficamente en 10 franjas. La "Guía" indica, para cada franja, qué fila
/// de la quiniela queda representada.
/// </summary>
public sealed partial class GraficoColumnasFrmPage : Page
{
    // Geometría legacy (GraficoColumnasFrm_Paint): 10 franjas de 959 px de ancho x 25 px de alto,
    // con marco en x=9. Las líneas por apuesta van de y=alto*25+51 a alto*25+74, x=ancho+10.
    private const int NumFranjas = 10;
    private const double AnchoFranja = 959;
    private const double AltoFranja = 25;
    private const double MargenX = 9;     // x del marco de cada franja (legacy: DrawRectangle 9,...)
    private const double YBase = 50;      // y de la primera franja (legacy: (y*25)+50)
    private const double LienzoAncho = MargenX + AnchoFranja + MargenX;            // ~977
    private const double LienzoAlto = YBase + NumFranjas * AltoFranja + MargenX;   // ~309

    public GraficoColumnasFrmViewModel ViewModel { get; } = new();

    public GraficoColumnasFrmPage()
    {
        InitializeComponent();

        // El render real del gráfico (rectángulos de relleno granate + líneas verticales por apuesta
        // sobre 10 franjas de 959x25) se hace aquí con WinUI Shapes a partir de las coordenadas ya
        // calculadas por el VM en ViewModel.LineasGrafico y ViewModel.RectangulosRelleno. Se redibuja
        // cuando cualquiera de esas colecciones cambia.
        ViewModel.LineasGrafico.CollectionChanged += LineasGrafico_CollectionChanged;
        ViewModel.RectangulosRelleno.CollectionChanged += LineasGrafico_CollectionChanged;
        Loaded += (_, _) => Redibujar();
    }

    private void LineasGrafico_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => Redibujar();

    /// <summary>
    /// Dibuja en el Canvas las 10 franjas (marcos) y una línea vertical por apuesta, a la escala
    /// legacy. Réplica de GraficoColumnasFrm_Paint (Free1X2/UI/GraficoColumnasFrm.cs, líneas 220-301):
    /// DrawRectangle de los 10 marcos + DrawLine de cada apuesta. Las coordenadas (X, Y0, Y1) ya
    /// vienen calculadas en ViewModel.LineasGrafico.
    /// </summary>
    private void Redibujar()
    {
        if (LienzoGrafico == null) return;

        LienzoGrafico.Children.Clear();
        LienzoGrafico.Width = LienzoAncho;
        LienzoGrafico.Height = LienzoAlto;

        Color colorMarco = ColorDeRecurso("AppBorderBrush", Color.FromArgb(0xFF, 0xCB, 0xD5, 0xE1));
        Color colorLinea = ColorDeRecurso("AppAccentBrush", Color.FromArgb(0xFF, 0x4F, 0x46, 0xE5));
        // Granate (legacy: Color.Maroon = #800000) para los rectángulos de relleno.
        Color colorRelleno = Color.FromArgb(0xFF, 0x80, 0x00, 0x00);

        // Marcos de las 10 franjas (legacy: DrawRectangle(marco, 9, y*25+50, 959, 25)).
        var pincelMarco = new SolidColorBrush(colorMarco);
        for (int y = 0; y < NumFranjas; y++)
        {
            var rect = new Rectangle
            {
                Width = AnchoFranja,
                Height = AltoFranja,
                Stroke = pincelMarco,
                StrokeThickness = 1,
                Fill = null,
            };
            Canvas.SetLeft(rect, MargenX);
            Canvas.SetTop(rect, (y * AltoFranja) + YBase);
            LienzoGrafico.Children.Add(rect);
        }

        // Rectángulos de relleno granate (legacy: FillRectangle(SolidBrush(Maroon), ...), líneas
        // 268-273): delimitan la zona sobrante cuando la combinación es estrecha (diferencia < 9566).
        // Se pintan antes que las líneas para que estas queden visibles encima (orden del legacy).
        var pincelRelleno = new SolidColorBrush(colorRelleno);
        foreach (var (rx, ry, rw, rh) in ViewModel.RectangulosRelleno)
        {
            if (rw <= 0 || rh <= 0) continue; // sin área visible (ej. maximo >= 958).
            var rect = new Rectangle
            {
                Width = rw,
                Height = rh,
                Fill = pincelRelleno,
            };
            Canvas.SetLeft(rect, rx);
            Canvas.SetTop(rect, ry);
            LienzoGrafico.Children.Add(rect);
        }

        // Una línea vertical por apuesta (legacy: DrawLine(myPen, ancho+10, alto*25+51, ancho+10, alto*25+74)).
        var pincelLinea = new SolidColorBrush(colorLinea);
        foreach (var (x, y0, y1) in ViewModel.LineasGrafico)
        {
            var linea = new Line
            {
                X1 = x,
                Y1 = y0,
                X2 = x,
                Y2 = y1,
                Stroke = pincelLinea,
                StrokeThickness = 1,
            };
            LienzoGrafico.Children.Add(linea);
        }
    }

    /// <summary>Obtiene el Color de un SolidColorBrush de recursos, o un fallback si no existe.</summary>
    private Color ColorDeRecurso(string clave, Color porDefecto)
    {
        if (Application.Current.Resources.TryGetValue(clave, out var r) && r is SolidColorBrush b)
            return b.Color;
        return porDefecto;
    }
}
