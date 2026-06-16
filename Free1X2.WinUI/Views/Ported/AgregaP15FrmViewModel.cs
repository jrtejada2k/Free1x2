using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Añade P15 (Indeciso)".
/// Replica los parámetros de entrada del WinForms <c>AgregaP15Frm</c>:
/// gestiona el Pleno al 15 (P15) de un archivo de columnas existente,
/// generando un archivo de salida con uno de cuatro modos mutuamente
/// independientes.
/// </summary>
public partial class AgregaP15FrmViewModel : ObservableObject
{
    /// <summary>
    /// Opciones de signo (1 / X / 2) que alimentan los ComboBox de la página.
    /// Se exponen como <c>ItemsSource</c> para evitar declarar elementos
    /// <c>&lt;x:String&gt;</c> en línea junto a un <c>SelectedItem</c> enlazado con
    /// x:Bind TwoWay: esa combinación hace fallar (crash opaco) al XamlCompiler de
    /// Windows App SDK 1.6 (MarkupCompilePass1).
    /// </summary>
    public IReadOnlyList<string> Signos { get; } = new[] { "1", "X", "2" };

    // ===== Archivos =====
    [ObservableProperty]
    private string _archivoEntrada = "";

    [ObservableProperty]
    private string _archivoSalida = "";

    // ===== Modo 1: P15 fijo =====
    // checkBox1 + textBox1: añade un signo fijo a todas las columnas.
    [ObservableProperty]
    private bool _modoFijo;

    [ObservableProperty]
    private string _signoFijo = "";

    // ===== Modo 2: Condicional =====
    // checkBox2 + textBox2/3/4/5: si partido N == signo, P15 = signoSi, si no P15 = signoNo.
    [ObservableProperty]
    private bool _modoCondicional;

    [ObservableProperty]
    private double _partidoCondicion = 1;

    [ObservableProperty]
    private string _signoCondicion = "";

    [ObservableProperty]
    private string _signoSi = "";

    [ObservableProperty]
    private string _signoNo = "";

    // ===== Modo 3: Eliminar partido 15 (quita P15 y elimina columnas repetidas) =====
    // checkBox3: deja las 14 primeras y descarta duplicadas. label11 muestra el conteo.
    [ObservableProperty]
    private bool _modoEliminarRepetidas;

    // NotifyPropertyChangedFor mantiene sincronizada la proyección de texto que consume
    // el XAML (x:Bind a TextBlock.Text exige una cadena, no un int).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ColumnasRepetidasTexto))]
    private int _columnasRepetidas;

    /// <summary>
    /// Proyección en cadena de <see cref="ColumnasRepetidas"/> para enlazar a
    /// <c>TextBlock.Text</c> sin convertidor (x:Bind no convierte int -&gt; string).
    /// </summary>
    public string ColumnasRepetidasTexto => ColumnasRepetidas.ToString();

    // ===== Modo 4: Copiar resultado de otro partido =====
    // checkBox4 + textBox6: P15 = mismo resultado que el partido N.
    [ObservableProperty]
    private bool _modoCopiarPartido;

    [ObservableProperty]
    private double _partidoCopia = 1;

    // ===== Estado / progreso =====
    [ObservableProperty]
    private string _estado = "Preparado";

    // btnFileIn (BtnFileInClick): OpenFileDialog *.txt para el archivo de entrada.
    [RelayCommand]
    private async Task SeleccionarEntradaAsync()
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSingleFileAsync();
        if (file != null) ArchivoEntrada = file.Path;
    }

    // btnFileOut (BtnFileOutClick): SaveFileDialog *.txt para el archivo de salida.
    [RelayCommand]
    private async Task SeleccionarSalidaAsync()
    {
        var picker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "ColumnasP15",
        };
        picker.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

        var file = await picker.PickSaveFileAsync();
        if (file != null) ArchivoSalida = file.Path;
    }

    /// <summary>
    /// Equivale a <c>ComprobarEntradas()</c> del WinForms: valida archivos y modo.
    /// Devuelve cadena vacía si todo es correcto.
    /// </summary>
    public string ComprobarEntradas()
    {
        string error = "";

        if (string.IsNullOrEmpty(ArchivoSalida) || string.IsNullOrEmpty(ArchivoEntrada))
        {
            error = "Uno de los archivos necesarios no ha sido especificado\n";
        }
        else if (ArchivoSalida == ArchivoEntrada)
        {
            error = "El archivo de salida debe ser distinto al de entrada\n";
        }

        if (!ModoFijo && !ModoCondicional && !ModoEliminarRepetidas && !ModoCopiarPartido)
        {
            error += "No se ha especificado qué hacer con el Pleno al 15";
        }

        return error;
    }

    // BitArray con 3^14 posiciones para detectar columnas repetidas (modo "eliminar partido 15").
    // Igual que el campo Bits del WinForms AgregaP15Frm (new BitArray(4782969, false)).
    private const int TamanoBits = 4782969;

    /// <summary>
    /// Equivale a <c>Button1Click</c> del WinForms (Free1X2/UI/AgregaP15Frm.cs ~479-688):
    /// recorre el archivo de columnas de entrada y, según el modo activo, añade el Pleno al 15
    /// a cada columna de 14 signos, escribiendo el resultado en el archivo de salida. El cálculo
    /// se ejecuta en un hilo de fondo para no bloquear la UI.
    /// </summary>
    [RelayCommand]
    private async Task CalcularAsync()
    {
        string error = ComprobarEntradas();
        if (!string.IsNullOrEmpty(error))
        {
            Estado = error.Trim();
            return;
        }

        Estado = "Calculando";

        // Captura de los parámetros de UI antes de saltar al hilo de fondo.
        string archivoEntrada = ArchivoEntrada;
        string archivoSalida = ArchivoSalida;
        bool modoFijo = ModoFijo;
        bool modoCondicional = ModoCondicional;
        bool modoEliminar = ModoEliminarRepetidas;
        bool modoCopiar = ModoCopiarPartido;
        string signoFijo = SignoFijo ?? "";
        int partidoCondicion = (int)PartidoCondicion;
        string signoCondicion = SignoCondicion ?? "";
        string signoSi = SignoSi ?? "";
        string signoNo = SignoNo ?? "";
        int partidoCopia = (int)PartidoCopia;

        int repetidas = 0;

        try
        {
            await Task.Run(() => repetidas = EjecutarAgregaP15(
                archivoEntrada, archivoSalida,
                modoFijo, modoCondicional, modoEliminar, modoCopiar,
                signoFijo, partidoCondicion, signoCondicion, signoSi, signoNo, partidoCopia));
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("Error al añadir el P15: " + ex.Message);
            Estado = "Error";
            return;
        }

        ColumnasRepetidas = repetidas;
        Estado = "Terminado";
    }

    /// <summary>
    /// Transcripción fiel del bucle de cálculo de <c>Button1Click</c> del WinForms
    /// <c>AgregaP15Frm</c>. Devuelve el número de columnas repetidas (modo eliminar partido 15).
    /// </summary>
    private static int EjecutarAgregaP15(
        string archivoEntrada, string archivoSalida,
        bool modoFijo, bool modoCondicional, bool modoEliminar, bool modoCopiar,
        string signoFijo, int partidoCondicion, string signoCondicion, string signoSi, string signoNo,
        int partidoCopia)
    {
        // Buffers de 8 columnas por signo del P15 (1 / X / 2), igual que el WinForms (vuelca a
        // disco en bloques de 8 para agrupar por signo del pleno).
        string[] Buffer1 = new string[8];
        string[] BufferX = new string[8];
        string[] Buffer2 = new string[8];
        for (int i = 0; i < 8; i++)
        {
            Buffer1[i] = "";
            BufferX[i] = "";
            Buffer2[i] = "";
        }
        int Cont1 = 0, ContX = 0, Cont2 = 0;

        int noColumnasFinal = 0;
        int noColsRepetidas = 0;

        BitArray bits = new BitArray(TamanoBits, false);

        IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
        IArchivoColumnas sw = new ArchivoColumnasTexto(archivoSalida);
        ConvertidorDeBases conv = new ConvertidorDeBases();

        while (comBaseCols.SiguienteColumna())
        {
            string columna = comBaseCols.LeeColumnaSinComas();

            if (modoFijo)
            {
                // checkBox1: añade un signo fijo (textBox1) al final de cada columna.
                sw.GuardarCols(columna + signoFijo);
            }
            else
            {
                if (modoCondicional)
                {
                    // checkBox2: si el partido N (textBox2, 1-based) es igual a signoCondicion
                    // (textBox3), P15 = signoSi (textBox4); si no, P15 = signoNo (textBox5).
                    int tmp = partidoCondicion - 1;
                    char tmp2 = char.Parse(signoCondicion);
                    if (columna[tmp] == tmp2)
                    {
                        columna = columna + signoSi;
                    }
                    else
                    {
                        columna = columna + signoNo;
                    }
                    switch (columna.Substring(14, 1))
                    {
                        case "1":
                            Buffer1[Cont1] = columna;
                            Cont1++;
                            break;
                        case "x":
                        case "X":
                            BufferX[ContX] = columna;
                            ContX++;
                            break;
                        case "2":
                            Buffer2[Cont2] = columna;
                            Cont2++;
                            break;
                    }
                    if (Cont1 == 8)
                    {
                        for (int i = 0; i < 8; i++) sw.GuardarCols(Buffer1[i]);
                        Cont1 = 0;
                    }
                    else if (ContX == 8)
                    {
                        for (int i = 0; i < 8; i++) sw.GuardarCols(BufferX[i]);
                        ContX = 0;
                    }
                    else if (Cont2 == 8)
                    {
                        for (int i = 0; i < 8; i++) sw.GuardarCols(Buffer2[i]);
                        Cont2 = 0;
                    }
                }
                if (modoEliminar)
                {
                    // checkBox3: deja sólo los 14 primeros signos y descarta columnas repetidas.
                    columna = columna.Substring(0, 14);
                    int Num = conv.ConvColumnaANumero(columna);
                    if (bits[Num] == false)
                    {
                        bits[Num] = true;
                        sw.GuardarCols(columna);
                        noColumnasFinal++;
                    }
                    else
                    {
                        noColsRepetidas++;
                    }
                }
                if (modoCopiar)
                {
                    // checkBox4: P15 = mismo resultado que el partido N (textBox6, 1-based).
                    int tmp = partidoCopia - 1;
                    columna = columna + columna[tmp];
                    switch (columna.Substring(14, 1))
                    {
                        case "1":
                            Buffer1[Cont1] = columna;
                            Cont1++;
                            break;
                        case "x":
                        case "X":
                            BufferX[ContX] = columna;
                            ContX++;
                            break;
                        case "2":
                            Buffer2[Cont2] = columna;
                            Cont2++;
                            break;
                    }
                    if (Cont1 == 8)
                    {
                        for (int i = 0; i < 8; i++) sw.GuardarCols(Buffer1[i]);
                        Cont1 = 0;
                    }
                    else if (ContX == 8)
                    {
                        for (int i = 0; i < 8; i++) sw.GuardarCols(BufferX[i]);
                        ContX = 0;
                    }
                    else if (Cont2 == 8)
                    {
                        for (int i = 0; i < 8; i++) sw.GuardarCols(Buffer2[i]);
                        Cont2 = 0;
                    }
                }
            }
        }
        comBaseCols.Cerrar();

        // Vuelca los restos de cada buffer (columnas que no completaron un bloque de 8).
        if (Cont1 != 0)
        {
            for (int i = 0; i < Cont1; i++) sw.GuardarCols(Buffer1[i]);
        }
        if (ContX != 0)
        {
            for (int i = 0; i < ContX; i++) sw.GuardarCols(BufferX[i]);
        }
        if (Cont2 != 0)
        {
            for (int i = 0; i < Cont2; i++) sw.GuardarCols(Buffer2[i]);
        }

        sw.Cerrar();

        return noColsRepetidas;
    }
}
