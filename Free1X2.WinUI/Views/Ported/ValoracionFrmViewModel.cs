using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Valoración" (WinForms <c>ValoracionFrm</c> /
/// <c>FiltroValoracionSignos</c>). Asigna a cada partido unas valoraciones 1/X/2
/// (control de porcentajes) y filtra columnas cuya valoración Global, de Unos,
/// Equis y Doses caiga en los rangos indicados, en modo "suma" o "multiplo".
/// Toda la lógica de cálculo y persistencia está marcada como TODO citando la
/// clase legacy correspondiente.
/// </summary>
public partial class ValoracionFrmViewModel : ObservableObject
{
    // --- Tipo de valoración (RadioButtons radTipoVal_Suma / radTipoVal_Multiplo) ---
    // Se expone como ComboBox (regla anti-crash 3: ItemsSource desde el VM).
    // Valores legacy del campo tipoValoracion: "suma" / "multiplo".
    public IReadOnlyList<string> TiposValoracion { get; } = new[]
    {
        "Por sumas",
        "Por productos x 3E7",
    };

    // Opción seleccionada. "Por sumas" -> tipoValoracion = "suma";
    // "Por productos x 3E7" -> tipoValoracion = "multiplo".
    [ObservableProperty]
    private string _tipoValoracionSeleccionado = "Por sumas";

    // Cuando cambia el tipo, el form legacy reinicia los rangos (InicializaRangos()).
    partial void OnTipoValoracionSeleccionadoChanged(string value)
    {
        // Equivale a ValoracionFrm.InicializaRangos(): limpia los 4 rangos.
        ValorGlobal = string.Empty;
        ValorUnos = string.Empty;
        ValorEquis = string.Empty;
        ValorDoses = string.Empty;
        // TODO: Dominio legacy — radTipoVal_Clicked actualiza tipoValoracion.
    }

    // --- Mín-Máx informativos (readonly): TxMinMaxSumas / TxMinMaxProductos ---

    // Rango mínimo-máximo posible por sumas. Campo legacy TxMinMaxSumas (readonly).
    [ObservableProperty]
    private string _minMaxSumas = string.Empty;

    // Rango mínimo-máximo posible por productos. Campo legacy TxMinMaxProductos (readonly).
    [ObservableProperty]
    private string _minMaxProductos = string.Empty;

    // --- Rangos de filtrado (usar '#' como separador de rangos, ej. 100-200#200-600) ---

    // Rango Global. Campo legacy valGlobal.
    [ObservableProperty]
    private string _valorGlobal = string.Empty;

    // Rango para el signo 1. Campo legacy TxVal1.
    [ObservableProperty]
    private string _valorUnos = string.Empty;

    // Rango para el signo X. Campo legacy TxValX.
    [ObservableProperty]
    private string _valorEquis = string.Empty;

    // Rango para el signo 2. Campo legacy TxVal2.
    [ObservableProperty]
    private string _valorDoses = string.Empty;

    // --- Calcular valoración de una columna concreta ---

    // Columna de N signos a valorar. Campo legacy txtColumna (mayúsculas, MaxLength = NumeroPartidos).
    [ObservableProperty]
    private string _columna = string.Empty;

    // Resultado de la valoración de la columna. Campo legacy txResultado (readonly).
    [ObservableProperty]
    private string _resultado = string.Empty;

    // --- Utilidades: columnas base (txtColBase, readonly multilinea) ---

    // Tres columnas base (mayor/medio/menor por partido). Campo legacy txtColBase.
    [ObservableProperty]
    private string _columnasBase = string.Empty;

    // Matrices de porcentajes 1/X/2 cargadas del filtro. Sin control de porcentajes en WinUI,
    // se preservan tal cual (no se reescriben al Aceptar). Ver TODO en Calcular/Aceptar.
    private double[]? _valores1;
    private double[]? _valoresX;
    private double[]? _valores2;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    // tipoValoracion del dominio ("suma" / "multiplo") a partir de la opción de pantalla.
    private string TipoValoracionDominio =>
        TipoValoracionSeleccionado == "Por sumas" ? "suma" : "multiplo";

    /// <summary>
    /// Vuelca los valores del FiltroValoracionSignos del grupo en edición a la pantalla.
    /// Equivale a ValoracionFrm.InicializarPantalla() (Free1X2/UI/Filtros/ValoracionFrm.cs líneas 97-118).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroValoracionSignos)grupo.GetFiltro(Filtro.ValoracionSignos.ToString());
        if (!filtro.ContieneDatos) return;

        TipoValoracionSeleccionado = filtro.TipoValoracion == "suma" ? "Por sumas" : "Por productos x 3E7";
        // Preservar la matriz de porcentajes (no hay control para editarla en WinUI).
        _valores1 = filtro.Valores1;
        _valoresX = filtro.ValoresX;
        _valores2 = filtro.Valores2;
        ValorGlobal = filtro.ValorGlobal;
        ValorUnos = filtro.ValorUnos;
        ValorEquis = filtro.ValorEquis;
        ValorDoses = filtro.ValorDoses;
    }

    [RelayCommand]
    private void Calcular()
    {
        // TODO: Dominio legacy — ValoracionFrm.CalcularValoracionColumna():
        //   1. Validar que Columna tenga VariablesGlobales.NumeroPartidos signos.
        //   2. PrepararValores(): lee controlPorcentajes1.Valores -> valores1/valoresX/valores2,
        //      calcula MinMaxSumas y MinMaxProductos y rellena rangos vacíos por defecto.
        //   3. filtro.Valores{1,X,2} = ...; filtro.TipoValoracion = tipoValoracion.
        //   4. filtro.Analizar(UtilColumnas.ConvStrToLong(Columna));
        //   5. Resultado = filtro.ValoracionResultado.ToString("r").
        // FiltroValoracionSignos / ControlPorcentajes / UtilColumnas.
    }

    [RelayCommand]
    private void GenerarColumnasBase()
    {
        // TODO: Dominio legacy — ValoracionFrm.btnColsBase_Click() / ObtenerColsBase():
        //   recorre los porcentajes, ordena cada partido (ordenarValoracionPartido) y
        //   compone 3 columnas (mayor/medio/menor) -> ColumnasBase = c0 + "\n" + c1 + "\n" + c2.
    }

    [RelayCommand]
    private void BuscarLimite()
    {
        // TODO: Dominio legacy — ValoracionFrm.btnBuscarLimites_Click():
        //   abre el form BuscaLimsFrm (port: BuscaLimsFrmPage) como diálogo.
    }

    // --- Barra de condiciones (control legacy MenuCondiciones) ---

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a ValoracionFrm.menuCondiciones1_BOk -> ActualizarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/ValoracionFrm.cs líneas 344-363, 968-1004).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroValoracionSignos)grupo.GetFiltro(Filtro.ValoracionSignos.ToString());
        filtro.ReinicializaValores();

        filtro.TipoValoracion = TipoValoracionDominio;
        // TODO[porcentajes]: el form legacy recalcula valores1/X/2 desde controlPorcentajes1.Valores
        //   en PrepararValores() (ValoracionFrm.cs líneas 206-312) y luego PonerCondicionesFiltro().
        //   No hay control de porcentajes en la página WinUI todavía: se conserva la matriz cargada
        //   del filtro (o se deja vacía si era un filtro nuevo). Editar los porcentajes requiere
        //   portar el control ControlPorcentajes; mientras tanto solo se editan tipo y rangos.
        if (_valores1 != null) filtro.Valores1 = _valores1;
        if (_valoresX != null) filtro.ValoresX = _valoresX;
        if (_valores2 != null) filtro.Valores2 = _valores2;
        filtro.ValorGlobal = ValorGlobal ?? "";
        filtro.ValorUnos = ValorUnos ?? "";
        filtro.ValorEquis = ValorEquis ?? "";
        filtro.ValorDoses = ValorDoses ?? "";
        if (filtro.ContieneDatos == false)
        {
            filtro.IsActive = true;
        }
        filtro.ContieneDatos = true;

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO: Dominio legacy — menuCondiciones1_BEstadisticas():
        //   filtroTemp = ObtenerFiltroTemporal();
        //   CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/");
        //   abre VisorEstadisticas (port: AnastaticsPage / VisorEstadisticas).
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO: Dominio legacy — menuCondiciones1_BGuardar(): ActualizarDatos();
        //   SaveFileDialog (*.valor/*.xml) -> ArchivoCondiciones.GuardaArchivo(filtro).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO: Dominio legacy — menuCondiciones1_BAbrir(): ActualizarDatos();
        //   OpenFileDialog (*.valor/*.xml) -> ArchivoCondiciones.LeeCondicion() ->
        //   filtro = g.GetFiltro("ValoracionSignos"); InicializarPantalla().
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO: Dominio legacy — menuCondiciones1_BCopiar(): ActualizarDatos();
        //   guarda fichero temporal "Temp/tmp.valor" y habilita Pegar.
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO: Dominio legacy — menuCondiciones1_BPegar(): abrir "Temp/tmp.valor".
        //   El botón sólo se habilita si existe el temporal (compruebaPegar()).
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a menuCondiciones1_BBorrar: limpia la pantalla (rangos + min-max).
        ValorGlobal = string.Empty;
        ValorUnos = string.Empty;
        ValorEquis = string.Empty;
        ValorDoses = string.Empty;
        MinMaxSumas = string.Empty;
        MinMaxProductos = string.Empty;
        _valores1 = _valoresX = _valores2 = null;
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
