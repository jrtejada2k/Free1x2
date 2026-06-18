using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// UserControl reutilizable del boleto de la Quiniela. Expone su <see cref="BoletoViewModel"/>
/// para enlace compilado (x:Bind) desde la plantilla XAML.
/// </summary>
public sealed partial class BoletoControl : UserControl
{
    public BoletoViewModel ViewModel { get; } = new();

    public BoletoControl()
    {
        this.InitializeComponent();
    }
}
