using Microsoft.UI.Xaml.Controls;

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
}
