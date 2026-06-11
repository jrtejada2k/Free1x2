using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "ColumnasPremiadasFrm".
/// Muestra un resumen (rejilla de 6 columnas: Arch. Columnas, Jornada, Columna, Premio,
/// Nº Col., Nº Boleto) de las columnas premiadas encontradas, y permite exportar todas
/// las columnas o solo las seleccionadas a un fichero de texto.
/// </summary>
public sealed partial class ColumnasPremiadasFrmPage : Page
{
    public ColumnasPremiadasFrmViewModel ViewModel { get; } = new();

    public ColumnasPremiadasFrmPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Exporta únicamente las filas seleccionadas en el ListView.
    /// Se gestiona en code-behind porque la selección múltiple vive en el control
    /// (legacy: btnGuardarSeleccionadas_Click recorría listaResumen.Items[i].Selected).
    /// </summary>
    private void OnGuardarSeleccionadasClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var seleccionadas = new List<ColumnaPremiadaItem>();
        foreach (var item in ListaResumen.SelectedItems)
        {
            if (item is ColumnaPremiadaItem cp)
            {
                seleccionadas.Add(cp);
            }
        }

        ViewModel.GuardarSeleccionadas(seleccionadas);
    }
}
