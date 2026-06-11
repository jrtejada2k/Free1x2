using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Grabar apuestas".
/// Replica los parámetros de entrada del WinForms <c>DialogoGrabarBancoPruebasFrm</c>:
/// un diálogo que recoge el rango de filas a grabar (inicial/final), el número
/// máximo de apuestas y si solo deben grabarse las apuestas seleccionadas, antes
/// de exportar las columnas del Banco de Pruebas a un archivo.
/// </summary>
public partial class DialogoGrabarBancoPruebasFrmViewModel : ObservableObject
{
    // ===== Rango de filas (txColumnaInicial / txColumnaFinal del WinForms) =====
    // FilaInicial: fila de la apuesta inicial. NumberBox.Value es double.
    [ObservableProperty]
    private double _filaInicial = 0;

    // FilaFinal: fila de la apuesta final.
    [ObservableProperty]
    private double _filaFinal = 0;

    // ===== Nº máximo de apuestas (txNumColumns del WinForms) =====
    [ObservableProperty]
    private double _numMaxColumnas = 0;

    // ===== chkSoloSeleccionadas: por defecto activado (Checked en el WinForms) =====
    [ObservableProperty]
    private bool _soloSeleccionadas = true;

    // ===== Estado / resultado de la acción =====
    [ObservableProperty]
    private string _estado = "Preparado";

    // Equivale a Cancelado del WinForms: el diálogo legacy cerraba con Cancelado=true.
    [ObservableProperty]
    private bool _cancelado = false;

    /// <summary>
    /// Equivale a <c>btGrabar_Click</c> del WinForms: confirma el rango y lanza la grabación.
    /// </summary>
    [RelayCommand]
    private void Grabar()
    {
        Cancelado = false;
        Estado = "Grabando";

        // TODO(dominio): portar la lógica de DialogoGrabarBancoPruebasFrm.btGrabar_Click
        //   de Free1X2.UI. El WinForms solo recogía los valores
        //   (FilaInicial, FilaFinal, NumMaxColumnas, SoloSeleccionadas) y cerraba el
        //   diálogo con DialogResult; la grabación real la efectuaba el formulario
        //   llamante BancoPruebasFrm a partir de estos parámetros
        //   (Free1X2.UI.BancoPruebasFrm -> exportación de columnas a archivo).
        //   Validar que FilaInicial <= FilaFinal y NumMaxColumnas > 0 antes de grabar.

        Estado = "Terminado (pendiente de portar dominio)";
    }

    /// <summary>
    /// Equivale a <c>btCancelar_Click</c> del WinForms: cancela el diálogo.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        Cancelado = true;
        Estado = "Cancelado";

        // TODO(dominio): el WinForms cerraba el diálogo (this.Close()) devolviendo
        //   Cancelado=true al BancoPruebasFrm llamante. Aquí no hay diálogo modal;
        //   la navegación/cierre la gestiona el contenedor de páginas WinUI.
    }
}
