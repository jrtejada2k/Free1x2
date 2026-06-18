// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Estados posibles del semáforo de filtros (réplica del control WinForms).
/// </summary>
public enum EstadoSemaforo
{
    /// <summary>Filtro vacío / sin datos.</summary>
    Neutro,
    /// <summary>Filtro activo y correcto.</summary>
    Verde,
    /// <summary>Filtro con datos pero en error.</summary>
    Rojo
}

/// <summary>
/// Indicador visual de 3 estados que replica el semáforo de filtros del WinForms.
/// </summary>
public sealed partial class SemaforoControl : UserControl
{
    public SemaforoControl()
    {
        this.InitializeComponent();
        this.Loaded += (_, _) => ActualizarVisual();
    }

    /// <summary>
    /// Estado actual del semáforo. Controla color y texto accesible.
    /// </summary>
    public EstadoSemaforo Estado
    {
        get => (EstadoSemaforo)GetValue(EstadoProperty);
        set => SetValue(EstadoProperty, value);
    }

    public static readonly DependencyProperty EstadoProperty =
        DependencyProperty.Register(
            nameof(Estado),
            typeof(EstadoSemaforo),
            typeof(SemaforoControl),
            new PropertyMetadata(EstadoSemaforo.Neutro, OnEstadoChanged));

    /// <summary>Texto accesible derivado del estado (para AutomationProperties.Name).</summary>
    public string EstadoTexto => Estado switch
    {
        EstadoSemaforo.Verde => "Filtro activo",
        EstadoSemaforo.Rojo  => "Filtro con error",
        _                    => "Filtro vacío"
    };

    private static void OnEstadoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SemaforoControl ctrl)
            ctrl.ActualizarVisual();
    }

    private void ActualizarVisual()
    {
        if (Punto is null || Halo is null)
            return;

        switch (Estado)
        {
            case EstadoSemaforo.Verde:
                Punto.Fill = BrushDeRecurso("AppSuccessBrush");
                Punto.Opacity = 1;
                Halo.Stroke = BrushDeRecurso("AppSuccessBrush");
                Halo.Opacity = 0.35;
                break;
            case EstadoSemaforo.Rojo:
                Punto.Fill = BrushDeRecurso("AppErrorBrush");
                Punto.Opacity = 1;
                Halo.Stroke = BrushDeRecurso("AppErrorBrush");
                Halo.Opacity = 0.35;
                break;
            default:
                Punto.Fill = BrushDeRecurso("AppBorderBrush");
                Punto.Opacity = 0.55;
                Halo.Opacity = 0;
                break;
        }

        // Mantener sincronizado el nombre accesible.
        AutomationProperties.SetName(RootGrid, EstadoTexto);
    }

    private Brush BrushDeRecurso(string clave)
    {
        // Prefiere los recursos locales/tema del control; cae a los de App.
        if (Resources.TryGetValue(clave, out var local) && local is Brush lb)
            return lb;
        if (Application.Current.Resources.TryGetValue(clave, out var app) && app is Brush ab)
            return ab;
        return new SolidColorBrush(Microsoft.UI.Colors.Gray);
    }
}
