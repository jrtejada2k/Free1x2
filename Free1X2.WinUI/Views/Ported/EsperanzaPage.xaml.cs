// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// «Esperanza matemática» (W14, DT-91; pantalla nueva, no existe en el WinForms): EM exacta de una columna para 14,
/// 13, 12, 11 y 10 aciertos, como el Excel «EM Categorias.xlsm» del dueño.
/// </summary>
public sealed partial class EsperanzaPage : Page
{
    public EsperanzaViewModel ViewModel { get; } = new();

    public EsperanzaPage()
    {
        InitializeComponent();
    }
}
