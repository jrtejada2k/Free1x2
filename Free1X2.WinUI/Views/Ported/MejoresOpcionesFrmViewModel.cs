using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Escrutinio;
using Free1X2.WinUI.Services;
using Microsoft.UI.Dispatching;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Contexto del flujo legacy PosiblesPremiosFrm.btnMejoresOpciones_Click, que en WinForms se
/// inyectaba por propiedades antes de ShowDialog (ColumnaGanadora, ArchivoColumnas y el flag de
/// pleno del ctor). Aquí viaja como parámetro de navegación a MejoresOpcionesFrmPage y se reenvía
/// al ViewModel con EstablecerContexto.
/// </summary>
public sealed record MejoresOpcionesContexto(
    string ColumnaGanadora,
    IReadOnlyList<string> ArchivoColumnas,
    bool ContemplaPleno);

/// <summary>
/// Representa una casilla "Partido Involucrado" (legacy: ckb1..ckb16).
/// Numero = etiqueta del partido (1..16); Involucrado = estado marcado;
/// Visible = se muestra solo si el partido existe segun la longitud de la
/// columna ganadora (legacy: AdaptarInterfaz).
/// </summary>
public partial class PartidoInvolucradoItem : ObservableObject
{
    [ObservableProperty]
    private bool _involucrado;

    [ObservableProperty]
    private bool _visible = true;

    /// <summary>Numero de partido (1..16). String para enlazarlo directo a Text.</summary>
    public string Numero { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel de la pantalla "Mis Mejores Opciones" (legacy: MejoresOpcionesFrm).
///
/// Proposito legacy: a partir de una columna ganadora parcialmente conocida
/// (con comodines '*'), el usuario marca que partidos pueden variar
/// ("Partidos Involucrados"). El motor genera todas las columnas ganadoras
/// posibles (ObtenGanadoras), escruta el archivo de columnas jugadas contra cada
/// una (Escrutar / ObtenerResumen), las ordena por premios (OrdenarResumen) y
/// muestra las N primeras (MostrarResumen).
///
/// Toda la logica de calculo (ObtenGanadoras, Escrutar, ObtenerResumen,
/// OrdenarResumen, MostrarResumen) esta autocontenida en el WinForms original y
/// solo depende de Free1X2.Escrutinio.PosiblesPremiosContenedor /
/// PosiblesPremiosComparer, disponibles en Free1X2.Domain. Se replica aqui de
/// forma fiel (ver Free1X2/UI/MejoresOpcionesFrm.cs).
/// </summary>
public partial class MejoresOpcionesFrmViewModel : ObservableObject
{
    // ===== Entradas del flujo legacy (legacy: ctor + propiedades ArchivoColumnas/ColumnaGanadora) =====
    // En el WinForms estos valores los inyectaba PosiblesPremiosFrm.btnMejoresOpciones_Click:
    //   mejoresOpciones.ArchivoColumnas = arrayColumnas;
    //   mejoresOpciones.ColumnaGanadora = columnaGanadora;
    //   new MejoresOpcionesFrm(chkPleno.Checked)  -> ContemplaPleno.
    // En WinUI se reciben con EstablecerContexto(...) antes de mostrar la pagina.

    // Columna ganadora parcialmente conocida ('*' = signo no definitivo) (legacy: columnaGanadora).
    private string _columnaGanadora = "**************";

    // Columnas jugadas a escrutar (legacy: archivoColumnas, List<string>).
    private List<string> _archivoColumnas = new();

    // Considera el ultimo partido como pleno al 15 (legacy: ContemplaPleno del ctor).
    private bool _contemplaPleno;

    // Numero de partidos de la columna ganadora (legacy: noPartidos = ColumnaGanadora.Length).
    private int _noPartidos = 14;

    // Resultado del escrutinio (legacy: resumen, ordenado por PosiblesPremiosComparer).
    private readonly List<PosiblesPremiosContenedor> _listaResumen = new();

    // Todas las columnas ganadoras generadas (legacy: cGanadoras).
    private readonly List<string> _cGanadoras = new();

    /// <summary>
    /// Numero de partidos a mostrar (legacy: txtLimite, default 10). Se enlaza a un
    /// NumberBox.Value (double) respetando la regla anti-crash 7.
    /// </summary>
    [ObservableProperty]
    private double _limiteResultados = 10;

    /// <summary>
    /// Texto del resumen calculado (legacy: txtResumen, TextBox multilinea de solo lectura).
    /// </summary>
    [ObservableProperty]
    private string _resumen = string.Empty;

    /// <summary>
    /// Las 16 casillas de "Partidos Involucrados" (legacy: ckb1..ckb16).
    /// Su visibilidad se ajusta con AdaptarInterfaz segun la longitud de la columna ganadora.
    /// </summary>
    public ObservableCollection<PartidoInvolucradoItem> Partidos { get; } = new();

    public MejoresOpcionesFrmViewModel()
    {
        for (int i = 1; i <= 16; i++)
        {
            Partidos.Add(new PartidoInvolucradoItem { Numero = i.ToString() });
        }
        AdaptarInterfaz(_columnaGanadora.Length);
    }

    /// <summary>
    /// Recibe el contexto del flujo legacy (PosiblesPremiosFrm.btnMejoresOpciones_Click):
    /// columna ganadora, columnas jugadas y si se contempla el pleno al 15.
    /// </summary>
    public void EstablecerContexto(string columnaGanadora, IEnumerable<string> archivoColumnas, bool contemplaPleno)
    {
        _columnaGanadora = string.IsNullOrEmpty(columnaGanadora) ? "**************" : columnaGanadora;
        _archivoColumnas = new List<string>(archivoColumnas);
        _contemplaPleno = contemplaPleno;
        _noPartidos = _columnaGanadora.Length;
        AdaptarInterfaz(_columnaGanadora.Length);
    }

    /// <summary>
    /// Muestra/oculta las casillas segun el numero de partidos de la columna ganadora.
    /// Legacy: MejoresOpcionesFrm.AdaptarInterfaz(int partidos) ocultaba ckb10..ckb16
    /// cuando partidos era menor que su numero.
    /// </summary>
    public void AdaptarInterfaz(int partidos)
    {
        foreach (var p in Partidos)
        {
            if (int.TryParse(p.Numero, out int n))
            {
                p.Visible = partidos >= n;
            }
        }
    }

    /// <summary>
    /// Ejecuta el calculo de las mejores opciones (legacy: button1_Click).
    /// Replica fiel: txtResumen=""; resumen.Clear(); cGanadoras.Clear();
    /// ObtenGanadoras("",0); ObtenerResumen por cada ganadora; OrdenarResumen;
    /// MostrarResumen(ObtenerLimiteResultados()). El escrutinio se hace en un hilo
    /// de fondo y el texto se publica al hilo de UI.
    /// </summary>
    [RelayCommand]
    private async Task CalcularAsync()
    {
        // Validacion equivalente a ObtenerLimiteResultados (legacy): si el limite no es
        // valido, restablecer a 10.
        if (LimiteResultados < 1)
        {
            LimiteResultados = 10;
        }

        int limite = (int)LimiteResultados;
        // Snapshot de los flags de "partidos involucrados" para usarlos en el hilo de fondo
        // (legacy: bool[] partidosInvolucrados = { ckb1.Checked, ... }).
        bool[] partidosInvolucrados = new bool[16];
        for (int i = 0; i < Partidos.Count && i < 16; i++)
        {
            partidosInvolucrados[i] = Partidos[i].Involucrado;
        }

        string colGanadora = _columnaGanadora;
        var archivoColumnas = new List<string>(_archivoColumnas);
        bool contemplaPleno = _contemplaPleno;
        int noPartidos = _noPartidos;

        Resumen = "Calculando...";

        string texto = await Task.Run(() =>
        {
            _listaResumen.Clear();
            _cGanadoras.Clear();
            ObtenGanadoras(colGanadora, partidosInvolucrados, "", 0);
            for (int i = 0; i < _cGanadoras.Count; i++)
            {
                ObtenerResumen(_cGanadoras[i], archivoColumnas, noPartidos, contemplaPleno);
            }
            OrdenarResumen();
            return MostrarResumen(limite, colGanadora);
        });

        var disp = AppServices.UiDispatcher;
        if (disp is not null)
        {
            disp.TryEnqueue(DispatcherQueuePriority.Normal, () => Resumen = texto);
        }
        else
        {
            Resumen = texto;
        }
    }

    /// <summary>
    /// Genera recursivamente todas las columnas ganadoras posibles.
    /// Legacy: MejoresOpcionesFrm.ObtenGanadoras(string preString, int partidoNo).
    /// </summary>
    private void ObtenGanadoras(string columnaGanadora, bool[] partidosInvolucrados, string preString, int partidoNo)
    {
        string[] signos = { "1", "X", "2" };
        string newPreString;

        for (int i = 0; i < signos.Length; i++)
        {
            if (columnaGanadora[partidoNo].ToString() != "*")
            {
                newPreString = preString + columnaGanadora[partidoNo];
                i = 4;
            }
            else
            {
                if (partidosInvolucrados[partidoNo])
                {
                    newPreString = preString + signos[i];
                }
                else
                {
                    newPreString = preString + "*";
                    i = 4;
                }
            }

            if (partidoNo < columnaGanadora.Length - 1)
            {
                ObtenGanadoras(columnaGanadora, partidosInvolucrados, newPreString, partidoNo + 1);
            }
            else
            {
                _cGanadoras.Add(newPreString);
            }
        }
    }

    /// <summary>
    /// Escruta una columna analizada contra una ganadora y devuelve los aciertos.
    /// Legacy: MejoresOpcionesFrm.Escrutar(string cAnalizada, string cGanadora).
    /// </summary>
    private static int Escrutar(string cAnalizada, string cGanadora, int noPartidos, bool contemplaPleno)
    {
        int aciertos = 0;
        int posiblesAciertos = noPartidos;
        for (int i = 0; i < cAnalizada.Length - 1; i++)
        {
            if (posiblesAciertos < 10) { break; }
            if (cAnalizada[i] == cGanadora[i])
            {
                aciertos++;
            }
            else if (cGanadora[i].ToString() == "*")
            {
                aciertos++;
            }
            else
            {
                posiblesAciertos--;
            }
        }
        if (contemplaPleno)
        {
            if (aciertos == cAnalizada.Length - 1)
            {
                if (cAnalizada[cAnalizada.Length - 1] == cGanadora[cGanadora.Length - 1] || cGanadora[cGanadora.Length - 1].ToString() == "*")
                {
                    aciertos++;
                }
            }
        }
        return aciertos;
    }

    /// <summary>
    /// Calcula el contenedor de premios para una columna ganadora dada.
    /// Legacy: MejoresOpcionesFrm.ObtenerResumen(string cGanadora).
    /// </summary>
    private void ObtenerResumen(string cGanadora, List<string> archivoColumnas, int noPartidos, bool contemplaPleno)
    {
        PosiblesPremiosContenedor posiblesPremios = new PosiblesPremiosContenedor();
        for (int i = 0; i < archivoColumnas.Count; i++)
        {
            string cAnalizada = archivoColumnas[i];
            int aciertos = Escrutar(cAnalizada, cGanadora, noPartidos, contemplaPleno);
            if (aciertos > 9)
            {
                posiblesPremios.ColGanadora = cGanadora;
                switch (aciertos)
                {
                    case 10:
                        posiblesPremios.Col10.Add(cAnalizada);
                        break;
                    case 11:
                        posiblesPremios.Col11.Add(cAnalizada);
                        if (contemplaPleno && cAnalizada.Length == 11)
                        {
                            posiblesPremios.Col14.Add(cAnalizada);
                        }
                        break;
                    case 12:
                        posiblesPremios.Col12.Add(cAnalizada);
                        if (contemplaPleno && cAnalizada.Length == 12)
                        {
                            posiblesPremios.Col14.Add(cAnalizada);
                        }
                        break;
                    case 13:
                        posiblesPremios.Col13.Add(cAnalizada);
                        if (contemplaPleno && cAnalizada.Length == 13)
                        {
                            posiblesPremios.Col14.Add(cAnalizada);
                        }
                        break;
                    case 14:
                        posiblesPremios.Col14.Add(cAnalizada);
                        if (contemplaPleno && cAnalizada.Length == 14)
                        {
                            posiblesPremios.Col14.Add(cAnalizada);
                        }
                        break;
                    case 15:
                        posiblesPremios.Col15.Add(cAnalizada);
                        if (contemplaPleno && cAnalizada.Length == 15)
                        {
                            posiblesPremios.Col14.Add(cAnalizada);
                        }
                        break;
                    case 16:
                        posiblesPremios.Col16.Add(cAnalizada);
                        if (contemplaPleno && cAnalizada.Length == 16)
                        {
                            posiblesPremios.Col14.Add(cAnalizada);
                        }
                        break;
                }
            }
        }
        _listaResumen.Add(posiblesPremios);
    }

    /// <summary>Ordena el resumen por premios. Legacy: OrdenarResumen() -> resumen.Sort(new PosiblesPremiosComparer()).</summary>
    private void OrdenarResumen()
    {
        _listaResumen.Sort(new PosiblesPremiosComparer());
    }

    /// <summary>
    /// Construye el texto del resumen. Legacy: MejoresOpcionesFrm.MostrarResumen(int limiteResultados).
    /// </summary>
    private string MostrarResumen(int limiteResultados, string columnaGanadora)
    {
        var sb = new StringBuilder();
        sb.Append("Mis mejores opciones son: \r\n");
        sb.Append("------------------------\r\n");
        sb.Append("\r\n");
        for (int i = 0; i < _listaResumen.Count; i++)
        {
            if (i < limiteResultados)
            {
                PosiblesPremiosContenedor contenedor = _listaResumen[i];
                sb.Append(contenedor.ColGanadora + ": ");
                if (columnaGanadora.Length >= 16)
                {
                    sb.Append(contenedor.Col16.Count.ToString() + " de 16 + ");
                }
                if (columnaGanadora.Length >= 15)
                {
                    sb.Append(contenedor.Col15.Count.ToString() + " de 15 + ");
                }
                if (columnaGanadora.Length >= 14)
                {
                    sb.Append(contenedor.Col14.Count.ToString() + " de 14 + ");
                }
                if (columnaGanadora.Length >= 13)
                {
                    sb.Append(contenedor.Col13.Count.ToString() + " de 13 + ");
                }
                if (columnaGanadora.Length >= 12)
                {
                    sb.Append(contenedor.Col12.Count.ToString() + " de 12 + ");
                }
                if (columnaGanadora.Length >= 11)
                {
                    sb.Append(contenedor.Col11.Count.ToString() + " de 11 + ");
                }
                if (columnaGanadora.Length >= 10)
                {
                    sb.Append(contenedor.Col10.Count + " de 10");
                }
                sb.Append("\r\n");
            }
            else
            {
                break;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Devuelve la lista de numeros de partido marcados como involucrados.
    /// Auxiliar para cuando se porte el dominio (legacy: bool[] partidosInvolucrados).
    /// </summary>
    public IReadOnlyList<int> PartidosInvolucrados()
    {
        var marcados = new List<int>();
        foreach (var p in Partidos)
        {
            if (p.Involucrado && int.TryParse(p.Numero, out int n))
            {
                marcados.Add(n);
            }
        }
        return marcados;
    }
}
