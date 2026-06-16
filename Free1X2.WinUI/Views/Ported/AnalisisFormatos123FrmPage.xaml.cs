using Microsoft.UI.Xaml.Controls;

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
/// La lectura de archivos, la traducción (con la valoración de la rejilla
/// PorcentajesControl == valors1.RetVals()) y el conteo están portados; solo "Analizar"
/// queda pendiente de ArrayFormatos (lista inyectada externamente, no relacionada con valors).
/// </summary>
public sealed partial class AnalisisFormatos123FrmPage : Page
{
    public AnalisisFormatos123FrmViewModel ViewModel { get; } = new();

    public AnalisisFormatos123FrmPage()
    {
        this.InitializeComponent();
    }
}
