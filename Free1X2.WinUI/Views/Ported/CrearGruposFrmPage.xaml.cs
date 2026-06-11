using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Página WinUI portada del WinForms legacy CrearGruposFrm
    /// (UI/Filtros/CrearGruposFrm.cs). Pide cuántos grupos nuevos crear.
    /// </summary>
    public sealed partial class CrearGruposFrmPage : Page
    {
        public CrearGruposFrmViewModel ViewModel { get; } = new CrearGruposFrmViewModel();

        public CrearGruposFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
