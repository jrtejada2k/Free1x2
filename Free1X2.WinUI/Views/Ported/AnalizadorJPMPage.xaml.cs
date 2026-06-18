// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "AnalizadorJPM" (AnalizadorJPMFrm),
/// titulado "Sumas Pares Naturales (JPM)".
///
/// Permite asignar una puntuación a cada par de signos, clasificar las columnas
/// de un fichero en 36 casillas de suma (0..35), grabar las seleccionadas, y
/// analizar los premios (10..14) de una columna ganadora navegable.
///
/// La lógica de dominio (cálculo, ficheros, búsqueda de premios) está marcada
/// como TODO en AnalizadorJPMViewModel, citando los métodos legacy correspondientes.
/// </summary>
public sealed partial class AnalizadorJPMPage : Page
{
    public AnalizadorJPMViewModel ViewModel { get; } = new();

    public AnalizadorJPMPage()
    {
        InitializeComponent();
    }
}
