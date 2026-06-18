// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "FigurasFiltrosFrm".
/// Edita la lista de figuras de una condición de filtro mediante una rejilla de casillas
/// (cada casilla admite los valores 0..16 o el comodín "*"). Recibe la List&lt;long&gt; a editar
/// del form padre (Contactos / SignosSeguidos / PesosNum) vía el handoff estático
/// <c>FigurasFiltrosFrmViewModel.FigurasEnEdicion</c> y escribe los cambios sobre esa misma
/// referencia al Aceptar. La carga desde archivo .fig{Condicion} está implementada en el
/// comando Abrir del ViewModel.
/// </summary>
public sealed partial class FigurasFiltrosFrmPage : Page
{
    public FigurasFiltrosFrmViewModel ViewModel { get; } = new();

    public FigurasFiltrosFrmPage()
    {
        this.InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.CargarDesdeHandoff();
    }
}
