using System.Numerics;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Propiedad adjunta para aplicar elevación Fluent a las tarjetas (AppCardStyle).
///
/// Motivo: <see cref="UIElement.Translation"/> NO se puede establecer desde un
/// Setter de Style en WinUI 3 (el compilador XAML lo rechaza por ser una
/// propiedad respaldada por composición). Para que un <c>ThemeShadow</c>
/// proyecte sombra, el elemento que la emite necesita una Translation con
/// componente Z &gt; 0. Esta propiedad adjunta SÍ es válida en un Setter de
/// Style, de modo que un único estilo compartido la aplica a todas las
/// tarjetas a la vez sin tocar cada página.
///
/// Sólo es visual: ajusta la Translation (profundidad), no la posición X/Y ni
/// el tamaño/disposición del control.
/// </summary>
public static class CardElevation
{
    /// <summary>
    /// Profundidad (componente Z de la Translation). Establecerla a un valor
    /// &gt; 0 hace que la tarjeta proyecte la sombra Fluent compartida. 0 la
    /// desactiva. Valor sutil/profesional recomendado: 16.
    /// </summary>
    public static readonly DependencyProperty DepthProperty =
        DependencyProperty.RegisterAttached(
            "Depth",
            typeof(double),
            typeof(CardElevation),
            new PropertyMetadata(0.0, OnDepthChanged));

    public static double GetDepth(DependencyObject obj) => (double)obj.GetValue(DepthProperty);

    public static void SetDepth(DependencyObject obj, double value) => obj.SetValue(DepthProperty, value);

    private static void OnDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIElement element)
            return;

        var z = (float)(double)e.NewValue;

        // La Translation puede fijarse en cuanto el elemento existe; si aún no
        // está cargado, se vuelve a aplicar en Loaded por seguridad.
        element.Translation = new Vector3(0f, 0f, z);

        if (element is FrameworkElement fe)
        {
            fe.Loaded -= OnLoaded;
            fe.Loaded += OnLoaded;
        }
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is UIElement element)
        {
            var z = (float)GetDepth(element);
            element.Translation = new Vector3(0f, 0f, z);
        }
    }
}
