// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "DifCols" (título "Diferencias entre columnas").
/// Genera columnas — internamente las 14 triples o leyéndolas de un fichero — y las
/// acepta sólo si su número de diferencias respecto a la columna base (o fichero de
/// condiciones) y respecto a las columnas ya aceptadas cae dentro de los rangos
/// indicados. La lógica de dominio (VerColumna/PreparaDifs/MetodoInterno/MetodoExterno/
/// Proceso/Grabar, selección de archivos y apertura de GeneradorCPSDiferencias) queda
/// como TODO.
/// </summary>
public sealed partial class DifColsPage : Page
{
    public DifColsViewModel ViewModel { get; } = new();

    public DifColsPage()
    {
        InitializeComponent();
    }
}
