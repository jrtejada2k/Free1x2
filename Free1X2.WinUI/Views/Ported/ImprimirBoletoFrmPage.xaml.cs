using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

// Port de Free1X2.UI.ImprimirBoletoFrm (WinForms) a WinUI 3.
// Form legacy: "Impresión de boletos" — configura margenes, rango de boletos
// y la impresora para volcar columnas de quiniela sobre el boleto fisico.
public sealed partial class ImprimirBoletoFrmPage : Page
{
    public ImprimirBoletoFrmViewModel ViewModel { get; } = new();

    public ImprimirBoletoFrmPage()
    {
        InitializeComponent();
    }
}
