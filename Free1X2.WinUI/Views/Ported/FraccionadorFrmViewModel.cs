using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Fraccionador" (legacy: Free1X2.UI.FraccionadorFrm).
/// Carga un archivo de columnas de texto (*.txt) y lo divide en varios archivos
/// de salida numerados (BASE01.txt, BASE02.txt, …). Hay dos modos de fraccionar:
///   - "Por columnas": 20 cuotas independientes (tbcol01..tbcol20); cada archivo
///     recibe exactamente esa cantidad de columnas (legacy FraccionadorFrm.FracCols).
///   - "Por tramos": una cantidad de partes iguales (tbqnts); las columnas se
///     reparten equitativamente (legacy FraccionadorFrm.FracTrams).
/// Equivalente legacy: FraccionadorFrm.Entrada / Fraccionar.
/// </summary>
public partial class FraccionadorFrmViewModel : ObservableObject
{
    public FraccionadorFrmViewModel()
    {
        // 20 cuotas "Por columnas" (legacy tbcol01..tbcol20, todas con valor inicial 0).
        for (int i = 0; i < 20; i++)
        {
            CuotasColumnas.Add(new CuotaColumna(i + 1));
        }
    }

    // Modo de fraccionado seleccionado (legacy rbcols/radioButton2 dentro de groupBox1
    // "Fraccionar por"). true = "Por columnas" (rbcols.Checked por defecto),
    // false = "Por tramos".
    [ObservableProperty]
    private bool _porColumnas = true;

    // Habilita la tarjeta "Por columnas" (legacy gbcols.Enabled = rbcols.Checked).
    public bool ModoColumnasHabilitado => PorColumnas;

    // Habilita la tarjeta "Por tramos" (legacy gbtrams.Enabled = !rbcols.Checked).
    public bool ModoTramosHabilitado => !PorColumnas;

    partial void OnPorColumnasChanged(bool value)
    {
        OnPropertyChanged(nameof(ModoColumnasHabilitado));
        OnPropertyChanged(nameof(ModoTramosHabilitado));
    }

    // Lista de las 20 cuotas del modo "Por columnas".
    public ObservableCollection<CuotaColumna> CuotasColumnas { get; } = new();

    // Cantidad de partes iguales del modo "Por tramos" (legacy tbqnts, NumberBox -> double).
    [ObservableProperty]
    private double _cuantasPartes = 0;

    // Nombre del archivo de entrada cargado (legacy filein = Path.GetFileName(...)).
    [ObservableProperty]
    private string _nombreArchivoEntrada = "(ningún archivo)";

    // Total de columnas leídas del archivo de entrada (legacy lcols.Text = numcols).
    [ObservableProperty]
    private int _totalColumnas = 0;

    // Texto enlazable del total de columnas (regla anti-crash: no bindear int directo a Text).
    public string TotalColumnasTexto => TotalColumnas.ToString();

    partial void OnTotalColumnasChanged(int value) => OnPropertyChanged(nameof(TotalColumnasTexto));

    // Número de archivos de salida generados (legacy lfiles.Text = numfiles).
    [ObservableProperty]
    private int _archivosGenerados = 0;

    // Texto enlazable de archivos generados.
    public string ArchivosGeneradosTexto => ArchivosGenerados.ToString();

    partial void OnArchivosGeneradosChanged(int value) => OnPropertyChanged(nameof(ArchivosGeneradosTexto));

    // Tiempo transcurrido del último proceso (legacy ltime.Text = "hh:mm:ss.f").
    [ObservableProperty]
    private string _tiempoTranscurrido = "00:00:00.0";

    // Indica si ya hay un archivo de entrada cargado: habilita "Fraccionar"
    // (legacy bFraccionar.Enabled = true tras Entrada()).
    [ObservableProperty]
    private bool _entradaCargada = false;

    /// <summary>
    /// Selecciona y carga el archivo de columnas (*.txt) de entrada.
    /// Legacy: bEntrada_Click -> Entrada().
    /// </summary>
    [RelayCommand]
    private void Entrada()
    {
        // TODO[dominio]: abrir selector (FileOpenPicker) filtro "ColumnasEntrada (*.txt)".
        //   Legacy FraccionadorFrm.Entrada():
        //     - NombreArchivoEntrada = Path.GetFileName(ruta)
        //     - Leer todas las líneas a 'columnas[]' contando numcols
        //       (legacy usa StreamReader + Application.DoEvents en bucle).
        //     - TotalColumnas = numcols;  (legacy lcols.Text = numcols)
        //     - EntradaCargada = true;    (habilita Fraccionar)
        //     - Medir tiempo (time0/time9) -> TiempoTranscurrido.
    }

    /// <summary>
    /// Divide el archivo cargado en varios archivos de salida según el modo activo.
    /// Legacy: bFraccionar_Click -> Fraccionar().
    /// </summary>
    [RelayCommand]
    private void Fraccionar()
    {
        // TODO[dominio]: abrir selector (FileSavePicker) "Nombre BASE salida (*.txt)".
        //   Legacy FraccionadorFrm.Fraccionar():
        //     - fileout = Path.GetFileNameWithoutExtension(ruta)
        //     - si PorColumnas -> FracCols(): para cada cuota tbcolNN (1..20) que sea > 0,
        //         escribir esa cantidad de columnas consecutivas en fileoutNN.txt;
        //         parar al encontrar la primera cuota == 0.
        //     - si !PorColumnas -> FracTrams(): part = numcols / CuantasPartes;
        //         escribir 'CuantasPartes' archivos con 'part' columnas cada uno
        //         (el último recibe el resto).
        //     - Tras cada archivo: ArchivosGenerados = (numfiles+1).
        //     - Medir tiempo (time0/time9) -> TiempoTranscurrido.
    }

    /// <summary>
    /// Cierra/regresa de la pantalla (no existe botón legacy explícito; el form
    /// se cerraba con la X). Provisto para navegación WinUI.
    /// </summary>
    [RelayCommand]
    private void Cerrar()
    {
        // TODO[dominio]: navegación WinUI — Frame.GoBack() o cerrar el host contenedor.
    }
}

/// <summary>
/// Una cuota del modo "Por columnas": cuántas columnas van al archivo de salida N.
/// Legacy: pares (labelNN, tbcolNN) dentro de gbcols. El valor es double porque la
/// UI usa NumberBox (regla anti-crash 7).
/// </summary>
public partial class CuotaColumna : ObservableObject
{
    public CuotaColumna(int indice)
    {
        Indice = indice;
        Etiqueta = indice.ToString();
    }

    // Número de archivo de salida (1..20). Mostrado como etiqueta a la izquierda.
    public int Indice { get; }

    // Texto enlazable de la etiqueta (regla anti-crash: no bindear int directo a Text).
    public string Etiqueta { get; }

    // Cantidad de columnas para este archivo (legacy tbcolNN.Text, default "0").
    [ObservableProperty]
    private double _cantidad = 0;
}
