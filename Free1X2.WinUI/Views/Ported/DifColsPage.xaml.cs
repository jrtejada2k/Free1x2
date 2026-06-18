// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "DifCols" (título "Diferencias entre columnas").
/// Genera columnas — internamente las 14 triples o leyéndolas de un fichero — y las
/// acepta sólo si su número de diferencias respecto a la columna base (o fichero de
/// condiciones) y respecto a las columnas ya aceptadas cae dentro de los rangos
/// indicados. La lógica de dominio (VerColumna/PreparaDifs/MetodoInterno/MetodoExterno/
/// Proceso/Grabar y la selección de archivos) está implementada en el ViewModel; la apertura
/// de GeneradorCPSDiferencias se delega al host de la Page (Frame.Navigate).
/// </summary>
public sealed partial class DifColsPage : Page
{
    public DifColsViewModel ViewModel { get; } = new();

    public DifColsPage()
    {
        InitializeComponent();
    }
}
