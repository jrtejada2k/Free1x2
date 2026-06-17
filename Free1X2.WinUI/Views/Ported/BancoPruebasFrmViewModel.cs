using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de resultados del escrutinio por columna (legacy: cada elemento de
/// <c>ResCol[]</c> — clase privada <c>ResultadosPorColumna</c> de BancoPruebasFrm — enlazado
/// al DataGridView <c>dgResultadoEscrutinio</c>). <see cref="Seleccionado"/> es editable
/// (casilla de la rejilla); el resto se proyecta a <c>string</c> para enlazar a TextBlock.Text
/// (mismo patrón que <see cref="ResultadoEscrutinioItem"/>).
/// </summary>
public partial class ResultadoColumnaItem : ObservableObject
{
    /// <summary>Casilla de selección de la fila (legacy: dgResultadoEscrutinio.IsSelected/Select/UnSelect).</summary>
    [ObservableProperty]
    private bool _seleccionado;

    /// <summary>Nº de orden de la fila (legacy: ResCol[i].Num).</summary>
    public string Numero { get; init; } = string.Empty;

    /// <summary>Columna en formato 1/X/2 (legacy: ConvNumAColumna de ResCol[i].Columna).</summary>
    public string Columna { get; init; } = string.Empty;

    /// <summary>Valor numérico de la columna (legacy: ResCol[i].Columna, base ternaria).</summary>
    public int ColumnaNumero { get; init; }

    // Veces premiada por categoría (legacy: ResCol[i].Veces14..Veces10).
    public string Veces14 { get; init; } = string.Empty;
    public string Veces13 { get; init; } = string.Empty;
    public string Veces12 { get; init; } = string.Empty;
    public string Veces11 { get; init; } = string.Empty;
    public string Veces10 { get; init; } = string.Empty;

    /// <summary>Premio acumulado de la fila (legacy: ResCol[i].PremioAcumulado).</summary>
    public string Premio { get; init; } = string.Empty;

    /// <summary>% recuperación de la fila (legacy: ResCol[i].Recuperacion).</summary>
    public string Recuperacion { get; init; } = string.Empty;

    /// <summary>Resumen para mostrar en una sola línea de la lista (aciertos por categoría 14..10).</summary>
    public string Resumen =>
        $"{Columna}    14:{Veces14}  13:{Veces13}  12:{Veces12}  11:{Veces11}  10:{Veces10}    " +
        $"premio:{Premio}  {Recuperacion}".Trim();
}

/// <summary>
/// Una fila de resultados del escrutinio por jornadas (legacy: cada elemento de
/// <c>ResultadoPorJornadas[]</c> — clase privada <c>ResultadosJornada</c> de BancoPruebasFrm —
/// enlazado al DataGridView <c>dgResultadoEscrutinio</c> vía la tabla "ResultadosJornada[]";
/// ver InicializaGridResultadoEscrutinioPorJornadas). Cada propiedad se proyecta a <c>string</c>
/// para enlazar a TextBlock.Text (mismo patrón que <see cref="ResultadoColumnaItem"/>).
/// </summary>
public partial class ResultadoJornadaItem : ObservableObject
{
    /// <summary>Nº de orden de la jornada simulada (legacy: ResultadosJornada.Jornada).</summary>
    public string Jornada { get; init; } = string.Empty;

    // Nº de columnas premiadas por categoría (legacy: AciertosDe14..AciertosDe10).
    public string AciertosDe14 { get; init; } = string.Empty;
    public string AciertosDe13 { get; init; } = string.Empty;
    public string AciertosDe12 { get; init; } = string.Empty;
    public string AciertosDe11 { get; init; } = string.Empty;
    public string AciertosDe10 { get; init; } = string.Empty;

    /// <summary>Importe total de premios de la jornada (legacy: ResultadosJornada.TotalPremios).</summary>
    public string ImportePremios { get; init; } = string.Empty;

    /// <summary>Saldo acumulado tras la jornada (legacy: ResultadosJornada.Saldo).</summary>
    public string Saldo { get; init; } = string.Empty;

    /// <summary>Resumen para mostrar en una sola línea de la lista.</summary>
    public string Resumen =>
        $"Jornada {Jornada}    14:{AciertosDe14}  13:{AciertosDe13}  12:{AciertosDe12}  " +
        $"11:{AciertosDe11}  10:{AciertosDe10}    premios:{ImportePremios}  saldo:{Saldo}".Trim();
}

/// <summary>
/// ViewModel del formulario legacy WinForms "BancoPruebasFrm"
/// (Simulador de escrutinios — Banco de Pruebas).
///
/// El asistente legacy se compone de 4 pasos:
///   Paso 1 (Ficheros)      - selección del fichero de columnas a analizar.
///   Paso 2 (valoraciones)  - definición de la valoración apostada y real (% por jornada,
///                            parámetros LN / premio medio del 14 / nº columnas).
///   Paso 3 (simular 14's)  - origen de las columnas (aleatorias o fichero) y generación
///                            de columnas de 14 aciertos con la desviación típica deseada.
///   Paso 4 (Escrutinios)   - tipo de análisis, premios acumulados considerados, ejecución
///                            del escrutinio y simplificación de filas.
///
/// Toda la lógica de cálculo / persistencia / apertura de otros forms queda como TODO citando
/// la clase legacy correspondiente (no se implementa aquí).
/// </summary>
public partial class BancoPruebasFrmViewModel : ObservableObject
{
    // ---------------------------------------------------------------------
    // Paso 1 (Ficheros)
    // ---------------------------------------------------------------------

    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    [ObservableProperty]
    private string _numColumnasLeidasTexto = "0";

    // Columnas leidas del fichero de entrada como numero (legacy: Columnas, ArrayList<int>).
    private readonly List<int> _columnasLeidas = new();

    // Columnas aleatorias generadas (legacy: ColumnasAleatorias, ArrayList<int>).
    private readonly List<int> _columnasAleatorias = new();

    // ---------------------------------------------------------------------
    // Paso 2 (valoraciones)
    // ---------------------------------------------------------------------

    // Rejillas de porcentajes por jornada (1/X/2). Sustituyen a los dos UserControls
    // WinForms ControlPorcentajes (controlPorcentajesApostados / controlPorcentajesReales).
    // PorcentajesHelper.AMatriz(...) equivale a ControlPorcentajes.Valores (get); CargarMatriz al set.
    public ObservableCollection<FilaPorcentaje> PorcentajesApostados { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    public ObservableCollection<FilaPorcentaje> PorcentajesReales { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    // Pesos posicionales ternarios para 14 partidos (legacy: PotDe3 / DosPotDe3 de ConvertidorDeBases).
    private static readonly int[] PotDe3 = { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };
    private static readonly int[] DosPotDe3 = { 2, 6, 18, 54, 162, 486, 1458, 4374, 13122, 39366, 118098, 354294, 1062882, 3188646 };

    // Origen del parámetro de valoración real: true = usar LN, false = usar premio medio del 14.
    [ObservableProperty]
    private bool _usarLN = true;

    // Para deshabilitar/habilitar cada campo según el radio seleccionado (regla anti-crash:
    // el binding de IsEnabled va en el control, no en el panel contenedor).
    public bool UsarPremio => !UsarLN;

    [ObservableProperty]
    private double _lnProbMedia14 = -14.7;

    [ObservableProperty]
    private double _premioMedio14;

    [ObservableProperty]
    private double _numColumnasAConsiderar = 50000;

    // ---------------------------------------------------------------------
    // Paso 3 (simular 14's)
    // ---------------------------------------------------------------------

    // Origen de columnas: true = aleatorias, false = desde fichero.
    [ObservableProperty]
    private bool _origenAleatorias = true;

    public bool OrigenFichero => !OrigenAleatorias;

    [ObservableProperty]
    private double _numAleatorias = 1000;

    [ObservableProperty]
    private double _desviacionTipicaDeseada = 1.965561;

    [ObservableProperty]
    private double _lnMinimo = -11;

    [ObservableProperty]
    private string _ficheroAleatorias = string.Empty;

    // Resultados de la generación (sólo lectura para el usuario).
    [ObservableProperty]
    private string _desviacionTipicaObtenidaTexto = "0";

    [ObservableProperty]
    private string _lnMediaObtenidaTexto = "0";

    // ---------------------------------------------------------------------
    // Paso 4 (Escrutinios)
    // ---------------------------------------------------------------------

    // Tipo de análisis (radios del grupo "Tipo de análisis").
    // Opciones del ComboBox (regla anti-crash 3: ItemsSource desde propiedad del VM).
    public IReadOnlyList<string> TiposAnalisis { get; } = new[]
    {
        "Combinación",
        "Columnas",
        "Jornadas",
        "Autoescrutinio",
    };

    [ObservableProperty]
    private string _tipoAnalisisSeleccionado = "Columnas";

    // Premios acumulados que se consideran (checkboxes 10..14).
    [ObservableProperty]
    private bool _acumula10;

    [ObservableProperty]
    private bool _acumula11;

    [ObservableProperty]
    private bool _acumula12;

    [ObservableProperty]
    private bool _acumula13;

    [ObservableProperty]
    private bool _acumula14 = true;

    // Simplificación de filas.
    // true = respecto de cada fila seleccionada, false = sólo respecto de la fila actual.
    [ObservableProperty]
    private bool _simplificarParaCadaFila;

    // Inverso para el radio "Sólo respecto de la fila actual" (evita usar un Converter,
    // que no está en la lista de recursos permitidos).
    public bool SimplificarSoloFilaActual
    {
        get => !SimplificarParaCadaFila;
        set => SimplificarParaCadaFila = !value;
    }

    [ObservableProperty]
    private double _diferenciasMinimas = 2;

    [ObservableProperty]
    private double _numFila;

    // Resultados globales (sólo lectura).
    [ObservableProperty]
    private string _numColumnasResultadoTexto = "0";

    [ObservableProperty]
    private string _gastoTotalTexto = "0";

    [ObservableProperty]
    private string _premioTotalTexto = "0";

    [ObservableProperty]
    private string _porcentajeRecuperadoTexto = "0";

    [ObservableProperty]
    private string _vecesPremiadaTexto = "0";

    [ObservableProperty]
    private string _vecesBeneficioTexto = "0";

    // Filas de resultado del escrutinio (grid legacy dgResultadoEscrutinio) en modo "Combinación".
    public ObservableCollection<string> FilasResultado { get; } = new();

    // Filas de resultado por columna (grid legacy dgResultadoEscrutinio en modos "Columnas" y
    // "Autoescrutinio"), con casilla de selección por fila. Equivale a ResCol[] enlazado al
    // DataGridView WinForms (mismo enfoque que EscrutiniosFrmViewModel.Resultados).
    public ObservableCollection<ResultadoColumnaItem> ColumnasResultado { get; } = new();

    // Filas de resultado por jornadas (grid legacy dgResultadoEscrutinio en modo "Jornadas",
    // enlazado a ResultadoPorJornadas[] vía la tabla "ResultadosJornada[]"). Una fila por columna
    // aleatoria simulada, con su saldo acumulado.
    public ObservableCollection<ResultadoJornadaItem> JornadasResultado { get; } = new();

    // ---------------------------------------------------------------------
    // Dependencias entre opciones (equivalentes a los CheckedChanged legacy)
    // ---------------------------------------------------------------------

    partial void OnUsarLNChanged(bool value)
    {
        OnPropertyChanged(nameof(UsarPremio));
    }

    partial void OnOrigenAleatoriasChanged(bool value)
    {
        OnPropertyChanged(nameof(OrigenFichero));
    }

    partial void OnSimplificarParaCadaFilaChanged(bool value)
    {
        OnPropertyChanged(nameof(SimplificarSoloFilaActual));
    }

    // ---------------------------------------------------------------------
    // Comandos (la lógica de dominio queda como TODO)
    // ---------------------------------------------------------------------

    /// <summary>Paso 1: abre el diálogo de fichero y lee las columnas a analizar (btLeerColumnas + CargarFicheroDeColumnas).</summary>
    [RelayCommand]
    private async Task LeerColumnasAsync()
    {
        // Legacy btLeerColumnas_Click (Free1X2/UI/BancoPruebasFrm.cs línea 3224): OpenFileDialog (*.txt).
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        FicheroEntrada = file.Path;

        // Legacy CargarFicheroDeColumnas (línea 3237): ArchivoColumnasTexto + ConvertidorDeBases,
        // convierte cada columna a numero.
        _columnasLeidas.Clear();
        IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(file.Path);
        ConvertidorDeBases col = new ConvertidorDeBases();
        while (comBaseCols.SiguienteColumna())
        {
            _columnasLeidas.Add(col.ConvColumnaANumero(comBaseCols.LeeColumnaSinComas()));
        }
        comBaseCols.Cerrar();

        NumColumnasLeidasTexto = _columnasLeidas.Count.ToString();
    }

    /// <summary>Paso 2: calcula la valoración real a partir de la apostada (btCalcularReales).</summary>
    [RelayCommand(CanExecute = nameof(NoOcupado))]
    private async Task CalcularReales()
    {
        // btCalcularReales_Click (BancoPruebasFrm.cs 3333): v = controlPorcentajesApostados.Valores;
        // ValoresNeperianos() y cálculo de complementarios; luego Calcula14Triples.
        v = PorcentajesHelper.AMatriz(PorcentajesApostados);
        float[,] va = new Porcentajes(v).ValoresNeperianos();
        float Prob = 0;
        for (int Partido = 0; Partido < 14; Partido++)
        {
            Prob += va[Partido, 0];
            Cra[Partido, 1] = (float)(va[Partido, 1] - va[Partido, 0]);
            Cra[Partido, 2] = (float)(va[Partido, 2] - va[Partido, 0]);
        }

        // Snapshot de los parámetros de UI (txLN / txNumCol legacy).
        double lnTxt = LnProbMedia14;
        int numCol = (int)NumColumnasAConsiderar;

        Ocupado = true;
        try
        {
            double[,] pResultado = await Task.Run(() => Calcula14Triples(Prob, lnTxt, numCol));
            // controlPorcentajesReales.Valores = p (BancoPruebasFrm.cs 3388).
            PorcentajesHelper.CargarMatriz(PorcentajesReales, pResultado);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error calculando la valoración real: " + ex.Message);
        }
        finally
        {
            Ocupado = false;
        }
    }

    /// <summary>Paso 3: genera columnas aleatorias de 14 aciertos (btGenerarAleatorias).</summary>
    [RelayCommand]
    private void GenerarAleatorias()
    {
        // Legacy: btGenerarAleatorias_Click (BancoPruebasFrm.cs línea 3168). Genera NumAleatorias
        // columnas usando las matrices de % reales (p) y apostados (v) de los dos ControlPorcentajes.
        // No depende del motor de 4,7M: sólo de las dos matrices -> totalmente cableable.
        double[,] p = PorcentajesHelper.AMatriz(PorcentajesReales);
        double[,] v = PorcentajesHelper.AMatriz(PorcentajesApostados);

        _columnasAleatorias.Clear();
        var aleatorio = new Random(unchecked((int)DateTime.Now.Ticks));
        int numAleatorias = (int)NumAleatorias;

        double mpx = 0, lnMedia = 0, lnVariMedia = 0, lnDTmedia = 0;
        for (int z = 0; z < numAleatorias; z++)
        {
            double prob = 1;
            int b = 0;
            for (int j = 0; j < 14; j++)
            {
                double num = aleatorio.NextDouble();
                if (num < p[j, 0] / 100)
                {
                    prob *= v[j, 0] / 100;
                }
                else if (num < ((p[j, 0] + p[j, 1]) / 100))
                {
                    prob *= v[j, 1] / 100;
                    b += PotDe3[j];
                }
                else
                {
                    prob *= v[j, 2] / 100;
                    b += DosPotDe3[j];
                }
            }

            mpx = (mpx * z + prob) / (z + 1);
            double ln = Math.Log(prob);
            double lnm = Math.Log(mpx);
            lnMedia = (lnMedia * z + ln) / (z + 1);
            lnVariMedia = (lnVariMedia * z + ((ln - lnm) * (ln - lnm))) / (z + 1);
            lnDTmedia = Math.Sqrt(lnVariMedia);
            _columnasAleatorias.Add(b);
        }

        DesviacionTipicaObtenidaTexto = lnDTmedia.ToString();
        LnMediaObtenidaTexto = lnMedia.ToString();
    }

    /// <summary>Paso 3: guarda en fichero las columnas aleatorias generadas (btGuardarAleatorias).</summary>
    [RelayCommand]
    private async Task GuardarAleatoriasAsync()
    {
        // Legacy btGuardarAleatorias_Click (Free1X2/UI/BancoPruebasFrm.cs línea 3568): SaveFileDialog (*.txt)
        //   + ArchivoColumnasTexto.GuardarCols(int) por cada columna aleatoria.
        if (_columnasAleatorias.Count == 0)
        {
            // Las aleatorias las genera GenerarAleatorias, bloqueado por ControlPorcentajes.
            AppServices.MostrarInfo("No hay columnas aleatorias generadas que guardar.");
            return;
        }

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ColumnasAleatorias",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        IArchivoColumnas comCols = new ArchivoColumnasTexto(file.Path);
        foreach (int nr in _columnasAleatorias)
        {
            comCols.GuardarCols(nr);
        }
        comCols.Cerrar();
    }

    /// <summary>Paso 4: ejecuta el escrutinio / análisis seleccionado (btnOK "Analizar").</summary>
    [RelayCommand(CanExecute = nameof(NoOcupado))]
    private async Task Analizar()
    {
        // btnOK_Click (BancoPruebasFrm.cs 2732): según el tipo de análisis seleccionado se ejecuta
        //   EscrutarCombinacion / EscrutarColumnas / EscrutarColumnasAutoEscrutinio /
        //   EscrutarCombinacionPorJornadas. Los cuatro modos están cableados.
        if (string.IsNullOrEmpty(FicheroEntrada))
        {
            AppServices.MostrarInfo("Selecciona primero el fichero de columnas (paso 1).");
            return;
        }

        // Combinación, Columnas y Jornadas requieren las aleatorias generadas (paso 3); el
        // autoescrutinio no (legacy: rbOptionEspecial habilita btnOK sin ColumnasAleatorias).
        if (TipoAnalisisSeleccionado != "Autoescrutinio" && _columnasAleatorias.Count == 0)
        {
            AppServices.MostrarInfo("Genera primero las columnas aleatorias (paso 3).");
            return;
        }

        string archivoEntrada = FicheroEntrada;
        double[,] vApostados = PorcentajesHelper.AMatriz(PorcentajesApostados);
        string tipo = TipoAnalisisSeleccionado;

        Ocupado = true;
        try
        {
            await Task.Run(() =>
            {
                switch (tipo)
                {
                    case "Combinación":
                        EscrutarCombinacion(archivoEntrada, vApostados);
                        break;
                    case "Columnas":
                        EscrutarColumnas(archivoEntrada, vApostados);
                        break;
                    case "Jornadas":
                        EscrutarCombinacionPorJornadas(archivoEntrada, vApostados);
                        break;
                    case "Autoescrutinio":
                        EscrutarColumnasAutoEscrutinio(archivoEntrada, vApostados);
                        break;
                }
            });
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error en el escrutinio: " + ex.Message);
        }
        finally
        {
            Ocupado = false;
        }
    }

    /// <summary>Paso 4: graba las columnas del resultado por columnas (btGrabar).</summary>
    [RelayCommand]
    private async Task Grabar()
    {
        // btGrabar_Click (Free1X2/UI/BancoPruebasFrm.cs línea 3978). Graba las columnas
        // seleccionadas del resultado "Columnas"/"Autoescrutinio" vía ArchivoColumnasTexto.
        await GrabarAsync();
    }

    /// <summary>Paso 4: simplifica (deselecciona) filas según el criterio de diferencias (btEliminarFilas).</summary>
    [RelayCommand]
    private void Simplificar()
    {
        // btEliminarFilas_Click (Free1X2/UI/BancoPruebasFrm.cs línea 4399) +
        // DeseleccionaColumnasPorDiferencias (4427). Opera sobre ColumnasResultado y su selección.
        SimplificarFilas();
    }

    /// <summary>Paso 4: abre la selección de jornadas (btSeleccionJornadas).</summary>
    [RelayCommand]
    private void SeleccionarJornadas()
    {
        // El botón legacy btSeleccionJornadas (BancoPruebasFrm.cs línea 148) está oculto
        // (Visible=false) y NO tiene handler Click cableado en el formulario original, por lo que no
        // hay comportamiento que transcribir. Si en el futuro se reactiva, abriría el equivalente WinUI
        // del formulario de selección de jornadas (requiere el host de navegación, fuera de alcance).
    }

    // =====================================================================
    // Motor del Banco de Pruebas (transcrito literalmente de BancoPruebasFrm.cs).
    // =====================================================================

    private const int NumColumnas14T = 4782969; // 3^14

    // Bloquea reentradas mientras corre un cálculo pesado y habilita/inhabilita los comandos.
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CalcularRealesCommand))]
    [NotifyCanExecuteChangedFor(nameof(AnalizarCommand))]
    private bool _ocupado;

    private bool NoOcupado() => !Ocupado;

    // Campos de instancia equivalentes a los del formulario legacy (BancoPruebasFrm.cs 45-75).
    private double[,] v = new double[14, 3];
    private double[,] p = new double[14, 3];
    private readonly float[] Cr = new float[14];
    private float[,] pa = new float[14, 3];
    private float[,] Cra = new float[14, 3];
    private ApuestaProbableCentral[] Ap14T = Array.Empty<ApuestaProbableCentral>();
    private BitArray Bits = new BitArray(NumColumnas14T, false);
    private int Profundidad = 0;
    private double _LN = -14.7;

    // Parámetros L.A.E. hardcodeados en el formulario legacy (BancoPruebasFrm.cs 55-56 + ctor 544-548).
    private const double Recaudacion = 14000000;
    private const double PrecioApuesta = 0.5;
    private readonly double[] PctDestinadoAPremiosCategoria = new double[5] { 0.12, 0.08, 0.08, 0.08, 0.09 };
    private readonly double[] DestinadoAPremiosCategoria = new double[5];
    private double ProbabilidadCategoria14 = 1;
    private readonly double[] SumaProbabilidades = new double[5];
    private readonly double[] Premios = new double[5];

    // Resultados por columna del último análisis "Columnas"/"Autoescrutinio" (legacy: ResCol[]).
    // Se conservan para que Grabar y Simplificar operen sobre las mismas filas (sincronizado con
    // la colección observable ColumnasResultado a través de Seleccionado).
    private ResultadosPorColumna[]? _resCol;
    private int _numApuestas;

    // Categorías de premio a acumular para la columna del resumen (legacy: AcumularPremio[5];
    // por defecto sólo el 14, igual que el WinForms {false,false,false,false,true}).
    private bool[] AcumularPremioActual() =>
        new[] { Acumula14, Acumula13, Acumula12, Acumula11, Acumula10 };

    public BancoPruebasFrmViewModel()
    {
        // ctor legacy (BancoPruebasFrm.cs 544-546): DestinadoAPremiosCategoria = Recaudacion * Pct.
        for (int i = 0; i < 5; i++)
            DestinadoAPremiosCategoria[i] = Recaudacion * PctDestinadoAPremiosCategoria[i];
    }

    // statusBarPanel*.Text legacy -> propiedades de estado marshaladas al hilo de UI.
    private void EnUi(Action accion)
    {
        var disp = AppServices.UiDispatcher;
        if (disp is null) { accion(); return; }
        disp.TryEnqueue(() => accion());
    }

    // BancoPruebasFrm.cs 3350-3389. Devuelve la matriz p[14,3] de la valoración real obtenida.
    private double[,] Calcula14Triples(float Prob, double lnTxt, int NumCol)
    {
        Ap14T = new ApuestaProbableCentral[NumColumnas14T];
        Bits = new BitArray(NumColumnas14T, true);
        _LN = lnTxt;
        Profundidad = 0;
        EncontrarDistantes1(Prob, 0, 0, 14);
        Ap14T[0].ProbabilidadDiferencial = Math.Abs(Prob - (float)_LN);
        Ap14T[0].Probabilidad = Prob;
        Ap14T[0].Columna = 0;

        ordena(0, 4782968);
        Array.Clear(p, 0, 42);
        for (int i = 0; i < NumCol; i++)
        {
            for (byte partido = 0; partido < 14; partido++)
            {
                p[partido, (Ap14T[i].Columna / PotDe3[partido]) % 3]++;
            }
        }
        for (byte partido = 0; partido < 14; partido++)
        {
            p[partido, 0] /= (NumCol / 100);
            p[partido, 1] /= (NumCol / 100);
            p[partido, 2] /= (NumCol / 100);
        }
        return p;
    }

    // BancoPruebasFrm.cs 3390-3426.
    private void EncontrarDistantes1(float pProb, int IndiceInicial, int PosicionInicial, int pProfundidad)
    {
        int Partido;
        int z;
        int Indice;
        float Prob;
        Profundidad++;

        //'--encontramos las apuestas que se diferencian en un solo signo ----
        for (Partido = PosicionInicial; Partido < 14; Partido++)
        {
            for (z = 1; z < 3; z++)
            {
                Indice = IndiceInicial + PotDe3[Partido] * z;
                Prob = pProb + Cra[Partido, z];

                if (Bits[Indice])
                {
                    Ap14T[Indice].Columna = Indice;
                    Ap14T[Indice].ProbabilidadDiferencial = Math.Abs(Prob - (float)_LN);
                    Ap14T[Indice].Probabilidad = Prob;
                }
                else
                {
                    Ap14T[Indice].ProbabilidadDiferencial = (float)3E+7;
                    Ap14T[Indice].Probabilidad = Prob;
                }

                if (Profundidad < pProfundidad)
                {
                    EncontrarDistantes1(Prob, Indice, Partido + 1, pProfundidad);
                }
            }
        }
        Profundidad--;
    }

    // BancoPruebasFrm.cs 3427-3448.
    private void ordena(int izq, int der)
    {
        int i = 0, j = 0;
        ApuestaProbableCentral x = new ApuestaProbableCentral();
        ApuestaProbableCentral aux = new ApuestaProbableCentral();
        i = izq; j = der;
        x = Ap14T[(izq + der) / 2];
        do
        {
            while (Ap14T[i].ProbabilidadDiferencial < x.ProbabilidadDiferencial && j <= der) i++;
            while (x.ProbabilidadDiferencial < Ap14T[j].ProbabilidadDiferencial && j > izq) j--;
            if (i <= j)
            {
                aux = Ap14T[i];
                Ap14T[i] = Ap14T[j];
                Ap14T[j] = aux;
                i++; j--;
            }
        } while (i <= j);
        if (izq < j) ordena(izq, j);
        if (i < der) ordena(i, der);
    }

    // BancoPruebasFrm.cs 3237-3249.
    private static void CargarFicheroDeColumnas(string archivoEntrada, List<int> columnas)
    {
        IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
        ConvertidorDeBases col = new ConvertidorDeBases();
        columnas.Clear();
        while (comBaseCols.SiguienteColumna())
        {
            columnas.Add(col.ConvColumnaANumero(comBaseCols.LeeColumnaSinComas()));
        }
        comBaseCols.Cerrar();
    }

    // BancoPruebasFrm.cs 3271-3279.
    private static byte Aciertos(int col1, int col2)
    {
        byte a = 0;
        for (int Partido = 0; Partido < 14; Partido++)
        {
            if (((col1 / PotDe3[Partido]) % 3) == ((col2 / PotDe3[Partido]) % 3)) a++;
        }
        return a;
    }

    // BancoPruebasFrm.cs 3280-3307.
    private void CalcularPremios(int ColumnaGanadora)
    {
        int Partido;
        int i;
        int signo;

        ProbabilidadCategoria14 = 1;
        for (i = 0; i < 5; i++) SumaProbabilidades[i] = 0;

        for (Partido = 0; Partido < 14; Partido++)
        {
            signo = (ColumnaGanadora / PotDe3[Partido]) % 3;
            ProbabilidadCategoria14 *= pa[Partido, signo];
            Cr[Partido] = (float)((1 - pa[Partido, signo]) / pa[Partido, signo]);
        }
        Premios[0] = Math.Round(PctDestinadoAPremiosCategoria[0] * PrecioApuesta / ProbabilidadCategoria14, 2);

        CalcularSumaProbabilidades(ProbabilidadCategoria14, 0, 4);

        for (i = 1; i < 5; i++)
        {
            Premios[i] = Math.Round(PctDestinadoAPremiosCategoria[i] * PrecioApuesta / SumaProbabilidades[i], 2);
        }
        CorreccionesDeCalculo();
    }

    // BancoPruebasFrm.cs 3308-3323.
    private void CalcularSumaProbabilidades(double pProb, int PosicionInicial, int pProfundidad)
    {
        double Prob = 0;
        Profundidad++;

        for (int Partido = PosicionInicial; Partido < 14; Partido++)
        {
            Prob = pProb * Cr[Partido];
            SumaProbabilidades[Profundidad] += Prob;
            if (Profundidad < pProfundidad)
            {
                CalcularSumaProbabilidades(Prob, Partido + 1, pProfundidad);
            }
        }
        Profundidad--;
    }

    // BancoPruebasFrm.cs 3324-3331.
    private void CorreccionesDeCalculo()
    {
        for (byte i = 0; i < 5; i++)
        {
            if (Premios[i] > DestinadoAPremiosCategoria[i]) Premios[i] = DestinadoAPremiosCategoria[i];
        }
        if (Premios[4] < 1) Premios[4] = 0;
    }

    // BancoPruebasFrm.cs 2740-2808 (rama EscrutarCombinacion). Rellena FilasResultado + totales del VM.
    private void EscrutarCombinacion(string archivoEntrada, double[,] vApostados)
    {
        int[] aciertos = new int[15];
        double[] ingresos = new double[5] { 0, 0, 0, 0, 0 };
        var columnas = new List<int>();
        int numAleatorias = _columnasAleatorias.Count; // legacy: NumAleatorias
        int VecesPremiada = 0;
        int VecesBeneficio = 0;
        double PremioTotal = 0;

        CargarFicheroDeColumnas(archivoEntrada, columnas);
        double CosteCombinacion = columnas.Count * PrecioApuesta;
        EnUi(() => NumColumnasResultadoTexto = columnas.Count.ToString());
        double GastoTotal = CosteCombinacion * numAleatorias;
        double PremioCombinacion;
        EnUi(() => GastoTotalTexto = GastoTotal.ToString());

        //---Leer Porcentajes --------------
        v = vApostados;
        pa = new Porcentajes(v).ValoresBase100();
        bool premiada;
        double beneficio;
        foreach (int i in _columnasAleatorias)
        {
            CalcularPremios(i);
            PremioCombinacion = 0;
            premiada = false;
            beneficio = -CosteCombinacion;
            foreach (int j in columnas)
            {
                byte NumAciertos = Aciertos(i, j);
                aciertos[NumAciertos]++;
                if (NumAciertos > 9)
                {
                    ingresos[14 - NumAciertos] += Premios[14 - NumAciertos];
                    premiada = true;
                    PremioCombinacion += ingresos[14 - NumAciertos];
                    beneficio += Premios[14 - NumAciertos];
                }
            }
            if (premiada == true) VecesPremiada++;
            if (beneficio > 0) VecesBeneficio++;
        }

        // Res[] legacy -> filas de texto (Concepto/Valor/Premios) para FilasResultado.
        var filas = new List<string>();
        for (int i = 0; i < 15; i++)
        {
            double premiosCat = 0;
            if (i >= 10) premiosCat = Math.Round(ingresos[14 - i], 0);
            double mediaCat = aciertos[i] > 0 ? Math.Round(premiosCat / aciertos[i], 2) : 0;
            filas.Add($"{i} aciertos\tveces: {aciertos[i]}\tpremios: {premiosCat}\tmedia: {mediaCat}");
        }
        for (int i = 10; i < 15; i++) PremioTotal += ingresos[14 - i];
        PremioTotal = Math.Round(PremioTotal, 0);

        double Recuperacion = Math.Round(100 * PremioTotal / GastoTotal, 0);

        EnUi(() =>
        {
            FilasResultado.Clear();
            foreach (var f in filas) FilasResultado.Add(f);
            PremioTotalTexto = PremioTotal.ToString();
            VecesBeneficioTexto = VecesBeneficio.ToString();
            VecesPremiadaTexto = VecesPremiada.ToString();
            PorcentajeRecuperadoTexto = Recuperacion.ToString() + "%";
        });
    }

    // BancoPruebasFrm.cs 3600-3771 (rama EscrutarColumnas, tipo "Columnas"). Escruta cada columna
    // del fichero contra todas las aleatorias y produce una fila de resultado por columna (ResCol[]).
    private void EscrutarColumnas(string archivoEntrada, double[,] vApostados)
    {
        int[] aciertos = new int[15];
        var columnas = new List<int>();
        int VecesPremiada = 0;
        int VecesBeneficio = 0;
        double PremioTotal = 0;
        int numAleatorias = _columnasAleatorias.Count; // legacy: NumAleatorias

        CargarFicheroDeColumnas(archivoEntrada, columnas);
        double CosteCombinacion = columnas.Count * PrecioApuesta;
        int numApuestas = columnas.Count; // legacy: NumApuestas
        var resCol = new ResultadosPorColumna[numApuestas + 1];
        EnUi(() => NumColumnasResultadoTexto = numApuestas.ToString());
        double GastoTotal = CosteCombinacion * numAleatorias;
        EnUi(() => GastoTotalTexto = GastoTotal.ToString());

        //---Leer Porcentajes --------------
        v = vApostados;
        pa = new Porcentajes(v).ValoresBase100();
        bool[] acumular = AcumularPremioActual();

        int contador = 0;
        foreach (int j in columnas)
        {
            resCol[contador] = new ResultadosPorColumna(j, numAleatorias);
            resCol[contador].Num = (++contador).ToString();
        }

        bool premiada;
        double beneficio;
        foreach (int i in _columnasAleatorias)
        {
            CalcularPremios(i);
            premiada = false;
            beneficio = -CosteCombinacion;
            int contacolumnas = 0;
            foreach (int j in columnas)
            {
                byte Ac = Aciertos(i, j);
                aciertos[Ac]++;
                if (Ac > 9)
                {
                    resCol[contacolumnas].ContarPremio(14 - Ac);
                    resCol[contacolumnas].SumarPremio(14 - Ac, Premios[14 - Ac]);
                    premiada = true;
                    beneficio += Premios[14 - Ac];
                }
                contacolumnas++;
            }
            if (premiada) VecesPremiada++;
            if (beneficio > 0) VecesBeneficio++;
        }

        Array.Clear(Premios, 0, 5);
        for (int k = 0; k < numApuestas; k++)
        {
            PremioTotal += resCol[k].PremioDe14 + resCol[k].PremioDe13 + resCol[k].PremioDe12 +
                           resCol[k].PremioDe11 + resCol[k].PremioDe10;
            resCol[k].Acumula = acumular;
            Premios[0] += resCol[k].PremioDe14;
            Premios[1] += resCol[k].PremioDe13;
            Premios[2] += resCol[k].PremioDe12;
            Premios[3] += resCol[k].PremioDe11;
            Premios[4] += resCol[k].PremioDe10;
        }
        resCol[numApuestas] = new ResultadosPorColumna(numAleatorias, aciertos, Premios, acumular);

        PremioTotal = Math.Round(PremioTotal, 0);
        double Recuperacion = GastoTotal > 0 ? Math.Round(100 * PremioTotal / GastoTotal, 0) : 0;

        _resCol = resCol;
        _numApuestas = numApuestas;
        PublicarColumnasResultado(resCol, numApuestas);

        EnUi(() =>
        {
            PremioTotalTexto = PremioTotal.ToString();
            VecesBeneficioTexto = VecesBeneficio.ToString();
            VecesPremiadaTexto = VecesPremiada.ToString();
            PorcentajeRecuperadoTexto = Recuperacion.ToString() + "%";
        });
    }

    // BancoPruebasFrm.cs 3772-3941 (rama EscrutarColumnasAutoEscrutinio, tipo "Autoescrutinio").
    // Escruta cada columna del fichero contra TODAS las demás del mismo fichero (autoescrutinio).
    private void EscrutarColumnasAutoEscrutinio(string archivoEntrada, double[,] vApostados)
    {
        int[] aciertos = new int[15];
        var columnas = new List<int>();
        int VecesPremiada = 0;
        int VecesBeneficio = 0;

        CargarFicheroDeColumnas(archivoEntrada, columnas);
        double CosteCombinacion = columnas.Count * PrecioApuesta;
        int numApuestas = columnas.Count; // legacy: NumApuestas
        var resCol = new ResultadosPorColumna[numApuestas + 1];
        EnUi(() => NumColumnasResultadoTexto = numApuestas.ToString());
        double GastoTotal = CosteCombinacion;
        EnUi(() => GastoTotalTexto = GastoTotal.ToString());

        //---Leer Porcentajes --------------
        v = vApostados;
        pa = new Porcentajes(v).ValoresBase100();
        bool[] acumular = AcumularPremioActual();

        int contador = 0;
        foreach (int j in columnas)
        {
            resCol[contador] = new ResultadosPorColumna(j, numApuestas);
            resCol[contador].Num = (++contador).ToString();
        }

        bool premiada;
        double beneficio;
        int contacolumnas = 0;
        foreach (int i in columnas)
        {
            CalcularPremios(i);
            premiada = false;
            beneficio = -CosteCombinacion;
            foreach (int j in columnas)
            {
                byte Ac = Aciertos(i, j);
                aciertos[Ac]++;
                if (Ac > 9)
                {
                    resCol[contacolumnas].ContarPremio(14 - Ac);
                    resCol[contacolumnas].SumarPremio(14 - Ac, Premios[14 - Ac]);
                    premiada = true;
                    beneficio += Premios[14 - Ac];
                }
            }
            contacolumnas++;
            if (premiada) VecesPremiada++;
            if (beneficio > 0) VecesBeneficio++;
        }

        Array.Clear(Premios, 0, 5);
        for (int k = 0; k < numApuestas; k++)
        {
            // Legacy: en autoescrutinio NO se acumula PremioTotal (línea comentada en 3838).
            resCol[k].Acumula = acumular;
            Premios[0] += resCol[k].PremioDe14;
            Premios[1] += resCol[k].PremioDe13;
            Premios[2] += resCol[k].PremioDe12;
            Premios[3] += resCol[k].PremioDe11;
            Premios[4] += resCol[k].PremioDe10;
        }
        resCol[numApuestas] = new ResultadosPorColumna(numApuestas, aciertos, Premios, acumular);

        _resCol = resCol;
        _numApuestas = numApuestas;
        PublicarColumnasResultado(resCol, numApuestas);

        EnUi(() =>
        {
            // Legacy: lblPremioTotal / lblVecesPremiada / lblRecuperacion quedan vacíos en autoescrutinio.
            PremioTotalTexto = "";
            VecesBeneficioTexto = VecesBeneficio.ToString();
            VecesPremiadaTexto = "";
            PorcentajeRecuperadoTexto = "";
        });
    }

    // BancoPruebasFrm.cs 2809-2857 (rama EscrutarCombinacionPorJornadas, tipo "Jornadas"). Trata
    // cada columna aleatoria como una "jornada": la escruta contra todo el fichero, acumula los
    // premios obtenidos y arrastra el saldo de una jornada a la siguiente.
    private void EscrutarCombinacionPorJornadas(string archivoEntrada, double[,] vApostados)
    {
        var resultadoPorJornadas = new ResultadosJornada[_columnasAleatorias.Count];
        double SaldoInicial = 0;
        var columnas = new List<int>();
        int contador = 0;

        CargarFicheroDeColumnas(archivoEntrada, columnas);
        double CosteCombinacion = columnas.Count * PrecioApuesta;
        EnUi(() => NumColumnasResultadoTexto = columnas.Count.ToString());

        //---Leer Porcentajes --------------
        v = vApostados;
        pa = new Porcentajes(v).ValoresBase100();

        foreach (int i in _columnasAleatorias)
        {
            int[] aciertos = new int[5];
            double[] ingresos = new double[5];
            int indice = contador;
            contador++;
            CalcularPremios(i);

            foreach (int j in columnas)
            {
                byte NumAciertos = Aciertos(i, j);
                if (NumAciertos > 9)
                {
                    aciertos[14 - NumAciertos]++;
                    ingresos[14 - NumAciertos] += Premios[14 - NumAciertos];
                }
            }
            resultadoPorJornadas[indice] = new ResultadosJornada(contador, SaldoInicial, CosteCombinacion, ingresos, aciertos);
            SaldoInicial = resultadoPorJornadas[indice].Saldo;
        }

        PublicarJornadasResultado(resultadoPorJornadas);
    }

    // Proyecta ResultadoPorJornadas[] a la colección observable (legacy:
    // dgResultadoEscrutinioPorJornadasDataBind — DataSource = ResultadoPorJornadas). Marshalado a UI.
    private void PublicarJornadasResultado(ResultadosJornada[] resultadoPorJornadas)
    {
        var filas = new List<ResultadoJornadaItem>();
        foreach (var rj in resultadoPorJornadas)
        {
            if (rj is null) continue;
            filas.Add(new ResultadoJornadaItem
            {
                Jornada = rj.Jornada.ToString(),
                AciertosDe14 = rj.AciertosDe14.ToString(),
                AciertosDe13 = rj.AciertosDe13.ToString(),
                AciertosDe12 = rj.AciertosDe12.ToString(),
                AciertosDe11 = rj.AciertosDe11.ToString(),
                AciertosDe10 = rj.AciertosDe10.ToString(),
                ImportePremios = Math.Round(rj.TotalPremios, 2).ToString(),
                Saldo = Math.Round(rj.Saldo, 2).ToString(),
            });
        }

        EnUi(() =>
        {
            JornadasResultado.Clear();
            foreach (var f in filas) JornadasResultado.Add(f);
        });
    }

    // Proyecta ResCol[] (sin la fila resumen final [numApuestas]) a la colección observable
    // (legacy: dgResultadoEscrutinioDataBindPorColumnas — DataSource = ResCol). Marshalado a UI.
    private void PublicarColumnasResultado(ResultadosPorColumna[] resCol, int numApuestas)
    {
        var con = new ConvertidorDeBases();
        var filas = new List<ResultadoColumnaItem>();
        for (int i = 0; i < numApuestas; i++)
        {
            var rc = resCol[i];
            filas.Add(new ResultadoColumnaItem
            {
                Numero = rc.Num,
                Columna = con.ConvNumAColumna(rc.Columna),
                ColumnaNumero = rc.Columna,
                Veces14 = rc.Veces14.ToString(),
                Veces13 = rc.Veces13.ToString(),
                Veces12 = rc.Veces12.ToString(),
                Veces11 = rc.Veces11.ToString(),
                Veces10 = rc.Veces10.ToString(),
                Premio = rc.PremioAcumulado.ToString(),
                Recuperacion = rc.Recuperacion,
            });
        }

        EnUi(() =>
        {
            ColumnasResultado.Clear();
            foreach (var f in filas) ColumnasResultado.Add(f);
        });
    }

    // =====================================================================
    // Grabar (btGrabar_Click) y Simplificar (btEliminarFilas_Click) — operan sobre ResCol[]
    // y la selección de las filas (ColumnasResultado[i].Seleccionado).
    // =====================================================================

    // BancoPruebasFrm.cs 3978-4039 (btGrabar_Click). Versión portada sin el diálogo de rango
    // (DialogoGrabarBancoPruebasFrm): graba las columnas seleccionadas o, si no hay ninguna marcada,
    // todas las del resultado, vía ArchivoColumnasTexto + FileSavePicker.
    private async Task GrabarAsync()
    {
        if (_resCol is null || _numApuestas == 0 || ColumnasResultado.Count == 0)
        {
            AppServices.MostrarInfo("No hay resultado por columnas que grabar. Ejecuta primero un análisis " +
                "por 'Columnas' o 'Autoescrutinio' (paso 4).");
            return;
        }

        // Legacy btGrabar_Click: recorre las filas seleccionadas (dgResultadoEscrutinio.IsSelected).
        // El diálogo de rango sólo restringía [c1..c2] y el nº máximo; aquí, sin ese diálogo, se
        // graba la selección completa (o todas si no hay selección, equivalente a SoloSeleccionadas=false).
        bool haySeleccion = false;
        foreach (var item in ColumnasResultado)
            if (item.Seleccionado) { haySeleccion = true; break; }

        var columnasAGrabar = new List<string>();
        foreach (var item in ColumnasResultado)
        {
            if (haySeleccion && !item.Seleccionado) continue;
            columnasAGrabar.Add(item.Columna);
        }

        if (columnasAGrabar.Count == 0)
        {
            AppServices.MostrarInfo("No hay columnas que grabar.");
            return;
        }

        // Legacy: SaveFileDialog (carpeta Columnas, filtro "Columnas(*.txt)|*.txt|...").
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "columnas",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        int conta = 0;
        await Task.Run(() =>
        {
            // Legacy: IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
            //   por cada fila -> comCols.GuardarCols(col.ConvNumAColumna(ResCol[i].Columna)); Cerrar();
            IArchivoColumnas comCols = new ArchivoColumnasTexto(file.Path);
            foreach (string columna in columnasAGrabar)
            {
                comCols.GuardarCols(columna);
                conta++;
            }
            comCols.Cerrar();
        });

        // Legacy: statusBarPanel3.Text = "Grabadas N columnas".
        AppServices.MostrarInfo($"Grabadas {conta} columna(s) en {file.Name}.");

        // TODO: portar el diálogo de rango DialogoGrabarBancoPruebasFrm (FilaInicial/FilaFinal/
        //   NumMaxColumnas/SoloSeleccionadas) que restringe [c1..c2] y el nº máximo de columnas.
        //   Requiere mostrar DialogoGrabarBancoPruebasFrmPage como ContentDialog (fuera del scope
        //   de este ViewModel/Page). Free1X2/UI/BancoPruebasFrm.cs línea 3996.
    }

    // BancoPruebasFrm.cs 4399-4440 (btEliminarFilas_Click + DeseleccionaColumnasPorDiferencias).
    // Deselecciona (simplifica) las filas demasiado parecidas a una de referencia.
    private void SimplificarFilas()
    {
        if (_resCol is null || ColumnasResultado.Count == 0)
        {
            AppServices.MostrarInfo("No hay resultado por columnas que simplificar. Ejecuta primero un " +
                "análisis por 'Columnas' o 'Autoescrutinio' (paso 4).");
            return;
        }

        // Legacy: byte Dif = (byte)(14 - Convert.ToByte(txDiferencias.Text));
        byte dif = (byte)(14 - (int)DiferenciasMinimas);
        var con = new ConvertidorDeBases();

        if (SimplificarSoloFilaActual)
        {
            // Legacy rama rbSoloFilaActual: c = ConvColumnaANumero(txColumna.Text);
            //   DeseleccionaColumnasPorDiferencias(c, 0, Dif); dgResultadoEscrutinio.Select(CurrentCell).
            // El form legacy tenía dos campos independientes (txColumna = columna de referencia,
            // txNumFila = celda a reseleccionar). La Page sólo expone NumFila, así que la columna de
            // referencia se toma de la fila NumFila (su Columna), equivalente al uso habitual.
            int fila = (int)NumFila;
            if (fila < 0 || fila >= ColumnasResultado.Count)
            {
                AppServices.MostrarInfo("El nº de fila está fuera de rango.");
                return;
            }
            int c = con.ConvColumnaANumero(ColumnasResultado[fila].Columna);
            DeseleccionaColumnasPorDiferencias(c, 0, dif);
            ColumnasResultado[fila].Seleccionado = true; // legacy: dgResultadoEscrutinio.Select(CurrentCell).
        }
        else
        {
            // Legacy rama "respecto de cada fila seleccionada": por cada fila seleccionada llama a
            // DeseleccionaColumnasPorDiferencias(c, i, Dif) con c == 0 (variable nunca reasignada en
            // esta rama del WinForms original). Se respeta tal cual para mantener la paridad exacta.
            int c = 0;
            for (int i = 0; i < ColumnasResultado.Count; i++)
            {
                if (ColumnasResultado[i].Seleccionado)
                {
                    DeseleccionaColumnasPorDiferencias(c, i, dif);
                }
            }
        }
    }

    // BancoPruebasFrm.cs 4427-4440. Deselecciona las filas (desde PosicionInicial) cuya columna
    // coincide con 'c' en más de NumMinimoAciertos signos (demasiado parecidas).
    private void DeseleccionaColumnasPorDiferencias(int c, int PosicionInicial, byte NumMinimoAciertos)
    {
        for (int i = PosicionInicial; i < ColumnasResultado.Count; i++)
        {
            if (ColumnasResultado[i].Seleccionado)
            {
                if (c == ColumnasResultado[i].ColumnaNumero) continue;
                if (Aciertos(c, ColumnasResultado[i].ColumnaNumero) > NumMinimoAciertos)
                {
                    ColumnasResultado[i].Seleccionado = false;
                }
            }
        }
    }

    // ---------------------------------------------------------------------
    // ResultadosJornada (transcrita de la clase privada de BancoPruebasFrm.cs 254-334).
    // Acumula los premios de una "jornada" (columna aleatoria) y arrastra el saldo.
    // ---------------------------------------------------------------------
    private sealed class ResultadosJornada
    {
        private readonly int _Jornada;
        private readonly double _SaldoInicial;
        private readonly double[] _Premios = new double[5];
        private readonly int[] _Aciertos = new int[5];
        private readonly double _Coste;

        public ResultadosJornada(int pJornada, double pSaldoInicial, double pCoste, double[] pPremios, int[] pAciertos)
        {
            _Jornada = pJornada;
            _SaldoInicial = pSaldoInicial;
            _Coste = pCoste;
            _Premios = pPremios;
            _Aciertos = pAciertos;
        }

        public int Jornada => _Jornada;

        // Saldo = SaldoInicial - Coste + TotalPremios (legacy 278-281).
        public double Saldo => _SaldoInicial - _Coste + TotalPremios;

        public double TotalPremios
        {
            get
            {
                double resultado = 0;
                for (int i = 0; i < 5; i++) resultado += _Premios[i];
                return resultado;
            }
        }

        public int AciertosDe14 => _Aciertos[0];
        public int AciertosDe13 => _Aciertos[1];
        public int AciertosDe12 => _Aciertos[2];
        public int AciertosDe11 => _Aciertos[3];
        public int AciertosDe10 => _Aciertos[4];
    }

    // ---------------------------------------------------------------------
    // ResultadosPorColumna (transcrita de la clase privada de BancoPruebasFrm.cs 336-534).
    // ---------------------------------------------------------------------
    private sealed class ResultadosPorColumna
    {
        private readonly int _Columna;
        private readonly int[] _NumVeces = new int[5];
        private double[] _Premios = new double[5];
        private readonly double[] _PremioUnitario = new double[5];
        private bool[] _Acumula = new bool[5];
        private string _Num = string.Empty;
        private double _Recuperacion;
        private readonly int _NumEscrutinios;

        public ResultadosPorColumna(int pNumEscrutinios, int[] pNumVeces, double[] pPremios, bool[] pAcumula)
        {
            _Num = "SUMAS ";
            _NumEscrutinios = pNumEscrutinios;
            for (int i = 0; i < 5; i++) { _NumVeces[i] = pNumVeces[14 - i]; }
            _Premios = pPremios;
            _Acumula = pAcumula;
        }

        public ResultadosPorColumna(int pColumna, int pNumEscrutinios)
        {
            _Columna = pColumna;
            _NumEscrutinios = pNumEscrutinios;
        }

        public void ContarPremio(int categoria) => _NumVeces[categoria]++;
        public void SumarPremio(int categoria, double premio) => _Premios[categoria] += premio;

        public string Num { get => _Num; set => _Num = value; }
        public bool[] Acumula { set => _Acumula = value; }
        public int Columna => _Columna;

        public double PremioDe14 => Math.Round(_Premios[0]);
        public double PremioDe13 => Math.Round(_Premios[1]);
        public double PremioDe12 => Math.Round(_Premios[2]);
        public double PremioDe11 => Math.Round(_Premios[3]);
        public double PremioDe10 => Math.Round(_Premios[4]);

        public int Veces14 => _NumVeces[0];
        public int Veces13 => _NumVeces[1];
        public int Veces12 => _NumVeces[2];
        public int Veces11 => _NumVeces[3];
        public int Veces10 => _NumVeces[4];

        public double PremioAcumulado
        {
            get
            {
                double Suma = 0;
                for (byte i = 0; i < 5; i++) { if (_Acumula[i]) Suma += _Premios[i]; }
                _Recuperacion = _NumEscrutinios > 0
                    ? Math.Round(Suma * 100 / _NumEscrutinios / 0.5, 0)
                    : 0;
                return Math.Round(Suma, 1);
            }
        }

        public string Recuperacion
        {
            get
            {
                if (_Num == "SUMAS ") return "";
                // Legacy: la recuperación se calcula como efecto colateral de PremioAcumulado.
                _ = PremioAcumulado;
                return _Recuperacion.ToString() + " %";
            }
        }
    }
}
