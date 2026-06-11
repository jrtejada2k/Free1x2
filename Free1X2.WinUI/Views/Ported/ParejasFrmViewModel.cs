using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Pares" (legacy: Free1X2.UI.ParejasFrm).
/// Filtra columnas de un fichero de entrada contando, para cada uno de los 14
/// partidos, el resultado de la "pareja" formada por otros dos partidos
/// (combinaciones 11, 1x, 12, x1, xx, x2, 21, 2x, 22). Cada combinación asigna
/// un nivel (1..7); luego, por nivel, se exige un número de aciertos dentro de
/// un rango [min, max] (las "Condiciones"). Las columnas que cumplen se guardan.
///
/// Datos de dominio legacy: matriz int[14,11] nivells (P1, P2 + 9 valores de
/// pareja por fila) y matriz int[7,3] rks (min, max, recuento por nivel).
/// Métodos legacy: LeeCondis/SalvaCondis (E/S de ficheros *.par),
/// Calcular (lee *.txt y valida), GrabarCols (escribe resultado),
/// Analizar (analiza una sola "Columna Ganadora"), Valida/s1n/n1s.
/// </summary>
public partial class ParejasFrmViewModel : ObservableObject
{
    public ParejasFrmViewModel()
    {
        // 14 partidos (filas de "Niveles"). Legacy: nivells[0..13, 0..10].
        for (int i = 0; i < 14; i++)
        {
            Niveles.Add(new NivelPareja(i + 1));
        }

        // 7 niveles de "Condiciones" con sus min/max por defecto del legacy.
        // Legacy InitializeComponent: c11=0/c12=10 ... c71=0/c72=10
        // (cMin por defecto 0; cMax por defecto 10; c11/c21 etc. = 0).
        for (int i = 0; i < 7; i++)
        {
            Condiciones.Add(new CondicionNivel(i + 1) { Min = "0", Max = "10" });
        }
    }

    /// <summary>14 filas de niveles de pareja (legacy: matriz nivells).</summary>
    public ObservableCollection<NivelPareja> Niveles { get; } = new();

    /// <summary>7 filas de condiciones por nivel (legacy: matriz rks min/max).</summary>
    public ObservableCollection<CondicionNivel> Condiciones { get; } = new();

    // Columna ganadora de prueba para "Analizar" (legacy: tCG, default "111X11X222X111").
    [ObservableProperty]
    private string _columnaGanadora = "111X11X222X111";

    // Contador de columnas procesadas (legacy: lproc, ctproc). String por regla anti-crash.
    [ObservableProperty]
    private string _procesadas = "0";

    // Contador de columnas válidas (legacy: lval, ctval). String por regla anti-crash.
    [ObservableProperty]
    private string _validas = "0";

    // Tiempo transcurrido (legacy: ltime). String por regla anti-crash.
    [ObservableProperty]
    private string _tiempo = "0";

    // Resultados del análisis de la columna ganadora, recuento por nivel 1..7
    // (legacy: r1..r7 = rks[0..6,2]). String "-" inicial por regla anti-crash.
    [ObservableProperty]
    private string _resultadoNivel1 = "-";

    [ObservableProperty]
    private string _resultadoNivel2 = "-";

    [ObservableProperty]
    private string _resultadoNivel3 = "-";

    [ObservableProperty]
    private string _resultadoNivel4 = "-";

    [ObservableProperty]
    private string _resultadoNivel5 = "-";

    [ObservableProperty]
    private string _resultadoNivel6 = "-";

    [ObservableProperty]
    private string _resultadoNivel7 = "-";

    // Habilita/deshabilita el botón Calcular durante el cálculo (legacy: bCalcular.Enabled).
    [ObservableProperty]
    private bool _puedeCalcular = true;

    // Muestra el botón "Grabar resultado" tras un cálculo (legacy: bGrabar.Visible).
    [ObservableProperty]
    private bool _puedeGrabar;

    /// <summary>
    /// Carga las condiciones desde un fichero *.par.
    /// Legacy: ParejasFrm.LeeCondis() — OpenFileDialog (*.par), lee 14 líneas de
    /// niveles (P1,P2 + 9 valores) y 7 líneas de condiciones (min,max).
    /// </summary>
    [RelayCommand]
    private void Leer()
    {
        // TODO[dominio]: abrir selector de archivo (FileOpenPicker, *.par) y rellenar
        //   Niveles y Condiciones con el contenido. Equivale a ParejasFrm.LeeCondis().
    }

    /// <summary>
    /// Guarda las condiciones actuales a un fichero *.par.
    /// Legacy: ParejasFrm.SalvaCondis() — SaveFileDialog (*.par).
    /// </summary>
    [RelayCommand]
    private void SalvarCondiciones()
    {
        // TODO[dominio]: abrir selector de guardado (FileSavePicker, *.par) y serializar
        //   Niveles + Condiciones en formato CSV por línea. Equivale a ParejasFrm.SalvaCondis().
    }

    /// <summary>
    /// Lee un fichero de columnas de entrada (*.txt) y filtra las que cumplen las
    /// condiciones de parejas. Legacy: ParejasFrm.Calcular() + RecuperarPantalla() + Valida().
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: portar ParejasFrm.Calcular():
        //   - PuedeCalcular = false; PuedeGrabar = false;
        //   - RecuperarPantalla(): volcar Niveles/Condiciones a nivells[14,11] y rks[7,3].
        //   - FileOpenPicker (*.txt); por cada línea: Valida(columna);
        //     si válida -> validas[s1n(columna)] = true; actualizar Procesadas/Validas/Tiempo.
        //   - Al terminar: PuedeCalcular = true; PuedeGrabar = true.
    }

    /// <summary>
    /// Guarda a un *.txt las columnas que pasaron el filtro.
    /// Legacy: ParejasFrm.GrabarCols() — recorre el BitArray validas (4.782.969) y
    /// escribe n1s(idx) por cada bit activo.
    /// </summary>
    [RelayCommand]
    private void GrabarResultado()
    {
        // TODO[dominio]: portar ParejasFrm.GrabarCols():
        //   - FileSavePicker (*.txt); recorrer validas; escribir n1s(idx) por cada bit activo.
    }

    /// <summary>
    /// Analiza una única "Columna Ganadora" mostrando el recuento por nivel 1..7.
    /// Legacy: ParejasFrm.Analizar() — RecuperarPantalla() + Valida(tCG.Text) y r1..r7 = rks[*,2].
    /// </summary>
    [RelayCommand]
    private void Analizar()
    {
        // TODO[dominio]: portar ParejasFrm.Analizar():
        //   - RecuperarPantalla(); Valida(ColumnaGanadora);
        //   - ResultadoNivel1..7 = rks[0..6,2].ToString();
    }

    /// <summary>
    /// Cierra/regresa sin guardar. Legacy: bCancelar -> Close().
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: navegación WinUI — Frame.GoBack() o cerrar el host contenedor
        //   (equivale a ParejasFrm.Close()).
    }
}

/// <summary>
/// Una fila de "Niveles": un partido (1..14) con sus posiciones de pareja (P1, P2)
/// y el nivel asignado a cada una de las 9 combinaciones de pareja.
/// Legacy: fila de nivells[r, 0..10]. Todas las propiedades son string por la
/// regla anti-crash (NumberBox.Value es double; aquí se usan TextBox numéricos).
/// </summary>
public partial class NivelPareja : ObservableObject
{
    public NivelPareja(int numero)
    {
        Numero = numero;
        NumeroTexto = numero.ToString();
    }

    /// <summary>Índice 1..14 del partido (solo lectura, para la cabecera de fila).</summary>
    public int Numero { get; }

    /// <summary>Etiqueta de fila ya formateada (legacy int->Text, expuesta como string).</summary>
    public string NumeroTexto { get; }

    // P1 / P2: posiciones (1..14) de los dos partidos que forman la pareja. Legacy: nivells[r,0], [r,1].
    [ObservableProperty]
    private string _p1 = "0";

    [ObservableProperty]
    private string _p2 = "0";

    // Nivel (1..7) asignado a cada combinación de pareja. Legacy: nivells[r,2..10].
    [ObservableProperty]
    private string _v11 = "0";

    [ObservableProperty]
    private string _v1X = "0";

    [ObservableProperty]
    private string _v12 = "0";

    [ObservableProperty]
    private string _vX1 = "0";

    [ObservableProperty]
    private string _vXX = "0";

    [ObservableProperty]
    private string _vX2 = "0";

    [ObservableProperty]
    private string _v21 = "0";

    [ObservableProperty]
    private string _v2X = "0";

    [ObservableProperty]
    private string _v22 = "0";
}

/// <summary>
/// Una fila de "Condiciones": para un nivel (1..7) define el rango [Min, Max] de
/// aciertos permitidos. Legacy: rks[n, 0] = min, rks[n, 1] = max.
/// </summary>
public partial class CondicionNivel : ObservableObject
{
    public CondicionNivel(int nivel)
    {
        Nivel = nivel;
        NivelTexto = nivel.ToString();
    }

    /// <summary>Índice 1..7 del nivel (solo lectura).</summary>
    public int Nivel { get; }

    /// <summary>Etiqueta de nivel formateada (legacy int->Text, expuesta como string).</summary>
    public string NivelTexto { get; }

    // Mínimo de aciertos para este nivel. Legacy: rks[n,0] (cX1).
    [ObservableProperty]
    private string _min = "0";

    // Máximo de aciertos para este nivel. Legacy: rks[n,1] (cX2).
    [ObservableProperty]
    private string _max = "10";
}
