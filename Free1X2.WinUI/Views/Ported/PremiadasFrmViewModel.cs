using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Análisis Premiadas" (legacy: PremiadasFrm).
///
/// Propósito del formulario legacy: a partir de un fichero de columnas ganadoras
/// (una combinación de 14 signos 1/X/2 por línea), recorre todas las variantes que
/// difieren en N signos y cuenta cuántas columnas del espacio total (3^14 = 4.782.969)
/// resultarían premiadas con 10/11/12/13 aciertos. Muestra las frecuencias resultantes,
/// permite grabar a fichero las columnas de una frecuencia seleccionada y analizar en
/// qué jornadas aparece dicha frecuencia.
///
/// El cálculo replica exactamente PremiadasFrm.Genera/Trasvasa/Examina/s1n/n1s del
/// WinForms original; sólo cambia la E/S (FileOpenPicker/FileSavePicker) y la salida
/// (colecciones observables en lugar de ListBox/Label).
/// </summary>
public partial class PremiadasFrmViewModel : ObservableObject
{
    // Espacio total de columnas (3^14). Legacy: PremiadasFrm.validas / bucles "nr<4782969".
    private const int EspacioColumnas = 4782969;

    // Potencias de 3 (legacy: PremiadasFrm.pot). Fija a 14 partidos como el original.
    private static readonly int[] Pot =
        { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323 };

    // Contador de premios por columna del espacio total (legacy: int[] validas).
    private int[] _validas = Array.Empty<int>();

    // Máximo nº de premios alcanzado por una columna (legacy: nmax).
    private int _nmax;

    // Ruta completa del último fichero seleccionado (legacy: faux; filein guarda sólo el nombre).
    private string _rutaFichero = string.Empty;

    /// <summary>
    /// Ruta/nombre del fichero de columnas ganadoras seleccionado
    /// (legacy: lFileIn.Text, asignado en SelFileIn() vía OpenFileDialog).
    /// </summary>
    [ObservableProperty]
    private string _ficheroGanadoras = "Seleccionar Previamente";

    /// <summary>
    /// Categoría de premio a estudiar: 13, 12, 11 o 10 aciertos
    /// (legacy: RadioButtons rb13/rb12/rb11/rb10 dentro de groupBox1 "Premios a estudiar";
    /// por defecto rb10.Checked = true). Se expone como índice del ComboBox.
    /// </summary>
    [ObservableProperty]
    private int _premioSeleccionadoIndex = 3;

    /// <summary>
    /// Índice de la frecuencia seleccionada en el ListView (legacy: lbPremis.SelectedIndex).
    /// La fija el code-behind de la Page al cambiar la selección, ya que la selección
    /// vive en el control de UI (mismo patrón que ColumnasPremiadasFrmPage).
    /// </summary>
    [ObservableProperty]
    private int _frecuenciaSeleccionadaIndex = -1;

    /// <summary>
    /// Opciones del ComboBox de premios (legacy: los 4 RadioButtons mutuamente excluyentes).
    /// </summary>
    public IReadOnlyList<string> OpcionesPremio { get; } = new List<string>
    {
        "13 aciertos",
        "12 aciertos",
        "11 aciertos",
        "10 aciertos",
    };

    /// <summary>
    /// Contador de combinaciones procesadas del fichero (legacy: lProc.Text, ctproc).
    /// String para enlazar directo a TextBlock.Text.
    /// </summary>
    [ObservableProperty]
    private string _procesadasTexto = "0";

    /// <summary>
    /// Tiempo transcurrido del último cálculo (legacy: lTime.Text, formato 00:00:00.0).
    /// String para enlazar directo a TextBlock.Text.
    /// </summary>
    [ObservableProperty]
    private string _tiempoTexto = "00:00:00.0";

    /// <summary>
    /// Listado de frecuencias resultantes (legacy: lbPremis, multiselección).
    /// Cada elemento es del tipo "N = M veces" (qdc[nr] = (nr+1) veces).
    /// </summary>
    public ObservableCollection<string> Frecuencias { get; } = new();

    /// <summary>
    /// Listado de jornadas en que aparece la frecuencia seleccionada
    /// (legacy: lbSecuencia, elementos "sem.J").
    /// </summary>
    public ObservableCollection<string> Jornadas { get; } = new();

    /// <summary>
    /// Selecciona el fichero de columnas ganadoras de entrada
    /// (legacy: PremiadasFrm.SelFileIn / BFileInClick).
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarFicheroAsync()
    {
        // Legacy SelFileIn(): OpenFileDialog filtro "Cols.Ganadoras(*.txt)|*.txt|...".
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        picker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSingleFileAsync();
        if (file is null) return;

        // Legacy: faux = ruta completa; filein/lFileIn.Text = sólo el nombre.
        _rutaFichero = file.Path;
        FicheroGanadoras = file.Name;
    }

    /// <summary>
    /// Lanza el cálculo de frecuencias de premios sobre el fichero
    /// (legacy: PremiadasFrm.Calcular / BCalcularClick + Genera + Trasvasa).
    /// </summary>
    [RelayCommand]
    private async Task CalcularAsync()
    {
        if (_rutaFichero.Length == 0 || !File.Exists(_rutaFichero))
        {
            AppServices.MostrarError("Selecciona primero un fichero de columnas ganadoras.");
            return;
        }

        // Categoría activa: 0=13, 1=12, 2=11, 3=10 (legacy rb13/rb12/rb11/rb10).
        int premio = PremioParaIndice(PremioSeleccionadoIndex);
        string ruta = _rutaFichero;

        var dt0 = DateTime.Now;
        Frecuencias.Clear();
        Jornadas.Clear();
        ProcesadasTexto = "0";

        // Cálculo pesado en hilo de fondo (legacy: bucle con Application.DoEvents()).
        var resultado = await Task.Run(() =>
        {
            // Legacy: int[] validas = new int[4782969]; ctproc=nmax=0; for(...) validas[nr]=0;
            var validas = new int[EspacioColumnas];
            _nmax = 0;
            int ctproc = 0;

            // Legacy: while(sr.Peek()>0) { idx=s1n(linea); ctproc++; Genera(idx); }
            foreach (string linea in File.ReadLines(ruta))
            {
                if (linea.Length < 14) continue;
                int idx = SignosANumero(linea);
                ctproc++;
                Genera(validas, idx, premio);
            }

            // Legacy Trasvasa(): qdc[n-1]++ para cada validas>0.
            var qdc = new int[_nmax];
            foreach (int n in validas)
            {
                if (n > 0) qdc[n - 1]++;
            }
            return (validas, qdc, ctproc);
        });

        _validas = resultado.validas;

        // Legacy Trasvasa(): lbPremis.Items.Add(String.Format("{0:d} = {1:d} veces", qdc[nr], nr+1)).
        for (int nr = 0; nr < resultado.qdc.Length; nr++)
        {
            Frecuencias.Add(string.Format("{0:d} = {1:d} veces", resultado.qdc[nr], nr + 1));
        }

        // Legacy veureelmeu(): lProc.Text = ctproc; lTime.Text = (dt9-dt0) truncado a 10 chars.
        ProcesadasTexto = resultado.ctproc.ToString();
        var dt9 = DateTime.Now;
        string temp = (dt9 - dt0) + "0000000000";
        TiempoTexto = temp.Substring(0, 10);
    }

    /// <summary>
    /// Graba a fichero las columnas correspondientes a la frecuencia seleccionada
    /// (legacy: PremiadasFrm.Grabar / BGrabarClick).
    /// </summary>
    [RelayCommand]
    private async Task GrabarAsync()
    {
        // Legacy: n = lbPremis.SelectedIndex + 1;
        if (FrecuenciaSeleccionadaIndex < 0)
        {
            AppServices.MostrarError("Selecciona una frecuencia en la lista.");
            return;
        }
        if (_validas.Length == 0)
        {
            AppServices.MostrarError("Calcula primero las frecuencias.");
            return;
        }

        int n = FrecuenciaSeleccionadaIndex + 1;

        // Legacy: SaveFileDialog filtro "Cols.Salida(*.txt)|*.txt|...".
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "columnas",
        };
        picker.FileTypeChoices.Add("Cols.Salida", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? file = await picker.PickSaveFileAsync();
        if (file is null) return;

        int[] validas = _validas;
        await Task.Run(() =>
        {
            // Legacy: for(nr=0..4782969) if(validas[nr]==n) sw.WriteLine(n1s(nr));
            using var sw = new StreamWriter(file.Path);
            for (int nr = 0; nr < validas.Length; nr++)
            {
                if (validas[nr] == n) sw.WriteLine(NumeroASignos(nr));
            }
        });

        AppServices.MostrarInfo($"Columnas grabadas en {file.Name}.");
    }

    /// <summary>
    /// Analiza en qué jornadas del fichero aparece la frecuencia seleccionada
    /// (legacy: PremiadasFrm.Analiza / BAnalizaClick + Examina).
    /// </summary>
    [RelayCommand]
    private async Task AnalizarAsync()
    {
        // Legacy: n = lbPremis.SelectedIndex + 1;
        if (FrecuenciaSeleccionadaIndex < 0)
        {
            AppServices.MostrarError("Selecciona una frecuencia en la lista.");
            return;
        }
        if (_validas.Length == 0)
        {
            AppServices.MostrarError("Calcula primero las frecuencias.");
            return;
        }
        if (_rutaFichero.Length == 0 || !File.Exists(_rutaFichero))
        {
            AppServices.MostrarError("Selecciona primero un fichero de columnas ganadoras.");
            return;
        }

        int n = FrecuenciaSeleccionadaIndex + 1;
        int premio = PremioParaIndice(PremioSeleccionadoIndex);
        string ruta = _rutaFichero;
        int[] validas = _validas;

        Jornadas.Clear();

        // Legacy Analiza(): por cada línea (jornada++), Examina recorre las variantes.
        var encontradas = await Task.Run(() =>
        {
            var lista = new List<string>();
            int jornada = 0;
            foreach (string linea in File.ReadLines(ruta))
            {
                if (linea.Length < 14) continue;
                int idx = SignosANumero(linea);
                jornada++;
                Examina(validas, idx, premio, n, jornada, lista);
            }
            return lista;
        });

        foreach (string sem in encontradas)
        {
            Jornadas.Add(sem);
        }
    }

    // ===== Núcleo de cálculo (réplica exacta de PremiadasFrm) =====

    // Legacy s1n: convierte 14 signos 1/X/2 a un índice en base 3.
    private static int SignosANumero(string ax)
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

    // Legacy n1s: convierte el índice base-3 de vuelta a la cadena de 14 signos.
    private static string NumeroASignos(int nx)
    {
        string ax = "";
        for (int nr = 0; nr < 14; nr++)
        {
            int nx2 = nx % 3; nx /= 3;
            if (nx2 == 1) ax = "1" + ax;
            else if (nx2 == 2) ax = "2" + ax;
            else ax = "X" + ax;
        }
        return ax;
    }

    // Legacy Genera(nsel): recorre las variantes que difieren en 1..4 signos y, según la
    // categoría activa (13/12/11/10), incrementa validas[col] llevando el máximo nmax.
    private void Genera(int[] validas, int nsel, int premio)
    {
        int n;
        for (int nr = 0; nr < 14; nr++)
        {
            int sign1 = (nsel / Pot[nr]) % 3;
            for (int z1 = 0; z1 < 3; z1++)
            {
                if (z1 == sign1) continue;
                int col1 = nsel + Pot[nr] * (z1 - sign1);
                if (premio == 13)
                {
                    n = validas[col1] + 1; if (n > _nmax) _nmax = n; validas[col1] = n;
                }
                for (int nr2 = nr + 1; nr2 < 14; nr2++)
                {
                    int sign2 = (col1 / Pot[nr2]) % 3;
                    for (int z2 = 0; z2 < 3; z2++)
                    {
                        if (z2 == sign2) continue;
                        int col2 = col1 + Pot[nr2] * (z2 - sign2);
                        if (premio == 12)
                        {
                            n = validas[col2] + 1; if (n > _nmax) _nmax = n; validas[col2] = n;
                        }
                        for (int nr3 = nr2 + 1; nr3 < 14; nr3++)
                        {
                            int sign3 = (col2 / Pot[nr3]) % 3;
                            for (int z3 = 0; z3 < 3; z3++)
                            {
                                if (z3 == sign3) continue;
                                int col3 = col2 + Pot[nr3] * (z3 - sign3);
                                if (premio == 11)
                                {
                                    n = validas[col3] + 1; if (n > _nmax) _nmax = n; validas[col3] = n;
                                }
                                for (int nr4 = nr3 + 1; nr4 < 14; nr4++)
                                {
                                    int sign4 = (col3 / Pot[nr4]) % 3;
                                    for (int z4 = 0; z4 < 3; z4++)
                                    {
                                        if (z4 == sign4) continue;
                                        int col4 = col3 + Pot[nr4] * (z4 - sign4);
                                        if (premio == 10)
                                        {
                                            n = validas[col4] + 1; if (n > _nmax) _nmax = n; validas[col4] = n;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Legacy Examina(nsel): misma estructura que Genera, pero si validas[col]==n añade
    // "sem."+jornada a la lista de resultados.
    private static void Examina(int[] validas, int nsel, int premio, int n, int jornada, List<string> resultado)
    {
        for (int nr = 0; nr < 14; nr++)
        {
            int sign1 = (nsel / Pot[nr]) % 3;
            for (int z1 = 0; z1 < 3; z1++)
            {
                if (z1 == sign1) continue;
                int col1 = nsel + Pot[nr] * (z1 - sign1);
                if (premio == 13)
                {
                    if (validas[col1] == n) resultado.Add("sem." + jornada);
                }
                for (int nr2 = nr + 1; nr2 < 14; nr2++)
                {
                    int sign2 = (col1 / Pot[nr2]) % 3;
                    for (int z2 = 0; z2 < 3; z2++)
                    {
                        if (z2 == sign2) continue;
                        int col2 = col1 + Pot[nr2] * (z2 - sign2);
                        if (premio == 12)
                        {
                            if (validas[col2] == n) resultado.Add("sem." + jornada);
                        }
                        for (int nr3 = nr2 + 1; nr3 < 14; nr3++)
                        {
                            int sign3 = (col2 / Pot[nr3]) % 3;
                            for (int z3 = 0; z3 < 3; z3++)
                            {
                                if (z3 == sign3) continue;
                                int col3 = col2 + Pot[nr3] * (z3 - sign3);
                                if (premio == 11)
                                {
                                    if (validas[col3] == n) resultado.Add("sem." + jornada);
                                }
                                for (int nr4 = nr3 + 1; nr4 < 14; nr4++)
                                {
                                    int sign4 = (col3 / Pot[nr4]) % 3;
                                    for (int z4 = 0; z4 < 3; z4++)
                                    {
                                        if (z4 == sign4) continue;
                                        int col4 = col3 + Pot[nr4] * (z4 - sign4);
                                        if (premio == 10)
                                        {
                                            if (validas[col4] == n) resultado.Add("sem." + jornada);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Mapea el índice del ComboBox (0..3) a la categoría de aciertos (13..10).
    private static int PremioParaIndice(int index) => index switch
    {
        0 => 13,
        1 => 12,
        2 => 11,
        _ => 10,
    };
}
