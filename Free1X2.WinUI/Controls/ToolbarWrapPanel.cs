// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Panel de flujo horizontal con salto de línea, usado por la barra de
/// herramientas de <c>MainWindow</c>.
///
/// Motivo: la barra original (WinForms) coloca los seis ToolStrips en fila y,
/// cuando el ancho disponible se agota, SALTA a una segunda fila
/// (<c>ObtenerPosicionBarraHerramientas</c>, MainForm.cs: <c>y += 200</c>). Con
/// ~55 botones la fila única no cabe en la ventana (~1020px) y un
/// <see cref="ScrollViewer"/> horizontal dejaba la cola (Utilidades) fuera de
/// vista. Este panel replica el comportamiento del original: nada se recorta,
/// los botones que no caben fluyen a la siguiente fila y la barra crece en alto.
///
/// WinUI 3 no trae un WrapPanel integrado, de ahí esta implementación mínima.
/// Sólo organiza la disposición (medida/posición); no cambia el orden, el
/// número ni la identidad de los elementos hijos.
/// </summary>
public sealed class ToolbarWrapPanel : Panel
{
    /// <summary>Separación horizontal entre elementos de la misma fila (px).</summary>
    public double HorizontalSpacing { get; set; } = 1;

    /// <summary>Separación vertical entre filas cuando se produce el salto (px).</summary>
    public double VerticalSpacing { get; set; } = 2;

    protected override Size MeasureOverride(Size availableSize)
    {
        double anchoLimite = double.IsInfinity(availableSize.Width) ? double.MaxValue : availableSize.Width;

        double xFila = 0;          // ancho acumulado de la fila en curso
        double altoFila = 0;       // alto de la fila en curso
        double anchoTotal = 0;     // ancho máximo medido (la fila más ancha)
        double altoTotal = 0;      // alto total acumulado de todas las filas

        foreach (var hijo in Children)
        {
            // Los elementos ocultos (grupo desactivado en "Ver → Barras de
            // Herramientas") no ocupan espacio ni dejan hueco.
            if (hijo.Visibility == Visibility.Collapsed)
                continue;

            hijo.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Size ds = hijo.DesiredSize;

            double avance = (xFila > 0 ? HorizontalSpacing : 0) + ds.Width;

            // Si no cabe en la fila actual (y ya hay algo en ella), salta a otra fila.
            if (xFila > 0 && xFila + avance > anchoLimite)
            {
                anchoTotal = Math.Max(anchoTotal, xFila);
                altoTotal += altoFila + VerticalSpacing;
                xFila = ds.Width;
                altoFila = ds.Height;
            }
            else
            {
                xFila += avance;
                altoFila = Math.Max(altoFila, ds.Height);
            }
        }

        anchoTotal = Math.Max(anchoTotal, xFila);
        altoTotal += altoFila;

        double anchoFinal = double.IsInfinity(availableSize.Width) ? anchoTotal : availableSize.Width;
        return new Size(anchoFinal, altoTotal);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double anchoLimite = finalSize.Width;

        double x = 0;
        double y = 0;
        double altoFila = 0;

        foreach (var hijo in Children)
        {
            if (hijo.Visibility == Visibility.Collapsed)
                continue;

            Size ds = hijo.DesiredSize;
            double avance = (x > 0 ? HorizontalSpacing : 0) + ds.Width;

            if (x > 0 && x + avance > anchoLimite)
            {
                // Salto de fila.
                x = 0;
                y += altoFila + VerticalSpacing;
                altoFila = 0;
                avance = ds.Width;
            }

            double offsetX = x + (avance - ds.Width); // respeta el espaciado previo
            hijo.Arrange(new Rect(offsetX, y, ds.Width, ds.Height));

            x += avance;
            altoFila = Math.Max(altoFila, ds.Height);
        }

        return finalSize;
    }
}
