using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    [RelayCommand]
    private void CalcularReales()
    {
        // La parte de matriz SÍ se cablea: v = controlPorcentajesApostados.Valores
        // (Free1X2/UI/BancoPruebasFrm.cs línea 3335) equivale a:
        double[,] v = PorcentajesHelper.AMatriz(PorcentajesApostados);

        // TODO(dominio): replicar btCalcularReales_Click + Calcula14Triples (BancoPruebasFrm.cs 3333/3350).
        //   BLOQUEADO por dependencias NO portadas al dominio que consumen 'v':
        //     - Free1X2.Utils.Porcentajes (ValoresNeperianos/ValoresBase100): vive en el proyecto
        //       WinForms Free1X2 (depende de System.Windows.Forms) y NO está en Free1X2.Domain, que es
        //       el único proyecto referenciado por la WinUI. Migrar primero ese helper al dominio.
        //     - Motor de probabilidades Ap14T = ApuestaProbableCentral[4782969] + EncontrarDistantes1 +
        //       ordena (quicksort) — no portado. Al terminar, el resultado p[14,3] se escribe de vuelta
        //       en la rejilla real con: PorcentajesHelper.CargarMatriz(PorcentajesReales, p)
        //       (equivale a controlPorcentajesReales.Valores = p, línea 3388).
        _ = v;
        AppServices.MostrarInfo(
            "La matriz apostada ya está disponible, pero el cálculo de la valoración real requiere el " +
            "helper Porcentajes y el motor de probabilidades (Ap14T), aún no portados al dominio.");
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
    [RelayCommand]
    private void Analizar()
    {
        // TODO(dominio): replicar btnOK_Click (Free1X2/UI/BancoPruebasFrm.cs línea 2732) y sus ramas
        //   EscrutarCombinacion / EscrutarColumnas / EscrutarColumnasAutoEscrutinio /
        //   EscrutarCombinacionPorJornadas. BLOQUEADO: dependen de CalcularPremios (que usa la matriz
        //   .Valores de ControlPorcentajes, NO portado) y, en la rama de jornadas, de EscrutadorComb
        //   (NO portado, Free1X2/Escrutinio/EscrutadorComb.cs). Las clases de resultado Resultados /
        //   ResultadosJornada son privadas del formulario y no están en el dominio.
        //   Al portar, rellenar FilasResultado y los totales (GastoTotalTexto, PremioTotalTexto,
        //   PorcentajeRecuperadoTexto, VecesPremiadaTexto, VecesBeneficioTexto).
        AppServices.MostrarInfo(
            "El análisis del banco de pruebas requiere el control de porcentajes y el motor de " +
            "escrutinio de combinaciones, aún no portados al dominio.");
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
        // TODO(dominio): replicar btEliminarFilas_Click (Free1X2/UI/BancoPruebasFrm.cs línea 4399):
        //   elimina apuestas con menos de DiferenciasMinimas diferencias, respecto de cada fila
        //   seleccionada o sólo de la fila actual (SimplificarParaCadaFila). BLOQUEADO: opera sobre las
        //   filas de resultado del escrutinio (no disponibles sin ControlPorcentajes / EscrutadorComb).
    }

    /// <summary>Paso 4: abre la selección de jornadas (btSeleccionJornadas).</summary>
    [RelayCommand]
    private void SeleccionarJornadas()
    {
        // TODO(dominio): abrir el equivalente WinUI del formulario legacy de selección de jornadas.
        //   BLOQUEADO por el host de navegación (fuera del alcance de este lote).
    }
}
