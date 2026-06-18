using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "Compresor" (título "Compresor *.z3q").
/// Comprime un archivo de columnas (*.txt) a formato propio *.z3q con un nivel
/// de compresión 0-9, y descomprime *.z3q de vuelta a *.txt. La lógica de
/// dominio (CompresorZip, ConvertidorDeBases, selección/guardado de archivos)
/// queda como TODO.
/// </summary>
public sealed partial class CompresorPage : Page
{
    public CompresorViewModel ViewModel { get; } = new();

    public CompresorPage()
    {
        InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }
}
