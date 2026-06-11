using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada desde el WinForms Free1X2.UI.TramificarGraficasFrm.
    /// Visor de gráficas de los resultados del análisis de tramos.
    /// </summary>
    public sealed partial class TramificarGraficasFrmPage : Page
    {
        public TramificarGraficasFrmViewModel ViewModel { get; } = new();

        public TramificarGraficasFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
