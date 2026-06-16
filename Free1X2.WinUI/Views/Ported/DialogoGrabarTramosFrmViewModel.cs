using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para el diálogo "Grabar Tramos" (legacy: Free1X2.UI.DialogoGrabarTramosFrm).
/// Recoge un rango de columnas (inicial/final) y un patrón de grabado:
/// "grabar N columnas de cada P columnas".
///
/// Contrato con el llamador (legacy: TramificarForm.DialogoGuardar, línea 2627):
/// tras pulsar "Aceptar", el llamador lee las propiedades públicas
/// <see cref="ColumnaInicial"/>, <see cref="ColumnaFinal"/>, <see cref="NumColsPorPaso"/>
/// y <see cref="Paso"/> (los mismos campos públicos int del Form legacy) y abre el
/// SaveFileDialog para el archivo de salida.
/// </summary>
public partial class DialogoGrabarTramosFrmViewModel : ObservableObject
{
    // Legacy: txColumnaInicial -> int ColumnaInicial. NumberBox.Value es double.
    [ObservableProperty]
    private double _columnaInicialEntrada;

    // Legacy: txColumnaFinal -> int ColumnaFinal.
    [ObservableProperty]
    private double _columnaFinalEntrada;

    // Legacy: txNunCols -> int NumColsPorPaso (cuántas columnas grabar por paso).
    [ObservableProperty]
    private double _numColsPorPasoEntrada = 1;

    // Legacy: txPaso -> int Paso (tamaño del paso).
    [ObservableProperty]
    private double _pasoEntrada = 1;

    // --- Resultado (campos públicos int del Form legacy, leídos por el llamador) ---

    /// <summary>Legacy: DialogoGrabarTramosFrm.ColumnaInicial (Convert.ToInt32 de txColumnaInicial).</summary>
    public int ColumnaInicial { get; private set; }

    /// <summary>Legacy: DialogoGrabarTramosFrm.ColumnaFinal (Convert.ToInt32 de txColumnaFinal).</summary>
    public int ColumnaFinal { get; private set; }

    /// <summary>Legacy: DialogoGrabarTramosFrm.NumColsPorPaso (Convert.ToInt32 de txNunCols).</summary>
    public int NumColsPorPaso { get; private set; }

    /// <summary>Legacy: DialogoGrabarTramosFrm.Paso (Convert.ToInt32 de txPaso).</summary>
    public int Paso { get; private set; }

    /// <summary>True si el usuario pulsó "Aceptar" (equivale a que el diálogo no se canceló).</summary>
    public bool Aceptado { get; private set; }

    /// <summary>
    /// Permite al llamador inicializar el rango (legacy: ctor DialogoGrabarTramosFrm(c1, c2)
    /// que asigna txColumnaInicial.Text / txColumnaFinal.Text).
    /// </summary>
    public void Inicializar(int columnaInicial, int columnaFinal)
    {
        ColumnaInicialEntrada = columnaInicial;
        ColumnaFinalEntrada = columnaFinal;
    }

    /// <summary>
    /// Se dispara cuando el usuario acepta el diálogo, para que el host (ContentDialog/Page)
    /// lo cierre y continúe con el SaveFileDialog. Análogo a Close() del Form legacy.
    /// </summary>
    public event EventHandler? CierreSolicitado;

    // Acción del botón "Aceptar" (legacy: btGrabar_Click).
    [RelayCommand]
    private void Grabar()
    {
        // Legacy btGrabar_Click: ColumnaInicial = Convert.ToInt32(txColumnaInicial.Text); etc.
        // Los NumberBox entregan double; el Form legacy usa ints, así que se trunca.
        ColumnaInicial = (int)ColumnaInicialEntrada;
        ColumnaFinal = (int)ColumnaFinalEntrada;
        NumColsPorPaso = (int)NumColsPorPasoEntrada;
        Paso = (int)PasoEntrada;
        Aceptado = true;

        // Legacy: Close(); el llamador lee luego los campos públicos.
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }
}
