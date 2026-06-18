// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Port WinUI 3 del WinForms <c>ControlTolFrm</c> ("Control Tolerancias").
    /// Edita las tolerancias y fallos permitidos del <c>ControladorTol</c> del grupo
    /// actual (AppState.Instancia.GrupoActual.ControladorTolerancias) y escribe los
    /// cambios de vuelta al motor al Aceptar.
    /// </summary>
    public sealed partial class ControlTolFrmPage : Page
    {
        public ControlTolFrmViewModel ViewModel { get; } = new ControlTolFrmViewModel();

        public ControlTolFrmPage()
        {
            this.InitializeComponent();
            ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.CargarDesdeMotor();
        }
    }
}
