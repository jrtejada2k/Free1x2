using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Copiar Datos" (WinForms <c>CopiarDatosCPFrm</c>).
/// Define un rango de columnas [Desde, Hasta] a copiar. El rango original se
/// recibía en el constructor del form como (desde, max) y los dos
/// <c>NumericUpDown</c> estaban encadenados: el mínimo de udMax sigue al valor de
/// udMin, y el máximo de udMin sigue al valor de udMax (no se cruzan).
/// </summary>
public partial class CopiarDatosCPFrmViewModel : ObservableObject
{
    // ===== Desde (udMin) =====
    // NumberBox.Value es double; la propiedad del VM debe ser double (regla anti-crash 7).
    [ObservableProperty]
    private double _desde = 1;

    // ===== Hasta (udMax) =====
    [ObservableProperty]
    private double _hasta = 1;

    // Máximo absoluto de "Hasta" (udMax.Maximum = max), fijado al inicializar.
    [ObservableProperty]
    private double _hastaMaximo = 1;

    // Mínimo dinámico de "Hasta": no puede ser menor que "Desde" (udMax.Minimum = udMin.Value).
    [ObservableProperty]
    private double _hastaMinimo = 1;

    // Máximo dinámico de "Desde": no puede ser mayor que "Hasta" (udMin.Maximum = udMax.Value).
    [ObservableProperty]
    private double _desdeMaximo = 1;

    // Mínimo de "Desde" (udMin.Minimum). El form lo bajaba a -1 al cancelar como bandera.
    [ObservableProperty]
    private double _desdeMinimo = 1;

    // ===== Estado =====
    [ObservableProperty]
    private string _estado = "Preparado";

    /// <summary>
    /// Inicializa el rango. Equivale al constructor <c>CopiarDatosCPFrm(long desde, long max)</c>:
    /// udMin va de 1..max con valor inicial = desde; udMax va de desde..max con valor inicial = max.
    /// </summary>
    public void Inicializar(double desde, double max)
    {
        Desde = desde;
        Hasta = max;
        HastaMaximo = max;
        HastaMinimo = desde;
        DesdeMaximo = max;
    }

    // udMin_ValueChanged: udMax.Minimum = udMin.Value.
    partial void OnDesdeChanged(double value)
    {
        HastaMinimo = value;
    }

    // udMax_ValueChanged: udMin.Maximum = udMax.Value.
    partial void OnHastaChanged(double value)
    {
        DesdeMaximo = value;
    }

    /// <summary>
    /// Equivale a <c>btnCopiar_Click</c> del WinForms (Hide()): acepta el rango elegido.
    /// El resultado son las propiedades <see cref="Desde"/> y <see cref="Hasta"/>.
    /// </summary>
    [RelayCommand]
    private void Copiar()
    {
        // TODO(dominio): el WinForms ocultaba el diálogo (Hide()) y el form llamador leía
        //   las propiedades Desde/Hasta (Convert.ToInt16(udMin/udMax.Value)) para copiar el
        //   rango de columnas. Conectar aquí con quien consuma este rango (p. ej. el flujo de
        //   copia de columnas/contraprueba que abría Free1X2.UI.Filtros.CopiarDatosCPFrm).
        Estado = $"Copiar columnas {(int)Desde} a {(int)Hasta}";
    }

    /// <summary>
    /// Equivale a <c>btnCancelar_Click</c> del WinForms: señala cancelación.
    /// El form ponía Desde = -1 (udMin.Minimum = -1; Desde = -1) como bandera de cancelado.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): el WinForms marcaba Desde = -1 como señal de cancelación y cerraba
        //   el diálogo (Close()). Conectar con la navegación/cierre de la Page anfitriona.
        DesdeMinimo = -1;
        Desde = -1;
        Estado = "Cancelado";
    }
}
