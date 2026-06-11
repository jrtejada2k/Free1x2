using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del WinForms <c>TramificarForm</c> ("Tramificar").
/// Reparte el universo de columnas (3^14 = 4.782.969) en tramos según su valoración,
/// usando los datos de premios de la L.A.E. de una jornada concreta, calcula la
/// probabilidad acumulada y permite filtrar por posiciones mínimas/máximas de cada
/// categoría de premio y localizar columnas concretas. Toda la lógica de dominio
/// (cálculo, escrutinio, persistencia, apertura de otros forms) está marcada como TODO.
/// </summary>
public partial class TramificarFormViewModel : ObservableObject
{
    // ===== Definición del tramo (GroupBox "Definición tramo") =====

    // Columna inicial del rango a tramificar. Campo legacy txValMin (default "0").
    [ObservableProperty]
    private double _columnaInicial = 0;

    // Columna final del rango a tramificar. Campo legacy txValMax (default 4782969).
    [ObservableProperty]
    private double _columnaFinal = 4782969;

    // Columnas por tramo. Campo legacy txIntervalo (default "531441").
    [ObservableProperty]
    private double _columnasPorTramo = 531441;

    // Nº de tramos seleccionado. Campo legacy cmbNumTrams (ComboBox de potencias de 3).
    [ObservableProperty]
    private string _numTramos = "9";

    // Opciones del ComboBox de nº de tramos (regla anti-crash 3: ItemsSource desde el VM).
    public IReadOnlyList<string> OpcionesNumTramos { get; } = new[]
    {
        "9", "27", "81", "243", "729", "2187", "6561", "19683", "59049", "177147", "531441"
    };

    // ===== Datos L.A.E. (GroupBox "Datos L.A.E.") =====

    // Columna premiada (combinación ganadora). Campo legacy txColumna (default "1X1222X2X12121").
    [ObservableProperty]
    private string _columnaPremiada = "1X1222X2X12121";

    // Temporada (año de inicio). Campo legacy txTemporada.
    [ObservableProperty]
    private string _temporada = "2004";

    // Temporada de fin (solo lectura, txTemporada2 deshabilitado).
    [ObservableProperty]
    private string _temporadaFin = "2005";

    // Jornada. Campo legacy numJornada (NumericUpDown 1..70). NumberBox.Value es double.
    [ObservableProperty]
    private double _jornada = 7;

    // Recaudación de la jornada. Campo legacy txRecaudacion.
    [ObservableProperty]
    private string _recaudacion = "7768049";

    // Importes de premio por categoría (GroupBox "Premios"). Campos legacy tx14..tx10.
    [ObservableProperty]
    private string _premio14 = "1165207,35";

    [ObservableProperty]
    private string _premio13 = "29877,11";

    [ObservableProperty]
    private string _premio12 = "1753,51";

    [ObservableProperty]
    private string _premio11 = "165,38";

    [ObservableProperty]
    private string _premio10 = "22,79";

    // Bloqueo de cada categoría de premio (checkboxes chkey14..chkey10).
    // El form legacy guarda el estado de bloqueo en el array PremioBloqueado[5].
    [ObservableProperty]
    private bool _bloqueado14;

    [ObservableProperty]
    private bool _bloqueado13;

    [ObservableProperty]
    private bool _bloqueado12;

    [ObservableProperty]
    private bool _bloqueado11;

    [ObservableProperty]
    private bool _bloqueado10;

    // ===== Posiciones mínimas y máximas (GroupBox "Posiciones mínimas y máximas") =====
    // Para cada categoría (14..10) un mínimo y un máximo de posición admitida en el tramo.
    // Campos legacy txMin14/txMax14 ... txMin10/txMax10.
    [ObservableProperty]
    private string _min14 = string.Empty;

    [ObservableProperty]
    private string _max14 = string.Empty;

    [ObservableProperty]
    private string _min13 = string.Empty;

    [ObservableProperty]
    private string _max13 = string.Empty;

    [ObservableProperty]
    private string _min12 = string.Empty;

    [ObservableProperty]
    private string _max12 = string.Empty;

    [ObservableProperty]
    private string _min11 = string.Empty;

    [ObservableProperty]
    private string _max11 = string.Empty;

    [ObservableProperty]
    private string _min10 = string.Empty;

    [ObservableProperty]
    private string _max10 = string.Empty;

    // Nº de columnas que pasan el filtro (etiqueta lbColumnasAGrabar, default "0").
    [ObservableProperty]
    private string _columnasFiltro = "0";

    // ===== Ver columna (GroupBox "Ver columna") =====

    // Columna en la posición actual del tramo. Campo legacy TxColumnaEnPosicion.
    [ObservableProperty]
    private string _columnaEnPosicion = string.Empty;

    // Aciertos de la columna mostrada respecto a la premiada. Campo legacy txAciertos.
    [ObservableProperty]
    private string _aciertos = "0";

    // Valoración/probabilidad de la columna mostrada. Campo legacy txProbabilidad.
    [ObservableProperty]
    private string _probabilidad = "0";

    // Aciertos a buscar al saltar de columna. Campo legacy cmbAciertosABuscar (DropDownList).
    [ObservableProperty]
    private string _aciertosABuscar = "14";

    public IReadOnlyList<string> OpcionesAciertosABuscar { get; } = new[]
    {
        "14", "13", "12", "11", "10"
    };

    // ===== Otros parámetros =====

    // LN central (parámetro de valoración logarítmica). Campo legacy txLNCentral (default "0").
    [ObservableProperty]
    private string _lnCentral = "0";

    // Acumular resultados de jornadas distintas. Campo legacy chkAcumular (oculto por defecto).
    [ObservableProperty]
    private bool _acumular;

    // Mensaje de estado (StatusBar legacy, panel 4: "Faltan datos").
    [ObservableProperty]
    private string _estado = "Faltan datos";

    // ===== Acciones (botones del form legacy) =====

    [RelayCommand]
    private void Tramificar()
    {
        // TODO: Dominio legacy — TramificarForm.btTramificar_Click():
        //   Reparte el rango [ColumnaInicial..ColumnaFinal] en tramos de tamaño ColumnasPorTramo
        //   (o NumTramos tramos), escruta cada columna con MotorCalculo/Escrutinio usando la
        //   ColumnaPremiada y los importes de premio (Premio14..Premio10, Recaudacion), y vuelca
        //   los Tramos/Jornadas/PrAcumulados en la rejilla dgResultados.
    }

    [RelayCommand]
    private void GrabarTramos()
    {
        // TODO: Dominio legacy — TramificarForm.button1_Click() (btGrabarTramos):
        //   Abre DialogoGrabarTramosFrm y persiste los tramos calculados a fichero.
    }

    [RelayCommand]
    private void Filtrar()
    {
        // TODO: Dominio legacy — TramificarForm.btFiltrar_Click():
        //   Aplica el filtro de posiciones mínimas/máximas (Min14/Max14..Min10/Max10) sobre las
        //   columnas del tramo y actualiza ColumnasFiltro (lbColumnasAGrabar).
    }

    [RelayCommand]
    private void GrabarFiltro()
    {
        // TODO: Dominio legacy — TramificarForm.button1_Click_1() (btGrabarFiltro):
        //   Persiste las columnas que pasan el filtro de posiciones a fichero.
    }

    [RelayCommand]
    private void BuscarColumna()
    {
        // TODO: Dominio legacy — TramificarForm.button1_Click_2() (btBuscarColumna):
        //   Localiza ColumnaEnPosicion dentro del tramo escrutado (Ap14T) y sitúa el cursor en ella.
    }

    [RelayCommand]
    private void ColumnaAnterior()
    {
        // TODO: Dominio legacy — TramificarForm.btAnterior_Click():
        //   Retrocede a la columna anterior del tramo; actualiza ColumnaEnPosicion/Probabilidad/Aciertos.
    }

    [RelayCommand]
    private void ColumnaSiguiente()
    {
        // TODO: Dominio legacy — TramificarForm.btSiguiente_Click():
        //   Avanza a la siguiente columna del tramo; actualiza ColumnaEnPosicion/Probabilidad/Aciertos.
    }

    [RelayCommand]
    private void GuardarLimites()
    {
        // TODO: Dominio legacy — TramificarForm.btGuardarLimites_Click(): guarda los límites de posiciones.
    }

    [RelayCommand]
    private void LeerLimites()
    {
        // TODO: Dominio legacy — TramificarForm.btLeerLimites_Click(): carga los límites de posiciones.
    }

    [RelayCommand]
    private void GuardarJornadaLae()
    {
        // TODO: Dominio legacy — TramificarForm.menuItem7_Click(): guarda los datos de premios de la jornada (L.A.E.).
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: Dominio legacy — TramificarForm.btCancelar_Click(): cierra el formulario.
    }
}
