// Free1X2 · WinUI 3 — WIN3
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Conversión entre la colección editable de <see cref="FilaPorcentaje"/> (lo que muestra
/// <see cref="PorcentajesControl"/>) y la matriz <c>double[N,3]</c> que consume el motor —
/// el mismo contrato que <c>valors.RetVals()</c> y <c>ControlPorcentajes.Valores</c> del WinForms.
/// </summary>
public static class PorcentajesHelper
{
    /// <summary>Crea una colección de <paramref name="n"/> filas (partidos 1..n) a cero.</summary>
    public static ObservableCollection<FilaPorcentaje> Crear(int n)
    {
        var col = new ObservableCollection<FilaPorcentaje>();
        for (int i = 0; i < n; i++) col.Add(new FilaPorcentaje(i + 1));
        return col;
    }

    /// <summary>Vuelca las filas a una matriz <c>double[N,3]</c> (equivale a RetVals/Valores get).</summary>
    public static double[,] AMatriz(IEnumerable<FilaPorcentaje> filas)
    {
        var lista = new List<FilaPorcentaje>(filas);
        var m = new double[lista.Count, 3];
        for (int i = 0; i < lista.Count; i++)
        {
            m[i, 0] = lista[i].Uno;
            m[i, 1] = lista[i].Equis;
            m[i, 2] = lista[i].Dos;
        }
        return m;
    }

    /// <summary>Carga una matriz <c>double[N,3]</c> en una colección existente (equivale al set de Valores).</summary>
    public static void CargarMatriz(ObservableCollection<FilaPorcentaje> destino, double[,] m)
    {
        destino.Clear();
        for (int i = 0; i < m.GetLength(0); i++)
            destino.Add(new FilaPorcentaje(i + 1, m[i, 0], m[i, 1], m[i, 2]));
    }
}
