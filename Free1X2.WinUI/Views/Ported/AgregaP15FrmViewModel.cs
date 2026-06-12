using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    /// <summary>
    /// Equivale a <c>Button1Click</c> del WinForms: ejecuta el cálculo del P15.
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        string error = ComprobarEntradas();
        if (!string.IsNullOrEmpty(error))
        {
            Estado = error.Trim();
            return;
        }

        Estado = "Calculando";

        // TODO(algoritmo): el cálculo del P15 (4 modos) vive en Button1Click del form
        //   WinForms Free1X2.UI.AgregaP15Frm, NO en un método del motor. Los tipos de motor
        //   que usa ya están en Free1X2.Domain y son accesibles desde aquí:
        //     - Free1X2.EntradaSalida.ArchivoColumnasTexto (IArchivoColumnas: SiguienteColumna /
        //       LeeColumnaSinComas / GuardarCols / Cerrar).
        //     - Free1X2.Utils.ConvertidorDeBases + BitArray para el modo "eliminar repetidas".
        //   Portar ese algoritmo es transcribir lógica de la UI legacy (fuera del alcance de
        //   "cablear dominio": aquí solo se conectan llamadas al motor existente). Los selectores
        //   de archivo (SeleccionarEntrada/SeleccionarSalida) ya usan el motor vía pickers.
        //   Al portarlo: recorrer el archivo de entrada con ArchivoColumnasTexto, aplicar el modo
        //   activo y escribir con GuardarCols; fijar ColumnasRepetidas y Estado = "Terminado".

        Estado = "Terminado (algoritmo P15 pendiente de portar)";
    }
}
