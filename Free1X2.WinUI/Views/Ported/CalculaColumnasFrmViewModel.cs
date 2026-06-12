using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del formulario legacy WinForms "CalculaColumnasFrm" (Calcular columnas).
///
/// Propósito: lanzar el cálculo/generación de las columnas de la combinación actual
/// (clase legacy <c>Free1X2.MotorCalculo.Analizador</c>) en uno de tres modos y mostrar
/// en vivo las estadísticas del proceso (columnas procesadas/aceptadas/estimadas/máximas
/// con su coste, tiempos y porcentaje de aceptación).
///
/// Modos (RadioButtons del form legacy chckCalcular / chckAnalizar / chckGrabar):
///   - <see cref="ModoCalcular"/>: Analizador.AnalizaCombinacion(false)  → sólo cuenta columnas.
///   - <see cref="ModoAnalizar"/>: Analizador.AnalizaCombinacion(14 ó 15) → calcula y analiza
///       (pleno al 15 si <see cref="IncluirPleno"/> está activo).
///   - <see cref="ModoGrabar"/>:   Analizador.AnalizaCombinacion(archivoResultados) → graba a fichero.
///
/// Diferencia con WinForms: el form legacy recibía un Analizador ya poblado desde MainForm.
/// En WinUI todavía no existe ese flujo, así que el ViewModel construye su propio Analizador
/// y permite editar los 14 pronósticos (SetPronostico). El motor se invoca igual que en
/// WinForms; el bucle de cálculo corre en Task.Run y los contadores se refrescan en el hilo
/// de UI con un DispatcherQueueTimer (equivalente al Timer de 500 ms del form legacy).
/// </summary>
public partial class CalculaColumnasFrmViewModel : ObservableObject
{
    private readonly Analizador _analizador = new();
    private long _colsMaximas;
    private CancellationTokenSource? _cts;

    // ---- Pronósticos (14 partidos; cada uno "1,X,2" / "1,X" / "1" ...) ----

    public ObservableCollection<string> Pronosticos { get; } = new();

    // ---- Modos (RadioButtons; exclusivos vía GroupName en el XAML) ----

    [ObservableProperty]
    private bool _modoCalcular = true;

    [ObservableProperty]
    private bool _modoAnalizar;

    [ObservableProperty]
    private bool _modoGrabar;

    // "Incluir pleno al 15" (chckPleno). Sólo habilitado cuando el modo es "Calcular y analizar".
    [ObservableProperty]
    private bool _incluirPleno;

    [ObservableProperty]
    private bool _incluirPlenoHabilitado;

    // Selección de archivo de resultados (sólo en modo Grabar).
    [ObservableProperty]
    private bool _seleccionArchivoHabilitado;

    [ObservableProperty]
    private string _nombreArchivo = "(Seleccionar)";

    private string _archivoResultados = "";

    // ---- Estado de ejecución ----

    [ObservableProperty]
    private bool _calculando;

    [ObservableProperty]
    private bool _progresoVisible;

    [ObservableProperty]
    private double _progreso; // 0..100

    [ObservableProperty]
    private string _estado = "Preparado";

    // ---- Estadísticas (todas string para bindear a TextBlock.Text; regla anti-crash 2) ----

    [ObservableProperty]
    private string _colsProcesadas = "0";

    [ObservableProperty]
    private string _costeProcesadas = "0.0";

    [ObservableProperty]
    private string _colsAceptadas = "0";

    [ObservableProperty]
    private string _costeAceptadas = "0.0";

    [ObservableProperty]
    private string _porcentaje = "0.00 %";

    [ObservableProperty]
    private string _colsEstimadas = "0";

    [ObservableProperty]
    private string _costeEstimadas = "0.0";

    [ObservableProperty]
    private string _colsMaximo = "0";

    [ObservableProperty]
    private string _costeMaximo = "0.0";

    [ObservableProperty]
    private string _horaComienzo = "00:00:00";

    [ObservableProperty]
    private string _horaFinal = "00:00:00";

    [ObservableProperty]
    private string _tiempoTranscurrido = "0.0";

    public CalculaColumnasFrmViewModel()
    {
        // Inicializa los 14 pronósticos a "1,X,2" (combinación abierta), igual que el
        // estado por defecto del boleto en WinForms, y los registra en el Analizador.
        for (int i = 0; i < Free1X2.VariablesGlobales.NumeroPartidos; i++)
        {
            Pronosticos.Add("1,X,2");
            _analizador.SetPronostico(i, "1,X,2");
        }
        RecalcularMaximas();
    }

    // ---- Dependencias de UI (equivalen a los CheckedChanged del form legacy) ----

    partial void OnModoAnalizarChanged(bool value)
    {
        // chckAnalizar_CheckedChanged: chckPleno.Enabled = chckAnalizar.Checked
        IncluirPlenoHabilitado = value;
        if (!value) IncluirPleno = false;
    }

    partial void OnModoGrabarChanged(bool value)
    {
        // chckGrabar_CheckedChanged: habilita selección de archivo en modo Grabar.
        SeleccionArchivoHabilitado = value;
    }

    /// <summary>
    /// Aplica los pronósticos editados al Analizador y recalcula las columnas máximas.
    /// Equivale a CalculaColumnasFrm_Load (rama sin filtro base: 2^dobles * 3^triples).
    /// </summary>
    public void RecalcularMaximas()
    {
        int dobles = 0, triples = 0;
        for (int i = 0; i < Pronosticos.Count; i++)
        {
            string p = (Pronosticos[i] ?? "").Replace(",", "");
            _analizador.SetPronostico(i, Pronosticos[i] ?? "1,X,2");
            if (p.Length == 2) dobles++;
            if (p.Length == 3) triples++;
        }
        _colsMaximas = Convert.ToInt64(Math.Pow(2, dobles) * Math.Pow(3, triples));

        ColsMaximo = _colsMaximas.ToString("#,##0;0");
        double costeMaximo = _colsMaximas * Free1X2.VariablesGlobales.PrecioApuesta;
        CosteMaximo = costeMaximo.ToString(Free1X2.VariablesGlobales.Moneda + "#,##0.00;0.0");
    }

    /// <summary>Selecciona el archivo de resultados (botón btnSelArch del form legacy).</summary>
    [RelayCommand]
    private async Task SeleccionarArchivoAsync()
    {
        // Equivale a CalculaColumnasFrm.ObtenNombreArchivoResultados() (SaveFileDialog *.txt).
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Columnas",
        };
        picker.FileTypeChoices.Add("Columnas", new System.Collections.Generic.List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file != null)
        {
            _archivoResultados = file.Path;
            NombreArchivo = file.Name;
        }
    }

    /// <summary>Lanza el cálculo de columnas (botón btnCalcular del form legacy).</summary>
    [RelayCommand]
    private async Task CalcularAsync()
    {
        if (Calculando) return;

        RecalcularMaximas();

        if (ModoGrabar && string.IsNullOrEmpty(_archivoResultados))
        {
            Free1X2.Abstractions.UserDialogs.ShowError("Seleccione un archivo de resultados.");
            return;
        }

        Calculando = true;
        ProgresoVisible = true;
        Estado = "Calculando...";
        _cts = new CancellationTokenSource();

        var dt1 = DateTime.Now;
        HoraComienzo = dt1.ToLongTimeString();

        // Timer de refresco en el hilo de UI (equivale al Timer 500 ms + ActualizaDatosCalculo).
        var dispatcher = AppServices.UiDispatcher;
        var timer = dispatcher?.CreateTimer();
        if (timer != null)
        {
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (_, _) => ActualizaDatosCalculo();
            timer.Start();
        }

        try
        {
            // BtnCalcularClick: llama a AnalizaCombinacion en el modo elegido. Se ejecuta en
            // un hilo de fondo para no bloquear la UI (en WinForms corría en el hilo de UI
            // con Application.DoEvents); el shim UiPump.Pump es no-op en WinUI.
            await Task.Run(() =>
            {
                if (ModoGrabar)
                {
                    _analizador.AnalizaCombinacion(_archivoResultados);
                }
                else if (ModoAnalizar)
                {
                    _analizador.AnalizaCombinacion(IncluirPleno ? 15 : 14);
                }
                else
                {
                    _analizador.AnalizaCombinacion(false);
                }
            }, _cts.Token);
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("Error en el cálculo: " + ex.Message);
        }
        finally
        {
            timer?.Stop();
            var dt2 = DateTime.Now;
            HoraFinal = dt2.ToLongTimeString();
            TiempoTranscurrido = dt2.Subtract(dt1).ToString();
            ActualizaDatosCalculo();

            Calculando = false;
            ProgresoVisible = false;
            Estado = "Listo";
        }
    }

    /// <summary>Cancela el cálculo en curso (botón btnCancelar del form legacy).</summary>
    [RelayCommand]
    private void Cancelar()
    {
        // BtnCancelarClick: analizador.PararAnalisis() detiene el GeneradorColumnas.
        _analizador.PararAnalisis();
        _cts?.Cancel();
        ProgresoVisible = false;
        Estado = "Cancelado";
    }

    // Equivale a ActualizaDatosCalculo() del form legacy: refresca contadores/costes/porcentaje.
    private void ActualizaDatosCalculo()
    {
        int procesadas = _analizador.ColsAnalizadas;
        int aceptadas = _analizador.ColsAceptadas;
        double precio = Free1X2.VariablesGlobales.PrecioApuesta;
        string moneda = Free1X2.VariablesGlobales.Moneda;

        ColsProcesadas = procesadas.ToString("#,##0;0");
        CosteProcesadas = (procesadas * precio).ToString(moneda + "#,##0.00;0.0");

        Progreso = _colsMaximas > 0
            ? Math.Min(100.0, procesadas * 100.0 / _colsMaximas)
            : 0;

        ColsAceptadas = aceptadas.ToString("#,##0;0");
        CosteAceptadas = (aceptadas * precio).ToString(moneda + "#,##0.00;0.0");

        double porcentaje = procesadas > 0 ? aceptadas * 100.0 / procesadas : 0;
        Porcentaje = porcentaje.ToString("#,##0.00;0.00") + " %";

        double colsEstimadas = Math.Round(_colsMaximas * porcentaje / 100, 0);
        ColsEstimadas = colsEstimadas.ToString("#,##0;0");
        CosteEstimadas = (colsEstimadas * precio).ToString(moneda + "#,##0.00;0.0");
    }
}
