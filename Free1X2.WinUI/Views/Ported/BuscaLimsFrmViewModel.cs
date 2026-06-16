using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.Utils;
using Free1X2.WinUI.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del "Buscador límites" (WinForms <c>BuscaLimsFrm</c>).
/// Recorre todas las columnas posibles (3^N) y, para las que aciertan dentro del
/// rango de aciertos permitido respecto a la columna base, calcula los valores
/// mínimo/máximo de Sumas y de Productos. Muestra además columnas Procesadas,
/// Admitidas y Tiempo.
/// </summary>
public partial class BuscaLimsFrmViewModel : ObservableObject
{
    // Rejilla de valoraciones 1/X/2 por partido (reemplaza el UserControl WinForms 'valors'
    // / valors1). PorcentajesHelper.AMatriz(Porcentajes) == valors1.RetVals() (double[N,3]).
    public ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(VariablesGlobales.NumeroPartidos);

    // Rango de aciertos permitidos respecto a la columna base. Campo legacy tblac (default "4-9").
    [ObservableProperty]
    private string _rangoAciertos = "4-9";

    // --- Resultados (GroupBox "Resultados" del form legacy; readonly) ---

    // Suma mínima. Campo legacy tbsmin.
    [ObservableProperty]
    private string _sumaMin = string.Empty;

    // Suma máxima. Campo legacy tbsmax.
    [ObservableProperty]
    private string _sumaMax = string.Empty;

    // Producto mínimo. Campo legacy tbpmin.
    [ObservableProperty]
    private string _productoMin = string.Empty;

    // Producto máximo. Campo legacy tbpmax.
    [ObservableProperty]
    private string _productoMax = string.Empty;

    // --- Estado del proceso (etiquetas lColProc / lColAdm / lTime) ---

    // Nº de columnas procesadas. Campo legacy lColProc (nca).
    [ObservableProperty]
    private string _columnasProcesadas = "0";

    // Nº de columnas admitidas (dentro del rango). Campo legacy lColAdm (ncv).
    [ObservableProperty]
    private string _columnasAdmitidas = "0";

    // Tiempo empleado. Campo legacy lTime.
    [ObservableProperty]
    private string _tiempo = string.Empty;

    // Columna base calculada a partir de los valores (etiquetas lb01..lbl16 / C.B.).
    [ObservableProperty]
    private string _columnaBase = string.Empty;

    // Deshabilita el botón Calcular durante el proceso (legacy: bCalcular.Enabled = false).
    [ObservableProperty]
    private bool _puedeCalcular = true;

    // --- Estado interno del cálculo (campos de instancia del Form legacy) ---
    private int _noPartidos;
    private long _columnaBaseLong;
    private double[,] _dv = new double[14, 3];
    private BitArray _rangos = new(VariablesGlobales.NumeroPartidos + 1, false);

    private double _min;
    private double _max;
    private double _minProductos;
    private double _maxProductos;
    private int _nca;
    private int _ncv;

    [RelayCommand]
    private async Task CalcularAsync()
    {
        // Legacy ctor: noPartidos = VariablesGlobales.NumeroPartidos (con fallback 14).
        try { _noPartidos = VariablesGlobales.NumeroPartidos; }
        catch { _noPartidos = 14; }
        if (_noPartidos <= 0) _noPartidos = 14;

        // Valoración real desde la rejilla de porcentajes. Equivale a
        // BuscaLimsFrm.RecuperaPantalla() línea 111: dv = valors1.RetVals().
        _dv = PorcentajesHelper.AMatriz(Porcentajes);  // == valors1.RetVals()

        string rango = RangoAciertos;
        PuedeCalcular = false;
        SumaMin = SumaMax = ProductoMin = ProductoMax = string.Empty;
        try
        {
            // El recorrido es 3^N columnas: ejecuta fuera del hilo de UI (legacy usa Timer + DoEvents).
            var resultado = await Task.Run(() => Calcular(rango));

            ColumnaBase = resultado.ColumnaBaseTexto;

            // Legacy Refrescar(): vuelca min/max y contadores.
            if (resultado.Min > -1) SumaMin = resultado.Min.ToString();
            if (resultado.Max > 0) SumaMax = resultado.Max.ToString();
            if (resultado.MinProductos > -1) ProductoMin = resultado.MinProductos.ToString();
            if (resultado.MaxProductos > 0) ProductoMax = resultado.MaxProductos.ToString();
            ColumnasProcesadas = resultado.Nca.ToString();
            ColumnasAdmitidas = resultado.Ncv.ToString();
            // Legacy formatea (time9-time0)+"00000000000" y toma 11 caracteres.
            string tmp = resultado.Tiempo + "00000000000";
            Tiempo = tmp.Length >= 11 ? tmp.Substring(0, 11) : tmp;
        }
        catch (Exception ex)
        {
            Tiempo = "Error: " + ex.Message;
        }
        finally
        {
            PuedeCalcular = true;
        }
    }

    /// <summary>
    /// Cálculo completo, equivalente a BuscaLimsFrm.Calcular():
    /// determina columna base, obtiene rangos y recorre todas las columnas (AnalizarColumnas).
    /// Puro (sin UI); apto para Task.Run.
    /// </summary>
    private ResultadoBusqueda Calcular(string rangoTexto)
    {
        DateTime time0 = DateTime.Now;

        // Reinicio del estado de acumulación (legacy: campos de instancia).
        _min = -1;
        _max = 0;
        _minProductos = -1;
        _maxProductos = 0;
        _nca = 0;
        _ncv = 0;

        string columnaBaseTexto = CalculoCB(); // legacy CalculoCB() -> columnaBase
        ObtenerRangos(rangoTexto);             // legacy ObtenerRangos()
        AnalizarColumnas("", 0);               // legacy AnalizarColumnas("", 0)

        DateTime time9 = DateTime.Now;
        return new ResultadoBusqueda
        {
            ColumnaBaseTexto = columnaBaseTexto,
            Min = _min,
            Max = _max,
            MinProductos = _minProductos,
            MaxProductos = _maxProductos,
            Nca = _nca,
            Ncv = _ncv,
            Tiempo = (time9 - time0).ToString(),
        };
    }

    /// <summary>
    /// Legacy ObtenerRangos(): convierte el texto de aciertos permitidos en un BitArray
    /// con UtilidadesEntradasValores.ObtenerListaFromTxtAciertos.
    /// </summary>
    private void ObtenerRangos(string rangoTexto)
    {
        _rangos = new BitArray(VariablesGlobales.NumeroPartidos + 1, false);
        List<int> aciertosPermitidos = UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(rangoTexto);
        for (int i = 0; i < aciertosPermitidos.Count; i++)
        {
            if (aciertosPermitidos[i] >= 0 && aciertosPermitidos[i] < _rangos.Length)
            {
                _rangos[aciertosPermitidos[i]] = true;
            }
        }
    }

    /// <summary>
    /// Legacy CalculoCB(): determina el signo de mayor valor por partido y construye la columna base.
    /// </summary>
    private string CalculoCB()
    {
        string temp = "";
        for (int i = 0; i < _dv.GetLength(0); i++)
        {
            // Réplica exacta de la condición legacy (incluida la referencia dv[0,2]).
            if (_dv[i, 0] >= _dv[i, 1] && _dv[i, 0] >= _dv[0, 2]) temp += "1";
            else if (_dv[i, 2] >= _dv[i, 0] && _dv[i, 2] >= _dv[i, 1]) temp += "2";
            else temp += "X";
        }
        _columnaBaseLong = UtilColumnas.ConvStrToLong(temp);
        return temp;
    }

    /// <summary>
    /// Legacy ObtenerValoracion(col): acumula los mínimos/máximos de Suma y Producto.
    /// </summary>
    private void ObtenerValoracion(string col)
    {
        double valor = 0;
        double valorProductos = 1;
        for (int i = 0; i < col.Length; i++)
        {
            switch (col[i])
            {
                case '1':
                    valor += _dv[i, 0];
                    valorProductos *= _dv[i, 0] * 0.03420425138;
                    break;
                case 'X':
                    valor += _dv[i, 1];
                    valorProductos *= _dv[i, 1] * 0.03420425138;
                    break;
                case '2':
                    valor += _dv[i, 2];
                    valorProductos *= _dv[i, 2] * 0.03420425138;
                    break;
            }
        }
        if (_min > valor || _min == -1) _min = valor;
        if (_max < valor) _max = valor;
        if (_minProductos > valorProductos || _minProductos == -1) _minProductos = valorProductos;
        if (_maxProductos < valorProductos) _maxProductos = valorProductos;
    }

    /// <summary>
    /// Legacy AnalizarColumnas(preString, partidoNo): recursión sobre 3^N columnas; por cada una
    /// cuenta aciertos respecto a la columna base y, si está en rango, acumula la valoración.
    /// </summary>
    private void AnalizarColumnas(string preString, int partidoNo)
    {
        string[] signos = { "1", "X", "2" };
        for (int i = 0; i < signos.Length; i++)
        {
            string newPreString = preString + signos[i];
            if (partidoNo < _noPartidos - 1)
            {
                AnalizarColumnas(newPreString, partidoNo + 1);
            }
            else
            {
                _nca++;
                long columnaGenerada = UtilColumnas.ConvStrToLong(newPreString);
                long resultado = _columnaBaseLong & columnaGenerada;
                int aciertos = UtilColumnas.ContarBitsA1(resultado);
                if (aciertos >= 0 && aciertos < _rangos.Length && _rangos[aciertos])
                {
                    _ncv++;
                    ObtenerValoracion(newPreString);
                }
            }
        }
    }

    /// <summary>Agrupa el resultado del cálculo para devolverlo desde el hilo de fondo.</summary>
    private sealed class ResultadoBusqueda
    {
        public string ColumnaBaseTexto = string.Empty;
        public double Min;
        public double Max;
        public double MinProductos;
        public double MaxProductos;
        public int Nca;
        public int Ncv;
        public string Tiempo = string.Empty;
    }
}
