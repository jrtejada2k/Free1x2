using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

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
/// </summary>
public partial class MejoresOpcionesFrmViewModel : ObservableObject
{
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
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: portar el calculo de "mejores opciones".
        //   Legacy: MejoresOpcionesFrm.button1_Click
        //     1. resumen.Clear(); cGanadoras.Clear();
        //     2. ObtenGanadoras("", 0)  -> genera recursivamente todas las columnas
        //        ganadoras posibles. Para cada posicion con '*' en ColumnaGanadora,
        //        si el partido esta marcado (Partidos[i].Involucrado) expande a {1,X,2};
        //        si no, fija '*'. (usa columnaGanadora, ColumnaGanadora.Length, los flags
        //        partidosInvolucrados[] = Partidos[i].Involucrado).
        //     3. for cada cGanadoras[i]: ObtenerResumen(cGanadora)
        //        -> escruta archivoColumnas (ArchivoColumnas) con Escrutar(...) y acumula
        //           aciertos en un PosiblesPremiosContenedor (Col10..Col16), respetando
        //           ContemplaPleno (premio al pleno al 15).
        //     4. OrdenarResumen() -> resumen.Sort(new PosiblesPremiosComparer());
        //     5. MostrarResumen(ObtenerLimiteResultados())
        //        -> construye el texto: por cada contenedor (hasta el limite) imprime
        //           "<ColGanadora>: N de 16 + N de 15 + ... + N de 10".
        //   Dependencias legacy: Free1X2.Escrutinio.PosiblesPremiosContenedor,
        //   PosiblesPremiosComparer; entradas ColumnaGanadora (string con '*'),
        //   ArchivoColumnas (List<string>), ContemplaPleno (bool).
        //   El resultado final debe asignarse a la propiedad Resumen.

        // Validacion equivalente a ObtenerLimiteResultados (legacy): si el limite no es
        // valido, restablecer a 10.
        if (LimiteResultados < 1)
        {
            LimiteResultados = 10;
        }

        Resumen = string.Empty;
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
