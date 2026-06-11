using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    // ---------------------------------------------------------------------
    // Paso 2 (valoraciones)
    // ---------------------------------------------------------------------

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

    /// <summary>Paso 1: abre el diálogo de fichero y lee las columnas a analizar (btLeerColumnas).</summary>
    [RelayCommand]
    private void LeerColumnas()
    {
        // TODO(dominio): seleccionar fichero y cargar columnas con
        // Free1X2.EntradaSalida.* (lectura del fichero de columnas) y rellenar la lista de
        // Free1X2.MotorCalculo.ApuestaProbableCentral. Actualizar NumColumnasLeidasTexto.
    }

    /// <summary>Paso 2: calcula la valoración real a partir de la apostada (btCalcularReales).</summary>
    [RelayCommand]
    private void CalcularReales()
    {
        // TODO(dominio): replicar el cálculo legacy de "Valoración real obtenida a partir de % apostado"
        // usando Free1X2.MotorCalculo (probabilidades, ordenación de apuestas) y los parámetros
        // LnProbMedia14 / PremioMedio14 / NumColumnasAConsiderar.
    }

    /// <summary>Paso 3: genera columnas aleatorias de 14 aciertos (btGenerarAleatorias).</summary>
    [RelayCommand]
    private void GenerarAleatorias()
    {
        // TODO(dominio): generar NumAleatorias columnas con DesviacionTipicaDeseada / LnMinimo
        // mediante Free1X2.MotorCalculo; actualizar DesviacionTipicaObtenidaTexto y LnMediaObtenidaTexto.
    }

    /// <summary>Paso 3: guarda en fichero las columnas aleatorias generadas (btGuardarAleatorias).</summary>
    [RelayCommand]
    private void GuardarAleatorias()
    {
        // TODO(dominio): persistir las columnas generadas con Free1X2.EntradaSalida.*.
    }

    /// <summary>Paso 4: ejecuta el escrutinio / análisis seleccionado (btnOK "Analizar").</summary>
    [RelayCommand]
    private void Analizar()
    {
        // TODO(dominio): ejecutar el escrutinio según TipoAnalisisSeleccionado y los premios
        // acumulados (Acumula10..14) usando Free1X2.MotorCalculo / Free1X2.Analisis.
        // Rellenar FilasResultado y los totales (GastoTotalTexto, PremioTotalTexto, etc.).
    }

    /// <summary>Paso 4: graba los resultados del escrutinio (btGrabar).</summary>
    [RelayCommand]
    private void Grabar()
    {
        // TODO(dominio): grabar las columnas/resultados con Free1X2.EntradaSalida.*.
    }

    /// <summary>Paso 4: simplifica (elimina) filas según el criterio de diferencias (btEliminarFilas).</summary>
    [RelayCommand]
    private void Simplificar()
    {
        // TODO(dominio): eliminar apuestas con menos de DiferenciasMinimas diferencias,
        // respecto de cada fila seleccionada o sólo de la fila actual (SimplificarParaCadaFila),
        // mediante Free1X2.MotorCalculo (lógica de Reducción).
    }

    /// <summary>Paso 4: abre la selección de jornadas (btSeleccionJornadas).</summary>
    [RelayCommand]
    private void SeleccionarJornadas()
    {
        // TODO(dominio): abrir el formulario legacy de selección de jornadas y aplicar la selección.
    }
}
