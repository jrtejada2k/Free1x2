using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del port WinUI 3 del WinForms <c>SelectorMS</c> ("Selector MarioSan").
///
/// Propósito (legacy): leer un fichero de columnas (.txt), calcular para cada columna
/// cuántas columnas del propio fichero están a "aspecto" 13, 12 u 11 (distancia de
/// signos), agruparlas en una tabla de distribución (Q = nº de equivalentes, C = cuántas
/// columnas caen en ese grupo, P14..P10 = premios al analizar contra una columna ganadora),
/// permitir seleccionar grupos y sumar/grabar las columnas seleccionadas. Incluye una
/// sección de análisis contra un fichero de "Ganadoras" navegable.
///
/// Toda la lógica de dominio (s2n/n2s, calcular11/12/13, Comparador2, neq, ArchivoColumnasTexto,
/// OpenFileDialog/SaveFileDialog) queda marcada como TODO.
/// </summary>
public partial class SelectorMSViewModel : ObservableObject
{
    // ===== Aspecto a calcular (lbasp + bMas/bMenos, rango 11..13) =====
    // Regla anti-crash 3: ComboBox con SelectedItem usa ItemsSource desde el VM.
    public IReadOnlyList<string> OpcionesAspecto { get; } = new[] { "11", "12", "13" };

    // Selección actual del aspecto (lbasp.Text). Por defecto "13" como en el legacy.
    [ObservableProperty]
    private string _aspectoSeleccionado = "13";

    // ===== Fichero de columnas de entrada (lFileIn) =====
    [ObservableProperty] private string _ficheroEntrada = "";

    // ===== Tabla de distribución (dataGrid1 "Distribución") =====
    // Cada fila: Q (índice de grupo), C (nº columnas), P14..P10 (premios tras analizar).
    public ObservableCollection<FilaDistribucion> Distribucion { get; } = new();

    // Índices seleccionados en la tabla (dataGrid1.IsSelected). El legacy permite
    // multiselección; aquí se expone el índice simple para enlazar el ListView.
    [ObservableProperty]
    private int _filaSeleccionada = -1;

    // ===== Estadísticas (labels del form) =====
    // Regla anti-crash 2: numéricos expuestos como string.
    [ObservableProperty] private string _totalColumnas = "0";   // lCol
    [ObservableProperty] private string _tiempo = " ";          // lTime
    [ObservableProperty] private string _sumaSeleccion = "0";   // lSumSel
    [ObservableProperty] private string _ficheroSalida = "";    // lFileOut

    // ===== Sección "Análisis Resultados" (groupBox3) =====
    [ObservableProperty] private string _ficheroGanadoras = "Fichero Ganadoras"; // lFGR
    [ObservableProperty] private string _columnaGanadora = "";                   // tbColR (max 14)
    [ObservableProperty] private string _indiceGanadora = "0";                   // lbCGR

    // bAnalizar.Enabled: sólo activo cuando se ha cargado un fichero de ganadoras.
    [ObservableProperty]
    private bool _puedeAnalizar;

    [RelayCommand]
    private void AspectoMas()
    {
        // lbasp.Text en rango 11..13 (AspMas del legacy: tope 13).
        if (int.TryParse(AspectoSeleccionado, out var n) && n < 13)
            AspectoSeleccionado = (n + 1).ToString();
    }

    [RelayCommand]
    private void AspectoMenos()
    {
        // AspMenos del legacy: suelo 11.
        if (int.TryParse(AspectoSeleccionado, out var n) && n > 11)
            AspectoSeleccionado = (n - 1).ToString();
    }

    [RelayCommand]
    private void Iniciar()
    {
        // TODO: Dominio legacy — SelectorMS.Iniciar():
        //   1. OpenFileDialog filtro "Columnas(*.txt)|*.txt".
        //   2. Leer columnas con IArchivoColumnas = new ArchivoColumnasTexto(ruta);
        //      por cada columna: nx = s2n(columna); marcar existe[nx]; almacenar en tabla.
        //   3. Según AspectoSeleccionado (13/12/11) llamar calcular13()/calcular12()/calcular11()
        //      (cuentan vecinos a distancia 1/2/3 signos que existan en el fichero).
        //   4. Acumular lbtab[] = nº de columnas por valor de equivalentes y poblar
        //      Distribucion (InicializaFuenteDatos).
        //   5. Actualizar TotalColumnas (conta) y Tiempo (DateTime.Now - time0).
        //   Mientras corre, un DispatcherTimer refresca TotalColumnas/Tiempo (CalculoColumnas).
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: Dominio legacy — BCancelarClick: salida = true (aborta el bucle de cálculo).
    }

    [RelayCommand]
    private void Sumar()
    {
        // TODO: Dominio legacy — SumSel(): suma la columna C (nº de cols) de todas las
        //   filas seleccionadas en la tabla y la vuelca en SumaSeleccion (lSumSel).
    }

    [RelayCommand]
    private void Grabar()
    {
        // TODO: Dominio legacy — Grabar():
        //   SaveFileDialog filtro "Columnas(*.txt)|*.txt"; IArchivoColumnas w = new ArchivoColumnasTexto(ruta);
        //   por cada columna cuyo grupo (tabla[nr,1]) esté seleccionado en la tabla,
        //   w.GuardarCols(n2s(tabla[nr,0])); al terminar w.Cerrar() y mostrar FicheroSalida.
    }

    [RelayCommand]
    private void CargarGanadoras()
    {
        // TODO: Dominio legacy — EntraCGsR():
        //   OpenFileDialog filtro "F.Ganadoras(*.txt)|*.txt"; leer líneas (>=14 chars) a colgsR[].
        //   FicheroGanadoras = nombre; IndiceGanadora = nº total; ColumnaGanadora = última col;
        //   PuedeAnalizar = true.
        PuedeAnalizar = true;
    }

    [RelayCommand]
    private void GanadoraMas()
    {
        // TODO: Dominio legacy — GRMas(): avanza nrfCGR; actualiza IndiceGanadora y ColumnaGanadora.
    }

    [RelayCommand]
    private void GanadoraMenos()
    {
        // TODO: Dominio legacy — GRMenos(): retrocede nrfCGR; actualiza IndiceGanadora y ColumnaGanadora.
    }

    [RelayCommand]
    private void Analizar()
    {
        // TODO: Dominio legacy — Analizar():
        //   Requiere fichero cargado (conta>0). Reinicia resuls[]; Comparador2() (tabla de
        //   coincidencias de 7 signos); ncg = s2n(ColumnaGanadora); por cada columna calcula
        //   na = neq(nx) (aciertos vs ganadora) y si na>9 incrementa resuls[grupo,14-na].
        //   Repuebla Distribucion (P14..P10) vía InicializaFuenteDatos.
    }
}

/// <summary>
/// Fila de la tabla de distribución (dataGrid1 "Resultados"/"Distribución" del legacy).
/// Q = índice de grupo; C = nº de columnas; P14..P10 = premios tras analizar.
/// Regla anti-crash 2: todos los valores se exponen como string para enlazar a TextBlock.Text.
/// </summary>
public sealed class FilaDistribucion
{
    public string Q { get; init; } = "";
    public string C { get; init; } = "";
    public string P14 { get; init; } = "";
    public string P13 { get; init; } = "";
    public string P12 { get; init; } = "";
    public string P11 { get; init; } = "";
    public string P10 { get; init; } = "";
}
