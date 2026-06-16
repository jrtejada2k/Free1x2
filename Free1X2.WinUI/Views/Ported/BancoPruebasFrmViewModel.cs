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

    // Filas de resultado del escrutinio (grid legacy dgResultadoEscrutinio).
    public ObservableCollection<string> FilasResultado { get; } = new();

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
        // btnOK_Click (BancoPruebasFrm.cs 2732): según el tipo de análisis seleccionado.
        // De las 4 ramas legacy (Combinación / Columnas / Autoescrutinio / Jornadas) sólo la rama
        // "Combinación" se mapea limpiamente a FilasResultado + totales del VM; las otras tres
        // dependen del DataGrid WinForms (ResCol/ResultadoPorJornadas) — ver TODO al final.
        if (TipoAnalisisSeleccionado != "Combinación")
        {
            // TODO[grid]: replicar EscrutarColumnas (BancoPruebasFrm.cs 3600) /
            //   EscrutarColumnasAutoEscrutinio (3772) / EscrutarCombinacionPorJornadas (2809).
            //   BLOQUEADO: producen ResultadosPorColumna[] / ResultadosJornada[] que el form enlaza a
            //   la rejilla custom dgResultadoEscrutinio (MyDataGrid) con columnas/selección propias;
            //   el VM sólo expone FilasResultado (ObservableCollection<string>) y no modela esas
            //   clases ni la selección de la rejilla. Requiere un modelo de resultado por columnas.
            AppServices.MostrarInfo(
                "Sólo el análisis por 'Combinación' está portado. Los modos por columnas, autoescrutinio " +
                "y jornadas requieren la rejilla de resultados del formulario original.");
            return;
        }

        if (string.IsNullOrEmpty(FicheroEntrada))
        {
            AppServices.MostrarInfo("Selecciona primero el fichero de columnas (paso 1).");
            return;
        }
        if (_columnasAleatorias.Count == 0)
        {
            AppServices.MostrarInfo("Genera primero las columnas aleatorias (paso 3).");
            return;
        }

        string archivoEntrada = FicheroEntrada;
        double[,] vApostados = PorcentajesHelper.AMatriz(PorcentajesApostados);

        Ocupado = true;
        try
        {
            await Task.Run(() => EscrutarCombinacion(archivoEntrada, vApostados));
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

    /// <summary>Paso 4: graba los resultados del escrutinio (btGrabar).</summary>
    [RelayCommand]
    private void Grabar()
    {
        // TODO(dominio): replicar btGrabar_Click (Free1X2/UI/BancoPruebasFrm.cs línea 3978): graba las
        //   columnas/resultados del escrutinio. BLOQUEADO: requiere los resultados producidos por
        //   Analizar (dependiente de ControlPorcentajes / EscrutadorComb, no portados).
    }

    /// <summary>Paso 4: simplifica (elimina) filas según el criterio de diferencias (btEliminarFilas).</summary>
    [RelayCommand]
    private void Simplificar()
    {
        // TODO[grid]: replicar btEliminarFilas_Click (Free1X2/UI/BancoPruebasFrm.cs línea 4399) +
        //   DeseleccionaColumnasPorDiferencias (4427). BLOQUEADO de raíz: la simplificación opera sobre
        //   ResCol[] (ResultadosPorColumna del análisis "Columnas") y sobre la SELECCIÓN de filas de la
        //   rejilla custom dgResultadoEscrutinio (Select/IsSelected/UnSelect de MyDataGrid). El VM sólo
        //   expone FilasResultado (ObservableCollection<string>) sin estado de selección por columna ni
        //   el modelo ResCol, así que no hay nada equivalente que deseleccionar. Requiere portar antes
        //   el análisis por columnas con un modelo de resultado seleccionable.
        AppServices.MostrarInfo(
            "La simplificación de filas opera sobre el resultado del análisis por columnas y la selección " +
            "de la rejilla del formulario original, aún no portados.");
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
}
