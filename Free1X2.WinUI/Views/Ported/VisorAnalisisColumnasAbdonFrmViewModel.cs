using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una agrupación mostrada en la lista de "Paso Fijo" o "Solapadas".
/// Legacy: cada fila producida por VisorAnalisisColumnasAbdonFrm.ObtenTextoAgrupaciones()
/// y pintada por el UserControl CtrlAgrupacion.
/// </summary>
public partial class VisorAbdonAgrupacion : ObservableObject
{
    /// <summary>Descripción textual de la agrupación (p. ej. "3-5+2+1").</summary>
    public string Texto { get; }

    public VisorAbdonAgrupacion(string texto)
    {
        Texto = texto;
    }
}

/// <summary>
/// ViewModel de la pantalla "EstuCol - Visor de Informe" (legacy: VisorAnalisisColumnasAbdonFrm).
/// Muestra el análisis ABDON de las columnas generadas frente a las columnas ganadoras del
/// archivo, con dos modos (Global / Individual): rango de aciertos, agrupaciones paso fijo y
/// solapadas, escaleras (asc/desc/totales), sandwichs y suma total de aciertos. Permite además
/// generar un fichero de condición (filtro de columnas probables).
/// </summary>
public partial class VisorAnalisisColumnasAbdonFrmViewModel : ObservableObject
{
    // ------- Estado de modo y navegación -------

    /// <summary>true = modo Global (rangos sobre todas las ganadoras); false = modo Individual.</summary>
    [ObservableProperty]
    private bool _modoGlobal = true;

    /// <summary>Etiqueta del botón de cambio de modo (legacy: btnModo.Text).</summary>
    public string TextoBotonModo => ModoGlobal ? "Cambiar a modo individual" : "Cambiar a modo global";

    /// <summary>Posición de la columna generada actual: "n / total" (legacy: lblColumna).</summary>
    [ObservableProperty]
    private string _columnaTexto = "- / -";

    /// <summary>Posición de la columna ganadora actual: "n / total" (legacy: lblGanadora).</summary>
    [ObservableProperty]
    private string _ganadoraTexto = "- / -";

    // Visibilidad por modo. Se bindea en CADA control hijo (no en el panel) para
    // respetar la regla anti-crash del XamlCompiler.
    /// <summary>Visible para los controles propios del modo Individual (ganadoras, suma opcional).</summary>
    public Visibility VisibilidadIndividual =>
        ModoGlobal ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>Visible para los controles propios del modo Global (generar condición, aviso).</summary>
    public Visibility VisibilidadGlobal =>
        ModoGlobal ? Visibility.Visible : Visibility.Collapsed;

    // ------- Rango de aciertos de la columna generada -------

    /// <summary>Mínimo de aciertos de la columna actual (legacy: lblAciertosMin).</summary>
    [ObservableProperty]
    private string _aciertosMin = "-";

    /// <summary>Máximo de aciertos de la columna actual (legacy: lblAciertosMax).</summary>
    [ObservableProperty]
    private string _aciertosMax = "-";

    // ------- Agrupaciones Paso Fijo -------

    /// <summary>Filtro: mostrar sólo agrupaciones de N elementos (0 = todas). Legacy: txtTipoAgrupacion.</summary>
    [ObservableProperty]
    private double _tipoAgrupacionPasoFijo;

    /// <summary>Lista de agrupaciones paso fijo (legacy: CtrlAgrupacion "CtrlAgrupacionesPasoFijo").</summary>
    public ObservableCollection<VisorAbdonAgrupacion> AgrupacionesPasoFijo { get; } = new();

    /// <summary>Resumen "Hay X agrupaciones" en paso fijo (legacy: lblHayPasoFijo).</summary>
    [ObservableProperty]
    private string _hayPasoFijo = "";

    /// <summary>Nº de elementos del último filtro paso fijo aplicado (legacy: lblNumElementosPasoFijo).</summary>
    [ObservableProperty]
    private string _numElementosPasoFijo = "";

    // ------- Agrupaciones Solapadas -------

    /// <summary>Filtro: mostrar sólo agrupaciones solapadas de N elementos. Legacy: txtTipoAgrupacionSolapada.</summary>
    [ObservableProperty]
    private double _tipoAgrupacionSolapada;

    /// <summary>Lista de agrupaciones solapadas (legacy: CtrlAgrupacion "CtrlAgrupacionesSolapadas").</summary>
    public ObservableCollection<VisorAbdonAgrupacion> AgrupacionesSolapadas { get; } = new();

    /// <summary>Resumen "Hay X agrupaciones" en solapadas (legacy: lblHaySolapadas).</summary>
    [ObservableProperty]
    private string _haySolapadas = "";

    /// <summary>Nº de elementos del último filtro solapado aplicado (legacy: lblNumElementosSolapadas).</summary>
    [ObservableProperty]
    private string _numElementosSolapadas = "";

    // ------- Suma de aciertos -------

    /// <summary>Suma total de la serie de aciertos (valor o rango). Legacy: lblSumaTotalAciertos.</summary>
    [ObservableProperty]
    private string _sumaTotalAciertos = "-";

    /// <summary>Columnas a sumar de forma opcional (modo individual). Legacy: txtSumaAciertosOpcional.</summary>
    [ObservableProperty]
    private string _sumaAciertosOpcionalEntrada = "";

    /// <summary>Resultado de la suma opcional de aciertos (legacy: lblSumaOpcional).</summary>
    [ObservableProperty]
    private string _sumaOpcional = "-";

    // ------- Escaleras y sandwichs -------

    /// <summary>Escaleras ascendentes (valor o rango). Legacy: lblNumEscalerasAsc.</summary>
    [ObservableProperty]
    private string _escalerasAsc = "-";

    /// <summary>Escaleras descendentes (valor o rango). Legacy: lblNumEscalerasDesc.</summary>
    [ObservableProperty]
    private string _escalerasDesc = "-";

    /// <summary>Escaleras totales (valor o rango). Legacy: lblNumEscalerasTotales.</summary>
    [ObservableProperty]
    private string _escalerasTotales = "-";

    /// <summary>Número de sandwichs (valor o rango). Legacy: lblNumSandwichs.</summary>
    [ObservableProperty]
    private string _sandwichs = "-";

    public VisorAnalisisColumnasAbdonFrmViewModel()
    {
        // TODO[dominio]: el form legacy recibe en su constructor:
        //   List<InformeColumnasABDON> informePorCols, List<InformeColumnasABDON> informePorGans,
        //   List<long> columnas. Inyectar esos datos aquí y llamar a MostrarInformePorCols()/
        //   MostrarInformePorGans() para poblar los campos. Legacy: VisorAnalisisColumnasAbdonFrm(..).
    }

    partial void OnModoGlobalChanged(bool value)
    {
        OnPropertyChanged(nameof(TextoBotonModo));
        OnPropertyChanged(nameof(VisibilidadIndividual));
        OnPropertyChanged(nameof(VisibilidadGlobal));
    }

    /// <summary>
    /// Alterna entre modo Global e Individual y refresca el informe de ganadoras.
    /// Legacy: VisorAnalisisColumnasAbdonFrm.btnModo_Click + MostrarInformePorGans().
    /// </summary>
    [RelayCommand]
    private void CambiarModo()
    {
        ModoGlobal = !ModoGlobal;
        // Limpieza de filtros igual que el legacy al cambiar de modo.
        HayPasoFijo = "";
        HaySolapadas = "";
        NumElementosPasoFijo = "";
        NumElementosSolapadas = "";
        TipoAgrupacionPasoFijo = 0;
        TipoAgrupacionSolapada = 0;
        // TODO[dominio]: recalcular informe por ganadoras según el modo activo.
        //   Legacy: MostrarInformePorGans() -> ObtenInfoPorGans()/ObtenInfoPorGansGlobal().
    }

    /// <summary>Avanza a la siguiente columna generada. Legacy: btnAdelante_Click.</summary>
    [RelayCommand]
    private void AdelanteColumna()
    {
        // TODO[dominio]: noColumna++ si procede y MostrarInformePorCols().
    }

    /// <summary>Retrocede a la columna generada anterior. Legacy: btnAtras_Click.</summary>
    [RelayCommand]
    private void AtrasColumna()
    {
        // TODO[dominio]: noColumna-- si procede y MostrarInformePorCols().
    }

    /// <summary>Avanza a la siguiente columna ganadora (modo individual). Legacy: btnAdelanteGanadoras_Click.</summary>
    [RelayCommand]
    private void AdelanteGanadora()
    {
        // TODO[dominio]: noColumnaGanadora++ , reiniciar contenedores de agrupaciones y MostrarInformePorGans().
    }

    /// <summary>Retrocede a la columna ganadora anterior (modo individual). Legacy: btnAtrasGanadoras_Click.</summary>
    [RelayCommand]
    private void AtrasGanadora()
    {
        // TODO[dominio]: noColumnaGanadora-- , reiniciar contenedores de agrupaciones y MostrarInformePorGans().
    }

    /// <summary>
    /// Calcula y muestra las agrupaciones paso fijo del tipo/elementos indicado.
    /// Legacy: btnMostrarAgrupaciones_Click -> MostrarAgrupacionesPasoFijo[Global].
    /// </summary>
    [RelayCommand]
    private void MostrarAgrupacionesPasoFijo()
    {
        // TODO[dominio]: tipo = (int)TipoAgrupacionPasoFijo; según modo, recalcular
        //   ContenedorAgrupacionesPasoFijo[Global], poblar AgrupacionesPasoFijo y fijar
        //   HayPasoFijo (total individual o "min-max" global) y NumElementosPasoFijo.
        AgrupacionesPasoFijo.Clear();
        NumElementosPasoFijo = ((int)TipoAgrupacionPasoFijo).ToString();
    }

    /// <summary>
    /// Calcula y muestra las agrupaciones solapadas del tipo/elementos indicado.
    /// Legacy: btnMostrarAgrupacionesSolapadas_Click -> MostrarAgrupacionesSolapadas[Global].
    /// </summary>
    [RelayCommand]
    private void MostrarAgrupacionesSolapadas()
    {
        // TODO[dominio]: tipo = (int)TipoAgrupacionSolapada; según modo, recalcular
        //   ContenedorAgrupacionesSolapadas[Global], poblar AgrupacionesSolapadas y fijar
        //   HaySolapadas (total individual o "min-max" global) y NumElementosSolapadas.
        AgrupacionesSolapadas.Clear();
        NumElementosSolapadas = ((int)TipoAgrupacionSolapada).ToString();
    }

    /// <summary>
    /// Suma los aciertos de las columnas indicadas (modo individual).
    /// Legacy: btnSumaOpcional_Click -> MostrarSumaOpcionalDeAciertos().
    /// </summary>
    [RelayCommand]
    private void CalcularSumaOpcional()
    {
        // TODO[dominio]: parsear SumaAciertosOpcionalEntrada con
        //   Utils.UtilidadesEntradasValores.ObtenerListaFromTxt(...) y sumar inf.SerieAciertos
        //   de la ganadora actual sobre esos índices; reflejar en SumaOpcional.
    }

    /// <summary>
    /// Genera y guarda un fichero de condición (filtro de columnas probables) a partir de los
    /// valores globales mostrados. Legacy: btnGenerarCondicion_Click -> GenerarCondicion().
    /// </summary>
    [RelayCommand]
    private void GenerarCondicion()
    {
        // TODO[dominio]: portar VisorAnalisisColumnasAbdonFrm.GenerarCondicion().
        //   Mostrar FileSavePicker (*.cps / *.xml), construir un FiltroColProbables con las
        //   ColumnaProbable de cada columna (min/max aciertos), añadir RelacionCP1 (suma de
        //   aciertos) y RelacionCP3 (escaleras asc/desc/totales, sandwichs y agrupaciones paso
        //   fijo/solapadas) y guardarlo con ArchivoCondiciones.GuardaArchivo(filtro).
    }
}
