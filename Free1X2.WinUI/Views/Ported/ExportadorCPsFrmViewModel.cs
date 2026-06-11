using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Exportador de Columnas Probables".
/// Replica el WinForms <c>ExportadorCPsFrm</c>, un diálogo sin campos de entrada
/// que recibe una lista de <c>ColumnaProbable</c> y ofrece dos formas de
/// exportarla a fichero (más cancelar):
///   - Simple: sólo las columnas/pronósticos a un .txt (btnExportarSimples).
///   - Con aciertos: pronósticos + aciertos, aciertos seguidos y fallos seguidos
///     a un .clm (btnExportarClm).
/// La selección de fichero (SaveFileDialog) y la escritura quedan como TODO
/// hasta portar el dominio (MotorCalculo.ColumnaProbable / EntradaSalida).
/// </summary>
public partial class ExportadorCPsFrmViewModel : ObservableObject
{
    // ===== Estado / feedback (no existía en el WinForms; sustituye al cierre del diálogo) =====
    [ObservableProperty]
    private string _estado = "Elige el formato de exportación de las columnas probables.";

    // TODO(dominio): el constructor del WinForms recibía
    //   List<ColumnaProbable> lista (las columnas a exportar). Aquí debería
    //   inyectarse/recibirse esa lista al navegar a la página. Se expone el
    //   número de columnas como texto para mostrarlo en la cabecera.
    [ObservableProperty]
    private string _numeroColumnasTexto = "0 columnas para exportar.";

    public ExportadorCPsFrmViewModel()
    {
        // TODO(dominio): recibir la List<ColumnaProbable> de origen (el WinForms la
        //   pasaba por constructor: ExportadorCPsFrm(List<ColumnaProbable> lista))
        //   y actualizar NumeroColumnasTexto con lista.Count.
    }

    /// <summary>
    /// Equivale a <c>btnExportarSimples_Click</c> del WinForms: exporta sólo las
    /// columnas (PronosticosString) a un fichero de texto (*.txt).
    /// </summary>
    [RelayCommand]
    private void ExportarSimples()
    {
        // TODO(dominio): portar ExportadorCPsFrm.btnExportarSimples_Click():
        //   - Abrir un selector de fichero (equivalente a SaveFileDialog,
        //     InitialDirectory "Columnas\\", filtro "Columnas Simples (*.txt)").
        //   - Construir string[] columnas con lista[i].PronosticosString.
        //   - Usar EntradaSalida.IArchivoColumnas / ArchivoColumnasTexto:
        //       comBaseCols.GuardarTodasCols(columnas, true); comBaseCols.Cerrar();
        //   - Cerrar la página al terminar.
        Estado = "Exportar columnas simples a .txt (pendiente de portar dominio).";
    }

    /// <summary>
    /// Equivale a <c>btnExportarClm_Click</c> del WinForms: exporta cada columna con
    /// sus aciertos (Ac), aciertos seguidos (Acs) y fallos seguidos (Fs) a un *.clm.
    /// </summary>
    [RelayCommand]
    private void ExportarConAciertos()
    {
        // TODO(dominio): portar ExportadorCPsFrm.btnExportarClm_Click():
        //   - Abrir selector de fichero (SaveFileDialog, "Columnas\\",
        //     filtro "Columnas Con Aciertos (*.clm)").
        //   - Por cada ColumnaProbable cp escribir una línea:
        //       cp.PronosticosString + "#" + cp.GetAciertos() + "#" +
        //       cp.GetAciertosSeguidos() + "#" + cp.GetFallosSeguidos()
        //     (StreamWriter por línea, como en el WinForms).
        //   - Cerrar la página al terminar.
        Estado = "Exportar columnas con aciertos a .clm (pendiente de portar dominio).";
    }

    /// <summary>
    /// Equivale a <c>btnCancelar_Click</c> del WinForms (Close()).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio/navegación): cerrar/navegar atrás. En el WinForms era Close().
        Estado = "Cancelado.";
    }
}
