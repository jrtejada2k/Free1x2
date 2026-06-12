using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Reduccion;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la página portada del WinForms <c>ReductorFrm</c> (Free1X2/UI/ReductorFrm.cs).
/// Reproduce los campos de entrada (archivo entrada/salida, máx. columnas, máx. %, nivel,
/// método y columnas externas) y los resultados de la reducción (col. iniciales, procesadas,
/// admitidas, porcentaje y tiempo). La lógica de dominio (IReduccion: JDC, JDCDobleContador,
/// Redu1305Xfsf, ReductorTM) NO se implementa: queda marcada como TODO.
/// </summary>
public partial class ReductorFrmViewModel : ObservableObject
{
    // ----- Datos de entrada (groupBox "Datos de Entrada") -----

    // lblSelFile: muestra el nombre del archivo de entrada seleccionado.
    [ObservableProperty]
    private string _archivoEntradaTexto = "(falta selección)";

    // lblFicheroSalida: muestra el nombre del archivo de salida seleccionado.
    [ObservableProperty]
    private string _archivoSalidaTexto = "(falta selección)";

    // ----- Nivel de reducción (grNivelReduccion / cmbNivel) -----
    // En el legacy se llena con NumeroPartidos-1 .. 1 (VariablesGlobales.NumeroPartidos).
    // Aquí lo dejamos como lista de strings para ComboBox (regla anti-crash #3).
    public IReadOnlyList<string> NivelesReduccion { get; } =
        new List<string> { "13", "12", "11", "10", "9", "8", "7", "6", "5", "4", "3", "2", "1" };

    [ObservableProperty]
    private string _nivelSeleccionado = "13";

    // ----- Método de reducción (groupBox1 / cBoxMetodo) -----
    public IReadOnlyList<string> MetodosReduccion { get; } =
        new List<string> { "menos tiempo", "menos columnas", "grandes archivos" };

    [ObservableProperty]
    private string _metodoSeleccionado = "menos tiempo";

    // chkExternas: "Admitir columnas externas". En el legacy se oculta para
    // "grandes archivos" y "menos columnas 2".
    [ObservableProperty]
    private bool _admitirColumnasExternas;

    // Habilita/oculta el toggle de columnas externas según el método (legacy
    // cBoxMetodo_SelectedIndexChanged). Lo exponemos como bool para bindear en
    // el propio Control (ToggleSwitch.IsEnabled), nunca en un panel (regla #1).
    public bool ColumnasExternasHabilitado => MetodoSeleccionado != "grandes archivos";

    partial void OnMetodoSeleccionadoChanged(string value)
    {
        OnPropertyChanged(nameof(ColumnasExternasHabilitado));
    }

    // ----- Opciones (groupBox2) -----

    // tBoxMaxCols: máximo de columnas (NumberBox => double).
    [ObservableProperty]
    private double _maxColumnas;

    // tBoxMaxPercent: máximo porcentaje (NumberBox => double, default 100).
    [ObservableProperty]
    private double _maxPorcentaje = 100;

    // ----- Resultados (labels lblColsIni / lblColsProc / lblColsAdm / lblPercent / lblTempo) -----

    [ObservableProperty]
    private string _colsInicialesTexto = "-";

    [ObservableProperty]
    private string _colsProcesadasTexto = "-";

    [ObservableProperty]
    private string _colsAdmitidasTexto = "-";

    [ObservableProperty]
    private string _porcentajeTexto = "-";

    [ObservableProperty]
    private string _tiempoTexto = "-";

    // ----- Estado del botón Reducir -----
    // Legacy: btnReducir.Enabled = true cuando hay archivo de entrada Y salida.
    [ObservableProperty]
    private bool _puedeReducir;

    [ObservableProperty]
    private bool _reduciendo;

    // Rutas completas (las propiedades *Texto sólo muestran el nombre, como en el legacy).
    private string _archivoEntrada = "";
    private string _archivoSalida = "";
    private IReduccion? _reductor;

    private void ActualizarPuedeReducir()
    {
        PuedeReducir = _archivoEntrada.Length > 0 && _archivoSalida.Length > 0 && !Reduciendo;
    }

    // EsArchivoEntradaValido de ReductorFrm.cs: el archivo de entrada debe tener 14 signos.
    private static bool EsArchivoEntradaValido(string aEntrada)
    {
        try
        {
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(aEntrada);
            comBaseCols.LeerTodasColsANumero();
            comBaseCols = new ArchivoColumnasTexto(aEntrada);
            int numSignos = comBaseCols.ObtenNumSignos();
            comBaseCols.Cerrar();
            return numSignos == 14;
        }
        catch
        {
            return false;
        }
    }

    // ----- Acciones (botones) -----

    // btnSelFile: OpenFileDialog para el archivo de entrada (.txt en Columnas\).
    [RelayCommand]
    private async Task SeleccionarArchivoEntradaAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        if (!EsArchivoEntradaValido(file.Path))
        {
            Free1X2.Abstractions.UserDialogs.ShowError(
                "El archivo de entrada debe tener 14 partidos.\n" +
                "Compruebe además que no hay líneas en blanco adicionales al final del archivo.");
            return;
        }

        _archivoEntrada = file.Path;
        ArchivoEntradaTexto = file.Name;
        ActualizarPuedeReducir();
    }

    // btnFileOutput: SaveFileDialog para el archivo de salida.
    [RelayCommand]
    private async Task SeleccionarArchivoSalidaAsync()
    {
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Reducido",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        _archivoSalida = file.Path;
        ArchivoSalidaTexto = file.Name;
        ActualizarPuedeReducir();
    }

    // btnReducir: lanza la reducción en un hilo aparte (legacy ComienzaReduccion).
    [RelayCommand]
    private async Task ReducirAsync()
    {
        if (Reduciendo) return;

        // Selección del algoritmo según el método (cBoxMetodo del form legacy).
        bool externas = AdmitirColumnasExternas;
        _reductor = MetodoSeleccionado switch
        {
            "grandes archivos" => new Redu1305Xfsf(externas),
            "menos tiempo"     => new JDC(externas),
            "menos columnas"   => new JDCDobleContador(externas),
            "menos columnas 2" => new ReductorTM(),
            _                  => new JDC(externas),
        };

        int nivel = int.Parse(NivelSeleccionado);
        int maxCol = (int)MaxColumnas;
        int percent = (int)MaxPorcentaje;

        Reduciendo = true;
        ActualizarPuedeReducir();
        ColsInicialesTexto = "-";
        ColsProcesadasTexto = "-";
        ColsAdmitidasTexto = "-";
        PorcentajeTexto = "-";
        TiempoTexto = "-";

        var hora0 = DateTime.Now;

        // Timer de refresco (equivale al myTimer de 3000 ms + TimerEventProcessor).
        var timer = AppServices.UiDispatcher?.CreateTimer();
        if (timer != null)
        {
            timer.Interval = TimeSpan.FromMilliseconds(3000);
            timer.Tick += (_, _) => RefrescarContadores(hora0);
            timer.Start();
        }

        try
        {
            await Task.Run(() =>
                _reductor.ComienzaReduccion(_archivoEntrada, _archivoSalida, nivel, maxCol, percent));
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("Error en la reducción: " + ex.Message);
        }
        finally
        {
            timer?.Stop();
            RefrescarContadores(hora0);
            Reduciendo = false;
            ActualizarPuedeReducir();
        }
    }

    // TimerEventProcessor / final de ComienzaReduccion: vuelca los contadores del reductor.
    private void RefrescarContadores(DateTime hora0)
    {
        if (_reductor == null) return;
        int ini = _reductor.NoColumnasIniciales;
        int proc = _reductor.NoColumnasProcesadas;
        int fin = _reductor.NoColumnasFinales;

        ColsInicialesTexto = " = " + ini;
        ColsProcesadasTexto = " = " + proc;
        ColsAdmitidasTexto = " = " + fin;
        PorcentajeTexto = ini > 0 ? " = " + (proc * 100 / ini) : "-";
        TiempoTexto = (DateTime.Now - hora0).ToString();
    }

    // btnCancel: cancela la reducción y cierra el formulario.
    [RelayCommand]
    private void Cancelar()
    {
        // BtnCancelClick: reductor.Cancelar(). El cierre/navegación lo gestiona la Page.
        _reductor?.Cancelar();
        Reduciendo = false;
        ActualizarPuedeReducir();
    }
}
