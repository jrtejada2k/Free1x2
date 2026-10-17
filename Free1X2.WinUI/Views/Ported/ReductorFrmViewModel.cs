// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Reduccion;
using Free1X2.WinUI.Services;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la página portada del WinForms <c>ReductorFrm</c> (Free1X2/UI/ReductorFrm.cs).
/// Reproduce los campos de entrada (archivo entrada/salida, máx. columnas, máx. %, nivel,
/// método y columnas externas) y los resultados de la reducción (col. iniciales, procesadas,
/// admitidas, porcentaje y tiempo). La lógica de dominio (IReduccion: JDC, JDCDobleContador,
/// Redu1305Xfsf, ReductorTM) está implementada.
/// </summary>
public partial class ReductorFrmViewModel : ObservableObject
{
    // ----- Datos de entrada (groupBox "Datos de Entrada") -----

    // lblSelFile: muestra el nombre del archivo de entrada seleccionado.
    [ObservableProperty]
    private string _archivoEntradaTexto = "(falta selección)";

    // lblFicheroSalida: muestra el nombre del archivo de salida seleccionado. W10 (DT-85): sin
    // salida elegida se reduce a Temp/Reducido.txt y se guarda después con «Guardar resultado…».
    private const string SalidaTemporalTexto = "(temporal: guárdalo después con «Guardar resultado…»)";

    [ObservableProperty]
    private string _archivoSalidaTexto = SalidaTemporalTexto;

    // «Columnas de Inicio» generándose (el botón muestra «Generando columnas…»).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TextoColumnasInicio))]
    [NotifyCanExecuteChangedFor(nameof(UsarColumnasInicioCommand))]
    private bool _generandoInicio;

    public string TextoColumnasInicio =>
        GenerandoInicio ? ColumnasInicioServicio.TextoGenerando : ColumnasInicioServicio.TextoBoton;

    // Lo último reducido, para «Guardar resultado…».
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarResultadoCommand))]
    private string? _reducido;

    public ReductorFrmViewModel()
    {
        // Inicio ▸ Reducir deja aquí sus columnas (W10, DT-85).
        string? entrada = AppState.EntradaReductor;
        AppState.EntradaReductor = null;
        if (!string.IsNullOrEmpty(entrada)) UsarEntrada(entrada);
    }

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

    // chkExternas: "Admitir columnas externas". En el legacy
    // (cBoxMetodo_SelectedIndexChanged) se OCULTA — no sólo se deshabilita —
    // para "grandes archivos" y "menos columnas 2" (chkExternas.Visible=false).
    [ObservableProperty]
    private bool _admitirColumnasExternas;

    // Réplica de chkExternas.Visible del legacy: Collapsed para los dos métodos
    // que ocultan el control, Visible para el resto. Se expone como Visibility
    // para bindear directamente en el propio Control (ToggleSwitch.Visibility),
    // nunca en un panel (regla #1).
    public Visibility ColumnasExternasVisibility =>
        MetodoSeleccionado is "grandes archivos" or "menos columnas 2"
            ? Visibility.Collapsed
            : Visibility.Visible;

    partial void OnMetodoSeleccionadoChanged(string value)
    {
        OnPropertyChanged(nameof(ColumnasExternasVisibility));
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

    // W10 (DT-85): basta con la entrada; sin salida se reduce a Temp/Reducido.txt.
    private void ActualizarPuedeReducir()
    {
        PuedeReducir = _archivoEntrada.Length > 0 && !Reduciendo;
        GuardarResultadoCommand.NotifyCanExecuteChanged();
        UsarColumnasInicioCommand.NotifyCanExecuteChanged();
    }

    private static string SalidaTemporal => System.IO.Path.Combine(ColumnasInicioServicio.CarpetaTemp, "Reducido.txt");

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
        var file = await PickerHelper.AbrirAsync(".txt");
        if (file == null) return;

        UsarEntrada(file.Path);
    }

    private void UsarEntrada(string ruta)
    {
        if (!EsArchivoEntradaValido(ruta))
        {
            AppServices.MostrarError(
                "El archivo de entrada debe tener 14 partidos.\n" +
                "Compruebe además que no hay líneas en blanco adicionales al final del archivo.");
            return;
        }

        _archivoEntrada = ruta;
        ArchivoEntradaTexto = System.IO.Path.GetFileName(ruta);
        ActualizarPuedeReducir();
    }

    private bool PuedeUsarColumnasInicio() => !GenerandoInicio && !Reduciendo;

    /// <summary>W10 (DT-85): «Columnas de Inicio» como archivo de entrada.</summary>
    [RelayCommand(CanExecute = nameof(PuedeUsarColumnasInicio))]
    private async Task UsarColumnasInicioAsync()
    {
        GenerandoInicio = true;
        try
        {
            string? ruta = await ColumnasInicioServicio.GenerarAsync();
            if (ruta != null) UsarEntrada(ruta);
        }
        finally
        {
            GenerandoInicio = false;
        }
    }

    private bool PuedeGuardarResultado() => Reducido != null && !Reduciendo;

    /// <summary>W10 (DT-85): guarda donde se elija lo último reducido.</summary>
    [RelayCommand(CanExecute = nameof(PuedeGuardarResultado))]
    private async Task GuardarResultadoAsync()
    {
        if (Reducido == null) return;
        var destino = await PickerHelper.GuardarAsync("Reducido", ("Columnas", ".txt"));
        if (destino == null) return;
        try
        {
            string origen = Reducido;
            await Task.Run(() => System.IO.File.Copy(origen, destino.Path, overwrite: true));
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar la reducción: " + ex.Message);
        }
    }

    // btnFileOutput: SaveFileDialog para el archivo de salida.
    [RelayCommand]
    private async Task SeleccionarArchivoSalidaAsync()
    {
        var file = await PickerHelper.GuardarAsync("Reducido", ("Columnas", ".txt"));
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

        // W10 (DT-85): sin «Archivo salida» elegido, la reducción va a Temp/Reducido.txt.
        string salida = _archivoSalida.Length > 0 ? _archivoSalida : SalidaTemporal;
        Reducido = null;

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
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(salida)!);
                _reductor.ComienzaReduccion(_archivoEntrada, salida, nivel, maxCol, percent);
            });
            Reducido = salida;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error en la reducción: " + ex.Message);
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
