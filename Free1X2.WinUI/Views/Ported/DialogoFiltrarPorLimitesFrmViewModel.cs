using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Filtrar por límites" (legacy: DialogoFiltrarPorLimitesFrm).
///
/// El formulario legacy trabaja sobre una matriz <c>int[10,4]</c> llamada <c>extremos</c>,
/// con 10 filas (rangos de aciertos) y 4 columnas por fila:
///   [,0] posición inicial del rango de aciertos (lblextremoN)
///   [,1] posición final del rango de aciertos (lblextremoNd)
///   [,2] número mínimo de diferencias a eliminar (txdifN)
///   [,3] número máximo de diferencias a eliminar (txdifNd)
///
/// En el legacy las columnas [,0]/[,1] (rango de aciertos) se muestran como texto
/// (TextBox de solo lectura, calculadas por CoherenciarExtremos) y solo las columnas
/// [,2]/[,3] (diferencias) son realmente editables por el usuario.
/// Aquí se replica ese propósito: el rango se muestra como texto y las diferencias
/// se editan con NumberBox.
/// </summary>
public partial class DialogoFiltrarPorLimitesFrmViewModel : ObservableObject
{
    // La fila de la matriz 'extremos' se define como clase pública de nivel superior
    // en FilaLimite.cs (necesario para x:DataType="local:FilaLimite" en el DataTemplate).
    public IReadOnlyList<FilaLimite> Filas { get; }

    public DialogoFiltrarPorLimitesFrmViewModel()
    {
        // TODO[dominio]: poblar las filas a partir de la matriz int[10,4] 'extremos'
        //   que el legacy DialogoFiltrarPorLimitesFrm recibe por constructor.
        //   - extremos[i,0]/[i,1] -> rango de aciertos (texto)
        //   - extremos[i,2]/[i,3] -> DifMin/DifMax (editables)
        //   Antes de mostrar, el legacy llama CoherenciarExtremos() para garantizar
        //   que extremos[i+1,0] >= extremos[i,0]+1. Esa lógica de dominio NO se implementa aquí.
        //   Los valores siguientes son los valores por defecto del Designer legacy.
        Filas = new List<FilaLimite>
        {
            new("Rango de aciertos < 10", "0 a 0", 0, 4),
            new("de 10 a < 11",           "1 a 0", 1, 3),
            new("de 11 a < 12",           "1 a 0", 1, 2),
            new("de 12 a < 13",           "1 a 0", 1, 1),
            new("de < 13 a 12",           "1 a 0", 1, 0),
            new("de < 12 a 11",           "1 a 0", 1, 0),
            new("de < 11 a 10",           "1 a 0", 1, 1),
            new("< 10",                   "1 a 0", 1, 2),
            new("fila 9",                 "1 a 0", 0, 0),
            new("fila 10",                "1 a 0", 0, 0),
        };
    }

    /// <summary>
    /// Equivalente a btAceptar_Click del legacy: vuelca los valores de UI a la matriz
    /// 'extremos', re-coherencia y marca ValoresAceptados = true.
    /// </summary>
    [RelayCommand]
    private void Aceptar()
    {
        // TODO[dominio]: replicar DialogoFiltrarPorLimitesFrm.PonerTextoEnVariables() +
        //   CoherenciarExtremos() volcando Filas[i].DifMin/DifMax (double -> int) a
        //   extremos[i,2]/[i,3], y marcar ValoresAceptados = true para el caller.
    }
}
