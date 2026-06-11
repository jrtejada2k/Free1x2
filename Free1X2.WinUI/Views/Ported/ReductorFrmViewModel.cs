using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    private void ActualizarPuedeReducir()
    {
        PuedeReducir = ArchivoEntradaTexto != "(falta selección)"
                       && ArchivoSalidaTexto != "(falta selección)";
    }

    // ----- Acciones (botones) -----

    // btnSelFile: OpenFileDialog para el archivo de entrada (.txt en Columnas\).
    [RelayCommand]
    private void SeleccionarArchivoEntrada()
    {
        // TODO: portar BtnSelFileClick de ReductorFrm.cs.
        //   - Abrir FileOpenPicker (.txt) en la carpeta "Columnas".
        //   - Validar con EsArchivoEntradaValido (ArchivoColumnasTexto.ObtenNumSignos() == 14).
        //   - Asignar ArchivoEntradaTexto = Path.GetFileName(...).
        ActualizarPuedeReducir();
    }

    // btnFileOutput: SaveFileDialog para el archivo de salida.
    [RelayCommand]
    private void SeleccionarArchivoSalida()
    {
        // TODO: portar btnFileOutput_Click de ReductorFrm.cs.
        //   - Abrir FileSavePicker (.txt) en la carpeta "Columnas".
        //   - Asignar ArchivoSalidaTexto = Path.GetFileName(...).
        ActualizarPuedeReducir();
    }

    // btnReducir: lanza la reducción en un hilo aparte (legacy ComienzaReduccion).
    [RelayCommand]
    private void Reducir()
    {
        // TODO: portar ComienzaReduccion de ReductorFrm.cs.
        //   - Seleccionar IReduccion según MetodoSeleccionado:
        //       "grandes archivos" -> new Redu1305Xfsf(AdmitirColumnasExternas)
        //       "menos tiempo"     -> new JDC(AdmitirColumnasExternas)
        //       "menos columnas"   -> new JDCDobleContador(AdmitirColumnasExternas)
        //       (legacy también: "menos columnas 2" -> new ReductorTM())
        //   - reductor.ComienzaReduccion(archivoEntrada, archivoSalida,
        //       int.Parse(NivelSeleccionado), (int)MaxColumnas, (int)MaxPorcentaje)
        //     ejecutado fuera del hilo de UI (Task.Run); usar un DispatcherTimer
        //     (equivalente a myTimer, 3000 ms) para refrescar los textos de resultado.
        //   - Al terminar, volcar NoColumnasIniciales / NoColumnasProcesadas /
        //     NoColumnasFinales en ColsInicialesTexto/ColsProcesadasTexto/ColsAdmitidasTexto,
        //     PorcentajeTexto y TiempoTexto.
    }

    // btnCancel: cancela la reducción y cierra el formulario.
    [RelayCommand]
    private void Cancelar()
    {
        // TODO: portar BtnCancelClick de ReductorFrm.cs (reductor.Cancelar() + cerrar/navegar atrás).
    }
}
