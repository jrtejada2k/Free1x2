using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
    /// Lanza el análisis de fallos de la combinación frente a la columna ganadora.
    /// </summary>
    [RelayCommand]
    private void Analizar()
    {
        if (!ColumnaGanadoraValida)
        {
            // TODO[dominio]: mostrar error "La columna ganadora debe tener N signos".
            //   Legacy: MessageBox.Show(... VariablesGlobales.NumeroPartidos ...) en btnComienzo_Click.
            return;
        }

        if (AnalizarEnPantalla)
        {
            // TODO[dominio]: analizar la combinación en pantalla.
            //   Legacy: usa nombreComb / analizadorBase / listaPronosticos recibidos en el ctor.
        }
        else
        {
            // TODO[dominio]: abrir una combinación desde archivo y cargarla.
            //   Legacy: OpenFileDialog (Combinaciones\*.comb;*.xml) ->
            //   Free1X2.EntradaSalida.ArchivoCombinacion.AbrirArchivoCombinacion / CargaControladorGrupos / LeePronosticos,
            //   normalizando pronósticos con Replace(",", "").
        }

        // TODO[dominio]: ejecutar el análisis de fallos de la combinación.
        //   Legacy: Free1X2.Analisis.AnalisisCombinacion.AnalizarCombinacion(
        //       archivo, UtilColumnas.ConvStrToLong(cg), analizador, miListaPronosticos).
    }
}
