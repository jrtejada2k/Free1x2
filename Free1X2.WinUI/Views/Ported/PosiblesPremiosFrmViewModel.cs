using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de un partido en la pantalla "Posibles Premios" (legacy: cada terna
/// etiqueta cabecera P1..P16 + TextBox txt1..txt16 con el signo de la columna
/// ganadora + las casillas lblP{n}C1..C8 con los signos de la columna jugada).
/// </summary>
public partial class PartidoPremioItem : ObservableObject
{
    /// <summary>Número de partido 1..16 (legacy: etiquetas P1..P16).</summary>
    public int Numero { get; init; }

    /// <summary>Etiqueta del número de partido como texto (regla anti-crash 2: no se bindea int a Text).</summary>
    public string NumeroTexto => Numero.ToString();

    /// <summary>
    /// Signo ganador introducido por el usuario para este partido
    /// (legacy: TextBox txt{n}; valores admitidos "1" / "X" / "2", o "*" para no definitivo).
    /// </summary>
    [ObservableProperty]
    private string _signoGanador = string.Empty;

    /// <summary>
    /// Signos de las hasta 8 columnas jugadas leídas del fichero
    /// (legacy: lblP{n}C1..lblP{n}C8). Se muestran como texto de solo lectura.
    /// </summary>
    public ObservableCollection<string> SignosColumnas { get; } = new();
}

/// <summary>
/// ViewModel de la pantalla "Posibles Premios" (legacy: PosiblesPremiosFrm).
///
/// Propósito: a partir de una columna ganadora introducida por el usuario y de uno o
/// varios boletos/columnas jugadas leídas de un fichero, calcula a cuántos premios
/// (pleno-16/15, 14, 13, 12, 11 y 10 aciertos) está optando cada columna y resume las
/// columnas con premio. Permite navegar entre boletos del fichero, ver/copiar/guardar
/// el resumen y abrir el diálogo de "Mis mejores opciones".
/// </summary>
public partial class PosiblesPremiosFrmViewModel : ObservableObject
{
    public PosiblesPremiosFrmViewModel()
    {
        for (int i = 1; i <= 16; i++)
        {
            Partidos.Add(new PartidoPremioItem { Numero = i });
        }
    }

    /// <summary>
    /// Los 16 partidos con su signo ganador editable y los signos de las columnas jugadas
    /// (legacy: rejilla txt1..txt16 + lblP{n}C1..C8).
    /// </summary>
    public ObservableCollection<PartidoPremioItem> Partidos { get; } = new();

    /// <summary>Opciones de signo para los ComboBox (regla anti-crash 3: ItemsSource desde el VM).</summary>
    public IReadOnlyList<string> SignosPosibles { get; } = new[] { "", "1", "X", "2", "*" };

    /// <summary>Nombre del fichero de columnas abierto (legacy: lblNombreArchivo.Text).</summary>
    [ObservableProperty]
    private string _nombreArchivo = string.Empty;

    /// <summary>
    /// Considerar el último partido como pleno al 15 (legacy: CheckBox chkPleno
    /// "Considerar último partido como pleno").
    /// </summary>
    [ObservableProperty]
    private bool _considerarPleno;

    /// <summary>Generar resumen al calcular (legacy: CheckBox ckbResumen "Resumen").</summary>
    [ObservableProperty]
    private bool _generarResumen;

    // --- Resultados (legacy: lblPremios16..lblPremios10 y lblColumnasConPremio) ---
    // Reglas anti-crash 2: se exponen como string para bindear a TextBlock.Text.

    [ObservableProperty]
    private string _optandoA16 = "Optando a 16: 0";

    [ObservableProperty]
    private string _optandoA15 = "Optando a 15: 0";

    [ObservableProperty]
    private string _optandoA14 = "Optando a 14: 0";

    [ObservableProperty]
    private string _optandoA13 = "Optando a 13: 0";

    [ObservableProperty]
    private string _optandoA12 = "Optando a 12: 0";

    [ObservableProperty]
    private string _optandoA11 = "Optando a 11: 0";

    [ObservableProperty]
    private string _optandoA10 = "Optando a 10: 0";

    /// <summary>Resumen de columnas con premio (legacy: lblColumnasConPremio.Text).</summary>
    [ObservableProperty]
    private string _columnasConPremio = "Columnas con premio: 0";

    /// <summary>
    /// Abre un fichero de columnas/boletos y carga el primer boleto en la rejilla
    /// (legacy: btnAbreArchivo_Click).
    /// </summary>
    [RelayCommand]
    private void AbrirArchivo()
    {
        // TODO[dominio]: abrir fichero de columnas con un selector.
        //   Legacy: PosiblesPremiosFrm.btnAbreArchivo_Click
        //     - OpenFileDialog -> Windows.Storage.Pickers.FileOpenPicker en WinUI.
        //     - Leer columnas con Free1X2.EntradaSalida (lector de columnas), poblar
        //       col10..col16 y arrayColumnas; lblNombreArchivo = Path.GetFileName(...).
        //     - Volcar el primer boleto en lblP{n}C1..C8 (aquí: Partidos[i].SignosColumnas).
    }

    /// <summary>Avanza al siguiente boleto del fichero (legacy: btnAdelante_Click, ">").</summary>
    [RelayCommand]
    private void Adelante()
    {
        // TODO[dominio]: mostrar el siguiente boleto (legacy: btnAdelante_Click, noBoleto++).
    }

    /// <summary>Retrocede al boleto anterior del fichero (legacy: btnAtras_Click, "<").</summary>
    [RelayCommand]
    private void Atras()
    {
        // TODO[dominio]: mostrar el boleto anterior (legacy: btnAtras_Click, noBoleto--).
    }

    /// <summary>
    /// Calcula los premios a los que opta cada columna frente a la columna ganadora
    /// (legacy: btnCalcular_Click).
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: escrutinio de las columnas contra la columna ganadora.
        //   Legacy: PosiblesPremiosFrm.btnCalcular_Click
        //     - Construir columnaGanadora a partir de txt1..txt16 (signos del usuario);
        //       "*" marca signos no definitivos (signosNoDefinitivos).
        //     - Usar Free1X2.Escrutinio para contar, por cada columna jugada, a qué
        //       categoría de premio opta (16/15/14/13/12/11/10), respetando chkPleno.
        //     - Actualizar OptandoA16..OptandoA10 y ColumnasConPremio; si GenerarResumen,
        //       poblar 'resumen' (List<PosiblesPremiosContenedor>).
    }

    /// <summary>Muestra el resumen detallado de premios (legacy: btnVer_Click).</summary>
    [RelayCommand]
    private void Ver()
    {
        // TODO[dominio]: abrir/mostrar el resumen detallado (legacy: btnVer_Click).
    }

    /// <summary>Copia el resumen al portapapeles (legacy: btnCopiar_Click).</summary>
    [RelayCommand]
    private void Copiar()
    {
        // TODO[dominio]: copiar el texto del resumen al portapapeles.
        //   Legacy: btnCopiar_Click componía 'info' con lblPremios16..lblPremios10.Text.
        //   En WinUI usar Windows.ApplicationModel.DataTransfer.Clipboard.
    }

    /// <summary>Guarda el resumen en un fichero de texto (legacy: btnGuardar_Click).</summary>
    [RelayCommand]
    private void Guardar()
    {
        // TODO[dominio]: guardar el resumen en un fichero.
        //   Legacy: btnGuardar_Click ("Guardar Resumen") con SaveFileDialog.
        //   En WinUI usar Windows.Storage.Pickers.FileSavePicker.
    }

    /// <summary>Abre el diálogo "Mis mejores opciones" (legacy: btnMejoresOpciones_Click).</summary>
    [RelayCommand]
    private void MejoresOpciones()
    {
        // TODO[dominio]: abrir la pantalla "Mis mejores opciones".
        //   Legacy: btnMejoresOpciones_Click instanciaba MejoresOpcionesFrm.
        //   En WinUI navegar a la Page correspondiente (MejoresOpcionesFrmViewModel ya existe).
    }
}
