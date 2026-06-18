// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Una curva a representar: nombre (leyenda), color del trazo y sus puntos (x, y)
/// en coordenadas del "mundo real" (sin escalar). El control escala todas las curvas
/// a un mismo sistema, como hacía Free1X2/Utils/Grafico.cs.
/// </summary>
public sealed class CurvaGrafico
{
    public CurvaGrafico(string nombre, Color color, IReadOnlyList<(double X, double Y)> puntos)
    {
        Nombre = nombre;
        Color = color;
        Puntos = puntos;
    }

    public string Nombre { get; }
    public Color Color { get; }
    public IReadOnlyList<(double X, double Y)> Puntos { get; }
}

/// <summary>
/// Lienzo de líneas reutilizable. Réplica WinUI nativa de Free1X2/Utils/Grafico.cs:
/// dibuja ejes (DibujarEjes), una cuadrícula (DibujaGrid) y una polilínea por curva
/// (DibujaCurva) usando Microsoft.UI.Xaml.Shapes — sin System.Drawing ni Win2D.
///
/// El anfitrión sólo aporta los puntos ya calculados (colección <see cref="Curvas"/>);
/// el control se encarga del escalado a píxeles y de redibujar al cambiar los datos
/// o el tamaño. Equivale al ciclo del legacy: por la primera curva se pinta grid + ejes
/// y se fija la escala; las curvas siguientes se superponen con esa misma escala.
/// </summary>
public sealed partial class GraficoLineasControl : UserControl
{
    // Márgenes interiores para dejar sitio a los ejes/etiquetas (legacy: el lienzo
    // ocupaba todo el PictureBox; aquí dejamos un margen para las etiquetas numéricas).
    private const double MargenIzq = 44;
    private const double MargenInf = 24;
    private const double MargenSup = 12;
    private const double MargenDer = 12;

    public GraficoLineasControl()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Colección de curvas a dibujar. La aporta el ViewModel anfitrión. Al asignarla
    /// (o al mutarla, si es <see cref="ObservableCollection{T}"/>) se redibuja el lienzo.
    /// </summary>
    public ObservableCollection<CurvaGrafico> Curvas
    {
        get => (ObservableCollection<CurvaGrafico>)GetValue(CurvasProperty);
        set => SetValue(CurvasProperty, value);
    }

    public static readonly DependencyProperty CurvasProperty = DependencyProperty.Register(
        nameof(Curvas),
        typeof(ObservableCollection<CurvaGrafico>),
        typeof(GraficoLineasControl),
        new PropertyMetadata(null, OnCurvasChanged));

    /// <summary>Separación entre líneas de la cuadrícula, en píxeles (legacy: DibujaGrid(50)).</summary>
    public double IntervaloGrid
    {
        get => (double)GetValue(IntervaloGridProperty);
        set => SetValue(IntervaloGridProperty, value);
    }

    public static readonly DependencyProperty IntervaloGridProperty = DependencyProperty.Register(
        nameof(IntervaloGrid),
        typeof(double),
        typeof(GraficoLineasControl),
        new PropertyMetadata(50.0, OnRedrawPropertyChanged));

    private static void OnCurvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (GraficoLineasControl)d;

        if (e.OldValue is INotifyCollectionChanged viejo)
            viejo.CollectionChanged -= control.Curvas_CollectionChanged;
        if (e.NewValue is INotifyCollectionChanged nuevo)
            nuevo.CollectionChanged += control.Curvas_CollectionChanged;

        control.Redibujar();
    }

    private static void OnRedrawPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((GraficoLineasControl)d).Redibujar();

    private void Curvas_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Redibujar();

    private void Lienzo_SizeChanged(object sender, SizeChangedEventArgs e) => Redibujar();

    /// <summary>
    /// Redibuja el lienzo: limpia, calcula la escala común a todas las curvas y pinta
    /// cuadrícula, ejes, etiquetas y una polilínea por curva. UI thread (lo invoca WinUI).
    /// </summary>
    public void Redibujar()
    {
        if (Lienzo == null) return;

        Lienzo.Children.Clear();

        double ancho = Lienzo.ActualWidth;
        double alto = Lienzo.ActualHeight;
        if (ancho <= 0 || alto <= 0) return; // aún sin medir; se redibuja en SizeChanged.

        // Recorta el contenido al área del lienzo (Canvas no recorta por defecto en WinUI).
        Lienzo.Clip = new Microsoft.UI.Xaml.Media.RectangleGeometry
        {
            Rect = new Rect(0, 0, ancho, alto),
        };

        DibujarGrid(ancho, alto);

        var curvas = Curvas;
        if (curvas == null || curvas.Count == 0)
        {
            // Sin datos: sólo grid + ejes neutros (legacy: lienzo Beige vacío).
            DibujarEjes(ancho, alto, 0, 1, 0, 1);
            return;
        }

        // Máximos y mínimos comunes a todas las curvas (legacy: Grafico.ObtenerMaximosyMinimos).
        double xMin = double.MaxValue, xMax = double.MinValue;
        double yMin = double.MaxValue, yMax = double.MinValue;
        bool hayPunto = false;
        foreach (var curva in curvas)
        {
            foreach (var (x, y) in curva.Puntos)
            {
                hayPunto = true;
                xMin = Math.Min(xMin, x);
                xMax = Math.Max(xMax, x);
                yMin = Math.Min(yMin, y);
                yMax = Math.Max(yMax, y);
            }
        }
        if (!hayPunto)
        {
            DibujarEjes(ancho, alto, 0, 1, 0, 1);
            return;
        }

        // Evita división por cero cuando todos los X o todos los Y coinciden.
        if (xMax - xMin < 1e-9) { xMax = xMin + 1; }
        if (yMax - yMin < 1e-9) { yMax = yMin + 1; }

        DibujarEjes(ancho, alto, xMin, xMax, yMin, yMax);

        foreach (var curva in curvas)
            DibujarCurva(curva, ancho, alto, xMin, xMax, yMin, yMax);
    }

    /// <summary>Cuadrícula de líneas equiespaciadas (legacy: Grafico.DibujaGrid / Grid()).</summary>
    private void DibujarGrid(double ancho, double alto)
    {
        double intervalo = IntervaloGrid > 4 ? IntervaloGrid : 50;
        var color = ColorDeRecurso("AppBorderBrush", Color.FromArgb(0x40, 0x80, 0x80, 0x80));

        for (double x = MargenIzq; x <= ancho - MargenDer; x += intervalo)
            Lienzo.Children.Add(NuevaLinea(x, MargenSup, x, alto - MargenInf, color, 0.5));

        for (double y = MargenSup; y <= alto - MargenInf; y += intervalo)
            Lienzo.Children.Add(NuevaLinea(MargenIzq, y, ancho - MargenDer, y, color, 0.5));
    }

    /// <summary>
    /// Ejes X/Y y etiquetas de los valores extremos (legacy: Grafico.DibujarEjes,
    /// que dibujaba el eje horizontal y vertical con un Pen morado).
    /// </summary>
    private void DibujarEjes(double ancho, double alto, double xMin, double xMax, double yMin, double yMax)
    {
        var colorEje = ColorDeRecurso("AppTextSecondaryBrush", Color.FromArgb(0xFF, 0x33, 0x41, 0x55));

        double x0 = MargenIzq;
        double y0 = alto - MargenInf;

        // Eje Y (vertical) y eje X (horizontal).
        Lienzo.Children.Add(NuevaLinea(x0, MargenSup, x0, y0, colorEje, 1.5));
        Lienzo.Children.Add(NuevaLinea(x0, y0, ancho - MargenDer, y0, colorEje, 1.5));

        // Etiquetas de los extremos de cada eje.
        AnadirTexto(FormatearValor(yMax), 2, MargenSup - 2, colorEje);
        AnadirTexto(FormatearValor(yMin), 2, y0 - 8, colorEje);
        AnadirTexto(FormatearValor(xMin), x0, y0 + 4, colorEje);
        AnadirTexto(FormatearValor(xMax), ancho - MargenDer - 28, y0 + 4, colorEje);
    }

    /// <summary>
    /// Dibuja una curva como polilínea escalada al área de dibujo (legacy: Grafico.DibujaCurva,
    /// que usaba DrawCurve con tensión 0.1; aquí una polilínea recta entre puntos).
    /// </summary>
    private void DibujarCurva(CurvaGrafico curva, double ancho, double alto,
        double xMin, double xMax, double yMin, double yMax)
    {
        if (curva.Puntos.Count == 0) return;

        double areaW = ancho - MargenIzq - MargenDer;
        double areaH = alto - MargenSup - MargenInf;
        if (areaW <= 0 || areaH <= 0) return;

        var poli = new Polyline
        {
            Stroke = new SolidColorBrush(curva.Color),
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Round,
        };

        foreach (var (x, y) in curva.Puntos)
        {
            double px = MargenIzq + (x - xMin) / (xMax - xMin) * areaW;
            // Y invertida: valores mayores arriba (legacy: _Ymax - Punto.Y).
            double py = MargenSup + (yMax - y) / (yMax - yMin) * areaH;
            poli.Points.Add(new Point(px, py));
        }

        Lienzo.Children.Add(poli);

        // Marcadores de punto, para curvas con pocos puntos (tramos).
        if (curva.Puntos.Count <= 64)
        {
            var relleno = new SolidColorBrush(curva.Color);
            foreach (var p in poli.Points)
            {
                var dot = new Ellipse { Width = 5, Height = 5, Fill = relleno };
                Canvas.SetLeft(dot, p.X - 2.5);
                Canvas.SetTop(dot, p.Y - 2.5);
                Lienzo.Children.Add(dot);
            }
        }
    }

    private static Line NuevaLinea(double x1, double y1, double x2, double y2, Color color, double grosor) => new()
    {
        X1 = x1,
        Y1 = y1,
        X2 = x2,
        Y2 = y2,
        Stroke = new SolidColorBrush(color),
        StrokeThickness = grosor,
    };

    private void AnadirTexto(string texto, double left, double top, Color color)
    {
        var tb = new TextBlock
        {
            Text = texto,
            FontSize = 11,
            Foreground = new SolidColorBrush(color),
        };
        Canvas.SetLeft(tb, left);
        Canvas.SetTop(tb, top);
        Lienzo.Children.Add(tb);
    }

    private static string FormatearValor(double v)
        => Math.Abs(v - Math.Round(v)) < 1e-6 ? ((long)Math.Round(v)).ToString() : v.ToString("0.##");

    /// <summary>Obtiene el Color de un SolidColorBrush de recursos, o un fallback si no existe.</summary>
    private Color ColorDeRecurso(string clave, Color porDefecto)
    {
        if (Resources.TryGetValue(clave, out var r) && r is SolidColorBrush b1) return b1.Color;
        if (Application.Current.Resources.TryGetValue(clave, out var r2) && r2 is SolidColorBrush b2) return b2.Color;
        return porDefecto;
    }
}
