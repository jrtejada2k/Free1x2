using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la rejilla de Condiciones del form legacy <c>Coincidencias</c> (DataGrid <c>tabcond</c>).
/// "Grupos" = lista de números de característica (1-108, p.ej. "1-9,33,40") cuya coincidencia se cuenta;
/// "Rango" = números de coincidencias admitidos para ese grupo (p.ej. "0-3"); "Rsl" = resultado calculado.
/// </summary>
public partial class CoincidenciaCondicionViewModel : ObservableObject
{
    public CoincidenciaCondicionViewModel(string grupos = "", string rango = "")
    {
        _grupos = grupos;
        _rango = rango;
    }

    // Columna "G" del DataGrid legacy (lista de características separadas por comas / rangos con guion).
    [ObservableProperty]
    private string _grupos = string.Empty;

    // Columna "M" del DataGrid legacy (rango de coincidencias admitidas).
    [ObservableProperty]
    private string _rango = string.Empty;

    // Columna "R" del DataGrid legacy (resultado tras Analizar/Calcular). Solo lectura en pantalla.
    [ObservableProperty]
    private int _resultado;

    // Bindeable a TextBlock (regla anti-crash: no bindear int directo a Text).
    public string ResultadoTexto => Resultado.ToString();

    partial void OnResultadoChanged(int value) => OnPropertyChanged(nameof(ResultadoTexto));
}

/// <summary>
/// Una fila de la "Radiografía" (DataGrid <c>tabrad</c>): código de característica + descripción + valor.
/// Las 108 descripciones provienen de <c>InitTexs()</c> del form legacy.
/// </summary>
public partial class RadiografiaFilaViewModel : ObservableObject
{
    public RadiografiaFilaViewModel(int codigo, string descripcion, int valor)
    {
        Codigo = codigo;
        Descripcion = descripcion;
        _valor = valor;
    }

    public int Codigo { get; }
    public string CodigoTexto => Codigo.ToString();
    public string Descripcion { get; }

    [ObservableProperty]
    private int _valor;

    public string ValorTexto => Valor.ToString();

    partial void OnValorChanged(int value) => OnPropertyChanged(nameof(ValorTexto));
}

/// <summary>
/// Una fila de la "Distribución de Aceptadas" (DataGrid <c>tabout</c>): nº de coincidencias (0-108),
/// un check (S) para incluirla al grabar, y los recuentos s/AC, s/GR y AMC del índice seleccionado.
/// </summary>
public partial class DistribucionFilaViewModel : ObservableObject
{
    public DistribucionFilaViewModel(int numero)
    {
        Numero = numero;
        _seleccionada = true;
    }

    public int Numero { get; }
    public string NumeroTexto => Numero.ToString();

    // Columna "S" (DataGridBoolColumn legacy): incluir esta fila al grabar resultados.
    [ObservableProperty]
    private bool _seleccionada;

    // Columna "Q" (s/AC: distribución por nº de aceptadas).
    [ObservableProperty]
    private int _porAceptadas;

    public string PorAceptadasTexto => PorAceptadas.ToString();

    // Columna "G" (s/GR: distribución por grupo/característica).
    [ObservableProperty]
    private int _porGrupo;

    public string PorGrupoTexto => PorGrupo.ToString();

    // Columna "M" (AMC del índice de grupo seleccionado).
    [ObservableProperty]
    private int _amc;

    public string AmcTexto => Amc.ToString();

    partial void OnPorAceptadasChanged(int value) => OnPropertyChanged(nameof(PorAceptadasTexto));
    partial void OnPorGrupoChanged(int value) => OnPropertyChanged(nameof(PorGrupoTexto));
    partial void OnAmcChanged(int value) => OnPropertyChanged(nameof(AmcTexto));
}

/// <summary>
/// ViewModel de la pantalla "Coincidencias" (port del WinForms <c>Coincidencias</c>).
///
/// Propósito del form legacy: a partir de un fichero de columnas de entrada y dos columnas
/// ganadoras de referencia (una "anterior" y una "reciente"), calcula la "radiografía" de 108
/// características de cada columna, cuenta las coincidencias por grupos definidos por el usuario
/// y filtra/clasifica las columnas válidas, mostrando distribuciones y permitiendo grabarlas.
///
/// Toda la lógica de dominio (lectura de ficheros, cálculo de radiografía, validación y grabado)
/// está marcada como TODO citando los métodos de la clase legacy <c>Coincidencias</c>.
/// </summary>
public partial class CoincidenciasViewModel : ObservableObject
{
    public CoincidenciasViewModel()
    {
        Condiciones = new ObservableCollection<CoincidenciaCondicionViewModel>
        {
            new("1-9", "0-9"),
        };

        Radiografia = new ObservableCollection<RadiografiaFilaViewModel>();
        for (int i = 0; i < Descripciones.Length; i++)
        {
            Radiografia.Add(new RadiografiaFilaViewModel(i + 1, Descripciones[i], 0));
        }

        Distribucion = new ObservableCollection<DistribucionFilaViewModel>();
        for (int n = 0; n < 109; n++)
        {
            Distribucion.Add(new DistribucionFilaViewModel(n));
        }

        // Índices AMC disponibles (uno por cada condición). ComboBox legacy lbAMC + botones +/-.
        IndicesAmc = new[] { "1" };
    }

    // ===== Ficheros y contadores (etiquetas lfilein/lcolsin/lfileout/lcolsout del form legacy) =====

    // Nombre del fichero de entrada seleccionado (label lfilein).
    [ObservableProperty]
    private string _ficheroEntrada = "(ningún fichero)";

    // Columnas leídas del fichero de entrada (label lcolsin).
    [ObservableProperty]
    private string _columnasLeidas = "0";

    // Nombre del fichero de condiciones (.cnd) cargado/guardado (label lfileconds).
    [ObservableProperty]
    private string _ficheroCondiciones = "(ninguno)";

    // Nombre del fichero de resultados grabado (label lfileout).
    [ObservableProperty]
    private string _ficheroSalida = "(sin grabar)";

    // Columnas grabadas en el resultado (label lcolsout).
    [ObservableProperty]
    private string _columnasGrabadas = "0";

    // ===== Estado de proceso (labels lColsProc/lColsAdm/lTime del form legacy) =====

    [ObservableProperty]
    private string _columnasProcesadas = "0";

    [ObservableProperty]
    private string _columnasValidas = "0";

    [ObservableProperty]
    private string _tiempo = "00:00:00.0";

    // ===== Columna ganadora ANTERIOR (grupo bFGA / bMas / bMenos / lbCGA / ltColA) =====

    // Fichero de ganadoras anteriores (label lFGA).
    [ObservableProperty]
    private string _ficheroGanadorasAnterior = "(sin fichero)";

    // Índice / total de la ganadora anterior seleccionada (label lbCGA).
    [ObservableProperty]
    private string _indiceGanadoraAnterior = "0";

    // Columna ganadora anterior actual (label ltColA).
    [ObservableProperty]
    private string _columnaGanadoraAnterior = "··············";

    // ===== Columna ganadora RECIENTE (grupo bFGR / bMasR / bMenosR / lbCGR / ltColR) =====

    // Fichero de ganadoras recientes (label lFGR).
    [ObservableProperty]
    private string _ficheroGanadorasReciente = "(sin fichero)";

    // Índice / total de la ganadora reciente seleccionada (label lbCGR).
    [ObservableProperty]
    private string _indiceGanadoraReciente = "0";

    // Columna ganadora reciente actual (label ltColR).
    [ObservableProperty]
    private string _columnaGanadoraReciente = "··············";

    // ===== Selector AMC (label lbAMC + botones bAMC1/bAMC2) =====

    public IReadOnlyList<string> IndicesAmc { get; }

    // Índice AMC seleccionado para la columna "AMC" de la distribución.
    [ObservableProperty]
    private string _indiceAmcSeleccionado = "1";

    // ===== Colecciones bindeadas a las tres rejillas =====

    public ObservableCollection<CoincidenciaCondicionViewModel> Condiciones { get; }
    public ObservableCollection<RadiografiaFilaViewModel> Radiografia { get; }
    public ObservableCollection<DistribucionFilaViewModel> Distribucion { get; }

    // ===== Comandos: condiciones =====

    [RelayCommand]
    private void AgregarCondicion()
    {
        // Equivale a añadir una fila al DataGrid tabcond del form legacy.
        Condiciones.Add(new CoincidenciaCondicionViewModel());
    }

    [RelayCommand]
    private void QuitarCondicion()
    {
        if (Condiciones.Count > 0)
        {
            Condiciones.RemoveAt(Condiciones.Count - 1);
        }
    }

    // ===== Comandos: ficheros de entrada / ganadoras =====

    [RelayCommand]
    private void SeleccionarFicheroEntrada()
    {
        // TODO(dominio): equivale a Coincidencias.LeerFileIn().
        //   OpenFileDialog (*.txt) -> leer cada columna, entrada[s2n(col)] = true, ctproc++.
        //   Actualizar FicheroEntrada y ColumnasLeidas.
    }

    [RelayCommand]
    private void SeleccionarGanadorasAnterior()
    {
        // TODO(dominio): equivale a Coincidencias.EntraCGsA().
        //   OpenFileDialog (*.txt) -> colgsA[], limcgsA, nrfCGA; actualizar
        //   FicheroGanadorasAnterior, IndiceGanadoraAnterior, ColumnaGanadoraAnterior.
    }

    [RelayCommand]
    private void SeleccionarGanadorasReciente()
    {
        // TODO(dominio): equivale a Coincidencias.EntraCGsR().
        //   OpenFileDialog (*.txt) -> colgsR[], limcgsR, nrfCGR; actualizar
        //   FicheroGanadorasReciente, IndiceGanadoraReciente, ColumnaGanadoraReciente.
    }

    [RelayCommand]
    private void GanadoraAnteriorSiguiente()
    {
        // TODO(dominio): equivale a Coincidencias.GAMas() (nrfCGA++ si nrfCGA<limcgsA).
    }

    [RelayCommand]
    private void GanadoraAnteriorAnterior()
    {
        // TODO(dominio): equivale a Coincidencias.GAMenos() (nrfCGA-- si nrfCGA>1).
    }

    [RelayCommand]
    private void GanadoraRecienteSiguiente()
    {
        // TODO(dominio): equivale a Coincidencias.GRMas() (nrfCGR++ si nrfCGR<limcgsR).
    }

    [RelayCommand]
    private void GanadoraRecienteAnterior()
    {
        // TODO(dominio): equivale a Coincidencias.GRMenos() (nrfCGR-- si nrfCGR>1).
    }

    // ===== Comandos: condiciones (fichero .cnd) =====

    [RelayCommand]
    private void LeerCondiciones()
    {
        // TODO(dominio): equivale a Coincidencias.LeeCondis().
        //   OpenFileDialog (*.cnd) -> rellenar la rejilla Condiciones (G;M por línea).
    }

    [RelayCommand]
    private void GuardarCondiciones()
    {
        // TODO(dominio): equivale a Coincidencias.SalvaCondis().
        //   SaveFileDialog (*.cnd) -> escribir "G;M" por cada fila de Condiciones.
    }

    // ===== Comandos: cálculo y análisis =====

    [RelayCommand]
    private void CalcularRadiografia()
    {
        // TODO(dominio): equivale a Coincidencias.CalRadioB().
        //   Calcondis(columnaGanadoraAnterior) -> condis[0..107] -> volcar a Radiografia[i].Valor.
    }

    [RelayCommand]
    private void Calcular()
    {
        // TODO(dominio): equivale a Coincidencias.Calcular().
        //   InitConds(); recorrer todas las columnas de 'entrada', Valida(col),
        //   acumular agrabarAC/agrabarGR/agrabarAMC y volcar a Distribucion.
        //   Actualizar ColumnasProcesadas, ColumnasValidas, Tiempo.
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): equivale a Coincidencias.salida = true (interrumpe el bucle de Calcular).
    }

    [RelayCommand]
    private void Analizar()
    {
        // TODO(dominio): equivale a Coincidencias.Analizar().
        //   Compara la radiografía de la ganadora anterior con la reciente, cuenta coincidencias
        //   por grupo y vuelca el resultado a Condiciones[i].Resultado.
    }

    // ===== Comandos: grabado de resultados =====

    [RelayCommand]
    private void GrabarResultado()
    {
        // TODO(dominio): equivale a Coincidencias.Grabar().
        //   SaveFileDialog (*.txt) -> grabar columnas válidas cuyas filas estén Seleccionada=true.
        //   Actualizar FicheroSalida y ColumnasGrabadas.
    }

    [RelayCommand]
    private void ImprimirRadiografia()
    {
        // TODO(dominio): equivale a Coincidencias.PrintRadio().
        //   Escribe radiografia.txt y radioExcel.txt con las 108 características.
    }

    [RelayCommand]
    private void GrabarExcel()
    {
        // TODO(dominio): equivale a Coincidencias.GrabExcel().
        //   SaveFileDialog (*.xls) -> volcar agrabarAC/agrabarGR/agrabarAMC en formato tabulado.
    }

    // 108 descripciones de características de la radiografía (Coincidencias.InitTexs()).
    private static readonly string[] Descripciones =
    {
        "V14","X14","D14","V7P","X7P","D7P","V7U","X7U","D7U","SSV14",
        "SSU14","SSX14","SSD14","SSV7P","SSU7P","SSX7P","SSD7P","SSV7U","SSU7U","SSX7U",
        "SSD7U","disV14","disU14","disX14","disD14","disV7P","disU7P","disX7P","disD7P","disV7U",
        "disU7U","disX7U","disD7U","con(1x)14","con(12)14","con(x2)14","con(11)14","con(xx)14","con(22)14","con(1v)14",
        "con(xv)14","con(2v)14","con(vv)14","con(1x)7P","con(12)7P","con(x2)7P","con(11)7P","con(xx)7P","con(22)7P","con(1v)7P",
        "con(xv)7P","con(2v)7P","con(vv)7P","con(1x)7U","con(12)7U","con(x2)7U","con(11)7U","con(xx)7U","con(22)7U","con(1v)7U",
        "con(xv)7U","con(2v)7U","con(vv)7U","PNG14","PNV14","PNU14","PNX14","PND14","PNG7P","PNV7P",
        "PNU7P","PNX7P","PND7P","PNG7U","PNV7U","PNU7U","PNX7U","PND7U","IG14","IV14",
        "IU14","IX14","ID14","IG7P","IV7P","IU7P","IX7P","ID7P","IG7U","IV7U",
        "IU7U","IX7U","ID7U","ISG14","ISV14","ISU14","ISX14","ISD14","ISG7P","ISV7P",
        "ISU7P","ISX7P","ISD7P","ISG7U","ISV7U","ISU7U","ISX7U","ISD7U",
    };
}
