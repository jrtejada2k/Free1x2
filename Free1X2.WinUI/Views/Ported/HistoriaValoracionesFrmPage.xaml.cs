// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page WinUI portada del WinForms legacy "HistoriaValoracionesFrm".
/// Guarda las valoraciones 1X2 de una jornada/temporada en el fichero historico.
/// </summary>
public sealed partial class HistoriaValoracionesFrmPage : Page
{
    /// <summary>
    /// Handoff estático con el contexto que el WinForms recibía por constructor
    /// (legacy: new HistoriaValoracionesFrm(v, temporada, jornada, archivoHistorico) en
    /// TramificarForm.AfegirAlHistoric). Lo deja el productor (TramificarForm) antes de navegar
    /// y lo consume <see cref="OnNavigatedTo"/>. Null = página vacía (rejilla editable).
    /// </summary>
    public static (double[,] valores, int temporada, int jornada, string? archivoHistorico)? Contexto { get; set; }

    public HistoriaValoracionesFrmViewModel ViewModel { get; } = new();

    public HistoriaValoracionesFrmPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (Contexto is { } c)
        {
            Contexto = null; // se consume una sola vez
            ViewModel.Inicializar(c.valores, c.temporada, c.jornada, c.archivoHistorico);
        }
    }
}
