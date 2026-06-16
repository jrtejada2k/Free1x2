using CommunityToolkit.Mvvm.ComponentModel;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de la matriz <c>extremos</c> del legacy DialogoFiltrarPorLimitesFrm:
/// rango de aciertos (solo lectura, texto) y los límites de diferencias
/// mínimo/máximo (editables).
///
/// Se define como clase pública de nivel superior (no anidada) para que el
/// compilador XAML pueda referenciarla con <c>x:DataType="local:FilaLimite"</c>
/// dentro del DataTemplate; el syntax de clase anidada (<c>ViewModel+FilaLimite</c>)
/// genera código .g.cs inválido.
/// </summary>
public partial class FilaLimite : ObservableObject
{
    public FilaLimite(string etiquetaRango, string rangoTexto, double difMin, double difMax)
    {
        EtiquetaRango = etiquetaRango;
        RangoTexto = rangoTexto;
        _difMin = difMin;
        _difMax = difMax;
    }

    public FilaLimite(string etiquetaRango, int posInicial, int posFinal, double difMin, double difMax)
    {
        EtiquetaRango = etiquetaRango;
        PosInicial = posInicial;
        PosFinal = posFinal;
        RangoTexto = posInicial + " a " + posFinal;
        _difMin = difMin;
        _difMax = difMax;
    }

    /// <summary>Descripción del tramo de aciertos (legacy: label6/7/8/11/14/17/20/23/26 + label1/2 cabecera).</summary>
    public string EtiquetaRango { get; }

    /// <summary>Rango de posiciones "X a Y" como texto (legacy: lblextremoN / lblextremoNd, solo lectura).</summary>
    public string RangoTexto { get; }

    /// <summary>Posición inicial del rango de aciertos (legacy: extremos[fila,0], mostrado por lblextremoN).</summary>
    public int PosInicial { get; }

    /// <summary>Posición final del rango de aciertos (legacy: extremos[fila,1], mostrado por lblextremoNd).</summary>
    public int PosFinal { get; }

    // Límite inferior de diferencias a eliminar (legacy: txdifN -> extremos[fila,2]).
    [ObservableProperty]
    private double _difMin;

    // Límite superior de diferencias a eliminar (legacy: txdifNd -> extremos[fila,3]).
    [ObservableProperty]
    private double _difMax;
}
