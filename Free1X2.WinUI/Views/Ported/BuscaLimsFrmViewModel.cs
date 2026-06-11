using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del "Buscador límites" (WinForms <c>BuscaLimsFrm</c>).
/// Recorre todas las columnas posibles (3^N) y, para las que aciertan dentro del
/// rango de aciertos permitido respecto a la columna base, calcula los valores
/// mínimo/máximo de Sumas y de Productos. Muestra además columnas Procesadas,
/// Admitidas y Tiempo. Toda la lógica de cálculo está marcada como TODO.
/// </summary>
public partial class BuscaLimsFrmViewModel : ObservableObject
{
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
    // Se expone como string para poder bindearla directamente a un TextBlock.
    [ObservableProperty]
    private string _columnaBase = string.Empty;

    [RelayCommand]
    private void Calcular()
    {
        // TODO: Dominio legacy — equivale a BuscaLimsFrm.Calcular():
        //   1. dv = valors1.RetVals();                         // matriz [partidos,3] de valoraciones (control 'valors').
        //   2. CalculoCB();                                    // determina la columna base (signo mayor por partido) -> ColumnaBase.
        //   3. ObtenerRangos();                                // UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(RangoAciertos) -> BitArray.
        //   4. AnalizarColumnas("", 0);                        // recursión sobre 3^N columnas; por cada una:
        //        - columnaGenerada = UtilColumnas.ConvStrToLong(col);
        //        - aciertos = UtilColumnas.ContarBitsA1(ColumnaBase & columnaGenerada);
        //        - si rangos[aciertos] -> ObtenerValoracion(col) acumula min/max de Suma y Producto.
        //   5. Refrescar();                                    // vuelca min/max -> SumaMin/SumaMax/ProductoMin/ProductoMax,
        //                                                       // nca -> ColumnasProcesadas, ncv -> ColumnasAdmitidas, tiempo -> Tiempo.
        // Nota: el form legacy usa un Timer cada 3 s para refrescar el progreso (CalculoColumnas).
    }
}
