using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms legacy Free1X2.UI.ListaImpresoras
/// ("Lista Impresoras Conocidas").
/// </summary>
public sealed partial class ListaImpresorasPage : Page
{
    public ListaImpresorasViewModel ViewModel { get; } = new();

    public ListaImpresorasPage()
    {
        InitializeComponent();
    }

    // Legacy: listBox1_DoubleClick — selecciona la impresora y aplica su configuración.
    private void OnImpresoraDoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        if (sender is ListView { SelectedItem: string modelo })
        {
            ViewModel.SeleccionarImpresoraCommand.Execute(modelo);
        }
    }
}
