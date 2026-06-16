using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Escrutinio;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una columna jugada que opta a premio, dentro de un grupo del visor
/// (legacy: cada string de las listas Col16..Col10 de PosiblesPremiosContenedor,
/// pintado como un ControlPosiblesPremios vertical en el ContainerControl cctrl).
/// </summary>
public partial class ColumnaPremioItem : ObservableObject
{
    /// <summary>Signos de la columna jugada, de arriba a abajo (legacy: string 'columna').</summary>
    public IReadOnlyList<string> Signos { get; init; } = new List<string>();

    /// <summary>
    /// Categoría de premio a la que opta esta columna (16/15/14/13/12/11/10 aciertos);
    /// legacy: lista de la que procede (Col16..Col10).
    /// </summary>
    public int Categoria { get; init; }

    /// <summary>Etiqueta de la categoría como texto (regla anti-crash 2: no se bindea int a Text).</summary>
    public string CategoriaTexto => Categoria.ToString();
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
    /// TODO[productor]: PosiblesPremiosFrmViewModel.Ver (stub fuera del alcance de este lote,
    ///   Free1X2/UI/PosiblesPremiosFrm.cs línea 3489) debe fijar UltimoResumen = _resumen y navegar aquí.
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
        AgregarCategoria(contenedor.Col16, 16);
        AgregarCategoria(contenedor.Col15, 15);
        AgregarCategoria(contenedor.Col14, 14);
        AgregarCategoria(contenedor.Col13, 13);
        AgregarCategoria(contenedor.Col12, 12);
        AgregarCategoria(contenedor.Col11, 11);
        AgregarCategoria(contenedor.Col10, 10);

        ActualizarContador();
    }

    // Crea un ColumnaPremioItem por cada columna jugada de la categoría dada (legacy: arrayColumnas).
    private void AgregarCategoria(List<string> categoria, int aciertos)
    {
        if (categoria == null) return;
        for (int i = 0; i < categoria.Count; i++)
        {
            string columna = categoria[i] ?? string.Empty;
            var signos = new List<string>(columna.Length);
            for (int s = 0; s < columna.Length; s++)
            {
                signos.Add(columna[s].ToString());
            }
            ColumnasPremiadas.Add(new ColumnaPremioItem { Signos = signos, Categoria = aciertos });
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
