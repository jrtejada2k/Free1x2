using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "DiFiltros" ("Diferencias entre filtros").
/// Carga una lista de archivos de columnas (filtros): el primero es la base y, para
/// cada filtro siguiente, conserva solo las columnas que difieren según la distancia
/// (difs 0-3) y el mínimo configurados por fila. Incluye un análisis de resultados
/// sobre una columna ganadora (14) y sus 28 variantes a 13.
/// </summary>
public sealed partial class DiFiltrosPage : Page
{
    public DiFiltrosViewModel ViewModel { get; } = new();

    public DiFiltrosPage()
    {
        InitializeComponent();
    }

    private void ActivarTodas_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        // ckMD.CheckedChanged -> MarcaDesmarca() en el WinForms legacy.
        ViewModel.MarcarDesmarcarCommand.Execute(null);
    }
}
