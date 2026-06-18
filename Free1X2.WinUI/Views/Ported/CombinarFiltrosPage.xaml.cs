// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

public sealed partial class CombinarFiltrosPage : Page
{
    public CombinarFiltrosViewModel ViewModel { get; } = new();

    public CombinarFiltrosPage()
    {
        this.InitializeComponent();
    }

    // ckMD legacy "Activa / Desactiva": al cambiar el ToggleSwitch, propaga el
    // estado a todas las filas (CombinarFiltros.MarcaDesmarca()).
    private void ActivarTodas_Toggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.MarcarDesmarcarCommand.Execute(null);
    }
}
