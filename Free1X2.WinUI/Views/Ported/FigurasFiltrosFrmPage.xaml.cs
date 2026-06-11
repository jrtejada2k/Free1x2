using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "FigurasFiltrosFrm".
/// Edita la lista de figuras de una condición de filtro mediante una rejilla de casillas
/// (cada casilla admite los valores 0..16 o el comodín "*"), con acciones Aceptar, Abrir
/// (cargar desde archivo .fig), Borrar y Cancelar.
/// La conversión texto→figura (long), la validación (EsFiguraValida) y la persistencia
/// dependen del dominio legacy (Utils.UtilidadesEntradasValores / IFiltro) y están marcadas
/// como TODO en el ViewModel.
/// </summary>
public sealed partial class FigurasFiltrosFrmPage : Page
{
    public FigurasFiltrosFrmViewModel ViewModel { get; } = new();

    public FigurasFiltrosFrmPage()
    {
        this.InitializeComponent();
    }
}
