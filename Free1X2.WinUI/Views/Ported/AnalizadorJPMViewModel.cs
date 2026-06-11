using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Representa una "casilla de suma" (bucket 0..35) del analizador.
/// Cada casilla puede marcarse para seleccionar las columnas cuya puntuación
/// coincide con ese valor, y muestra cuántas columnas cayeron en él.
/// Las propiedades expuestas a la UI son string (regla anti-crash: no bindear
/// int/bool directo a TextBlock.Text).
/// </summary>
public partial class CasillaSumaViewModel : ObservableObject
{
    public int Indice { get; }

    /// <summary>Etiqueta del bucket (0..35) como texto para el TextBlock.</summary>
    public string IndiceTexto { get; }

    [ObservableProperty]
    private bool _seleccionada;

    [ObservableProperty]
    private string _conteoTexto = "0";

    public CasillaSumaViewModel(int indice)
    {
        Indice = indice;
        IndiceTexto = indice.ToString();
    }
}

/// <summary>
/// ViewModel para la pantalla "Sumas Pares Naturales (JPM)" (legacy: AnalizadorJPM / AnalizadorJPMFrm).
///
/// Propósito legacy: asignar una puntuación a cada par de signos de una columna
/// (11, 1X, 12, X1, XX, X2, 21, 2X, 22) y clasificar todas las columnas de un
/// fichero en 36 "casillas de suma" (0..35). El usuario marca casillas, suma sus
/// conteos, graba las columnas seleccionadas y analiza premios (10..14) de una
/// columna ganadora navegable.
///
/// Toda la lógica de dominio (cálculo, lectura/escritura de ficheros, búsqueda de
/// premios) queda marcada como TODO citando la clase legacy; aquí solo se modela el estado de UI.
/// </summary>
public partial class AnalizadorJPMViewModel : ObservableObject
{
    // --- Valores a usar (max. 5): puntuación por tipo de par de signos ---
    // Legacy: tbv0..tbv8 -> vals[0..8] (11,1X,12,X1,XX,X2,21,2X,22), recortados a [0,5].
    [ObservableProperty]
    private double _valor11;

    [ObservableProperty]
    private double _valor1X;

    [ObservableProperty]
    private double _valor12;

    [ObservableProperty]
    private double _valorX1;

    [ObservableProperty]
    private double _valorXX;

    [ObservableProperty]
    private double _valorX2;

    [ObservableProperty]
    private double _valor21;

    [ObservableProperty]
    private double _valor2X;

    [ObservableProperty]
    private double _valor22;

    // --- Casillas de suma (0..35) ---
    public ObservableCollection<CasillaSumaViewModel> Casillas { get; } = new();

    // --- Estado de proceso / ficheros ---
    [ObservableProperty]
    private string _ficheroEntrada = "-";       // legacy: lFileIn

    [ObservableProperty]
    private string _ficheroSalida = "-";         // legacy: lFileOut

    [ObservableProperty]
    private string _totalColumnasTexto = "0";    // legacy: lCol

    [ObservableProperty]
    private string _tiempoTexto = "-";           // legacy: lTime

    [ObservableProperty]
    private string _sumaSeleccionTexto = "0";    // legacy: lSumSel

    // --- Columna ganadora navegable (groupBox4 "analisis resultados") ---
    [ObservableProperty]
    private string _columnaGanadora = "COL.GANADORA"; // legacy: tbCG

    [ObservableProperty]
    private string _ficheroGanadorasTexto = "-";      // legacy: lFGR

    [ObservableProperty]
    private string _indiceGanadoraTexto = "0";        // legacy: lbCGR (nrfCGR)

    [ObservableProperty]
    private string _puntuacionGanadoraTexto = "-";    // legacy: lbCG (nx2)

    // --- Resultados del Análisis (premios 10..14) ---
    [ObservableProperty]
    private string _premios14Texto = "0";    // legacy: lpr14

    [ObservableProperty]
    private string _premios13Texto = "0";    // legacy: lpr13

    [ObservableProperty]
    private string _premios12Texto = "0";    // legacy: lpr12

    [ObservableProperty]
    private string _premios11Texto = "0";    // legacy: lpr11

    [ObservableProperty]
    private string _premios10Texto = "0";    // legacy: lpr10

    public AnalizadorJPMViewModel()
    {
        // Valores por defecto del WinForms original (tbv0..tbv8).
        Valor11 = 0; Valor1X = 1; Valor12 = 2;
        ValorX1 = 1; ValorXX = 3; ValorX2 = 4;
        Valor21 = 2; Valor2X = 4; Valor22 = 5;

        for (int i = 0; i < 36; i++)
            Casillas.Add(new CasillaSumaViewModel(i));
    }

    /// <summary>
    /// Legacy: Iniciar() -> abre OpenFileDialog, lee columnas, las puntúa y clasifica
    /// en lbtab[0..35], actualiza conteos, total y tiempo.
    /// </summary>
    [RelayCommand]
    private void Iniciar()
    {
        // TODO[dominio]: portar AnalizadorJPM.Iniciar().
        //   - Pedir fichero de entrada (legacy: OpenFileDialog "ColumnasEntrada(*.txt)").
        //   - RecuperaValores(): leer Valor11..Valor22 (double->int, recorte [0,5]) y marcas[] de las casillas.
        //   - Por cada columna: s1n(), Puntuar() con vals[], detectar repetidas (BitArray repes),
        //     acumular en lbtab[36] y validas[].
        //   - Asignar(): volcar lbtab[i] en Casillas[i].ConteoTexto; actualizar TotalColumnasTexto y TiempoTexto.
        //   Requiere motor de dominio (no migrado a Free1X2.Domain todavía).
    }

    /// <summary>
    /// Legacy: Grabar() -> SaveFileDialog, recorre validas[] y escribe las columnas
    /// cuyo bucket está marcado (ck00..ck35).
    /// </summary>
    [RelayCommand]
    private void Grabar()
    {
        // TODO[dominio]: portar AnalizadorJPM.Grabar() + Grab1Col(n1s()).
        //   - Pedir fichero de salida (legacy: SaveFileDialog "ColumnasSalida(*.txt)").
        //   - Escribir n1s(nr) para cada columna cuyo bucket (validas[nr]) tenga Casillas[b].Seleccionada.
        //   - Actualizar FicheroSalida.
    }

    /// <summary>
    /// Legacy: SumSel() -> suma lbtab[i] de las casillas marcadas en lSumSel.
    /// </summary>
    [RelayCommand]
    private void Sumar()
    {
        // TODO[dominio]: portar AnalizadorJPM.SumSel().
        //   Sumar los conteos (lbtab[i]) de las Casillas con Seleccionada == true y volcar en SumaSeleccionTexto.
        //   Aquí no se calcula porque los conteos provienen del motor de dominio (Iniciar()).
    }

    /// <summary>
    /// Legacy: BCancelarClick -> salida = true (aborta el bucle de proceso).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: portar señal de cancelación (legacy: bool salida = true)
        //   para abortar el procesado largo de Iniciar()/Analizar().
    }

    /// <summary>
    /// Legacy: EntraCGsR() / BFGClick -> abre fichero de columnas ganadoras y carga la lista navegable.
    /// </summary>
    [RelayCommand]
    private void CargarGanadoras()
    {
        // TODO[dominio]: portar AnalizadorJPM.EntraCGsR().
        //   - OpenFileDialog "F.Ganadoras(*.txt)".
        //   - VerColumna() valida cada línea (14 chars de {1,2,X,x}).
        //   - Cargar colgsR[], fijar nrfCGR = total, ColumnaGanadora = última, IndiceGanadoraTexto, FicheroGanadorasTexto.
    }

    /// <summary>
    /// Legacy: GRMas() / BMasRClick -> avanza a la siguiente columna ganadora.
    /// </summary>
    [RelayCommand]
    private void SiguienteGanadora()
    {
        // TODO[dominio]: portar AnalizadorJPM.GRMas().
        //   Si nrfCGR < limcgsR: nrfCGR++, ColumnaGanadora = colgsR[nrfCGR-1], IndiceGanadoraTexto = nrfCGR.
    }

    /// <summary>
    /// Legacy: GRMenos() / BMenosRClick -> retrocede a la columna ganadora anterior.
    /// </summary>
    [RelayCommand]
    private void AnteriorGanadora()
    {
        // TODO[dominio]: portar AnalizadorJPM.GRMenos().
        //   Si nrfCGR > 1: nrfCGR--, ColumnaGanadora = colgsR[nrfCGR-1], IndiceGanadoraTexto = nrfCGR.
    }

    /// <summary>
    /// Legacy: Analizar() -> puntúa la columna ganadora y cuenta premios 14/13/12/11/10
    /// recorriendo variaciones (BuscaPremios recursivo).
    /// </summary>
    [RelayCommand]
    private void Analizar()
    {
        // TODO[dominio]: portar AnalizadorJPM.Analizar() + BuscaPremios().
        //   - RecuperaValores() (vals[] + marcas[]).
        //   - columna = ColumnaGanadora.Trim().ToUpper(); validar longitud >= 14.
        //   - PuntuacionGanadoraTexto = Puntuar(columna).
        //   - BuscaPremios(s1n(columna), 13): contar nprs[0..4] sobre buckets marcados.
        //   - Premios14/13/12/11/10 Texto = nprs[0], nprs[1], nprs[2]-nprs[1], nprs[3]-nprs[2], nprs[4]-nprs[3].
        //   Algoritmo intensivo: ejecutar fuera del hilo de UI en la versión final.
    }
}
