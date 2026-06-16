using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Controls;
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
    // --- Rejilla de porcentajes 1/X/2 por partido (reemplaza el UserControl WinForms
    //     ControlPorcentajes / controlPorcentajes1). PorcentajesHelper.AMatriz(Porcentajes)
    //     equivale a controlPorcentajes1.Valores (matriz double[NumeroPartidos,3]). ---
    public ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(VariablesGlobales.NumeroPartidos);

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

    // Arrays 1/X/2 por partido. Los recalcula PrepararValores() a partir de la rejilla
    // de porcentajes (Porcentajes) — equivalen a los campos valores1/valoresX/valores2 del form.
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
        // Volcar la matriz del filtro a la rejilla de porcentajes (equivale a
        // ValoracionFrm.PonerValoracionPantalla(filtro.Valores1, ValoresX, Valores2)).
        PonerValoracionPantalla(filtro.Valores1, filtro.ValoresX, filtro.Valores2);
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
        // Equivale a ValoracionFrm.CalcularValoracionColumna()
        //   (Free1X2/UI/Filtros/ValoracionFrm.cs líneas 385-404).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;
        var filtro = (FiltroValoracionSignos)grupo.GetFiltro(Filtro.ValoracionSignos.ToString());

        if (string.IsNullOrEmpty(Columna) || Columna.Length < VariablesGlobales.NumeroPartidos)
        {
            // El form legacy muestra un MessageBox y de todas formas prepara los valores
            // (para refrescar Mín-Máx). Aquí solo se preparan; el aviso lo dará la UI si procede.
            PrepararValores();
            return;
        }

        PrepararValores();

        filtro.Valores1 = _valores1!;
        filtro.ValoresX = _valoresX!;
        filtro.Valores2 = _valores2!;
        filtro.TipoValoracion = TipoValoracionDominio;

        filtro.Analizar(UtilColumnas.ConvStrToLong(Columna));
        Resultado = filtro.ValoracionResultado.ToString("r");
    }

    [RelayCommand]
    private void GenerarColumnasBase()
    {
        // Equivale a ValoracionFrm.btnColsBase_Click() / ObtenerColsBase()
        //   (Free1X2/UI/Filtros/ValoracionFrm.cs líneas 120-146, 961-966).
        string[] columna = ObtenerColsBase();
        ColumnasBase = columna[0] + "\r\n" + columna[1] + "\r\n" + columna[2];
    }

    // === Lógica de cálculo portada de ValoracionFrm (sin tocar el motor) ======

    // Equivale a ValoracionFrm.PonerValoracionPantalla(): vuelca 3 arrays a la rejilla.
    private void PonerValoracionPantalla(double[] v1, double[] vX, double[] v2)
    {
        var m = new double[VariablesGlobales.NumeroPartidos, 3];
        for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
        {
            m[i, 0] = i < v1.Length ? v1[i] : 0;
            m[i, 1] = i < vX.Length ? vX[i] : 0;
            m[i, 2] = i < v2.Length ? v2[i] : 0;
        }
        PorcentajesHelper.CargarMatriz(Porcentajes, m);
    }

    // Equivale a ValoracionFrm.PrepararValores() (líneas 206-312): lee la rejilla
    // (controlPorcentajes1.Valores -> matriz), separa en valores1/X/2, calcula los
    // Mín-Máx por sumas y productos y rellena los rangos que estén vacíos.
    private void PrepararValores()
    {
        double[,] nvals = PorcentajesHelper.AMatriz(Porcentajes);  // == controlPorcentajes1.Valores

        double sumaValorMin = 0, sumaValorMax = 0;
        double sumaValor1Max = 0, sumaValorXMax = 0, sumaValor2Max = 0;
        double ProductoValorMin = 1, ProductoValorMax = 1;
        double ProductoValor1Max = 1, ProductoValorXMax = 1, ProductoValor2Max = 1;

        // REINICIALIZA VALORES
        _valores1 = new double[VariablesGlobales.NumeroPartidos];
        _valoresX = new double[VariablesGlobales.NumeroPartidos];
        _valores2 = new double[VariablesGlobales.NumeroPartidos];
        for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
        {
            _valores1[i] = nvals[i, 0];
            _valoresX[i] = nvals[i, 1];
            _valores2[i] = nvals[i, 2];
        }

        for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
        {
            // globales
            sumaValorMax += ObtenValorMaximo(_valores1[i], _valoresX[i], _valores2[i]);
            sumaValorMin += ObtenValorMinimo(_valores1[i], _valoresX[i], _valores2[i]);
            if ((_valores1[i] + _valoresX[i] + _valores2[i]) > 0)
            {
                ProductoValorMax *= ObtenValorMaximo(_valores1[i], _valoresX[i], _valores2[i]) * 0.03420425138;
                ProductoValorMin *= ObtenValorMinimo(_valores1[i], _valoresX[i], _valores2[i]) * 0.03420425138;
            }

            // por sumas
            sumaValor1Max += _valores1[i];
            sumaValorXMax += _valoresX[i];
            sumaValor2Max += _valores2[i];

            // por múltiplos: solo valores mayores de 29.236 producen aumento de la valoración
            if (_valores1[i] > 29.236) ProductoValor1Max *= _valores1[i] * 0.03420425138;
            if (_valoresX[i] > 29.236) ProductoValorXMax *= _valoresX[i] * 0.03420425138;
            if (_valores2[i] > 29.236) ProductoValor2Max *= _valores2[i] * 0.03420425138;
        }

        ProductoValorMax = Math.Round(ProductoValorMax + 0.000999999, 3);
        ProductoValorMin = Math.Round(ProductoValorMin - 0.000999999, 3);
        if (ProductoValorMin < 0) ProductoValorMin = 0;
        MinMaxSumas = sumaValorMin + "-" + sumaValorMax;
        MinMaxProductos = ProductoValorMin + "-" + ProductoValorMax;

        bool esSuma = TipoValoracionDominio == "suma";

        if (string.IsNullOrEmpty(ValorGlobal))
            ValorGlobal = esSuma ? sumaValorMin + "-" + sumaValorMax
                                 : ProductoValorMin + "-" + ProductoValorMax;

        if (string.IsNullOrEmpty(ValorUnos))
            ValorUnos = esSuma ? "0-" + sumaValor1Max
                               : "0-" + Math.Round(ProductoValor1Max + 0.000999999, 3);

        if (string.IsNullOrEmpty(ValorEquis))
            ValorEquis = esSuma ? "0-" + sumaValorXMax
                                : "0-" + Math.Round(ProductoValorXMax + 0.000999999, 3);

        if (string.IsNullOrEmpty(ValorDoses))
            ValorDoses = esSuma ? "0-" + sumaValor2Max
                                : "0-" + Math.Round(ProductoValor2Max + 0.000999999, 3);
    }

    // Equivale a ValoracionFrm.ObtenerColsBase() (líneas 120-146).
    private string[] ObtenerColsBase()
    {
        double[,] nvals = PorcentajesHelper.AMatriz(Porcentajes);
        string[] columna = new string[3] { "", "", "" };

        var v1 = new double[VariablesGlobales.NumeroPartidos];
        var vX = new double[VariablesGlobales.NumeroPartidos];
        var v2 = new double[VariablesGlobales.NumeroPartidos];
        for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
        {
            v1[i] = nvals[i, 0];
            vX[i] = nvals[i, 1];
            v2[i] = nvals[i, 2];
        }

        for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
        {
            string tmp = ordenarValoracionPartido(v1[i], vX[i], v2[i]);
            for (int j = 0; j < 3; j++) columna[j] += tmp.Substring(j, 1);
        }
        return columna;
    }

    // Equivale a ValoracionFrm.ordenarValoracionPartido() (líneas 148-204).
    private static string ordenarValoracionPartido(double v1, double vX, double v2)
    {
        string mayor, medio, menor;
        if ((v1 >= v2) && (v1 >= vX))
        {
            mayor = "1";
            if (vX >= v2) { medio = "X"; menor = "2"; }
            else { medio = "2"; menor = "X"; }
        }
        else if (vX >= v2)
        {
            mayor = "X";
            if (v1 >= v2) { medio = "1"; menor = "2"; }
            else { medio = "2"; menor = "1"; }
        }
        else
        {
            mayor = "2";
            if (vX > v1) { medio = "X"; menor = "1"; }
            else { medio = "1"; menor = "X"; }
        }
        return mayor + medio + menor;
    }

    private static double ObtenValorMaximo(double valor1, double valor2, double valor3)
    {
        double m = valor1;
        if (valor2 > m) m = valor2;
        if (valor3 > m) m = valor3;
        return m;
    }

    private static double ObtenValorMinimo(double valor1, double valor2, double valor3)
    {
        double m = valor1;
        if (valor2 < m) m = valor2;
        if (valor3 < m) m = valor3;
        return m;
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

        // Equivale a ValoracionFrm.ActualizarDatos() = PrepararValores() + PonerCondicionesFiltro()
        //   (ValoracionFrm.cs líneas 968-972, 344-363). PrepararValores() lee la rejilla de
        //   porcentajes (PorcentajesHelper.AMatriz) en valores1/X/2 antes de poblar el filtro.
        PrepararValores();

        var filtro = (FiltroValoracionSignos)grupo.GetFiltro(Filtro.ValoracionSignos.ToString());
        filtro.ReinicializaValores();

        filtro.TipoValoracion = TipoValoracionDominio;
        filtro.Valores1 = _valores1!;
        filtro.ValoresX = _valoresX!;
        filtro.Valores2 = _valores2!;
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
