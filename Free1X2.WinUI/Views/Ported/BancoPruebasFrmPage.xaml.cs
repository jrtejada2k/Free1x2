// Free1X2 · WinUI 3 — WIN3
using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "BancoPruebasFrm"
/// (Simulador de escrutinios — Banco de Pruebas).
///
/// Asistente de 4 pasos: Ficheros, Valoraciones, Simular 14's y Escrutinios.
/// La lógica de cálculo / persistencia está implementada en el ViewModel.
/// </summary>
public sealed partial class BancoPruebasFrmPage : Page
{
    public BancoPruebasFrmViewModel ViewModel { get; } = new();

    public BancoPruebasFrmPage()
    {
        this.InitializeComponent();

        // Legacy btGrabar_Click: new DialogoGrabarBancoPruebasFrm(c1, c2, c).ShowDialog(). El VM pide
        // el rango de grabación y aquí se muestra DialogoGrabarBancoPruebasFrmPage como ContentDialog
        // (modal, fiel al ShowDialog original), devolviendo el resultado al VM.
        ViewModel.SolicitarRangoGrabacion = MostrarDialogoRangoGrabacionAsync;

        // W19 (DT-95): la selección de filas vive en ResultadoColumnaItem.Seleccionado (la usan Grabar y Simplificar);
        // el ListView (selección múltiple) y el filtro la mantienen sincronizada.
        ViewModel.SeleccionActualizada += (_, _) => SincronizarSeleccion();
        ViewModel.SolicitarFiltro = MostrarFiltroAsync;
    }

    private bool _sincronizando;

    // Selección con el ratón (Ctrl / Mayús) ⇒ Seleccionado de cada fila.
    private void ListaColumnas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_sincronizando) return;
        foreach (var o in e.AddedItems) if (o is ResultadoColumnaItem a) a.Seleccionado = true;
        foreach (var o in e.RemovedItems) if (o is ResultadoColumnaItem r) r.Seleccionado = false;
        ViewModel.ActualizarFilasSeleccionadas();
    }

    // Seleccionado (filtro, Simplificar, nueva tabla u orden) ⇒ selección del ListView.
    private void SincronizarSeleccion()
    {
        _sincronizando = true;
        try
        {
            ListaColumnas.SelectedItems.Clear();
            foreach (var item in ViewModel.ColumnasResultado)
                if (item.Seleccionado) ListaColumnas.SelectedItems.Add(item);
        }
        finally
        {
            _sincronizando = false;
        }
    }

    private void CabeceraColumna_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: string t } && int.TryParse(t, out int cabecera))
            ViewModel.OrdenarPor(cabecera);
    }

    /// <summary>
    /// «Selección de columnas» (WinForms DialogoSeleccionBancoPruebasFrm): 18 conceptos con casilla, mínimo y máximo;
    /// valores iniciales = mínimo y máximo de cada concepto entre las filas, ninguno marcado. Devuelve los marcados (un
    /// campo vacío llega como NaN, que no limita) o null si se cancela.
    /// </summary>
    private async Task<(int Concepto, double Minimo, double Maximo)[]?> MostrarFiltroAsync((double Minimo, double Maximo)[] limites)
    {
        var conceptos = Free1X2.Analisis.BancoPruebasTabla.Conceptos;
        var casillas = new CheckBox[conceptos.Length];
        var minimos = new NumberBox[conceptos.Length];
        var maximos = new NumberBox[conceptos.Length];

        var tabla = new Grid { ColumnSpacing = 8, RowSpacing = 2 };
        tabla.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(210) });
        tabla.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        tabla.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
        tabla.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        var cabMin = new TextBlock { Text = "Mínimo", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
        var cabMax = new TextBlock { Text = "Máximo", FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
        Grid.SetColumn(cabMin, 1);
        Grid.SetColumn(cabMax, 2);
        tabla.Children.Add(cabMin);
        tabla.Children.Add(cabMax);

        for (int i = 0; i < conceptos.Length; i++)
        {
            tabla.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            casillas[i] = new CheckBox { Content = conceptos[i], MinWidth = 0 };
            minimos[i] = new NumberBox { Value = limites[i].Minimo, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Hidden };
            maximos[i] = new NumberBox { Value = limites[i].Maximo, SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Hidden };
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(minimos[i], conceptos[i] + ": mínimo");
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(maximos[i], conceptos[i] + ": máximo");
            Grid.SetRow(casillas[i], i + 1);
            Grid.SetRow(minimos[i], i + 1);
            Grid.SetRow(maximos[i], i + 1);
            Grid.SetColumn(minimos[i], 1);
            Grid.SetColumn(maximos[i], 2);
            tabla.Children.Add(casillas[i]);
            tabla.Children.Add(minimos[i]);
            tabla.Children.Add(maximos[i]);
        }

        var contenido = new StackPanel { Spacing = 10 };
        contenido.Children.Add(new TextBlock
        {
            Text = "Se seleccionarán las columnas comprendidas entre los valores mínimo y máximo de los conceptos marcados.",
            TextWrapping = TextWrapping.Wrap,
        });
        contenido.Children.Add(tabla);

        var dialogo = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "Selección de columnas",
            Content = new ScrollViewer { Content = contenido, MaxHeight = 520 },
            PrimaryButtonText = "Aceptar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            RequestedTheme = Services.TemaApp.TemaDeElemento,
        };
        if (await dialogo.ShowAsync() != ContentDialogResult.Primary) return null;

        var marcados = new System.Collections.Generic.List<(int, double, double)>();
        for (int i = 0; i < conceptos.Length; i++)
            if (casillas[i].IsChecked == true) marcados.Add((i, minimos[i].Value, maximos[i].Value));
        return marcados.ToArray();
    }

    /// <summary>
    /// Muestra el diálogo de rango de grabación hospedando DialogoGrabarBancoPruebasFrmPage en un
    /// ContentDialog. Inicializa los campos (c1, c2, c) como el ctor legacy y, cuando la VM del
    /// diálogo solicita el cierre (Aceptar/Cancelar), recoge el resultado.
    /// </summary>
    private async Task<(bool aceptado, int filaInicial, int filaFinal, int numMaxColumnas, bool soloSeleccionadas)>
        MostrarDialogoRangoGrabacionAsync(int c1, int c2, int c)
    {
        var dialogPage = new DialogoGrabarBancoPruebasFrmPage();
        dialogPage.ViewModel.Inicializar(c1, c2, c);

        var contentDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Content = dialogPage,
        };

        // La VM del diálogo levanta CierreSolicitado al Aceptar/Cancelar (sus botones internos);
        // cerramos el ContentDialog para devolver el control al flujo de grabación.
        void OnCierre(object? s, EventArgs e) => contentDialog.Hide();
        dialogPage.ViewModel.CierreSolicitado += OnCierre;

        try
        {
            await contentDialog.ShowAsync();
        }
        finally
        {
            dialogPage.ViewModel.CierreSolicitado -= OnCierre;
        }

        var vm = dialogPage.ViewModel;
        return (vm.Aceptado, vm.FilaInicialResultado, vm.FilaFinalResultado, vm.NumMaxColumnasResultado, vm.SoloSeleccionadas);
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // El form legacy cerraba la ventana (btnCancel). Aquí se navega hacia atrás si es posible.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
