// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Control del boleto base editable de la Quiniela, cableado al motor real vía
/// <see cref="BoletoBaseViewModel"/> (que lee/escribe <c>AppState.Analizador</c>).
/// Cada fila es un partido con nombre editable y los tres signos 1 / X / 2.
/// Réplica del UserControl WinForms <c>Pronosticos</c>.
/// </summary>
public sealed partial class BoletoBaseControl : UserControl
{
    public BoletoBaseViewModel ViewModel { get; } = new();

    public BoletoBaseControl()
    {
        this.InitializeComponent();
    }
}
