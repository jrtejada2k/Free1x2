// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "FrmReducidasPerfectas" (Reducciones Perfectas).
///
/// Permite definir una columna base (un signo 1/X/2 por partido) y un pronóstico (qué signos
/// juega cada partido) y, a partir de ahí, generar una "reducción perfecta" (4/13/11 triples ó
/// 7/15 dobles) que se graba en un archivo de columnas. El método está descrito por "Fortuna"
/// en foro1x2. La lógica de generación se deja como TODO de dominio en el ViewModel.
/// </summary>
public sealed partial class FrmReducidasPerfectasPage : Page
{
    public FrmReducidasPerfectasViewModel ViewModel { get; } = new();

    public FrmReducidasPerfectasPage()
    {
        this.InitializeComponent();
    }
}
