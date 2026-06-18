// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

using Free1X2.MotorCalculo;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms "AnalisisFormatos123Frm" (Text = "Analizador Formatos123").
///
/// Analiza los "formatos" de una columna de quiniela: traduce cada signo (1/X/2) a la
/// posición que ocupa en la valoración del partido (1 = más probable … 3 = menos
/// probable) y, sobre esa columna traducida, cuenta cuántas veces aparece cada formato
/// (subsecuencia). El usuario carga un archivo de columnas, navega columna a columna y
/// lanza el análisis de los formatos predefinidos ("Analizar") o de todos los formatos
/// posibles ("Mostrar todos"); el resultado se muestra como un informe Formato/Apariciones.
///
/// La UI y el estado en memoria viven en <see cref="AnalisisFormatos123FrmViewModel"/>.
/// La lista de formatos predefinidos (ArrayFormatos) llega por el handoff estático
/// <see cref="FormatosEntrada"/> desde Formatos123Frm (legacy: btnAnalisis_Click fijaba
/// analisisff.ArrayFormatos = filtro.ArrayFormatos antes de ShowDialog).
/// </summary>
public sealed partial class AnalisisFormatos123FrmPage : Page
{
    /// <summary>
    /// Handoff estático con los formatos predefinidos a analizar (legacy:
    /// analisisff.ArrayFormatos = this.filtro.ArrayFormatos). Lo deja el productor
    /// (Formatos123FrmViewModel) antes de navegar y lo consume <see cref="OnNavigatedTo"/>.
    /// </summary>
    public static List<Formato123>? FormatosEntrada { get; set; }

    public AnalisisFormatos123FrmViewModel ViewModel { get; } = new();

    public AnalisisFormatos123FrmPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (FormatosEntrada is { } formatos)
        {
            FormatosEntrada = null; // se consume una sola vez
            ViewModel.ArrayFormatos = formatos;
        }
    }
}
