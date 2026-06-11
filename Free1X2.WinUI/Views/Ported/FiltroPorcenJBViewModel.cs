using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

// ViewModel para FiltroPorcenJBPage.
// Porta el estado de entrada del WinForms legacy Free1X2.UI.FiltroPorcenJB.
// La logica de dominio (calculo de bandas, validacion de columnas, persistencia
// de ficheros, navegacion de columnas ganadoras) queda como TODO citando la clase legacy.
public partial class FiltroPorcenJBViewModel : ObservableObject
{
    // --- Limites de banda (legacy tblim1..tblim6, "<=" cut-offs). Defaults del Designer. ---
    [ObservableProperty]
    private double _limite1 = 15;

    [ObservableProperty]
    private double _limite2 = 22;

    [ObservableProperty]
    private double _limite3 = 29;

    [ObservableProperty]
    private double _limite4 = 36;

    [ObservableProperty]
    private double _limite5 = 48;

    [ObservableProperty]
    private double _limite6 = 61;

    // --- Rangos "min-max" por banda (legacy tbrank1..tbrank7), texto libre tipo "0-3". ---
    [ObservableProperty]
    private string _rango1 = "0-3";

    [ObservableProperty]
    private string _rango2 = "0-3";

    [ObservableProperty]
    private string _rango3 = "0-3";

    [ObservableProperty]
    private string _rango4 = "0-3";

    [ObservableProperty]
    private string _rango5 = "0-3";

    [ObservableProperty]
    private string _rango6 = "0-3";

    [ObservableProperty]
    private string _rango7 = "0-3";

    // Rango de recorrido max-min (legacy tbmgreco), "0-14".
    [ObservableProperty]
    private string _recorrido = "0-14";

    // --- Columna ganadora a analizar (legacy tbCG). ---
    [ObservableProperty]
    private string _columnaGanadora = "COL.GANADORA";

    // --- Etiquetas de resultado (legacy labels read-only). string para bindear a TextBlock.Text. ---
    [ObservableProperty]
    private string _columnasProcesadas = "procesadas";

    [ObservableProperty]
    private string _columnasAdmitidas = "admitidas";

    [ObservableProperty]
    private string _tiempo = "tiempo";

    [ObservableProperty]
    private string _ficheroResultado = "fichero";

    [ObservableProperty]
    private string _ficheroRangos = "fichero";

    [ObservableProperty]
    private string _ficheroGanadoras = "fichero ganadoras";

    [ObservableProperty]
    private string _contadorGanadoras = "-";

    // Resultado del analisis por banda (legacy lrk1..lrk7 + lreco).
    [ObservableProperty]
    private string _analisisBanda1 = "-";

    [ObservableProperty]
    private string _analisisBanda2 = "-";

    [ObservableProperty]
    private string _analisisBanda3 = "-";

    [ObservableProperty]
    private string _analisisBanda4 = "-";

    [ObservableProperty]
    private string _analisisBanda5 = "-";

    [ObservableProperty]
    private string _analisisBanda6 = "-";

    [ObservableProperty]
    private string _analisisBanda7 = "-";

    [ObservableProperty]
    private string _analisisRecorrido = "-";

    // --- Acciones (legacy botones). Logica de dominio diferida. ---

    [RelayCommand]
    private void Calcular()
    {
        // TODO: legacy FiltroPorcenJB.Calcular()
        // Lee fichero de columnas de entrada (OpenFileDialog), valida cada columna
        // contra las bandas/rangos via FiltroPorcenJB.Valida(string) y actualiza
        // ColumnasProcesadas/ColumnasAdmitidas/Tiempo.
    }

    [RelayCommand]
    private void GrabarResultado()
    {
        // TODO: legacy FiltroPorcenJB.GrabaCols()
        // Guarda las columnas admitidas (SaveFileDialog) y fija FicheroResultado.
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: legacy FiltroPorcenJB.BCancelarClick -> salida=true
        // Aborta el bucle de Calcular en curso.
    }

    [RelayCommand]
    private void SalvarRangos()
    {
        // TODO: legacy FiltroPorcenJB.SalvarConds()
        // Persiste rangos+limites a fichero .jb7 (SaveFileDialog).
    }

    [RelayCommand]
    private void LeerRangos()
    {
        // TODO: legacy FiltroPorcenJB.LeerConds()
        // Carga rangos+limites desde fichero .jb7 (OpenFileDialog) y repinta.
    }

    [RelayCommand]
    private void Analizar()
    {
        // TODO: legacy FiltroPorcenJB.Analizar()
        // Cuenta coincidencias de ColumnaGanadora por banda y fija AnalisisBanda1..7
        // y AnalisisRecorrido (max-min).
    }

    [RelayCommand]
    private void Exportar()
    {
        // TODO: legacy FiltroPorcenJB.ExporCols()
        // Exporta la rejilla de signos por banda a CSV (SaveFileDialog).
    }

    [RelayCommand]
    private void CargarGanadoras()
    {
        // TODO: legacy FiltroPorcenJB.EntraCGsR()
        // Carga fichero de columnas ganadoras (OpenFileDialog), fija FicheroGanadoras
        // y ContadorGanadoras, y selecciona la ultima columna.
    }

    [RelayCommand]
    private void GanadoraSiguiente()
    {
        // TODO: legacy FiltroPorcenJB.GRMas()
        // Avanza a la siguiente columna ganadora y actualiza ColumnaGanadora.
    }

    [RelayCommand]
    private void GanadoraAnterior()
    {
        // TODO: legacy FiltroPorcenJB.GRMenos()
        // Retrocede a la columna ganadora anterior y actualiza ColumnaGanadora.
    }
}
