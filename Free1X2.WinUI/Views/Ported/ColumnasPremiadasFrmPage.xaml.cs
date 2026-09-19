// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "ColumnasPremiadasFrm".
/// Muestra un resumen (rejilla de 6 columnas: Arch. Columnas, Jornada, Columna, Premio,
/// Nº Col., Nº Boleto) de las columnas premiadas encontradas, y permite exportar todas
/// las columnas o solo las seleccionadas a un fichero de texto.
/// </summary>
public sealed partial class ColumnasPremiadasFrmPage : Page
{
    /// <summary>
    /// Handoff estático con las columnas premiadas a mostrar (legacy: el productor —
    /// EscrutiniosFrm.btnVerPremiadas_Click / EscrutarCombinacionesFrm.btnVerPremiadas_Click —
    /// rellenaba form.listaResumen.Items ANTES de form.ShowDialog()). El productor coloca aquí
    /// las filas y navega a esta página; OnNavigatedTo las vuelca en el ViewModel (mismo patrón
    /// estático que DialogoFiltrarPorLimitesFrmPage.ExtremosEntrada).
    /// </summary>
    public static IReadOnlyList<ColumnaPremiadaItem>? Entrada { get; set; }

    public ColumnasPremiadasFrmViewModel ViewModel { get; } = new();

    public ColumnasPremiadasFrmPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        // Vuelca las premiadas que dejó el productor (Escrutinios / EscrutarCombinaciones) y
        // consume el handoff para no arrastrarlo a futuras navegaciones.
        if (Entrada is { } filas)
        {
            // C-13: un único Reset en lugar de un CollectionChanged por premiada (el resumen
            // puede traer miles de filas). El orden es el del productor, sin cambios.
            ViewModel.Columnas.ReemplazarTodo(filas);
            Entrada = null;
        }
    }

    /// <summary>
    /// Exporta únicamente las filas seleccionadas en el ListView.
    /// Se gestiona en code-behind porque la selección múltiple vive en el control
    /// (legacy: btnGuardarSeleccionadas_Click recorría listaResumen.Items[i].Selected).
    /// </summary>
    // C-27: el async void vive aquí (es un manejador de evento, que es donde procede) y hace
    // await del método del ViewModel, que ahora devuelve Task.
    private async void OnGuardarSeleccionadasClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var seleccionadas = new List<ColumnaPremiadaItem>();
        foreach (var item in ListaResumen.SelectedItems)
        {
            if (item is ColumnaPremiadaItem cp)
            {
                seleccionadas.Add(cp);
            }
        }

        await ViewModel.GuardarSeleccionadasAsync(seleccionadas);
    }
}
