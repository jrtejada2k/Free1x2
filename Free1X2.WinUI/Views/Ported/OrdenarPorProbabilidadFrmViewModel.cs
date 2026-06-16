using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
/// Lógica de dominio (cálculo y persistencia) NO portada: ver clases legacy
/// <c>Free1X2.Analisis.ApuestaProbableCentral</c>, <c>Free1X2.Analisis.Porcentajes</c>,
/// <c>Free1X2.EntradaSalida.AConfiguracion</c> y los métodos
/// OrdenaPorProductos / OrdenaPorSumas / OrdenacionMultiple del form original.
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

    // Rutas reales de los ficheros elegidos (legacy: archivoEntrada / archivoSalida).
    private string _rutaEntrada = string.Empty;
    private string _rutaSalida = string.Empty;

    public OrdenarPorProbabilidadFrmViewModel()
    {
        // TODO(dominio): el form legacy, en su constructor, llama
        //   AConfiguracion.ObtenValoresLAE(ref PrecioApuesta, ref PorcentajeDestinadoAlPremiode14,
        //                                   ref Recaudacion, ref moneda)
        // para inicializar los campos L.A.E. con la configuración persistida.
        // El dominio no está disponible aquí; se dejan los valores por defecto del Designer.
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
    [RelayCommand]
    private void Ordenar()
    {
        // Validación de ficheros igual que HabilitarCalcular() legacy.
        if (string.IsNullOrEmpty(_rutaSalida) ||
            (Origen14Triples == false && string.IsNullOrEmpty(_rutaEntrada)))
        {
            Estado = "Faltan datos";
            return;
        }

        // TODO: lógica de cálculo en Free1X2/UI/OrdenarPorProbabilidadFrm.cs:
        //   - btCalcular_Click (línea 1110) -> según PestanaSeleccionada:
        //       0 Productos -> OrdenaPorProductos()   (línea 1205)
        //       1 Sumas     -> OrdenaPorSumas()       (línea 1237)
        //       2 Multiple  -> OrdenacionMultiple()   (línea 1142) + GrabarAdmitidasMultiples (1462)
        //   Estas rutinas operan sobre Ap14T (ApuestaProbableCentral[4782969]),
        //   Free1X2.Analisis.Porcentajes y la configuración L.A.E.; es lógica extensa de la UI
        //   legacy (1814 líneas, no en el motor) y no se transcribe aquí para no inventar
        //   comportamiento. Los selectores de fichero ya están cableados.
        Estado = "Cálculo pendiente de portar (ver OrdenarPorProbabilidadFrm.cs)";
    }

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
}
