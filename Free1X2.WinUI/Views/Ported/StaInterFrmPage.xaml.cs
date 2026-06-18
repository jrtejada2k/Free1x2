// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Port WinUI 3 del WinForms legacy "StaInterFrm"
    /// (Free1X2/UI/Estadisticas/StaInterFrm.cs, Text = "interrupciones 0.1").
    ///
    /// Muestra la matriz de interrupciones (5 agregados: globales / cant. 1 /
    /// cant. X / cant. 2 / cant. V x 14 partidos) y permite alternar entre
    /// porcentajes y conteos crudos. La carga de datos de dominio (rsl[,], numcol)
    /// queda como TODO en el ViewModel.
    /// </summary>
    public sealed partial class StaInterFrmPage : Page
    {
        public StaInterFrmViewModel ViewModel { get; } = new StaInterFrmViewModel();

        public StaInterFrmPage()
        {
            InitializeComponent();
        }
    }
}
