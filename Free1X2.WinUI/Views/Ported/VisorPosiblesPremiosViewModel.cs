using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Escrutinio;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Un signo de una columna jugada con marca de acierto/fallo respecto a la columna ganadora
/// (legacy: ControlPosiblesPremios pinta de rojo el signo cuando difiere del ganador).
/// </summary>
public partial class SignoJugadoItem : ObservableObject
{
    /// <summary>Signo jugado en esta posición (legacy: columna[i]).</summary>
    public string Signo { get; init; } = string.Empty;

    /// <summary>true si coincide con el ganador; false marca un fallo (legacy: BackColor rojo).</summary>
    public bool EsAcierto { get; init; }
}

/// <summary>
/// Una columna jugada que opta a premio, dentro de un grupo del visor
/// (legacy: cada string de las listas Col16..Col10 de PosiblesPremiosContenedor,
/// pintado como un ControlPosiblesPremios vertical en el ContainerControl cctrl).
/// </summary>
public partial class ColumnaPremioItem : ObservableObject
{
    /// <summary>
    /// Signos de la columna jugada con su marca de acierto/fallo, de arriba a abajo
    /// (legacy: ControlPosiblesPremios recorre columnaGanadora.Length posiciones).
    /// </summary>
    public IReadOnlyList<SignoJugadoItem> Signos { get; init; } = new List<SignoJugadoItem>();

    /// <summary>
    /// Categoría de premio a la que opta esta columna (16/15/14/13/12/11/10 aciertos);
    /// legacy: lista de la que procede (Col16..Col10).
    /// </summary>
    public int Categoria { get; init; }

    /// <summary>Etiqueta de la categoría como texto (regla anti-crash 2: no se bindea int a Text).</summary>
    public string CategoriaTexto => Categoria.ToString();

    /// <summary>
    /// Nº de aciertos real de la columna, leído de los 2 últimos caracteres del string jugado
    /// (legacy: ControlPosiblesPremios lblP1 = columna.Substring(columna.Length - 2, 2)).
    /// </summary>
    public string AciertosTexto { get; init; } = string.Empty;
}

/// <summary>
/// Un signo de la columna ganadora con marca de si la columna jugada acierta o no
/// (legacy: comparación de 'columna' contra 'columnaGanadora' dentro de ControlPosiblesPremios).
/// Aquí se usa solo para pintar la columna ganadora vertical (lblCG1..lblCG16).
/// </summary>
public partial class SignoGanadorItem : ObservableObject
{
    /// <summary>Signo ganador en esta posición (legacy: lblCG{n}.Text).</summary>
    public string Signo { get; init; } = string.Empty;
}

/// <summary>
/// ViewModel del visor "Visor de Posibles Premios" (legacy: VisorPosiblesPremios).
///
/// Propósito: visor de SOLO LECTURA que recorre una lista de grupos
/// (List&lt;PosiblesPremiosContenedor&gt;). Por cada grupo muestra su columna ganadora
/// (ColGanadora) en vertical y, en horizontal, las columnas jugadas que optan a premio
/// agrupadas por categoría (Col16..Col10). Permite navegar adelante/atrás entre grupos
/// con un contador "X de N" (legacy: btnAdelante '>', btnAtras '&lt;', lblContador).
/// No tiene campos de entrada: los datos llegan ya calculados desde la pantalla anterior.
/// </summary>
public partial class VisorPosiblesPremiosViewModel : ObservableObject
{
    /// <summary>
    /// Handoff estático con el resumen de premios que calcula la pantalla anterior
    /// (PosiblesPremiosFrm). Equivale al argumento del ctor legacy
    /// VisorPosiblesPremios(List&lt;PosiblesPremiosContenedor&gt; resumenPremios). El visor lo lee
    /// al navegar (mismo patrón que EstucolFrmViewModel.UltimoInforme).
    /// Productor cableado: PosiblesPremiosFrmViewModel.Ver fija UltimoResumen y navega aquí.
    /// </summary>
    public static List<PosiblesPremiosContenedor>? UltimoResumen { get; set; }

    // Resumen recibido por handoff (legacy: List<PosiblesPremiosContenedor> resumen).
    private readonly List<PosiblesPremiosContenedor> _resumen;

    public VisorPosiblesPremiosViewModel()
    {
        // El constructor legacy recibe List<PosiblesPremiosContenedor> resumenPremios y arranca
        // en el grupo 0 (VisorPosiblesPremios_Load -> MostrarGrupos(grupoMostrado)).
        _resumen = UltimoResumen ?? new List<PosiblesPremiosContenedor>();
        TotalGrupos = _resumen.Count;
        IndiceGrupo = 0;
        if (_resumen.Count > 0)
        {
            MostrarGrupo(0);
        }
        else
        {
            ActualizarContador();
        }
    }

    // --- Estado de navegación (legacy: int grupoMostrado, List<...> resumen) ---

    /// <summary>Total de grupos del resumen (legacy: Resumen.Count).</summary>
    [ObservableProperty]
    private int _totalGrupos;

    /// <summary>Índice del grupo mostrado, base 0 (legacy: grupoMostrado).</summary>
    [ObservableProperty]
    private int _indiceGrupo;

    /// <summary>
    /// Texto del contador "X de N" (legacy: lblContador.Text = (grupoMostrado+1) + " de " + Resumen.Count).
    /// Regla anti-crash 2: se bindea string, no int.
    /// </summary>
    [ObservableProperty]
    private string _contador = "0 de 0";

    /// <summary>
    /// Columna ganadora del grupo actual, en vertical (legacy: lblCG1..lblCG16 vía IndicarColumnaGanadora).
    /// </summary>
    public ObservableCollection<SignoGanadorItem> ColumnaGanadora { get; } = new();

    /// <summary>
    /// Columnas jugadas que optan a premio en el grupo actual, agrupadas por categoría
    /// (legacy: Col16..Col10 volcadas en arrayColumnas y pintadas como ControlPosiblesPremios).
    /// </summary>
    public ObservableCollection<ColumnaPremioItem> ColumnasPremiadas { get; } = new();

    /// <summary>Avanza al siguiente grupo (legacy: btnAdelante_Click '>').</summary>
    [RelayCommand]
    private void Adelante()
    {
        if (IndiceGrupo < TotalGrupos - 1)
        {
            IndiceGrupo++;
            MostrarGrupo(IndiceGrupo);
        }
    }

    /// <summary>Retrocede al grupo anterior (legacy: btnAtras_Click '&lt;').</summary>
    [RelayCommand]
    private void Atras()
    {
        if (IndiceGrupo > 0)
        {
            IndiceGrupo--;
            MostrarGrupo(IndiceGrupo);
        }
    }

    /// <summary>
    /// Vuelca el grupo indicado en la UI (legacy: MostrarGrupos(noGrupo) + IndicarColumnaGanadora + AñadirControl).
    /// </summary>
    private void MostrarGrupo(int noGrupo)
    {
        ColumnaGanadora.Clear();
        ColumnasPremiadas.Clear();

        if (noGrupo < 0 || noGrupo >= _resumen.Count)
        {
            ActualizarContador();
            return;
        }

        PosiblesPremiosContenedor contenedor = _resumen[noGrupo];

        // Columna ganadora en vertical (legacy: IndicarColumnaGanadora -> lblCG1..lblCG16).
        string colGanadora = contenedor.ColGanadora ?? string.Empty;
        for (int i = 0; i < colGanadora.Length; i++)
        {
            ColumnaGanadora.Add(new SignoGanadorItem { Signo = colGanadora[i].ToString() });
        }

        // Columnas premiadas, en el mismo orden que el legacy (Col16..Col10), conservando su categoría.
        AgregarCategoria(contenedor.Col16, 16, colGanadora);
        AgregarCategoria(contenedor.Col15, 15, colGanadora);
        AgregarCategoria(contenedor.Col14, 14, colGanadora);
        AgregarCategoria(contenedor.Col13, 13, colGanadora);
        AgregarCategoria(contenedor.Col12, 12, colGanadora);
        AgregarCategoria(contenedor.Col11, 11, colGanadora);
        AgregarCategoria(contenedor.Col10, 10, colGanadora);

        ActualizarContador();
    }

    // Crea un ColumnaPremioItem por cada columna jugada de la categoría dada (legacy: arrayColumnas
    // pintado por ControlPosiblesPremios). Replica 1:1 ControlPosiblesPremios:
    //   - aciertos = 2 últimos caracteres del string jugado (lblP1).
    //   - se pintan columnaGanadora.Length posiciones; cada signo se marca como fallo si difiere.
    private void AgregarCategoria(List<string> categoria, int aciertos, string colGanadora)
    {
        if (categoria == null) return;
        for (int i = 0; i < categoria.Count; i++)
        {
            string columna = categoria[i] ?? string.Empty;

            // Nº de aciertos real: legacy columna.Substring(columna.Length - 2, 2).
            string aciertosTexto = columna.Length >= 2
                ? columna.Substring(columna.Length - 2, 2)
                : string.Empty;

            // Sólo se pintan columnaGanadora.Length signos (legacy); el resto del string son
            // los dígitos de aciertos, que no se muestran como signos.
            var signos = new List<SignoJugadoItem>();
            for (int s = 0; s < colGanadora.Length && s < columna.Length; s++)
            {
                string signo = columna[s].ToString();
                bool esAcierto = signo == colGanadora[s].ToString();
                signos.Add(new SignoJugadoItem { Signo = signo, EsAcierto = esAcierto });
            }

            ColumnasPremiadas.Add(new ColumnaPremioItem
            {
                Signos = signos,
                Categoria = aciertos,
                AciertosTexto = aciertosTexto,
            });
        }
    }

    /// <summary>Recalcula el texto del contador (legacy: lblContador.Text).</summary>
    private void ActualizarContador()
    {
        Contador = TotalGrupos == 0
            ? "0 de 0"
            : $"{IndiceGrupo + 1} de {TotalGrupos}";
    }
}
