using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
/// Reducciones contempladas (texto fijo txtReduccionesContempladas del form legacy):
///    4 TRIPLES → reducidos al 13   (matriz M4TR13)
///   13 TRIPLES → reducidos al 13   (matriz M13TR13)
///   11 TRIPLES → reducidos al 12   (matriz M11TR12)
///    7 DOBLES  → reducidos al 13   (matriz M7DR13)
///   15 DOBLES  → reducidos al 13   (matriz M15DR13)
///
/// La lógica de generación (btGenerar_Click / GeneraReduccion / ContarDoblesYtriples /
/// TestSignos / TestSignosDobles) NO se porta aquí: se deja como TODO de dominio.
/// </summary>
public partial class FrmReducidasPerfectasViewModel : ObservableObject
{
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
    /// </summary>
    [RelayCommand]
    private void SeleccionarArchivo()
    {
        // TODO(dominio): replicar FrmReducidasPerfectas.btSeleccionarFichero_Click:
        //   abrir un FileSavePicker (WinUI) hacia .../Columnas/ con filtro "Columnas (*.txt)|*.txt",
        //   guardar la ruta completa en _rutaArchivo y mostrar Path.GetFileName(...) en NombreArchivo.
        _ = _rutaArchivo; // evita warning de campo sin usar hasta portar el dominio
    }

    /// <summary>
    /// Genera la reducción perfecta y la graba en el archivo de salida
    /// (botón btGenerar del form legacy).
    /// </summary>
    [RelayCommand]
    private void Generar()
    {
        // TODO(dominio): replicar FrmReducidasPerfectas.btGenerar_Click:
        //   1. Validar que hay archivo de salida (nombreArchivo != "").
        //   2. TestSignos(): la columna base debe estar incluida en el pronóstico.
        //   3. ContarDoblesYtriples(): contar dobles/triples sobre Pronosticos[15,3]
        //      y marcar Involucrados[15]. No se permite mezclar dobles y triples.
        //   4. Seleccionar la matriz de reducción según el recuento:
        //         4 triples → M4TR13 | 13 triples → M13TR13 | 11 triples → M11TR12
        //         7 dobles  → M7DR13 | 15 dobles  → M15DR13
        //      (para dobles, además TestSignosDobles()).
        //   5. GeneraReduccion(matriz, esDeTriples): genera 3^(n-k) combinaciones,
        //      calcula los signos dependientes con la matriz, las traslada a los partidos
        //      Involucrados usando SignosBase y graba cada columna recortada a LongitudColumna
        //      con StreamWriter en _rutaArchivo.
        // Aquí sólo se deja el flujo de estado.
        Estado = "Lógica de generación pendiente de portar (ver TODO de dominio).";
    }

    /// <summary>
    /// Abre el enlace del método de Fortuna en foro1x2 (linkLabel1 del form legacy).
    /// </summary>
    [RelayCommand]
    private void AbrirEnlace()
    {
        // TODO(dominio): replicar linkLabel1_LinkClicked:
        //   abrir http://www.foro1x2.com/viewtopic.php?t=4445 con Windows.System.Launcher.
    }
}
