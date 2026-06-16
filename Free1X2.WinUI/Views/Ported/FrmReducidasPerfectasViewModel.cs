using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de un partido de la tabla de pronóstico/base del form legacy "FrmReducidasPerfectas".
///
/// En el WinForms cada partido tiene:
///   - Tres labels de Pronóstico (lblNewBaseN1 / lblNewBaseNX / lblNewBaseN2): se activan/desactivan
///     con clic (campo legacy <c>bool[15,3] Pronosticos</c>).
///   - Un label de columna Base (lblBaseN / label3) que cicla 1 → X → 2 con cada clic
///     (campo legacy <c>int[15] SignosBase</c>, 0=1, 1=X, 2=2).
///
/// Todas las propiedades expuestas a la UI son <c>string</c>/<c>bool</c> sobre controles concretos
/// (ToggleButton.IsChecked, TextBlock.Text) para cumplir las reglas anti-crash del XamlCompiler.
/// </summary>
public partial class PartidoReducidaFila : ObservableObject
{
    /// <summary>Índice de partido 0..14 (equivale al Tag del label legacy).</summary>
    public int Indice { get; }

    /// <summary>Etiqueta de fila ("1".."15") como string para bindear a TextBlock.Text (regla anti-crash 2).</summary>
    public string NumeroTexto { get; }

    public PartidoReducidaFila(int indice)
    {
        Indice = indice;
        NumeroTexto = (indice + 1).ToString();
    }

    // ---- Pronóstico: 3 signos seleccionables (Pronosticos[i,0..2]) ----

    [ObservableProperty]
    private bool _pron1;

    [ObservableProperty]
    private bool _pronX;

    [ObservableProperty]
    private bool _pron2;

    // ---- Columna base: signo único 1/X/2 que cicla (SignosBase[i]) ----

    // Texto visible del signo base ("1" | "X" | "2").
    [ObservableProperty]
    private string _baseTexto = "1";

    // Valor interno del signo base (0=1, 1=X, 2=2).
    public int BaseSigno { get; private set; }

    /// <summary>Cicla el signo base 1 → X → 2 → 1 (legacy GenericBaseLabel_Click).</summary>
    [RelayCommand]
    private void CiclarBase()
    {
        BaseSigno = (BaseSigno + 1) % 3;
        BaseTexto = BaseSigno switch
        {
            0 => "1",
            1 => "X",
            _ => "2",
        };
    }
}

/// <summary>
/// ViewModel del formulario legacy WinForms "FrmReducidasPerfectas" (Reducciones Perfectas).
///
/// Propósito: a partir de una columna BASE (un signo 1/X/2 por partido) y de un PRONÓSTICO
/// (qué signos juega cada partido: simple, doble o triple), genera una "reducción perfecta"
/// — un conjunto de columnas calculadas mediante matrices de reducción predefinidas — y las
/// graba en un archivo de texto. El método está descrito por "Fortuna" en foro1x2.
///
/// Reducciones contempladas:
///    4 TRIPLES → reducidos al 13   (matriz M4TR13)
///   13 TRIPLES → reducidos al 13   (matriz M13TR13)
///   11 TRIPLES → reducidos al 12   (matriz M11TR12)
///    7 DOBLES  → reducidos al 13   (matriz M7DR13)
///   15 DOBLES  → reducidos al 13   (matriz M15DR13)
///
/// La lógica de generación está portada fielmente del WinForms (btGenerar_Click /
/// GeneraReduccion / ContarDoblesYtriples / TestSignos / TestSignosDobles); es un flujo
/// archivo→archivo autocontenido que sólo escribe columnas con StreamWriter.
/// </summary>
public partial class FrmReducidasPerfectasViewModel : ObservableObject
{
    // Matrices de reducción del WinForms (FrmReducidasPerfectas.cs líneas 108-112).
    private static readonly byte[,] M4TR13 = { { 1, 1 }, { 1, 2 } };
    private static readonly byte[,] M13TR13 =
    {
        { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 },
        { 1, 1, 0, 0, 1, 1, 1, 2, 2, 2 },
        { 1, 2, 1, 2, 0, 1, 2, 0, 1, 2 },
    };
    private static readonly byte[,] M7DR13 = { { 0, 1, 1, 1 }, { 1, 0, 1, 1 }, { 1, 1, 0, 1 } };
    private static readonly byte[,] M11TR12 =
    {
        { 1, 0, 1, 2, 2, 2 },
        { 1, 1, 0, 1, 1, 2 },
        { 1, 1, 2, 0, 2, 1 },
        { 1, 2, 1, 1, 0, 1 },
        { 1, 2, 2, 2, 1, 0 },
    };
    private static readonly byte[,] M15DR13 =
    {
        { 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1 },
        { 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1 },
        { 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0 },
        { 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0 },
    };

    // Estado intermedio del cálculo (legacy: bool[] Involucrados).
    private readonly bool[] _involucrados = new bool[15];

    /// <summary>15 filas de partido (pronóstico + signo base).</summary>
    public IReadOnlyList<PartidoReducidaFila> Partidos { get; }

    public FrmReducidasPerfectasViewModel()
    {
        var lista = new List<PartidoReducidaFila>(15);
        for (int i = 0; i < 15; i++)
            lista.Add(new PartidoReducidaFila(i));
        Partidos = lista;
    }

    // ---- Número de partidos de la columna (txLongColumna, por defecto 15) ----
    // NumberBox.Value es double (regla anti-crash 7).
    [ObservableProperty]
    private double _longitudColumna = 15;

    // ---- Archivo de salida (nombreArchivo / txNombreArchivo del form legacy) ----
    [ObservableProperty]
    private string _nombreArchivo = "(Sin archivo seleccionado)";

    // Ruta completa del archivo de salida (campo legacy nombreArchivo).
    private string _rutaArchivo = "";

    // ---- Barra de estado (statusBarPanel1 / statusBarPanel2 del form legacy) ----
    [ObservableProperty]
    private string _estado = "Indique el archivo de salida y pulse Generar";

    [ObservableProperty]
    private string _estadoSalida = "";

    /// <summary>
    /// Selecciona el archivo de salida (botón btSeleccionarFichero del form legacy).
    /// Legacy: SaveFileDialog (carpeta Columnas, *.txt) -> nombreArchivo + Path.GetFileName.
    /// </summary>
    [RelayCommand]
    private async Task SeleccionarArchivo()
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            DefaultFileExtension = ".txt",
            SuggestedFileName = "reducida",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        picker.FileTypeChoices.Add("Todos los archivos", new List<string> { "." });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        StorageFile? archivo = await picker.PickSaveFileAsync();
        if (archivo is null)
        {
            return;
        }

        _rutaArchivo = archivo.Path;
        NombreArchivo = archivo.Name;
    }

    /// <summary>
    /// Genera la reducción perfecta y la graba en el archivo de salida (botón btGenerar del form legacy).
    /// Réplica de FrmReducidasPerfectas.btGenerar_Click (FrmReducidasPerfectas.cs línea 1432).
    /// </summary>
    [RelayCommand]
    private void Generar()
    {
        EstadoSalida = "";
        if (_rutaArchivo == "")
        {
            Estado = "Es necesario indicar el archivo de salida";
            return;
        }

        if (!TestSignos())
        {
            Estado = "La columna base no está incluida en el pronóstico";
            return;
        }

        ContarDoblesYtriples(out int numDobles, out int numTriples);

        if (numTriples != 0 && numDobles != 0)
        {
            Estado = "No se permite mezcla de dobles y triples";
            return;
        }

        if (numDobles == 0)
        {
            switch (numTriples)
            {
                case 4: GeneraReduccion(M4TR13, true); break;
                case 13: GeneraReduccion(M13TR13, true); break;
                case 11: GeneraReduccion(M11TR12, true); break;
                default: Estado = "Nº de triples incorrecto"; break;
            }
        }
        else
        {
            switch (numDobles)
            {
                case 7:
                    if (TestSignosDobles())
                    {
                        GeneraReduccion(M7DR13, false);
                    }
                    else
                    {
                        Estado = "Los signos base no forman parte del pronostico";
                    }
                    break;
                case 15:
                    if (TestSignosDobles())
                    {
                        GeneraReduccion(M15DR13, false);
                    }
                    else
                    {
                        Estado = "Los signos base no forman parte del pronostico";
                    }
                    break;
                default: Estado = "Nº de dobles incorrecto"; break;
            }
        }
    }

    /// <summary>Abre el enlace del método de Fortuna en foro1x2 (linkLabel1 del form legacy).</summary>
    [RelayCommand]
    private async Task AbrirEnlace()
    {
        // Legacy linkLabel1_LinkClicked: abría el navegador con el hilo del método.
        try
        {
            await Windows.System.Launcher.LaunchUriAsync(
                new Uri("http://www.foro1x2.com/viewtopic.php?t=4445"));
        }
        catch
        {
            // Lanzador no disponible: se ignora (equivale a no poder abrir el navegador).
        }
    }

    // ===== Lógica de dominio portada de FrmReducidasPerfectas =====

    // Legacy TestSignos(): la columna base debe estar incluida en el pronóstico.
    private bool TestSignos()
    {
        int num = (int)LongitudColumna;
        for (int i = 0; i < num; i++)
        {
            if (!PronosticoActivo(Partidos[i], Partidos[i].BaseSigno))
            {
                return false;
            }
        }
        return true;
    }

    // Legacy TestSignosDobles(): los signos base de los partidos involucrados están en el pronóstico.
    private bool TestSignosDobles()
    {
        for (int i = 0; i < 15; i++)
        {
            if (_involucrados[i] && !PronosticoActivo(Partidos[i], Partidos[i].BaseSigno))
            {
                return false;
            }
        }
        return true;
    }

    // Legacy ContarDoblesYtriples(): cuenta dobles/triples sobre Pronosticos[15,3] y marca Involucrados.
    private void ContarDoblesYtriples(out int numDobles, out int numTriples)
    {
        numDobles = 0;
        numTriples = 0;
        for (int i = 0; i < 15; i++)
        {
            int suma = (Partidos[i].Pron1 ? 1 : 0) + (Partidos[i].PronX ? 1 : 0) + (Partidos[i].Pron2 ? 1 : 0);
            switch (suma)
            {
                case 3: numTriples++; _involucrados[i] = true; break;
                case 2: numDobles++; _involucrados[i] = true; break;
                default: _involucrados[i] = false; break;
            }
        }
    }

    // Legacy GeneraReduccion(byte[,] Matriz, bool EsDeTriples): genera y graba las columnas reducidas.
    private void GeneraReduccion(byte[,] matriz, bool esDeTriples)
    {
        int signosAObtener = matriz.GetUpperBound(0) + 1;
        int numTriples = matriz.GetUpperBound(1) + 1 + signosAObtener;
        int modulo = esDeTriples ? 3 : 2;

        string[] s = { "1", "X", "2" };
        int maxLen;
        try
        {
            maxLen = (int)LongitudColumna;
        }
        catch
        {
            Estado = "Longitud incorrecta";
            return;
        }

        if (numTriples > maxLen)
        {
            Estado = "La longitud de la columna es incorrecta";
            return;
        }

        double maxCol = Math.Pow(3, numTriples - signosAObtener);
        int ncol = 0;

        try
        {
            using StreamWriter sw = File.CreateText(_rutaArchivo);

            for (int i = 0; i < maxCol; i++) // generamos las columnas
            {
                int[] signos = new int[maxLen];
                bool descartar = false;
                string columna = "";
                int[] suma = new int[5];

                // hacemos como si los triples estuvieran en los primeros partidos
                for (int z = 0; z < (numTriples - signosAObtener); z++) // signos independientes
                {
                    signos[z] = Convert.ToInt32(i / Math.Pow(3, z)) % 3;
                    // los dobles los tratamos inicialmente a 1X, descartamos el 2
                    if (!esDeTriples && signos[z] == 2) { descartar = true; break; }
                    // calculamos los signos dependientes
                    for (int j = 0; j < signosAObtener; j++) { suma[j] += matriz[j, z] * signos[z]; }
                }

                if (!descartar)
                {
                    for (int j = 0; j < signosAObtener; j++) { signos[numTriples - signosAObtener + j] = suma[j] % modulo; }
                    // lo trasladamos a los partidos involucrados
                    int partidoVirtual = 0;
                    for (int partido = 0; partido < 15; partido++)
                    {
                        int baseSigno = Partidos[partido].BaseSigno;
                        if (_involucrados[partido])
                        {
                            if (esDeTriples)
                            {
                                columna += s[(baseSigno + signos[partidoVirtual++]) % 3];
                            }
                            else
                            {
                                // tratamiento de los signos dobles
                                int[] signosDoble = new int[2];
                                signosDoble[0] = baseSigno;
                                signosDoble[1] = (baseSigno + 1) % 3;
                                if (!PronosticoActivo(Partidos[partido], signosDoble[1])) signosDoble[1] = (signosDoble[1] + 1) % 3;
                                columna += s[signosDoble[signos[partidoVirtual++]]];
                            }
                        }
                        else
                        {
                            columna += s[baseSigno];
                        }
                    }
                    columna = columna.Substring(0, maxLen);
                    sw.WriteLine(columna);
                    ncol++;
                }
            }
        }
        catch (Exception ex)
        {
            Estado = "Error al grabar las columnas: " + ex.Message;
            AppServices.MostrarError("Error al grabar las columnas: " + ex.Message);
            return;
        }

        Estado = "Se han grabado " + ncol + " columnas";
        EstadoSalida = "Fichero de salida: " + NombreArchivo;
    }

    // Mapea Pronosticos[i, signo] (0=1, 1=X, 2=2) a los toggles de la fila.
    private static bool PronosticoActivo(PartidoReducidaFila fila, int signo) => signo switch
    {
        0 => fila.Pron1,
        1 => fila.PronX,
        _ => fila.Pron2,
    };
}
