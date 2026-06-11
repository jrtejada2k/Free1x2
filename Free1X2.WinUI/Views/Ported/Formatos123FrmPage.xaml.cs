using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada de Free1X2.UI.Filtros.Formatos123Frm (WinForms).
/// Filtro de Formatos 123: lista editable de formatos con min/max aciertos,
/// líneas acertadas permitidas, opción "ignorar repeticiones" y traductor 1X2->123.
/// </summary>
public sealed partial class Formatos123FrmPage : Page
{
    public Formatos123FrmViewModel ViewModel { get; } = new();

    public Formatos123FrmPage()
    {
        InitializeComponent();
    }
}
