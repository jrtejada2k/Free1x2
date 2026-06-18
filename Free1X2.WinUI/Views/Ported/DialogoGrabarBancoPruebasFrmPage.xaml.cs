// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>DialogoGrabarBancoPruebasFrm</c> ("Grabar apuestas").
/// Diálogo que recoge el rango de filas (inicial/final), el número máximo de
/// apuestas y si solo deben grabarse las seleccionadas, para exportar las
/// columnas del Banco de Pruebas a un archivo.
/// </summary>
public sealed partial class DialogoGrabarBancoPruebasFrmPage : Page
{
    public DialogoGrabarBancoPruebasFrmViewModel ViewModel { get; } = new();

    public DialogoGrabarBancoPruebasFrmPage()
    {
        this.InitializeComponent();
    }
}
