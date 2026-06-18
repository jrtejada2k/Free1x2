// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Free1X2.WinUI.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Página WinUI portada del WinForms legacy Free1X2.UI.GuardarValoracionFrm.
    /// Permite elegir el formato del fichero y el separador de campos para
    /// exportar valoraciones a un .txt.
    /// </summary>
    public sealed partial class GuardarValoracionFrmPage : Page
    {
        /// <summary>
        /// Handoff estático con la matriz de valoraciones a guardar (legacy:
        /// <c>new GuardarValoracionFrm(Valores)</c> recibía la matriz por constructor desde
        /// ControlPorcentajes). Lo deja el productor (ValoracionFrmPage) antes de navegar y lo
        /// consume <see cref="OnNavigatedTo"/>, volcándolo en la rejilla. Mismo patrón
        /// static-handoff que BoletoFrmPage.ParametrosBoleto. Null = rejilla vacía editable.
        /// </summary>
        public static double[,]? ValoresIniciales { get; set; }

        public GuardarValoracionFrmViewModel ViewModel { get; } = new GuardarValoracionFrmViewModel();

        public GuardarValoracionFrmPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Legacy: GuardarValoracionFrm recibía la matriz Valores del ControlPorcentajes y
            // exportaba esos valores. Aquí se vuelca el handoff a la rejilla editable (Filas).
            if (ValoresIniciales is { } m)
            {
                ValoresIniciales = null; // se consume una sola vez
                PorcentajesHelper.CargarMatriz(ViewModel.Filas, m);
            }
        }
    }
}
