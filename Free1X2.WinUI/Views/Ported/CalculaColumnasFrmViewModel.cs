using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
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
/// Origen del Analizador (réplica de <c>MainForm.AbreCalculoColumnasFrm</c>, que pasa su
/// <c>analizador</c> ya poblado al constructor de <c>CalculaColumnasFrm</c>):
///   - Si la MainPage activó <see cref="AppState.UsarAnalizadorCompartido"/> al navegar
///     ("Calcular"), este ViewModel usa el <see cref="AppState.Analizador"/> compartido, que ya
///     lleva el boleto base, los grupos, las condiciones (IFiltro) y el If-Then, además del
///     <c>ArchivoColumnasBase</c> fijado por la MainPage si el filtro de columnas está activo.
///   - Si se abre de forma independiente desde la navegación, construye su PROPIO Analizador y
///     permite editar los 14 pronósticos (SetPronostico) como combinación abierta ("1,X,2").
/// El motor se invoca igual que en WinForms; el bucle de cálculo corre en Task.Run y los
/// contadores se refrescan en el hilo de UI con un DispatcherQueueTimer (equivalente al Timer
/// de 500 ms del form legacy).
/// </summary>
public partial class CalculaColumnasFrmViewModel : ObservableObject
{
    private readonly Analizador _analizador;
    private readonly bool _usandoCompartido;
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
        // Handoff desde la MainPage: si pidió usar el Analizador compartido, lo tomamos (ya lleva
        // boleto, grupos, condiciones, If-Then y ArchivoColumnasBase). Consumimos la bandera para
        // que una apertura posterior desde la navegación vuelva a crear uno propio editable.
        _usandoCompartido = AppState.UsarAnalizadorCompartido;
        AppState.UsarAnalizadorCompartido = false;

        if (_usandoCompartido)
        {
            // Usa el motor compartido: los pronósticos ya están fijados desde el boleto. Sólo
            // reflejamos su estado en la colección (informativo; el cálculo opera sobre el motor).
            _analizador = AppState.Instancia.Analizador;
            string[] pron = _analizador.Pronosticos;
            for (int i = 0; i < pron.Length; i++)
            {
                Pronosticos.Add(pron[i]);
            }
        }
        else
        {
            // Modo independiente: Analizador propio con los 14 pronósticos a "1,X,2"
            // (combinación abierta), igual que el estado por defecto del boleto en WinForms.
            _analizador = new Analizador();
            for (int i = 0; i < Free1X2.VariablesGlobales.NumeroPartidos; i++)
            {
                Pronosticos.Add("1,X,2");
                _analizador.SetPronostico(i, "1,X,2");
            }
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
    /// Recalcula las columnas máximas, réplica fiel de <c>CalculaColumnasFrm_Load</c>
    /// (Free1X2/UI/CalculaColumnas.cs ~832-856):
    ///   - Si hay <c>ArchivoColumnasBase</c> (filtro de columnas activo): máximas = nº de columnas
    ///     del archivo (ObtenNumCols).
    ///   - Si no: máximas = 2^dobles * 3^triples sobre los pronósticos.
    /// En modo independiente, además aplica al Analizador los pronósticos editados; en modo
    /// compartido NO los toca (ya vienen del boleto y de los grupos cargados en la MainPage).
    /// </summary>
    public void RecalcularMaximas()
    {
        if (_analizador.ArchivoColumnasBase.Length > 0)
        {
            // Hay un filtro de columnas base: las máximas son las columnas del archivo.
            try
            {
                IArchivoColumnas f = new ArchivoColumnasTexto(_analizador.ArchivoColumnasBase);
                _colsMaximas = f.ObtenNumCols();
                f.Cerrar();
            }
            catch
            {
                _colsMaximas = 0;
            }
        }
        else
        {
            int dobles = 0, triples = 0;
            for (int i = 0; i < Pronosticos.Count; i++)
            {
                string p = (Pronosticos[i] ?? "").Replace(",", "");
                // En modo independiente, aplica el pronóstico editado al Analizador propio.
                if (!_usandoCompartido)
                {
                    _analizador.SetPronostico(i, Pronosticos[i] ?? "1,X,2");
                }
                if (p.Length == 2) dobles++;
                if (p.Length == 3) triples++;
            }
            _colsMaximas = Convert.ToInt64(Math.Pow(2, dobles) * Math.Pow(3, triples));
        }

        ColsMaximo = _colsMaximas.ToString("#,##0;0");
        double costeMaximo = _colsMaximas * Free1X2.VariablesGlobales.PrecioApuesta;
        CosteMaximo = costeMaximo.ToString(Free1X2.VariablesGlobales.Moneda + "#,##0.00;0.0");
    }

    /// <summary>
    /// Réplica fiel de <c>CalculaColumnasFrm.HayConflictosEntreArchivos()</c>
    /// (Free1X2/UI/CalculaColumnas.cs ~116-143): comprueba si el archivo de resultados
    /// elegido coincide con un archivo ya usado en la combinación actual — el archivo de
    /// columnas base (grupo base → <c>ArchivoColumnasBase</c>) o el archivo de filtro parcial
    /// de algún grupo no base (<c>UsaFiltroParcial</c> → <c>ArchivoFiltroGrupo</c>). Grabar
    /// sobre uno de esos archivos lo machacaría mientras el motor lo está leyendo.
    /// </summary>
    private bool HayConflictosEntreArchivos()
    {
        bool hayConflictos = false;
        for (int i = 0; i < _analizador.CtrlGrupos.GruposPartidos.Count; i++)
        {
            Grupo grup = _analizador.CtrlGrupos.GruposPartidos[i];
            if (!grup.EsGrupoBase)
            {
                if (grup.UsaFiltroParcial)
                {
                    if (_archivoResultados == grup.ArchivoFiltroGrupo)
                    {
                        hayConflictos = true;
                        break;
                    }
                }
            }
            else
            {
                if (_archivoResultados == _analizador.ArchivoColumnasBase)
                {
                    hayConflictos = true;
                    break;
                }
            }
        }
        return hayConflictos;
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

        // Parity con CalculaColumnasFrm.BtnCalcularClick: en modo Grabar, no se puede usar como
        // archivo de resultados uno ya empleado en la combinación (machacaría el fichero que el
        // motor está leyendo). Se avisa con el mismo texto del form legacy y se aborta el cálculo.
        if (ModoGrabar && HayConflictosEntreArchivos())
        {
            Free1X2.Abstractions.UserDialogs.ShowError(
                "No puede usar como archivo de resultados un archivo usado ya en la combinación");
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
