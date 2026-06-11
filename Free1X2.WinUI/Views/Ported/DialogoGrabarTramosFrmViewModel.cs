using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para el diálogo "Grabar Tramos" (legacy: Free1X2.UI.DialogoGrabarTramosFrm).
/// Recoge un rango de columnas (inicial/final) y un patrón de grabado:
/// "grabar N columnas de cada P columnas".
/// </summary>
public partial class DialogoGrabarTramosFrmViewModel : ObservableObject
{
    // Legacy: txColumnaInicial -> int ColumnaInicial. NumberBox.Value es double.
    [ObservableProperty]
    private double _columnaInicial;

    // Legacy: txColumnaFinal -> int ColumnaFinal.
    [ObservableProperty]
    private double _columnaFinal;

    // Legacy: txNunCols -> int NumColsPorPaso (cuántas columnas grabar por paso).
    [ObservableProperty]
    private double _numColsPorPaso = 1;

    // Legacy: txPaso -> int Paso (tamaño del paso).
    [ObservableProperty]
    private double _paso = 1;

    // Acción del botón "Aceptar" (legacy: btGrabar_Click).
    [RelayCommand]
    private void Grabar()
    {
        // TODO (dominio): replicar Free1X2.UI.DialogoGrabarTramosFrm.btGrabar_Click:
        //   ColumnaInicial = Convert.ToInt32(txColumnaInicial.Text);
        //   ColumnaFinal   = Convert.ToInt32(txColumnaFinal.Text);
        //   NumColsPorPaso = Convert.ToInt32(txNunCols.Text);
        //   Paso           = Convert.ToInt32(txPaso.Text);
        //   Close();  // devolver estos valores al llamador y cerrar el diálogo.
        // En WinUI: convertir los double a int, validar y exponer el resultado
        // (p. ej. mediante un TaskCompletionSource o evento de cierre del ContentDialog/Page).
    }
}
