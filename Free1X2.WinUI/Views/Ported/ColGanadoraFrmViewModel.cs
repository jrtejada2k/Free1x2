using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Análisis de fallos en combinación" (legacy: ColGanadoraFrm).
/// El usuario introduce la columna ganadora y elige si analizar la combinación que está
/// en pantalla o abrir otra desde archivo, para detectar los fallos frente a esa columna.
/// </summary>
public partial class ColGanadoraFrmViewModel : ObservableObject
{
    /// <summary>
    /// Número de partidos de la combinación (legacy: campo numPartidos, por defecto 14).
    /// Determina la longitud exigida a la columna ganadora.
    /// </summary>
    private int _numPartidos = 14;

    // Contexto del flujo legacy (legacy: ctor ColGanadoraFrm(nPartidos, nombreCombi, analizador, listaPronosticos)).
    // Lo inyectaba la pantalla llamante para la rama "Analizar combinación en pantalla".
    private string _nombreComb = string.Empty;
    private Analizador? _analizadorBase;
    private string[]? _listaPronosticos;

    /// <summary>Texto de la columna ganadora (legacy: txtCG, mayúsculas, MaxLength 16).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ColumnaGanadoraValida))]
    private string _columnaGanadora = string.Empty;

    /// <summary>Analizar la combinación en pantalla (legacy: rbCombiPantalla, Checked por defecto).</summary>
    [ObservableProperty]
    private bool _analizarEnPantalla = true;

    /// <summary>Abrir una combinación desde archivo para analizar (legacy: rbAbrirCombi).</summary>
    [ObservableProperty]
    private bool _abrirCombinacion;

    /// <summary>Longitud requerida, expuesta como string para bindear a un TextBlock (regla anti-crash 2).</summary>
    public string LongitudRequeridaTexto => _numPartidos.ToString();

    /// <summary>
    /// True si la columna ganadora tiene exactamente la longitud requerida
    /// (legacy: ColGanadoraFrm.btnComienzo_Click valida cg.Length != numPartidos).
    /// </summary>
    public bool ColumnaGanadoraValida => ColumnaGanadora.Length == _numPartidos;

    /// <summary>
    /// Establece el número de partidos recibido del contexto (legacy: constructor nPartidos).
    /// </summary>
    public void EstablecerNumeroPartidos(int numPartidos)
    {
        _numPartidos = numPartidos;
        OnPropertyChanged(nameof(LongitudRequeridaTexto));
        OnPropertyChanged(nameof(ColumnaGanadoraValida));
    }

    /// <summary>
    /// Recibe el contexto del flujo legacy (ctor ColGanadoraFrm): nº de partidos, nombre de la
    /// combinación en pantalla, su Analizador y la lista de pronósticos base. Necesario para la
    /// rama "Analizar combinación en pantalla".
    /// </summary>
    public void EstablecerContexto(int numPartidos, string nombreCombi, Analizador analizador, string[] listaPronosticos)
    {
        EstablecerNumeroPartidos(numPartidos);
        _nombreComb = nombreCombi;
        _analizadorBase = analizador;
        _listaPronosticos = listaPronosticos;
    }

    /// <summary>
    /// Lanza el análisis de fallos de la combinación frente a la columna ganadora.
    /// Legacy: ColGanadoraFrm.btnComienzo_Click (Free1X2/UI/ColGanadoraFrm.cs línea 159).
    /// </summary>
    [RelayCommand]
    private async Task AnalizarAsync()
    {
        // Validación de longitud (legacy línea 162: cg.Length != numPartidos).
        if (!ColumnaGanadoraValida)
        {
            AppServices.MostrarError(
                "La columna ganadora debe tener " + Free1X2.VariablesGlobales.NumeroPartidos + " signos.");
            return;
        }

        string archivo;
        Analizador analizador;
        string[] miListaPronosticos;

        if (AnalizarEnPantalla)
        {
            // Rama "Analizar combinación en pantalla" (legacy líneas 172-178): usa el contexto
            // inyectado por la pantalla llamante.
            if (_analizadorBase is null || _listaPronosticos is null)
            {
                // El contexto de la combinación en pantalla aún no está conectado en WinUI.
                // TODO[navegación]: pasar nombreComb/analizador/listaPronosticos a EstablecerContexto
                //   desde la página que abre esta pantalla (legacy ctor ColGanadoraFrm).
                AppServices.MostrarError(
                    "No hay una combinación en pantalla disponible para analizar. " +
                    "Selecciona \"Abrir combinación para analizar\".");
                return;
            }
            archivo = _nombreComb;
            analizador = _analizadorBase;
            miListaPronosticos = _listaPronosticos;
        }
        else
        {
            // Rama "Abrir combinación" (legacy líneas 182-204): OpenFileDialog (*.comb, *.xml) +
            // ArchivoCombinacion.AbrirArchivoCombinacion / CargaControladorGrupos / LeePronosticos.
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".comb");
            picker.FileTypeFilter.Add(".xml");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSingleFileAsync();
            if (file == null) return;

            archivo = file.Path;
            ArchivoCombinacion archComb = new ArchivoCombinacion();
            archComb.AbrirArchivoCombinacion(archivo);
            analizador = new Analizador();
            archComb.CargaControladorGrupos(analizador.CtrlGrupos);
            miListaPronosticos = archComb.LeePronosticos();
            for (int i = 0; i < miListaPronosticos.Length; i++)
            {
                miListaPronosticos[i] = miListaPronosticos[i].Replace(",", "");
            }
        }

        // Conversión de la columna ganadora a su representación numérica (legacy línea 207).
        long columnaNumerica = Free1X2.Utils.UtilColumnas.ConvStrToLong(ColumnaGanadora);

        // TODO[dominio]: ejecutar el análisis de fallos.
        //   Legacy: new Free1X2.Analisis.AnalisisCombinacion().AnalizarCombinacion(
        //       archivo, columnaNumerica, analizador, miListaPronosticos);
        //   (Free1X2/UI/ColGanadoraFrm.cs línea 206-207).
        //   AnalisisCombinacion NO está portado al dominio: vive en Free1X2/Analisis/AnalisisCombinacion.cs
        //   y depende de AnalizarCombinacionFrm (TreeView WinForms con ShowDialog), por lo que su
        //   salida visual no tiene equivalente WinUI todavía. Cuando se porte, sustituir esta llamada
        //   y mostrar el árbol de fallos en una página/diálogo WinUI.
        _ = columnaNumerica; _ = analizador; _ = miListaPronosticos; _ = archivo;
        AppServices.MostrarInfo(
            "Combinación cargada y columna ganadora validada. El visor del árbol de fallos " +
            "(AnalisisCombinacion) está pendiente de portar al dominio/WinUI.");
    }
}
