using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms legacy "ColProbablesFrm" (Free1X2.UI.Filtros.ColProbablesFrm).
    /// Editor del filtro "Columnas Probables" con pestañas Columnas / Relaciones I-III / Control Fallos.
    /// </summary>
    public sealed partial class ColProbablesFrmPage : Page
    {
        public ColProbablesFrmViewModel ViewModel { get; } = new ColProbablesFrmViewModel();

        public ColProbablesFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
