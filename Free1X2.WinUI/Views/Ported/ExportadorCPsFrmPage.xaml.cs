// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>ExportadorCPsFrm</c>
/// ("Exportador de Columnas Probables"). Diálogo sin entradas que ofrece exportar
/// la lista de columnas probables a fichero en dos formatos (simple .txt y con
/// aciertos .clm) o cancelar. La selección de fichero y la escritura están
/// implementadas en el ViewModel
/// (MotorCalculo.ColumnaProbable / EntradaSalida.ArchivoColumnasTexto).
/// </summary>
public sealed partial class ExportadorCPsFrmPage : Page
{
    public ExportadorCPsFrmViewModel ViewModel { get; } = new();

    public ExportadorCPsFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }
}
