using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        // TODO(dominio): portar la lógica de Button1Click de Free1X2.UI.AgregaP15Frm.
        //   - Lectura/escritura: Free1X2.EntradaSalida.ArchivoColumnasTexto
        //     (implementa IArchivoColumnas: SiguienteColumna / LeeColumnaSinComas /
        //      GuardarCols / Cerrar).
        //   - Modo "Eliminar repetidas": Free1X2.Utils.ConvertidorDeBases.ConvColumnaANumero
        //     + BitArray(4782969) para marcar columnas ya vistas (14^? espacio 3^14).
        //   Estos tipos viven aún en el proyecto WinForms; se moverán a Free1X2.Domain.
        //   Al terminar: Estado = "Terminado" y ColumnasRepetidas = nº de repetidas.

        Estado = "Terminado (pendiente de portar dominio)";
    }
}
