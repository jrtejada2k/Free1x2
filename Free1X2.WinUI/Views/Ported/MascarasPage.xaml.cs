// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// «Máscaras» (W9 + W12, DT-84/DT-88; pantalla nueva, no existe en el WinForms): máscaras
/// guardadas, aplicarlas al boleto y escrutarlas contra la base de ganadoras de una liga.
/// </summary>
public sealed partial class MascarasPage : Page
{
    public MascarasViewModel ViewModel { get; } = new();

    public MascarasPage()
    {
        InitializeComponent();
    }

    // «Aplicar al boleto»: aplica y vuelve a Inicio (como raíz, igual que la app Mac).
    private void Aplicar_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not FilaMascaraGuardada fila) return;
        ViewModel.Aplicar(fila);
        Frame?.Navigate(typeof(MainPage));
        Frame?.BackStack.Clear();
    }

    private void Borrar_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is FilaMascaraGuardada fila)
            AppServices.EjecutarObservandoErrores(() => ViewModel.BorrarAsync(fila), "MascarasPage.Borrar");
    }

    private void Cabecera_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is string columna) ViewModel.OrdenarPor(columna);
    }
}
