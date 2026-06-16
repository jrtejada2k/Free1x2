using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
/// ViewModel para la pantalla "Rentabilidad" (legacy: Free1X2.UI.RentabilidadFrm).
/// La utilidad obtiene la Esperanza Matemática de premio (EM) de las columnas a partir de
/// la probabilidad real y de las frecuencias apostadas, recorriendo o bien las 14 triples
/// completas o las columnas de un fichero, y graba el resultado filtrado por límites de EM.
/// También permite calcular la valoración (EM) de una sola columna concreta.
///
/// El motor de recorrido (EncontrarDistantes1 / ordena / GrabacionColumnas) se transcribe
/// literalmente de RentabilidadFrm.cs operando sobre Ap14T = ApuestaProbable[3^14].
/// </summary>
public partial class RentabilidadFrmViewModel : ObservableObject
{
    // Rejillas de porcentajes por partido (1/X/2). Sustituyen a los dos UserControls WinForms
    // ControlPorcentajes (controlPorcentajesApostados / controlPorcentajesReales);
    // PorcentajesHelper.AMatriz(...) equivale a ControlPorcentajes.Valores (get).
    public ObservableCollection<FilaPorcentaje> PorcentajesApostados { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    public ObservableCollection<FilaPorcentaje> PorcentajesReales { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    // --- Origen de las columnas (legacy: rb14Triples / rbFichero, txFicheroEntrada) ---
    // OrigenEsFichero=false -> 14 triples (por defecto, legacy rb14Triples.Checked=true);
    // true -> fichero de entrada. OrigenEs14Triples es el inverso para el RadioButton.
    [ObservableProperty]
    private bool _origenEsFichero;

    public bool OrigenEs14Triples => !OrigenEsFichero;

    partial void OnOrigenEsFicheroChanged(bool value)
    {
        OnPropertyChanged(nameof(OrigenEs14Triples));
    }

    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    // --- Salida de resultados (legacy: txFicheroSalida) ---
    [ObservableProperty]
    private string _ficheroSalida = string.Empty;

    // --- Recaudación considerada (legacy: textBox1 + textBox1_TextChanged) ---
    // En el legacy fija el PremioTope = Recaudacion * Porcentaje14 / 100.
    [ObservableProperty]
    private double _recaudacion = 8000000;

    // --- Límites de Esperanza Matemática (legacy: txEMmin / txEMmax) ---
    [ObservableProperty]
    private double _emMin = 0.133;

    [ObservableProperty]
    private double _emMax = 50;

    // --- Opciones de salida (legacy: chkOrdenar / chkPonerEM) ---
    [ObservableProperty]
    private bool _ordenarPorEm;

    [ObservableProperty]
    private bool _anadirEmAlFichero;

    // --- Cálculo de valoración de una columna (legacy: txtColumna + btnCalculoVal) ---
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CalcularValoracionColumnaCommand))]
    private string _columna = string.Empty;

    // Resultados de la valoración de la columna. Texto para no bindear double->TextBlock.Text.
    // Legacy: txProbApostada (premio estimado de 14), txProbReal, txEM.
    [ObservableProperty]
    private string _premioEstimado14Texto = "-";

    [ObservableProperty]
    private string _probabilidadRealTexto = "-";

    [ObservableProperty]
    private string _esperanzaMatematicaTexto = "-";

    // Estado de la operación (legacy: statusBarPanel6: "Leyendo...", "Calculando...", "Finalizado (N columnas)").
    [ObservableProperty]
    private string _estadoTexto = "Listo";

    // Evita reentradas mientras se ejecuta el cálculo pesado.
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CalcularCommand))]
    private bool _calculando;

    private bool ColumnaValida => Columna != null && Columna.Length == 14;

    // ---------------------------------------------------------------------
    // Estado del motor (transcrito de los campos de instancia de RentabilidadFrm).
    // ---------------------------------------------------------------------
    private const int NumColumnas14T = 4782969; // 3^14

    private readonly int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };
    private float[,] pr = new float[14, 3];
    private float[,] pa = new float[14, 3];
    private float[,] Cra = new float[14, 3];
    private float[,] Crp = new float[14, 3];
    private ApuestaProbable[] Ap14T = Array.Empty<ApuestaProbable>();
    private BitArray Bits = new BitArray(NumColumnas14T, false);
    private float PremioTope;
    private float PremioDe14;
    private float Premio;
    private float Esperanza;
    private int Profundidad;
    private double EMmin;
    private double EMmax;
    private int NumCols;

    // ---------------------------------------------------------------------
    // Acciones.
    // ---------------------------------------------------------------------

    /// <summary>
    /// Selecciona el fichero de columnas de entrada.
    /// Legacy: RentabilidadFrm.btOpenFicheroEntrada_Click (OpenFileDialog *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarFicheroEntrada()
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        FicheroEntrada = file.Path;
    }

    /// <summary>
    /// Selecciona el fichero de salida de resultados.
    /// Legacy: RentabilidadFrm.button4_Click (SaveFileDialog *.txt; FilterIndex==2 -> salidaBinaria).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarFicheroSalida()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Rentabilidad",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        FicheroSalida = file.Path;
    }

    /// <summary>
    /// Calcula la valoración (EM) de la columna introducida.
    /// Legacy: RentabilidadFrm.btnCalculoVal_Click -> CalcularValoracionColumna().
    /// </summary>
    [RelayCommand(CanExecute = nameof(ColumnaValida))]
    private void CalcularValoracionColumna()
    {
        // Legacy: RentabilidadFrm.CalcularValoracionColumna() (línea 113). v/p = .Valores de los dos
        // ControlPorcentajes; pa/pr = Porcentajes.ValoresBase100() (normalización a tanto-por-uno).
        // No depende del motor de 4,7M -> totalmente cableable.
        double precioApuesta = Free1X2.VariablesGlobales.PrecioApuesta;
        double pctPremio14 = Free1X2.VariablesGlobales.Porcentaje14;
        float premioDe14 = (float)precioApuesta * (float)pctPremio14 / 100;
        float premioTope = (float)Recaudacion * (float)pctPremio14 / 100;

        // v/p = controlPorcentajes*.Valores ; pa/pr = Porcentajes.ValoresBase100() (RentabilidadFrm.cs 122-130).
        float[,] paLocal = new Porcentajes(PorcentajesHelper.AMatriz(PorcentajesApostados)).ValoresBase100();
        float[,] prLocal = new Porcentajes(PorcentajesHelper.AMatriz(PorcentajesReales)).ValoresBase100();

        string apuesta = Columna;
        float p14Apostada = 1, p14Real = 1;
        for (int partido = 0; partido < 14; partido++)
        {
            switch (apuesta[partido])
            {
                case '1': p14Apostada *= paLocal[partido, 0]; p14Real *= prLocal[partido, 0]; break;
                case '2': p14Apostada *= paLocal[partido, 2]; p14Real *= prLocal[partido, 2]; break;
                default: p14Apostada *= paLocal[partido, 1]; p14Real *= prLocal[partido, 1]; break;
            }
        }

        float premio = premioDe14 / p14Apostada;
        if (premio > premioTope) premio = premioTope;
        float esperanza = premio * p14Real;

        PremioEstimado14Texto = Math.Round(precioApuesta * pctPremio14 / 100 / p14Apostada, 2).ToString();
        ProbabilidadRealTexto = p14Real.ToString();
        EsperanzaMatematicaTexto = esperanza.ToString();
        EstadoTexto = "Valoración calculada";
    }

    /// <summary>
    /// Calcula la rentabilidad de todas las columnas y graba el fichero de salida.
    /// Legacy: RentabilidadFrm.btnOK_Click (EncontrarDistantes1 + ordena + GrabacionColumnas).
    /// Recorrido pesado del 3^14 -> Task.Run; estado por DispatcherQueue.
    /// </summary>
    [RelayCommand(CanExecute = nameof(PuedeCalcular))]
    private async Task Calcular()
    {
        // Validación de ficheros (origen fichero requiere FicheroEntrada; salida siempre).
        if (string.IsNullOrEmpty(FicheroSalida) ||
            (OrigenEsFichero && string.IsNullOrEmpty(FicheroEntrada)))
        {
            EstadoTexto = "Faltan datos";
            return;
        }

        // Snapshot de los parámetros de UI (se leen en el hilo de UI antes del Task.Run).
        double precioApuesta = Free1X2.VariablesGlobales.PrecioApuesta;
        double pctPremio14 = Free1X2.VariablesGlobales.Porcentaje14;
        PremioDe14 = (float)precioApuesta * (float)pctPremio14 / 100;
        PremioTope = (float)Recaudacion * (float)pctPremio14 / 100;

        // v/p = controlPorcentajes*.Valores -> Porcentajes.ValoresBase100() (RentabilidadFrm.cs 889-897).
        pa = new Porcentajes(PorcentajesHelper.AMatriz(PorcentajesApostados)).ValoresBase100();
        pr = new Porcentajes(PorcentajesHelper.AMatriz(PorcentajesReales)).ValoresBase100();

        bool origenFichero = OrigenEsFichero;
        string ficheroEntrada = FicheroEntrada;
        string ficheroSalida = FicheroSalida;
        bool ordenar = OrdenarPorEm;
        bool ponerEm = AnadirEmAlFichero;
        double emMinTxt = EmMin;
        double emMaxTxt = EmMax;

        Calculando = true;
        try
        {
            await Task.Run(() =>
            {
                ActualizarEstado("Inicializando...");

                // -- Inicializamos el array por si ejecutamos por 2ª vez -- (RentabilidadFrm.cs 859-863).
                Ap14T = new ApuestaProbable[NumColumnas14T];
                for (int i = 0; i < NumColumnas14T; i++)
                {
                    Ap14T[i].Columna = i;
                    Ap14T[i].Probabilidad = 0;
                }

                // Cargamos las columnas a analizar (RentabilidadFrm.cs 867-876).
                if (origenFichero)
                {
                    ActualizarEstado("Leyendo columnas...");
                    LeerColumnas(ficheroEntrada);   // Fichero
                }
                else
                {
                    Bits = new BitArray(NumColumnas14T, true); // 14 triples
                }

                ActualizarEstado("Calculando...");

                float[] p14 = new float[2];
                short Partido;

                p14[0] = 1; // Probabilidad apostada
                p14[1] = 1; // Probabilidad real

                //-- probabilidad de la apuesta 11111111111111 -- (RentabilidadFrm.cs 905-916).
                for (Partido = 0; Partido < 14; Partido++)
                {
                    p14[0] *= pa[Partido, 0];
                    p14[1] *= pr[Partido, 0];

                    //--valores cuando se falla cada uno de los signos, a usar para evaluar los 13's-----
                    Cra[Partido, 0] = pa[Partido, 1] / pa[Partido, 0];
                    Cra[Partido, 1] = pa[Partido, 2] / pa[Partido, 0];
                    Crp[Partido, 0] = pr[Partido, 1] / pr[Partido, 0];
                    Crp[Partido, 1] = pr[Partido, 2] / pr[Partido, 0];
                }

                Profundidad = 0;
                Premio = PremioDe14 / p14[0];
                if (Premio > PremioTope) Premio = PremioTope;
                Esperanza = Premio * p14[1];
                if (Bits[0])
                {
                    Ap14T[0].Columna = 0;
                    Ap14T[0].Probabilidad = -Esperanza;
                }
                else
                {
                    Ap14T[0].Probabilidad = (float)3E+7;
                }

                EncontrarDistantes1(p14[0], p14[1], 0, 0, 14);

                if (ordenar)
                {
                    ActualizarEstado("Ordenando...");
                    ordena(0, 4782968);
                }

                ActualizarEstado("Grabando...");
                GrabacionColumnas(ficheroSalida, emMinTxt, emMaxTxt, ponerEm);

                ActualizarEstado("Finalizado (" + NumCols.ToString() + " columnas)");
            });
        }
        catch (Exception ex)
        {
            ActualizarEstado("Error: " + ex.Message);
        }
        finally
        {
            Calculando = false;
        }
    }

    private bool PuedeCalcular() => !Calculando;

    // statusBarPanel6.Text legacy -> EstadoTexto marshalado al hilo de UI.
    private void ActualizarEstado(string texto)
    {
        var disp = AppServices.UiDispatcher;
        if (disp is null) { EstadoTexto = texto; return; }
        disp.TryEnqueue(() => EstadoTexto = texto);
    }

    // ---------------------------------------------------------------------
    // Motor transcrito literalmente de RentabilidadFrm.cs.
    // ---------------------------------------------------------------------

    // RentabilidadFrm.cs 1044-1049.
    private void LeerColumnas(string ficheroEntrada)
    {
        IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(ficheroEntrada);
        Bits = comBaseCols.LeerTodasColsABitArray(14);
        comBaseCols.Cerrar();
    }

    // RentabilidadFrm.cs 949-981.
    private void EncontrarDistantes1(float pProbA, float pProbR, int IndiceInicial, int PosicionInicial, int pProfundidad)
    {
        int Partido;
        int z;
        int Indice;
        float ProbA;
        float ProbR;
        Profundidad++;

        //'--encontramos las apuestas que se diferencian en un solo signo ----
        for (Partido = PosicionInicial; Partido < 14; Partido++)
        {
            for (z = 0; z < 2; z++)
            {
                Indice = IndiceInicial + pot[Partido] * (z + 1);
                ProbA = pProbA * Cra[Partido, z];
                ProbR = pProbR * Crp[Partido, z];
                Premio = PremioDe14 / ProbA;
                if (Premio > PremioTope) Premio = PremioTope;
                Esperanza = Premio * ProbR;
                if (Bits[Indice])
                {
                    Ap14T[Indice].Columna = Indice;
                    Ap14T[Indice].Probabilidad = -Esperanza;
                }
                if (Profundidad < pProfundidad)
                {
                    EncontrarDistantes1(ProbA, ProbR, Indice, Partido + 1, pProfundidad);
                }
            }
        }
        Profundidad--;
    }

    // RentabilidadFrm.cs 1078-1107.
    private void ordena(int izq, int der)
    {
        int i = 0, j = 0;
        ApuestaProbable x = new ApuestaProbable();
        ApuestaProbable aux = new ApuestaProbable();
        i = izq;
        j = der;
        x = Ap14T[(izq + der) / 2];

        do
        {
            while ((Ap14T[i].Probabilidad < x.Probabilidad) && (j <= der))
            {
                i++;
            }
            while ((x.Probabilidad < Ap14T[j].Probabilidad) && (j > izq))
            {
                j--;
            }
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

    // RentabilidadFrm.cs 1051-1077.
    private void GrabacionColumnas(string archivoSalida, double emMinTxt, double emMaxTxt, bool ponerEm)
    {
        EMmin = -emMinTxt;
        EMmax = -emMaxTxt;
        ConvertidorDeBases col = new ConvertidorDeBases();
        IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
        NumCols = 0;
        if (ponerEm)
        {
            for (int nr = 0; nr < NumColumnas14T; nr++)
            {
                if (Bits[Ap14T[nr].Columna] && Ap14T[nr].Probabilidad >= EMmax && Ap14T[nr].Probabilidad <= EMmin)
                { comCols.GuardarColsComa(col.ConvNumAColumna(Ap14T[nr].Columna) + (char)9 + (-Ap14T[nr].Probabilidad)); NumCols++; }
            }
        }
        else
        {
            for (int nr = 0; nr < NumColumnas14T; nr++)
            {
                if (Bits[Ap14T[nr].Columna] && Ap14T[nr].Probabilidad >= EMmax && Ap14T[nr].Probabilidad <= EMmin)
                { comCols.GuardarCols(col.ConvNumAColumna(Ap14T[nr].Columna)); NumCols++; }
            }
        }
        comCols.Cerrar();
    }
}
