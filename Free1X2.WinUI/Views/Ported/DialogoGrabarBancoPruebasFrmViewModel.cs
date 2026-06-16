using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para el diálogo "Grabar apuestas" (legacy: Free1X2.UI.DialogoGrabarBancoPruebasFrm).
///
/// Es un diálogo recolector de parámetros: recoge el rango de filas a grabar
/// (inicial/final), el número máximo de apuestas y si solo deben grabarse las apuestas
/// seleccionadas. NO ejecuta la grabación: igual que en WinForms, la exportación real la
/// efectúa el formulario llamante (BancoPruebasFrm) leyendo los campos públicos del diálogo.
///
/// Contrato con el llamador (legacy: BancoPruebasFrm lee FilaInicial / FilaFinal /
/// NumMaxColumnas / SoloSeleccionadas / Cancelado tras cerrar el diálogo). El ctor legacy
/// DialogoGrabarBancoPruebasFrm(c1, c2, c) inicializa los tres campos; aquí lo hace Inicializar.
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

    // ===== Resultado leído por el llamador (campos públicos int del Form legacy) =====

    /// <summary>Legacy DialogoGrabarBancoPruebasFrm.FilaInicial (Convert.ToInt32 de txColumnaInicial).</summary>
    public int FilaInicialResultado { get; private set; }

    /// <summary>Legacy DialogoGrabarBancoPruebasFrm.FilaFinal (Convert.ToInt32 de txColumnaFinal).</summary>
    public int FilaFinalResultado { get; private set; }

    /// <summary>Legacy DialogoGrabarBancoPruebasFrm.NumMaxColumnas (Convert.ToInt32 de txNumColumns).</summary>
    public int NumMaxColumnasResultado { get; private set; }

    /// <summary>True si el usuario pulsó "Aceptar" (equivale a Cancelado == false del legacy).</summary>
    public bool Aceptado { get; private set; }

    /// <summary>
    /// Se dispara al aceptar/cancelar para que el host cierre el diálogo y el llamante
    /// (BancoPruebasFrm) lea los resultados. Análogo a Close() del Form legacy.
    /// </summary>
    public event EventHandler? CierreSolicitado;

    /// <summary>
    /// Inicializa los tres campos (legacy: ctor DialogoGrabarBancoPruebasFrm(c1, c2, c) que
    /// asigna txColumnaInicial / txColumnaFinal / txNumColumns).
    /// </summary>
    public void Inicializar(int filaInicial, int filaFinal, int numMaxColumnas)
    {
        FilaInicial = filaInicial;
        FilaFinal = filaFinal;
        NumMaxColumnas = numMaxColumnas;
    }

    /// <summary>
    /// Equivale a <c>btGrabar_Click</c> del WinForms: recoge los valores (truncando a int como
    /// hacía Convert.ToInt32), marca Cancelado = false y cierra. La grabación la hace el llamante.
    /// </summary>
    [RelayCommand]
    private void Grabar()
    {
        // Validación básica equivalente a la semántica del diálogo (rango coherente).
        if (FilaInicial > FilaFinal)
        {
            AppServices.MostrarError("La fila inicial no puede ser mayor que la fila final.");
            return;
        }

        // Legacy btGrabar_Click: FilaInicial = Convert.ToInt32(txColumnaInicial.Text); etc.
        FilaInicialResultado = (int)FilaInicial;
        FilaFinalResultado = (int)FilaFinal;
        NumMaxColumnasResultado = (int)NumMaxColumnas;
        Cancelado = false;
        Aceptado = true;
        Estado = "Aceptado";

        // Legacy: Close(); el llamante (BancoPruebasFrm) lee los campos y exporta las columnas.
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Equivale a <c>btCancelar_Click</c> del WinForms: marca Cancelado = true y cierra.
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // Legacy btCancelar_Click: Cancelado = true; Close();
        Cancelado = true;
        Aceptado = false;
        Estado = "Cancelado";
        CierreSolicitado?.Invoke(this, EventArgs.Empty);
    }
}
