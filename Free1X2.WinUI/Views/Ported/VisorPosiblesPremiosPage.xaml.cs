// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "VisorPosiblesPremios" (título "Visor de Posibles Premios").
///
/// Propósito: visor de solo lectura que recorre una lista de grupos de premios ya calculados
/// (legacy: List&lt;PosiblesPremiosContenedor&gt;). Para el grupo actual muestra su columna
/// ganadora (ColGanadora) en vertical y las columnas jugadas que optan a premio agrupadas por
/// categoría (Col16..Col10), con navegación adelante/atrás y un contador "X de N"
/// (legacy: btnAdelante '>', btnAtras '&lt;', lblContador).
/// </summary>
public sealed partial class VisorPosiblesPremiosPage : Page
{
    public VisorPosiblesPremiosViewModel ViewModel { get; } = new();

    public VisorPosiblesPremiosPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Fondo de una casilla de signo: rojo cuando es un fallo respecto a la columna ganadora
    /// (legacy: ControlPosiblesPremios pinta BackColor = Red cuando columna[i] != ganadora[i]).
    /// </summary>
    public static Brush FondoSigno(bool esAcierto)
    {
        string clave = esAcierto ? "AppSurfaceAltBrush" : "AppErrorBrush";
        if (Application.Current.Resources.TryGetValue(clave, out var recurso) && recurso is Brush brush)
        {
            return brush;
        }
        return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }

    /// <summary>Color del texto de un signo: blanco sobre fondo rojo (fallo) para legibilidad.</summary>
    public static Brush TextoSigno(bool esAcierto)
    {
        if (esAcierto &&
            Application.Current.Resources.TryGetValue("AppTextBrush", out var recurso) && recurso is Brush brush)
        {
            return brush;
        }
        // Fallo: texto blanco para contraste sobre el rojo de error.
        return new SolidColorBrush(Microsoft.UI.Colors.White);
    }
}
