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
/// La lógica de dominio (lectura de archivos, traducción, conteo binario) está marcada
/// con TODO referenciando los tipos y métodos legacy a invocar.
/// </summary>
public sealed partial class AnalisisFormatos123FrmPage : Page
{
    public AnalisisFormatos123FrmViewModel ViewModel { get; } = new();

    public AnalisisFormatos123FrmPage()
    {
        this.InitializeComponent();
    }

    // Toda la lógica de dominio (EntradaFichero, TraducirColumna, TransformarValoracion,
    // ConvStrToLong, DeterminaApariciones, ObtenerFormatos, ObtenerTodosFormatos del
    // legacy AnalisisFormatos123Frm, más Free1X2.MotorCalculo.Formato123 y
    // Free1X2.EntradaSalida.ArchivoColumnasTexto) se invocará desde los RelayCommand del
    // ViewModel; ver los TODO en AnalisisFormatos123FrmViewModel.cs.
}
