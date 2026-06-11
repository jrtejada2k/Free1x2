using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Diferencias entre columnas" (legacy: DifCols).
/// Genera columnas de 14 signos (las 4.782.969 triples internamente — legacy
/// MetodoInterno — o leídas de un fichero — legacy MetodoExterno) y acepta cada una
/// sólo si el número de diferencias respecto a la columna base / fichero de condiciones
/// y respecto a las columnas ya aceptadas cae dentro de los rangos configurados
/// (legacy PreparaDifs / Proceso / compara).
/// </summary>
public partial class DifColsViewModel : ObservableObject
{
    // -------- Generación por (legacy groupBox "Generación por": rb14T / rbFile) --------

    // 14 triples internamente (legacy: rb14T.Checked). Marcado por defecto.
    [ObservableProperty]
    private bool _generaTriples = true;

    // Generar leyendo de un fichero de entrada (legacy: rbFile.Checked).
    [ObservableProperty]
    private bool _generaFichero;

    // Nombre del fichero de entrada seleccionado/usado (legacy: lblFileIn).
    [ObservableProperty]
    private string _nombreFicheroEntrada = string.Empty;

    // -------- Condicionada por (legacy groupBox2: rbBase / rbColsB) --------

    // Condicionar respecto a una columna base escrita a mano (legacy: rbBase.Checked).
    [ObservableProperty]
    private bool _condicionColumnaBase = true;

    // Condicionar respecto a un fichero de condiciones (legacy: rbColsB.Checked).
    [ObservableProperty]
    private bool _condicionFichero;

    // Columna base de 14 signos (legacy: tbColbase.Text, MaxLength 14, mayúsculas).
    [ObservableProperty]
    private string _columnaBase = "1X1212X2111121";

    // Nombre del fichero de condiciones usado (legacy: lblFileCond).
    [ObservableProperty]
    private string _nombreFicheroCondiciones = string.Empty;

    // Rangos de diferencias admitidas respecto a la columna base, formato "min(-max)"
    // separado por comas, p.ej. "0-5,6,7-14" (legacy: tbdifbase.Text, MaxLength 20).
    [ObservableProperty]
    private string _diferenciasBase = "0-5,6,7-14";

    // Rangos de diferencias admitidas respecto a las columnas ya aceptadas
    // (legacy: tbdifresul.Text, MaxLength 20).
    [ObservableProperty]
    private string _diferenciasResultado = "6-14";

    // -------- Proceso (legacy groupBox3 "Proceso") --------

    // Límite de columnas a grabar (legacy: tblimcol.Text; al grabar, si vacío/erróneo
    // se usa 4.782.969). NumberBox.Value es double por contrato.
    [ObservableProperty]
    private double _limiteColumnas;

    // Nº de columnas admitidas (legacy: lblResul, aceptadas.Count). String para TextBlock.
    [ObservableProperty]
    private string _admitidasTexto = "0";

    // Tiempo de cálculo (legacy: lblTime, (time9-time0)). String para TextBlock.
    [ObservableProperty]
    private string _tiempoTexto = "—";

    // Habilita el botón Calcular (legacy: bCalc.Enabled; se desactiva durante el cálculo).
    [ObservableProperty]
    private bool _puedeCalcular = true;

    // Habilita el botón Grabar (legacy: bGrab.Enabled; arranca deshabilitado, se activa
    // tras un cálculo con resultados).
    [ObservableProperty]
    private bool _puedeGrabar;

    // Habilita el botón Cancelar (legacy: bCancelar; sólo tiene efecto durante el cálculo).
    [ObservableProperty]
    private bool _puedeCancelar;

    /// <summary>
    /// Lanza el cálculo de columnas admitidas. Legacy: BCalcClick -> Calcular().
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: ejecutar el cálculo (legacy DifCols.Calcular()):
        //   - PuedeCalcular = false; PuedeGrabar = false; PuedeCancelar = true;
        //   - PreparaDifs(): parsear DiferenciasBase y DiferenciasResultado a las máscaras
        //     'difbase'/'difresul' de 15 chars ('F'=fuera de rango, 'A'=admitido).
        //   - Construir 'condis': si CondicionColumnaBase, VerColumna(ColumnaBase) (valida 14
        //     signos de "124xXF", x/X->4); si CondicionFichero, abrir OpenFileDialog (*.txt) y
        //     VerColumna por cada línea -> NombreFicheroCondiciones.
        //   - Generar candidatas: si GeneraTriples, MetodoInterno() (14 bucles 1..3, '3'->'4');
        //     si GeneraFichero, MetodoExterno() (OpenFileDialog *.txt, VerColumna por línea)
        //     -> NombreFicheroEntrada.
        //   - Proceso(columna): compara() cuenta diferencias con cada condición/aceptada usando
        //     la máscara; descarta si cae en posición 'F'; si pasa, añade a 'aceptadas'.
        //   - AdmitidasTexto = aceptadas.Count; TiempoTexto = (fin-inicio).
        //   - Al terminar: PuedeCalcular = true; PuedeGrabar = (aceptadas.Count > 0);
        //     PuedeCancelar = false.
        //   Nota: el bucle legacy usa Application.DoEvents()+flag 'salida'; en WinUI usar
        //   Task.Run + CancellationToken para no bloquear la UI.
    }

    /// <summary>
    /// Graba las columnas admitidas a un fichero de texto. Legacy: BGrabClick -> Grabar().
    /// </summary>
    [RelayCommand]
    private void Grabar()
    {
        // TODO[dominio]: guardar resultados (legacy DifCols.Grabar()):
        //   - SaveFileDialog (*.txt) en la carpeta de la app.
        //   - límite = (int)LimiteColumnas si > 0; si 0/erróneo -> 4.782.969.
        //   - Escribir cada columna de 'aceptadas' (Replace('4','X')) hasta alcanzar el límite.
    }

    /// <summary>
    /// Solicita cancelar el cálculo en curso. Legacy: BCancelarClick -> salida = true.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: señalar cancelación del cálculo (legacy: campo 'salida = true',
        //   comprobado dentro de MetodoInterno/MetodoExterno). En WinUI: CancellationTokenSource.Cancel().
    }

    /// <summary>
    /// Abre el generador de CPs por diferencias. Legacy: btnDiferencias_Click ->
    /// new GeneradorCPSDiferencias().ShowDialog().
    /// </summary>
    [RelayCommand]
    private void AbrirCpsPorDiferencias()
    {
        // TODO[dominio]: navegar/abrir la pantalla portada de GeneradorCPSDiferencias
        //   (equivale a GeneradorCPSDiferencias.ShowDialog()).
    }
}
