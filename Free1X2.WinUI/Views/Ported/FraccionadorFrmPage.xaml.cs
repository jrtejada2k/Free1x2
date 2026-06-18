// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "Fraccionador" (Free1X2.UI.FraccionadorFrm,
/// título "Fraccionador"). Carga un archivo de columnas (*.txt) y lo divide en
/// varios archivos de salida numerados (BASE01.txt, BASE02.txt, …) en dos modos:
///   - "Por columnas": 20 cuotas fijas (legacy FracCols / tbcol01..tbcol20).
///   - "Por tramos": N partes iguales (legacy FracTrams / tbqnts).
/// Muestra total de columnas, archivos generados y tiempo transcurrido.
/// La lógica de dominio (lectura/escritura de archivos, ConvertidorDeBases,
/// medición de tiempo) queda como TODO en el ViewModel.
/// </summary>
public sealed partial class FraccionadorFrmPage : Page
{
    public FraccionadorFrmViewModel ViewModel { get; } = new();

    public FraccionadorFrmPage()
    {
        InitializeComponent();
    }
}
