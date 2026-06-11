using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
/// </summary>
public partial class CalculaColumnasFrmViewModel : ObservableObject
{
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
        // TODO(dominio): el form legacy recibe (Analizador analizador, string[] arrayEquipos)
        // y en CalculaColumnasFrm_Load calcula colsMaximas:
        //   - si hay filtro base (analizador.ArchivoColumnasBase): ArchivoColumnasTexto.ObtenNumCols()
        //   - si no: 2^dobles * 3^triples sobre analizador.Pronosticos
        // y su coste con Free1X2.VariablesGlobales.PrecioApuesta / .Moneda.
        // El dominio aún no está disponible; se dejan los valores por defecto.
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

    /// <summary>Selecciona el archivo de resultados (botón btnSelArch del form legacy).</summary>
    [RelayCommand]
    private void SeleccionarArchivo()
    {
        // TODO(dominio): replicar CalculaColumnasFrm.ObtenNombreArchivoResultados():
        // abrir un FileSavePicker (WinUI) hacia .../Columnas/ con filtro *.txt,
        // guardar la ruta y mostrar Path.GetFileName(...) en NombreArchivo.
    }

    /// <summary>Lanza el cálculo de columnas (botón btnCalcular del form legacy).</summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO(dominio): replicar BtnCalcularClick:
        //   1. guardarCombinacionTemporal()  → ArchivoCombinacion en .../Temp/...comb
        //   2. iniciar timer (500 ms) que llama ActualizaDatosCalculo() para refrescar estadísticas
        //   3. según el modo:
        //        - ModoGrabar:   analizador.AnalizaCombinacion(archivoResultados)
        //                        (validando HayConflictosEntreArchivos())
        //        - ModoAnalizar: analizador.AnalizaCombinacion(IncluirPleno ? 15 : 14)
        //        - ModoCalcular: analizador.AnalizaCombinacion(false)
        //   4. al terminar, fijar HoraFinal / TiempoTranscurrido y llamar ActualizaDatosCalculo().
        // Aquí sólo se actualiza el estado visible.
        Estado = "Listo";
    }

    /// <summary>Cancela el cálculo en curso (botón btnCancelar del form legacy).</summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): replicar BtnCancelarClick: analizador.PararAnalisis().
        Calculando = false;
        ProgresoVisible = false;
        Estado = "Preparado";
    }
}
