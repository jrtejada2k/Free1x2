using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "AnalizarCombinacionFrm".
/// Visor de resultados (sólo lectura) que muestra, en forma de árbol, las condiciones y
/// filtros evaluados contra una combinación, con su estado (acertada / aceptada por
/// tolerancias / fallada) y una leyenda explicativa de los tres estados.
/// El original constaba de un TreeView (treeView1) y un panel de leyenda con 3 iconos.
/// El rellenado del árbol depende de la lógica de dominio (motor de cálculo/escrutinio),
/// marcado como TODO en el ViewModel; aún no portado a Free1X2.Domain.
/// </summary>
public sealed partial class AnalizarCombinacionFrmPage : Page
{
    public AnalizarCombinacionFrmViewModel ViewModel { get; } = new();

    public AnalizarCombinacionFrmPage()
    {
        this.InitializeComponent();
    }
}
