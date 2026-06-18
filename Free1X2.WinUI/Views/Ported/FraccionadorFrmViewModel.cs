using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

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

    // Columnas leídas en memoria (legacy: string[] columnas + numcols).
    private List<string> _columnas = new();

    /// <summary>
    /// Selecciona y carga el archivo de columnas (*.txt) de entrada.
    /// Legacy: bEntrada_Click -> Entrada().
    /// </summary>
    [RelayCommand]
    private async Task Entrada()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        string ruta = file.Path;
        var time0 = DateTime.Now;
        try
        {
            // Carga todas las columnas a memoria (legacy: bucle StreamReader -> columnas[]).
            var lista = await Task.Run(() =>
            {
                var cols = new List<string>();
                using var sr = new StreamReader(ruta);
                while (sr.Peek() > 0) cols.Add(sr.ReadLine() ?? string.Empty);
                return cols;
            });
            _columnas = lista;
            NombreArchivoEntrada = Path.GetFileName(ruta);
            TotalColumnas = _columnas.Count;
            EntradaCargada = true;
            TiempoTranscurrido = FormateaTiempo(DateTime.Now - time0);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al leer el archivo: " + ex.Message);
        }
    }

    /// <summary>
    /// Divide el archivo cargado en varios archivos de salida según el modo activo.
    /// Legacy: bFraccionar_Click -> Fraccionar().
    /// </summary>
    [RelayCommand]
    private async Task Fraccionar()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "BASE",
        };
        picker.FileTypeChoices.Add("Nombre BASE salida", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        // Legacy usaba GetFileNameWithoutExtension (nombre relativo). Aquí construimos la base
        // con el directorio + nombre sin extensión para que los archivos NN.txt vayan junto al
        // archivo elegido por el usuario.
        string dir = Path.GetDirectoryName(file.Path) ?? string.Empty;
        string baseNombre = Path.GetFileNameWithoutExtension(file.Path);
        string fileBase = Path.Combine(dir, baseNombre);

        bool porColumnas = PorColumnas;
        int cuantasPartes = (int)CuantasPartes;
        // Captura de cuotas para usar en el hilo de fondo.
        var cuotas = new int[20];
        for (int i = 0; i < CuotasColumnas.Count && i < 20; i++) cuotas[i] = (int)CuotasColumnas[i].Cantidad;
        var columnas = _columnas;

        var time0 = DateTime.Now;
        try
        {
            int numfiles = await Task.Run(() => porColumnas
                ? FracCols(fileBase, columnas, cuotas)
                : FracTrams(fileBase, columnas, cuantasPartes));
            ArchivosGenerados = numfiles;
            TiempoTranscurrido = FormateaTiempo(DateTime.Now - time0);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al fraccionar: " + ex.Message);
        }
    }

    /// <summary>
    /// FracCols() legacy: una cuota por archivo (1..20); para en la primera cuota == 0.
    /// Devuelve el nº de archivos generados.
    /// </summary>
    private static int FracCols(string fileBase, List<string> columnas, int[] cuotas)
    {
        int acum = 0;
        int numfiles = 0;
        int numcols = columnas.Count;
        for (int i = 0; i < 20; i++)
        {
            int qnts = cuotas[i];
            if (qnts == 0) break;
            string sfile = string.Format(fileBase + "{0:d2}.txt", i + 1);
            using var sw = new StreamWriter(sfile);
            int lim = acum + qnts;
            if (lim > numcols) lim = numcols;
            for (int nr = acum; nr < lim; nr++) sw.WriteLine(columnas[nr]);
            acum += qnts;
            numfiles = i + 1;
        }
        return numfiles;
    }

    /// <summary>
    /// FracTrams() legacy: 'qnts' partes iguales; el último archivo recibe el resto.
    /// Devuelve el nº de archivos generados.
    /// </summary>
    private static int FracTrams(string fileBase, List<string> columnas, int qnts)
    {
        if (qnts <= 0) return 0;
        int numcols = columnas.Count;
        int part = numcols / qnts;
        int acum = 0, lim = part;
        int numfiles = 0;
        for (numfiles = 0; numfiles < qnts; numfiles++)
        {
            string sfile = string.Format(fileBase + "{0:d2}.txt", numfiles + 1);
            using (var sw = new StreamWriter(sfile))
            {
                for (int nr = acum; nr < lim; nr++) sw.WriteLine(columnas[nr]);
            }
            acum += part;
            lim = acum + part;
            if (numcols - lim < part) lim = numcols;
        }
        return numfiles;
    }

    /// <summary>Formatea un lapso igual que veureelmeu() legacy ("hh:mm:ss.f" recortado a 10 chars).</summary>
    private static string FormateaTiempo(TimeSpan lapso)
    {
        string temp = lapso.ToString() + "0000000000";
        return temp.Substring(0, 10);
    }

    /// <summary>
    /// Cierra/regresa de la pantalla (no existe botón legacy explícito; el form
    /// se cerraba con la X). Provisto para navegación WinUI.
    /// </summary>
    [RelayCommand]
    private void Cerrar()
    {
        // Navegación WinUI (Frame.GoBack) es responsabilidad del host de la Page.
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
