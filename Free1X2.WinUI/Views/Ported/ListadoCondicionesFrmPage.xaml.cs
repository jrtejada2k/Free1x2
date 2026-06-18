using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms Free1X2.UI.ListadoCondicionesFrm.
    /// Muestra en árbol jerárquico las condiciones/filtros configurados de la combinación
    /// actual (AppState.Instancia.Analizador) y permite expandir/colapsar y exportar a
    /// texto/HTML.
    /// </summary>
    public sealed partial class ListadoCondicionesFrmPage : Page
    {
        public ListadoCondicionesFrmViewModel ViewModel { get; } = new();

        public ListadoCondicionesFrmPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Reconstruir();
        }
    }
}
