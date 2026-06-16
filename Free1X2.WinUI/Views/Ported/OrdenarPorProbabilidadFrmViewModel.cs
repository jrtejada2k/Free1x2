using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
/// ViewModel del formulario legacy WinForms "OrdenarPorProbabilidadFrm"
/// (título: "Ordenación de columnas por probabilidad").
///
/// Propósito: ordenar / filtrar las columnas de la quiniela (14 triples = 4.782.969 columnas,
/// o las leídas de un fichero) según un valor central de ordenación, escribiendo a un fichero
/// de salida un máximo de columnas. Soporta tres modos (las TabPages legacy):
///   - "Por productos": ordena por la probabilidad (producto de porcentajes) en torno a un valor
///       central expresado como Nº acertantes / Premio / Probabilidad / Log. neperiano (LN) / Columna.
///   - "Por Sumas": ordena por sumas en torno a un valor central de suma.
///   - "Multiple": genera varios tramos entre LN mínimo y LN máximo (o sumas), con N puntos
///       centrales y N columnas por punto.
///
/// El motor (OrdenaPorProductos / OrdenaPorSumas / OrdenacionMultiple / EncontrarDistantes1 /
/// ordena / GrabacionColumnas / GrabarAdmitidasMultiples) se transcribe literalmente de
/// OrdenarPorProbabilidadFrm.cs operando sobre Ap14T = ApuestaProbableCentral[4782969].
/// </summary>
public partial class OrdenarPorProbabilidadFrmViewModel : ObservableObject
{
    // ---- Origen de las columnas a ordenar (groupBox1: rb14Triples / rbFichero) ----

    [ObservableProperty]
    private bool _origen14Triples = true;

    [ObservableProperty]
    private bool _origenFichero;

    // Habilitación del selector de fichero de entrada (sólo activo con "Fichero").
    [ObservableProperty]
    private bool _ficheroEntradaHabilitado;

    [ObservableProperty]
    private string _ficheroEntrada = "(falta selección)";

    // ---- Fichero de salida (groupSalida) ----

    [ObservableProperty]
    private string _ficheroSalida = "(falta selección)";

    // txMaxColumnas: Nº máximo de columnas a escribir (NumberBox -> double; regla anti-crash 7).
    [ObservableProperty]
    private double _maxColumnas = 4782969;

    // checkValorOrdenacion: añadir probabilidad acumulada a la salida.
    [ObservableProperty]
    private bool _anadirProbabilidadAcumulada;

    // txtLimiteProbAcum: límite de la probabilidad acumulada.
    [ObservableProperty]
    private double _limiteProbAcumulada = 1;

    // chkValorPremio14: añadir Premio de 14 aciertos a la salida.
    [ObservableProperty]
    private bool _anadirPremio14;

    // ---- Pestaña activa (tabControl1: 0=Productos, 1=Sumas, 2=Multiple) ----

    [ObservableProperty]
    private int _pestanaSeleccionada;

    // ---- Tab "Por productos" (groupBox2: valor central de ordenación) ----
    // RadioButtons exclusivos: N° acertantes / Premio / Probabilidad / Log. neperiano / Columna.

    [ObservableProperty]
    private bool _modoAcertantes = true;

    [ObservableProperty]
    private bool _modoPremio;

    [ObservableProperty]
    private bool _modoProbabilidad;

    [ObservableProperty]
    private bool _modoLN;

    [ObservableProperty]
    private bool _modoColumna;

    // TextBox asociados a cada radio (sólo el del radio activo se habilita en el form legacy,
    // ver Generico_CheckedChanged).
    [ObservableProperty]
    private string _acertantes = "1,5";

    [ObservableProperty]
    private bool _acertantesHabilitado = true;

    [ObservableProperty]
    private string _premio = "";

    [ObservableProperty]
    private bool _premioHabilitado;

    [ObservableProperty]
    private string _probabilidad = "";

    [ObservableProperty]
    private bool _probabilidadHabilitado;

    [ObservableProperty]
    private string _ln = "";

    [ObservableProperty]
    private bool _lnHabilitado;

    [ObservableProperty]
    private string _columna = "";

    [ObservableProperty]
    private bool _columnaHabilitado;

    // comboBox1 (sólo visible con LN): valores de aciertos a considerar.
    public IReadOnlyList<string> OpcionesAciertos { get; } = new[] { "0", "10", "11", "12", "13", "14" };

    [ObservableProperty]
    private string _aciertosSeleccionados = "14";

    [ObservableProperty]
    private bool _comboAciertosVisible;

    // ---- Configuración L.A.E. (grLAE) ----
    // Valores cargados en el form legacy desde AConfiguracion.ObtenValoresLAE.

    [ObservableProperty]
    private string _recaudacion = "15000000";

    [ObservableProperty]
    private string _precioApuesta = "0,5";

    [ObservableProperty]
    private string _porcentajePremio14 = "15";

    // ---- Tab "Por Sumas" ----

    [ObservableProperty]
    private double _valorCentralSumas;

    // ---- Tab "Multiple" (groupBox3 + campos de tramos) ----

    [ObservableProperty]
    private bool _multiplePorProbabilidad = true;

    [ObservableProperty]
    private bool _multiplePorSumas;

    [ObservableProperty]
    private double _lnMinimo;

    [ObservableProperty]
    private double _lnMaximo;

    [ObservableProperty]
    private double _numPuntos = 1;

    [ObservableProperty]
    private double _numColumnasPorTramo;

    // ---- Estado (statusBarPanel4 del form legacy) ----

    [ObservableProperty]
    private string _estado = "Faltan datos";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OrdenarCommand))]
    private bool _calculando;

    // Rutas reales de los ficheros elegidos (legacy: archivoEntrada / archivoSalida).
    private string _rutaEntrada = string.Empty;
    private string _rutaSalida = string.Empty;

    // Rejilla de porcentajes editable (sustituye al UserControl WinForms controlPorcentajes1).
    public System.Collections.ObjectModel.ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    public OrdenarPorProbabilidadFrmViewModel()
    {
        // El form legacy, en su constructor, llama AConfiguracion.ObtenValoresLAE para inicializar
        // los campos L.A.E. Se replica con los valores globales ya disponibles en el dominio.
        Recaudacion = Free1X2.VariablesGlobales.Recaudacion.ToString();
        PrecioApuesta = Free1X2.VariablesGlobales.PrecioApuesta.ToString();
        PorcentajePremio14 = Free1X2.VariablesGlobales.Porcentaje14.ToString();
    }

    // ---- Dependencias de UI (equivalen a los CheckedChanged del form legacy) ----

    partial void OnOrigenFicheroChanged(bool value)
    {
        // rbFichero_CheckedChanged: habilita el selector de fichero de entrada.
        FicheroEntradaHabilitado = value;
    }

    // Generico_CheckedChanged: sólo el TextBox del radio activo se habilita,
    // y el combo de aciertos sólo es visible con "Log. neperiano".
    partial void OnModoAcertantesChanged(bool value) { if (value) ActualizarModoCentral(); }
    partial void OnModoPremioChanged(bool value) { if (value) ActualizarModoCentral(); }
    partial void OnModoProbabilidadChanged(bool value) { if (value) ActualizarModoCentral(); }
    partial void OnModoLNChanged(bool value) { if (value) ActualizarModoCentral(); }
    partial void OnModoColumnaChanged(bool value) { if (value) ActualizarModoCentral(); }

    private void ActualizarModoCentral()
    {
        AcertantesHabilitado = ModoAcertantes;
        PremioHabilitado = ModoPremio;
        ProbabilidadHabilitado = ModoProbabilidad;
        LnHabilitado = ModoLN;
        ColumnaHabilitado = ModoColumna;
        ComboAciertosVisible = ModoLN;
    }

    /// <summary>Selecciona el fichero de entrada (button1 del form legacy).</summary>
    [RelayCommand]
    private async Task SeleccionarFicheroEntrada()
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        _rutaEntrada = file.Path;
        FicheroEntrada = _rutaEntrada;
        ActualizarEstado();
    }

    /// <summary>Selecciona el fichero de salida (button2 del form legacy).</summary>
    [RelayCommand]
    private async Task SeleccionarFicheroSalida()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ColumnasOrdenadas",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        _rutaSalida = file.Path;
        FicheroSalida = _rutaSalida;
        ActualizarEstado();
    }

    /// <summary>Lanza la ordenación (btCalcular / "&amp;Ordenar" del form legacy).</summary>
    [RelayCommand(CanExecute = nameof(PuedeOrdenar))]
    private async Task Ordenar()
    {
        // Validación de ficheros igual que HabilitarCalcular() legacy.
        if (string.IsNullOrEmpty(_rutaSalida) ||
            (Origen14Triples == false && string.IsNullOrEmpty(_rutaEntrada)))
        {
            Estado = "Faltan datos";
            return;
        }

        // Snapshot de los parámetros L.A.E. y de salida (se leen en el hilo de UI).
        if (!TryParseEsp(Recaudacion, out _recaudacionNum)) _recaudacionNum = 0;
        if (!TryParseEsp(PrecioApuesta, out _precioApuestaNum)) _precioApuestaNum = 0;
        if (!TryParseEsp(PorcentajePremio14, out _porcentaje14Num)) _porcentaje14Num = 0;
        MaxColumnasMotor = (int)MaxColumnas;
        if (MaxColumnasMotor > 4782969) MaxColumnasMotor = 4782969;
        _origenFicheroMotor = OrigenFichero;
        _archivoEntrada = _rutaEntrada;
        _archivoSalida = _rutaSalida;

        // Valor central de ordenación a partir del modo seleccionado (Por productos).
        CalcularValorCentral();

        Calculando = true;
        try
        {
            await Task.Run(() =>
            {
                // btCalcular_Click (OrdenarPorProbabilidadFrm.cs 1110): según la pestaña activa.
                switch (PestanaSeleccionada)
                {
                    case 0: // Productos
                        CalculoMultiple = false;
                        OrdenaPorProductos();
                        break;
                    case 1: // Sumas
                        CalculoMultiple = false;
                        OrdenaPorSumas();
                        break;
                    case 2: // Multiple
                        CalculoMultiple = true;
                        OrdenacionMultiple();
                        break;
                }
            });
        }
        catch (Exception ex)
        {
            ActualizarEstadoMotor("Error: " + ex.Message);
        }
        finally
        {
            Calculando = false;
        }
    }

    private bool PuedeOrdenar() => !Calculando;

    /// <summary>Cancela / cierra (button3 / "&amp;Cancelar" del form legacy).</summary>
    [RelayCommand]
    private void Cancelar()
    {
        // Navegación WinUI (Frame.GoBack) es responsabilidad del host de la Page.
        Estado = "Faltan datos";
    }

    // HabilitarCalcular() legacy: estado "Preparado"/"Faltan datos" según ficheros.
    private void ActualizarEstado()
    {
        bool listo = !string.IsNullOrEmpty(_rutaSalida) &&
            (Origen14Triples || !string.IsNullOrEmpty(_rutaEntrada));
        Estado = listo ? "Preparado" : "Faltan datos";
    }

    // statusBarPanel4.Text legacy -> Estado marshalado al hilo de UI.
    private void ActualizarEstadoMotor(string texto)
    {
        var disp = AppServices.UiDispatcher;
        if (disp is null) { Estado = texto; return; }
        disp.TryEnqueue(() => Estado = texto);
    }

    // ---------------------------------------------------------------------
    // Estado del motor (transcrito de los campos de instancia del form legacy).
    // ---------------------------------------------------------------------
    private const int NumColumnas14T = 4782969; // 3^14

    private readonly int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };
    private float[,] p = new float[14, 3];
    private float[,] Cr = new float[14, 3];
    private double[,] v = new double[14, 3];
    private double _recaudacionNum;
    private double _precioApuestaNum;
    private double _porcentaje14Num;
    private double _LN;
    private double _Probabilidad;
    private double _Premio;
    private double _Acertantes;
    private int Profundidad = 0;
    private int NumApuestas;
    private int MaxColumnasMotor = 4782969;
    private float LimiteProbabilidadAcumulada;
    private float ProbabilidadAcumulada;
    private int c = 0;
    private bool CalculoMultiple = false;
    private string numIter = "";
    private ApuestaProbableCentral[] Ap14T = Array.Empty<ApuestaProbableCentral>();
    private BitArray Bits = new BitArray(NumColumnas14T, false);
    private BitArray Admitidas = new BitArray(NumColumnas14T, false);
    private bool _origenFicheroMotor;
    private string _archivoEntrada = string.Empty;
    private string _archivoSalida = string.Empty;

    // ---------------------------------------------------------------------
    // Valor central de ordenación (transcrito de los *_TextChanged del form legacy).
    // Calcula _Probabilidad/_LN/_Premio/_Acertantes a partir del modo activo.
    // ---------------------------------------------------------------------
    private void CalcularValorCentral()
    {
        double precio = _precioApuestaNum;
        double pct = _porcentaje14Num;
        double recaud = _recaudacionNum;

        if (ModoColumna)
        {
            // txColumna_TextChanged (1776): producto de los porcentajes de la columna.
            _Probabilidad = 1;
            double[,] vals = PorcentajesHelper.AMatriz(Porcentajes);
            string columna = Columna;
            for (int i = 0; i < columna.Length; i++)
            {
                switch (columna[i])
                {
                    case '1': _Probabilidad *= vals[i, 0] / 100; break;
                    case '2': _Probabilidad *= vals[i, 2] / 100; break;
                    default: _Probabilidad *= vals[i, 1] / 100; break;
                }
            }
            _LN = Math.Log(_Probabilidad);
            _Premio = precio * pct / _Probabilidad / 100;
            _Acertantes = recaud / precio * _Probabilidad;
        }
        else if (ModoLN)
        {
            // TxLN_TextChanged_1 (1703).
            if (TryParseEsp(Ln, out double ln)) _LN = ln;
            _Probabilidad = Math.Exp(_LN);
            _Premio = precio * pct / _Probabilidad / 100;
            _Acertantes = recaud / precio * _Probabilidad;
        }
        else if (ModoProbabilidad)
        {
            // TxProbabilidad_TextChanged_1 (1685).
            if (TryParseEsp(Probabilidad, out double prob)) _Probabilidad = prob;
            _LN = Math.Log(_Probabilidad);
            _Premio = precio * pct / _Probabilidad / 100;
            _Acertantes = recaud / precio * _Probabilidad;
        }
        else if (ModoPremio)
        {
            // TxPremio_TextChanged_1 (1643).
            if (TryParseEsp(Premio, out double premio)) _Premio = premio;
            _Probabilidad = precio * pct / _Premio / 100;
            _LN = Math.Log(_Probabilidad);
            _Acertantes = recaud / precio * _Probabilidad;
        }
        else // ModoAcertantes (por defecto)
        {
            // TxAcertantes_TextChanged_1 (1668).
            if (TryParseEsp(Acertantes, out double acertantes)) _Acertantes = acertantes;
            _Premio = recaud * pct / _Acertantes / 100;
            _Probabilidad = precio * pct / _Premio / 100;
            _LN = Math.Log(_Probabilidad);
        }
    }

    // Equivale a Convert.ToDouble(text.Replace(".",",")) con Porcentajes.EsNumero del form legacy.
    private static bool TryParseEsp(string texto, out double valor)
    {
        valor = 0;
        if (string.IsNullOrWhiteSpace(texto)) return false;
        string normal = texto.Trim().Replace(".", ",");
        return double.TryParse(normal, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("es-ES"), out valor);
    }

    // ---------------------------------------------------------------------
    // Motor transcrito literalmente de OrdenarPorProbabilidadFrm.cs.
    // ---------------------------------------------------------------------

    // OrdenarPorProbabilidadFrm.cs 1133-1141.
    private void InicializarApuestas()
    {
        Ap14T = new ApuestaProbableCentral[NumColumnas14T];
        for (int i = 0; i < NumColumnas14T; i++)
        {
            Ap14T[i].Columna = i;
            Ap14T[i].Probabilidad = 0;
            Ap14T[i].ProbabilidadDiferencial = 0;
        }
    }

    // OrdenarPorProbabilidadFrm.cs 1142-1204.
    private void OrdenacionMultiple()
    {
        double probMin = LnMinimo;
        double probMax = LnMaximo;
        int NumPuntos = (int)NumPuntos_();
        int i = 0;
        int x;

        int maxcol = MaxColumnasMotor;
        if (probMax < probMin)
        {
            double aux = probMax;
            probMax = probMin;
            probMin = aux;
        }
        if (MultiplePorProbabilidad)
        {
            if (probMax > 0)
            {
                ActualizarEstadoMotor("Los LN deben ser valores negativos");
                return;
            }
            // (form legacy: MessageBox.Show; aquí se refleja en la barra de estado)
        }

        double paso = (probMax - probMin) / (NumPuntos - 1);
        MaxColumnasMotor = (int)NumColumnasPorTramo;
        Admitidas.SetAll(false);

        for (double prCentral = probMin; prCentral <= probMax; prCentral += paso)
        {
            i++;
            numIter = i.ToString() + " ";
            if (MultiplePorProbabilidad)
            {
                _Probabilidad = Math.Exp(prCentral);
            }
            else
            {
                ValorCentralSumas = prCentral;
            }
            if (i == 1)
            {
                if (MultiplePorProbabilidad)
                { OrdenaPorProductos(); }
                else
                { OrdenaPorSumas(); }
            }
            else
            {
                for (x = 0; x < NumColumnas14T; x++)
                {
                    if (Ap14T[x].ProbabilidadDiferencial == (float)9E+10) continue;
                    Ap14T[x].ProbabilidadDiferencial = Math.Abs(Ap14T[x].Probabilidad - (float)prCentral);
                }
                ActualizarEstadoMotor(numIter + "Ordenando apuestas ...");
                ordena(0, 4782968);
                for (int nr = 0; nr < MaxColumnasMotor; nr++) Admitidas[Ap14T[nr].Columna] = true;
            }
        }
        GrabarAdmitidasMultiples();
        MaxColumnasMotor = maxcol;
    }

    // NumPuntos es propiedad observable double; se necesita el valor numérico para el bucle.
    private double NumPuntos_() => NumPuntos;

    // OrdenarPorProbabilidadFrm.cs 1205-1236.
    private void OrdenaPorProductos()
    {
        int Partido;
        float Prob = 0;
        if (_Probabilidad > 0) _LN = Math.Log(_Probabilidad); else _LN = -99999;
        InicializarApuestas();

        if (MaxColumnasMotor > 4782969) MaxColumnasMotor = 4782969;

        //---Leer Porcentajes --------------
        v = PorcentajesHelper.AMatriz(Porcentajes);
        Porcentajes Pct = new Porcentajes(v);
        p = Pct.ValoresNeperianos();

        // probabilidad del 14 de la 1ª apuesta y cálculo de valores complementarios
        for (Partido = 0; Partido < 14; Partido++)
        {
            Prob += p[Partido, 0];
            Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
            Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
        }

        if (Origen14Triples == true)
        {
            Calcula14Triples(Prob);
        }
        else
        {
            CalculaFichero(Prob);
        }
    }

    // OrdenarPorProbabilidadFrm.cs 1237-1268.
    private void OrdenaPorSumas()
    {
        int Partido;
        float Prob = 0;
        InicializarApuestas();
        _LN = ValorCentralSumas;

        if (MaxColumnasMotor > 4782969) MaxColumnasMotor = 4782969;

        //---Leer Porcentajes --------------
        v = PorcentajesHelper.AMatriz(Porcentajes);
        // Suma del 14 de la 1ª apuesta y cálculo de valores complementarios
        for (Partido = 0; Partido < 14; Partido++)
        {
            p[Partido, 0] = (float)v[Partido, 0];
            p[Partido, 1] = (float)v[Partido, 1];
            p[Partido, 2] = (float)v[Partido, 2];
            Prob += p[Partido, 0];
            Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
            Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
        }

        if (Origen14Triples == true)
        {
            Calcula14Triples(Prob);
        }
        else
        {
            CalculaFichero(Prob);
        }
    }

    // OrdenarPorProbabilidadFrm.cs 1270-1298.
    private void Calcula14Triples(float Prob)
    {
        Bits.SetAll(true);
        NumApuestas = 4782969;
        ActualizarEstadoMotor(numIter + "Calculando probabilidades ...");
        Profundidad = 0;
        EncontrarDistantes1(Prob, 0, 0, 14);
        Ap14T[0].ProbabilidadDiferencial = Math.Abs(Prob - (float)_LN);
        Ap14T[0].Probabilidad = Prob;
        Ap14T[0].Columna = 0;

        ActualizarEstadoMotor(numIter + "Ordenando apuestas ...");
        ordena(0, 4782968);

        ActualizarEstadoMotor(numIter + "Guardando apuestas ...");
        if (CalculoMultiple == false)
        {
            GrabacionColumnas();
            ActualizarEstadoMotor("Finalizado " + c.ToString() + " columnas");
        }
        else
        {
            for (int nr = 0; nr < MaxColumnasMotor; nr++) Admitidas[Ap14T[nr].Columna] = true;
        }
    }

    // OrdenarPorProbabilidadFrm.cs 1300-1333.
    private void CalculaFichero(float Prob)
    {
        LeerColumnas();
        ActualizarEstadoMotor("Calculando probabilidades ...");
        Profundidad = 0;
        EncontrarDistantes1(Prob, 0, 0, 14);
        if (Bits[0])
        {
            Ap14T[0].ProbabilidadDiferencial = Math.Abs(Prob - (float)_LN);
            Ap14T[0].Probabilidad = Prob;
            Ap14T[0].Columna = 0;
        }
        else
        {
            Ap14T[0].ProbabilidadDiferencial = (float)9E+10;
            Ap14T[0].Probabilidad = Prob;
        }
        ActualizarEstadoMotor(numIter + "Ordenando apuestas ...");
        ordena(0, 4782968);

        ActualizarEstadoMotor(numIter + "Guardando apuestas ...");
        if (CalculoMultiple == false)
        {
            GrabacionColumnas();
            ActualizarEstadoMotor("Finalizado " + c.ToString() + " columnas");
        }
        else
        {
            for (int nr = 0; nr < MaxColumnasMotor; nr++) Admitidas[Ap14T[nr].Columna] = true;
        }
    }

    // OrdenarPorProbabilidadFrm.cs 1334-1340.
    private void LeerColumnas()
    {
        IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(_archivoEntrada);
        NumApuestas = Convert.ToInt32(comBaseCols.ObtenNumCols());
        Bits = comBaseCols.LeerTodasColsABitArray(14);
        comBaseCols.Cerrar();
    }

    // OrdenarPorProbabilidadFrm.cs 1341-1377.
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
                Indice = IndiceInicial + pot[Partido] * z;
                Prob = pProb + Cr[Partido, z];

                if (Bits[Indice])
                {
                    Ap14T[Indice].Columna = Indice;
                    Ap14T[Indice].ProbabilidadDiferencial = Math.Abs(Prob - (float)_LN);
                    Ap14T[Indice].Probabilidad = Prob;
                }
                else
                {
                    Ap14T[Indice].ProbabilidadDiferencial = (float)9E+10;
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

    // OrdenarPorProbabilidadFrm.cs 1378-1399.
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

    // OrdenarPorProbabilidadFrm.cs 1401-1460.
    private void GrabacionColumnas()
    {
        string archivoSalida = _archivoSalida;
        float pr = (float)(_precioApuestaNum * _porcentaje14Num / 100);
        float prob;
        float premio;
        char sep = (char)9;
        float PremioMax = (float)(_recaudacionNum * _porcentaje14Num / 100);

        LimiteProbabilidadAcumulada = (float)LimiteProbAcumulada;

        ConvertidorDeBases col = new ConvertidorDeBases();
        IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
        if (MaxColumnasMotor < NumApuestas) NumApuestas = MaxColumnasMotor;
        ProbabilidadAcumulada = 0;
        c = 0;
        for (int nr = 0; nr < NumApuestas; nr++)
        {
            StringBuilder linea = new StringBuilder(col.ConvNumAColumna(Ap14T[nr].Columna));
            if (PestanaSeleccionada == 0)
            {
                prob = (float)Math.Exp(Ap14T[nr].Probabilidad);
                ProbabilidadAcumulada += prob;
                if (ProbabilidadAcumulada > LimiteProbabilidadAcumulada) break;
            }
            else
            {
                prob = Ap14T[nr].Probabilidad;
                ProbabilidadAcumulada = Ap14T[nr].Probabilidad;
            }
            if (AnadirProbabilidadAcumulada)
            {
                linea.Append(sep);
                linea.Append(ProbabilidadAcumulada);
            }

            if (AnadirPremio14)
            {
                premio = pr / prob;
                if (premio > PremioMax) premio = PremioMax;
                linea.Append(sep);
                linea.Append(premio);
            }

            comCols.GuardarColsComa(linea.ToString());

            c++;
        }
        comCols.Cerrar();
        if (PestanaSeleccionada == 0)
        {
            ActualizarEstadoMotor("Finalizado " + c.ToString() + " columnas Pr. acum. =" + (100 * ProbabilidadAcumulada).ToString());
        }
        else
        {
            ActualizarEstadoMotor("Finalizado " + c.ToString() + " columnas");
        }
    }

    // OrdenarPorProbabilidadFrm.cs 1462-1522.
    private void GrabarAdmitidasMultiples()
    {
        string archivoSalida = _archivoSalida;
        float pr = (float)(_precioApuestaNum * _porcentaje14Num / 100);
        float prob;
        float premio;
        char sep = (char)9;
        float PremioMax = (float)(_recaudacionNum * _porcentaje14Num / 100);

        LimiteProbabilidadAcumulada = (float)LimiteProbAcumulada;

        ConvertidorDeBases col = new ConvertidorDeBases();
        IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
        if (MaxColumnasMotor < NumApuestas) NumApuestas = MaxColumnasMotor;
        ProbabilidadAcumulada = 0;
        c = 0;
        for (int nr = 0; nr < NumColumnas14T; nr++)
        {
            if (Admitidas[nr] == false) continue;
            StringBuilder linea = new StringBuilder(col.ConvNumAColumna(nr));
            if (MultiplePorProbabilidad)
            {
                prob = (float)Math.Exp(Ap14T[nr].Probabilidad);
                ProbabilidadAcumulada += prob;
                if (ProbabilidadAcumulada > LimiteProbabilidadAcumulada) break;
            }
            else
            {
                prob = Ap14T[nr].Probabilidad;
                ProbabilidadAcumulada = Ap14T[nr].Probabilidad;
            }
            if (AnadirProbabilidadAcumulada)
            {
                linea.Append(sep);
                linea.Append(ProbabilidadAcumulada);
            }

            if (AnadirPremio14)
            {
                premio = pr / prob;
                if (premio > PremioMax) premio = PremioMax;
                linea.Append(sep);
                linea.Append(premio);
            }

            comCols.GuardarColsComa(linea.ToString());

            c++;
        }
        comCols.Cerrar();
        if (PestanaSeleccionada == 0)
        {
            ActualizarEstadoMotor("Finalizado " + c.ToString() + " columnas Pr. acum. =" + (100 * ProbabilidadAcumulada).ToString());
        }
        else
        {
            ActualizarEstadoMotor("Finalizado " + c.ToString() + " columnas");
        }
    }
}
