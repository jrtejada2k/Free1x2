// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "FrmDependenciaLineal" (título "Dependencia Lineal").
/// Recalcula el signo del partido a tratar como combinación lineal (módulo 3 ó 2) de
/// los signos del resto de partidos ponderados por sus coeficientes, y reescribe el
/// archivo de columnas. La lógica de dominio (lectura/escritura de columnas, cálculo
/// sobre las 4.782.969 combinaciones) queda como TODO.
/// </summary>
public sealed partial class FrmDependenciaLinealPage : Page
{
    public FrmDependenciaLinealViewModel ViewModel { get; } = new();

    public FrmDependenciaLinealPage()
    {
        InitializeComponent();
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }
}
