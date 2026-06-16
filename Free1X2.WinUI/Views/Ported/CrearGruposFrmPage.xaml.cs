using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Página WinUI portada del WinForms legacy CrearGruposFrm
    /// (UI/Filtros/CrearGruposFrm.cs). Pide cuántos grupos nuevos crear y los añade
    /// a la combinación actual (AppState.Instancia.Analizador.GruposPartidos).
    /// </summary>
    public sealed partial class CrearGruposFrmPage : Page
    {
        public CrearGruposFrmViewModel ViewModel { get; } = new CrearGruposFrmViewModel();

        public CrearGruposFrmPage()
        {
            this.InitializeComponent();
            ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        }
    }
}
