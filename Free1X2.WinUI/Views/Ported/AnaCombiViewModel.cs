using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
/// </summary>
public partial class AnaCombiViewModel : ObservableObject
{
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
    private void SeleccionarFichero()
    {
        // TODO[dominio]: mostrar FileOpenPicker (filtro *.txt), guardar ruta y reflejar el nombre.
        //   Legacy: OpenFileDialog -> filein = Path.GetFileName(...); lFileIn.Text = filein;
        //   En WinUI usar Windows.Storage.Pickers.FileOpenPicker inicializado con el HWND de la ventana.
    }

    /// <summary>
    /// Ejecuta el conteo de columnas por combinación de grupo.
    /// Legacy: AnaCombi.Calcular / RecuperaGrupo / Contabiliza / Mostraresuls.
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: portar el algoritmo de cálculo de AnaCombi.
        //   RecuperaGrupo(): a partir de cada Partido.Seleccion construir 'patron' (1/X/2/'-')
        //     y el array 'grup' con los índices marcados como 'G'; admfal = (int)FallosAdmitidos.
        //   Recorrer las ~4.782.969 combinaciones (3^14) y leer el fichero de entrada,
        //     Normaliza()/Contabiliza() cada columna, respetando 'admfal' fallos.
        //   Mostraresuls(): rellenar Resultados con (Grupo, Columnas).
        //   Mantener un cronómetro que actualice ProcesadasTexto y TiempoTexto (legacy Timer 'elmeu').
        //   Este cálculo es intensivo: ejecutarlo fuera del hilo de UI.
        Resultados.Clear();
        PuedeGrabar = false;
    }

    /// <summary>
    /// Cancela un cálculo en curso.
    /// Legacy: AnaCombi.BCancelarClick -> salida = true.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: señalizar la cancelación del bucle de cálculo (legacy: salida = true).
        Calculando = false;
    }

    /// <summary>
    /// Guarda en un fichero de texto las columnas de los grupos seleccionados.
    /// Legacy: AnaCombi.Grabar (SaveFileDialog + StreamWriter sobre filas marcadas del grid).
    /// </summary>
    [RelayCommand]
    private void Grabar()
    {
        // TODO[dominio]: portar AnaCombi.Grabar.
        //   Mostrar FileSavePicker (*.txt) y, por cada Resultado.Seleccionado, expandir las
        //   columnas (n14s/s14n) que pertenecen a ese grupo y escribirlas con un StreamWriter.
    }
}
