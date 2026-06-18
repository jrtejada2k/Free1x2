// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Microsoft.UI.Dispatching;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Representa la selección de un partido (1..14) en el análisis de grupos en combinación.
/// Cada partido puede fijarse a un signo (1/X/2), marcarse como integrante del grupo (G)
/// o ignorarse (-). Legacy: cada TextBox tb01..tb14 de AnaCombi.
/// </summary>
public partial class AnaCombiPartido : ObservableObject
{
    public int Numero { get; }

    /// <summary>Etiqueta visible del partido ("1".."14"). Evita bindear int a TextBlock.Text.</summary>
    public string NumeroTexto => Numero.ToString();

    /// <summary>Opciones del ComboBox del partido (ItemsSource desde el propio item, no items inline).</summary>
    public IReadOnlyList<string> OpcionesSigno { get; } = new[] { "-", "1", "X", "2", "G" };

    [ObservableProperty]
    private string _seleccion = "-";

    public AnaCombiPartido(int numero)
    {
        Numero = numero;
    }
}

/// <summary>
/// Una fila de resultado del cálculo: patrón de grupo y número de columnas que encajan.
/// Legacy: tabla "Resultados" (columnas "G" string y "C" int) del DataGrid de AnaCombi.
/// </summary>
public partial class AnaCombiResultado : ObservableObject
{
    public string Grupo { get; }
    public int Columnas { get; }

    /// <summary>Texto del conteo (string para no bindear int a TextBlock.Text).</summary>
    public string ColumnasTexto => Columnas.ToString();

    [ObservableProperty]
    private bool _seleccionado;

    public AnaCombiResultado(string grupo, int columnas)
    {
        Grupo = grupo;
        Columnas = columnas;
    }
}

/// <summary>
/// ViewModel para la pantalla "Análisis de grupos en combinación" (legacy: AnaCombi).
/// Permite definir un patrón/grupo sobre los 14 partidos, cargar un fichero de columnas,
/// contabilizar cuántas encajan en cada combinación de grupo y guardar la selección.
///
/// Algoritmo autocontenido (no depende del motor de Free1X2.Domain): se porta 1:1 desde
/// AnaCombi (RecuperaGrupo/Calcular/Contabiliza/Mostraresuls/Normaliza/s14n/n14s/Grabar),
/// ejecutando el bucle intensivo (3^14 = 4.782.969 combinaciones) en Task.Run.
/// </summary>
public partial class AnaCombiViewModel : ObservableObject
{
    private const int TotalCombinaciones = 4782969; // 3^14

    /// <summary>Los 14 partidos editables.</summary>
    public ObservableCollection<AnaCombiPartido> Partidos { get; } = new();

    /// <summary>Filas de resultado producidas por el cálculo.</summary>
    public ObservableCollection<AnaCombiResultado> Resultados { get; } = new();

    [ObservableProperty]
    private double _fallosAdmitidos;

    [ObservableProperty]
    private string _ficheroEntrada = "Fichero a procesar";

    [ObservableProperty]
    private string _procesadasTexto = "Procesadas";

    [ObservableProperty]
    private string _tiempoTexto = "Tiempo";

    [ObservableProperty]
    private bool _calculando;

    /// <summary>Visibilidad del botón Grabar: solo disponible tras un cálculo con resultados.</summary>
    [ObservableProperty]
    private bool _puedeGrabar;

    // Estado del algoritmo (legacy: campos de AnaCombi).
    private string _rutaFichero = string.Empty;     // ruta completa (legacy: filein, relativo a StartupPath)
    private int _ctgrup;                             // legacy: ctgrup (nº de partidos marcados como G)
    private string _patron = string.Empty;          // legacy: patron (1/X/2/'-' por partido)
    private readonly int[] _grup = new int[14];      // legacy: grup[] (índices de partidos en G)
    private int _admfal;                            // legacy: admfal (fallos admitidos)
    private volatile bool _salida;                  // legacy: salida (bandera de cancelación)

    public AnaCombiViewModel()
    {
        for (int i = 1; i <= 14; i++)
        {
            Partidos.Add(new AnaCombiPartido(i));
        }
    }

    /// <summary>
    /// Restablece los 14 partidos al estado neutro ("-").
    /// Legacy: AnaCombi.BLimpClick -> tbNN.Text = "-".
    /// </summary>
    [RelayCommand]
    private void Limpiar()
    {
        foreach (var p in Partidos)
        {
            p.Seleccion = "-";
        }
    }

    /// <summary>
    /// Abre el diálogo de selección del fichero de columnas de entrada.
    /// Legacy: AnaCombi.EntradaFichero (OpenFileDialog *.txt).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarFichero()
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;
        _rutaFichero = file.Path;
        FicheroEntrada = Path.GetFileName(file.Path);
    }

    /// <summary>
    /// Ejecuta el conteo de columnas por combinación de grupo.
    /// Legacy: AnaCombi.Calcular / RecuperaGrupo / Contabiliza / Mostraresuls.
    /// </summary>
    [RelayCommand]
    private async Task Calcular()
    {
        if (Calculando) return;

        RecuperaGrupo();
        if (_ctgrup == 0)
        {
            AppServices.MostrarError("Falta grupo");
            return;
        }
        if (string.IsNullOrEmpty(_rutaFichero) || !File.Exists(_rutaFichero))
        {
            AppServices.MostrarError("Falta fichero");
            return;
        }

        Calculando = true;
        _salida = false;
        PuedeGrabar = false;
        Resultados.Clear();
        ProcesadasTexto = "0";
        TiempoTexto = "0";

        DispatcherQueue dispatcher = AppServices.UiDispatcher!;
        DateTime dt0 = DateTime.Now;
        string ruta = _rutaFichero;

        try
        {
            List<AnaCombiResultado> resultados = await Task.Run(() => EjecutarCalculo(ruta, dispatcher, dt0));

            foreach (var r in resultados)
            {
                Resultados.Add(r);
            }
            TiempoTexto = FormatearTiempo(DateTime.Now - dt0);
            PuedeGrabar = Resultados.Count > 0;
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al calcular: " + ex.Message);
        }
        finally
        {
            Calculando = false;
        }
    }

    // Núcleo del cálculo (legacy: Calcular + Contabiliza + Mostraresuls), fuera del hilo de UI.
    private List<AnaCombiResultado> EjecutarCalculo(string ruta, DispatcherQueue dispatcher, DateTime dt0)
    {
        int ctproc = 0;
        var comptes = new int[TotalCombinaciones];
        var valides = new int[TotalCombinaciones];
        for (int nr = 0; nr < TotalCombinaciones; nr++)
        {
            comptes[nr] = -1;
            valides[nr] = -1;
        }

        // Inicializa los grupos posibles (legacy: bucle inicial de Calcular).
        string tmp = N14S(0);
        tmp = tmp.Substring(14 - _ctgrup, _ctgrup) + "11111111111111";
        tmp = tmp.Substring(0, 14);
        int idx0 = S14N(tmp);
        comptes[idx0] = 0;
        for (int nr = 1; nr < TotalCombinaciones; nr++)
        {
            tmp = N14S(nr);
            tmp = tmp.Substring(14 - _ctgrup, _ctgrup) + "11111111111111";
            tmp = tmp.Substring(0, 14);
            int idxn = S14N(tmp);
            if (idx0 == idxn) break;
            comptes[idxn] = 0;
        }

        // Procesa el fichero de entrada (legacy: while sr.Peek()>0).
        using (var sr = new StreamReader(ruta))
        {
            string? linea;
            while ((linea = sr.ReadLine()) != null)
            {
                if (_salida) break;
                string columna = Normaliza(linea);
                ctproc++;
                if (columna.Length < 14)
                {
                    throw new InvalidOperationException("Columna errónea = " + ctproc);
                }
                int ctfal = 0;
                for (int nr = 0; nr < 14; nr++)
                {
                    if (_patron[nr] == '-') continue;
                    if (columna[nr] != _patron[nr]) ctfal++;
                }
                if (ctfal <= _admfal) Contabiliza(columna, comptes, valides);

                if ((ctproc & 0x3FF) == 0)
                {
                    int procActual = ctproc;
                    DateTime ahora = DateTime.Now;
                    dispatcher.TryEnqueue(() =>
                    {
                        ProcesadasTexto = procActual.ToString();
                        TiempoTexto = FormatearTiempo(ahora - dt0);
                    });
                }
            }
        }

        int procFinal = ctproc;
        dispatcher.TryEnqueue(() => ProcesadasTexto = procFinal.ToString());

        // Mostraresuls(): genera las filas de resultado (legacy: comptes[nr] >= 0).
        var resultados = new List<AnaCombiResultado>();
        for (int nr = 0; nr < TotalCombinaciones; nr++)
        {
            int ncols = comptes[nr];
            if (ncols >= 0)
            {
                string g = N14S(nr).Substring(0, _ctgrup);
                resultados.Add(new AnaCombiResultado(g, ncols));
            }
        }
        return resultados;
    }

    // legacy: Contabiliza(string columna)
    private void Contabiliza(string columna, int[] comptes, int[] valides)
    {
        string xcol = "";
        for (int nr = 0; nr < _ctgrup; nr++)
        {
            xcol += columna[_grup[nr]];
        }
        for (int nr = _ctgrup; nr < 14; nr++) xcol += '1';
        int idxg = S14N(xcol);
        comptes[idxg]++;
        int idxc = S14N(columna);
        valides[idxc] = idxg;
    }

    /// <summary>
    /// Cancela un cálculo en curso.
    /// Legacy: AnaCombi.BCancelarClick -> salida = true.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        _salida = true;
    }

    /// <summary>
    /// Guarda en un fichero de texto las columnas de los grupos seleccionados.
    /// Legacy: AnaCombi.Grabar (SaveFileDialog + StreamWriter sobre filas marcadas del grid).
    /// </summary>
    [RelayCommand]
    private async Task Grabar()
    {
        if (Resultados.Count == 0) return;
        if (string.IsNullOrEmpty(_rutaFichero) || !File.Exists(_rutaFichero))
        {
            AppServices.MostrarError("Falta fichero");
            return;
        }

        // Conjunto de grupos marcados para grabar (legacy: filas seleccionadas del DataGrid).
        var gruposSeleccionados = new HashSet<string>();
        foreach (var r in Resultados)
        {
            if (r.Seleccionado) gruposSeleccionados.Add(r.Grupo);
        }
        if (gruposSeleccionados.Count == 0)
        {
            AppServices.MostrarInfo("No hay ningún grupo marcado para grabar.");
            return;
        }

        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Resultados",
        };
        picker.FileTypeChoices.Add("Resultados", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file == null) return;

        string rutaEntrada = _rutaFichero;
        string rutaSalida = file.Path;
        int ctgrup = _ctgrup;

        try
        {
            // Legacy Grabar(): por cada columna del fichero de entrada, calcula su grupo (n14s recortado
            // a ctgrup) y, si ese grupo está marcado, escribe la columna original.
            await Task.Run(() =>
            {
                using var sr = new StreamReader(rutaEntrada);
                using var sw = new StreamWriter(rutaSalida);
                string? linea;
                while ((linea = sr.ReadLine()) != null)
                {
                    string columna = Normaliza(linea);
                    if (columna.Length < 14) continue;
                    string grupoColumna = ObtenerGrupoColumna(columna, ctgrup);
                    if (gruposSeleccionados.Contains(grupoColumna))
                    {
                        sw.WriteLine(columna);
                    }
                }
            });
            AppServices.MostrarInfo("Resultados grabados en " + Path.GetFileName(rutaSalida));
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudieron grabar los resultados: " + ex.Message);
        }
    }

    // Calcula el grupo (concatenación de los signos de los partidos marcados como G) de una columna.
    private string ObtenerGrupoColumna(string columna, int ctgrup)
    {
        string xcol = "";
        for (int nr = 0; nr < ctgrup; nr++)
        {
            xcol += columna[_grup[nr]];
        }
        return xcol;
    }

    // legacy: RecuperaGrupo() — construye patron[] y grup[] a partir de la selección de los 14 partidos.
    private void RecuperaGrupo()
    {
        _ctgrup = 0;
        _patron = "";
        for (int i = 0; i < 14; i++)
        {
            string ch = Partidos[i].Seleccion;
            if (ch == "1" || ch == "X" || ch == "2") _patron += ch;
            else _patron += '-';
            if (ch == "G")
            {
                _grup[_ctgrup] = i;
                _ctgrup++;
            }
        }
        _admfal = (int)FallosAdmitidos;
    }

    // legacy: Normaliza(string columna) — deja sólo los caracteres 1/2/X en mayúsculas.
    private static string Normaliza(string columna)
    {
        const string chval = "12X";
        columna = columna.ToUpper();
        string xcol = "";
        for (int nr = 0; nr < columna.Length; nr++)
        {
            char ch = columna[nr];
            if (chval.IndexOf(ch) >= 0) xcol += ch;
        }
        return xcol;
    }

    // legacy: s14n(string) — convierte una columna de 14 signos a su índice base-3.
    private static int S14N(string ax)
    {
        int nx = 0;
        for (int nr = 0; nr < 14; nr++)
        {
            nx *= 3;
            char ch = ax[nr];
            if (ch == '1') nx += 1;
            else if (ch == '2') nx += 2;
        }
        return nx;
    }

    // legacy: n14s(int) — convierte un índice base-3 a su columna de 14 signos.
    private static string N14S(int nx)
    {
        string ax = "";
        for (int nr = 0; nr < 14; nr++)
        {
            int nx2 = nx % 3;
            nx /= 3;
            if (nx2 == 1) ax = "1" + ax;
            else if (nx2 == 2) ax = "2" + ax;
            else ax = "X" + ax;
        }
        return ax;
    }

    // legacy: veureelmeu() — formatea el tiempo transcurrido a 10 caracteres.
    private static string FormatearTiempo(TimeSpan transcurrido)
    {
        string temp = transcurrido + "0000000000";
        return temp.Substring(0, 10);
    }
}
