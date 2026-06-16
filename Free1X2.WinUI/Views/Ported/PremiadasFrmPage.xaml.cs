using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "PremiadasFrm" (título "Análisis Premiadas").
///
/// A partir de un fichero de columnas ganadoras, calcula cuántas columnas del espacio
/// total resultarían premiadas con 10/11/12/13 aciertos y muestra las frecuencias.
/// Permite grabar a fichero las columnas de una frecuencia seleccionada (legacy: bGrabar)
/// y analizar en qué jornadas aparece dicha frecuencia (legacy: bAnaliza).
///
/// El cálculo (Genera/Trasvasa/Examina), la grabación y el análisis están cableados
/// en el ViewModel replicando PremiadasFrm.
/// </summary>
public sealed partial class PremiadasFrmPage : Page
{
    public PremiadasFrmViewModel ViewModel { get; } = new();

    public PremiadasFrmPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Propaga al ViewModel el índice de la frecuencia seleccionada en el ListView
    /// (legacy: lbPremis.SelectedIndex, usado por Grabar y Analiza). La selección vive
    /// en el control de UI, igual que en ColumnasPremiadasFrmPage.
    /// </summary>
    private void OnFrecuenciaSeleccionada(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.FrecuenciaSeleccionadaIndex = ListaFrecuencias.SelectedIndex;
    }
}
