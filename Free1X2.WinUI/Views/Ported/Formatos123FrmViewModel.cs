using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una fila de formato 123 dentro de la lista (equivale al control legacy
/// Free1X2.UI.Controls.CtrlFormato123).
/// </summary>
public partial class Formato123Item : ObservableObject
{
    [ObservableProperty]
    private string _numero = string.Empty;

    [ObservableProperty]
    private string _formato = string.Empty;

    [ObservableProperty]
    private string _aciertosMin = string.Empty;

    [ObservableProperty]
    private string _aciertosMax = string.Empty;
}

/// <summary>
/// ViewModel portado de Free1X2.UI.Filtros.Formatos123Frm (WinForms).
/// Replica los campos de entrada: lista de formatos 123 (con min/max aciertos),
/// líneas acertadas, "ignorar repeticiones" y el traductor 1X2 -> 123.
/// La lógica de dominio (FiltroFormatos123, TransformarValoracion, persistencia,
/// estadísticas, analizador) queda como TODO citando la clase legacy.
/// </summary>
public partial class Formatos123FrmViewModel : ObservableObject
{
    public Formatos123FrmViewModel()
    {
        // Legacy: LlenarControles(100) precargaba 100 filas vacías.
        // Aquí arrancamos con unas pocas filas editables; AgregarFila añade más.
        for (int i = 1; i <= 8; i++)
        {
            Formatos.Add(new Formato123Item { Numero = i.ToString() });
        }
    }

    /// <summary>
    /// Filas de formato (legacy: cctrl con N CtrlFormato123).
    /// </summary>
    public ObservableCollection<Formato123Item> Formatos { get; } = new();

    /// <summary>
    /// Legacy: txtAciertos. Lista de líneas acertadas permitidas (ej. "10,12-14").
    /// </summary>
    [ObservableProperty]
    private string _aciertosPermitidos = string.Empty;

    /// <summary>
    /// Legacy: chckPasoFijo "Ignorar Repeticiones". Cuando está activo, la
    /// etiqueta de aciertos pasa de "Líneas Acertadas" a "Aciertos Globales"
    /// y se ignoran los min/max por fila.
    /// </summary>
    [ObservableProperty]
    private bool _ignorarRepeticiones;

    /// <summary>
    /// Etiqueta dinámica del grupo de aciertos (legacy: label41 cambia de texto
    /// en DeterminarRepeticiones).
    /// </summary>
    public string EtiquetaAciertos =>
        IgnorarRepeticiones ? "Aciertos Globales" : "Líneas Acertadas";

    partial void OnIgnorarRepeticionesChanged(bool value)
    {
        OnPropertyChanged(nameof(EtiquetaAciertos));
    }

    /// <summary>
    /// Legacy: txtColumna1x2 (entrada del traductor).
    /// </summary>
    [ObservableProperty]
    private string _columna1X2 = string.Empty;

    /// <summary>
    /// Legacy: txtColumna123 (salida del traductor, solo lectura).
    /// </summary>
    [ObservableProperty]
    private string _columna123 = string.Empty;

    [RelayCommand]
    private void AgregarFila()
    {
        // Legacy: Añadir_Enter agregaba un CtrlFormato123 nuevo al final.
        Formatos.Add(new Formato123Item { Numero = (Formatos.Count + 1).ToString() });
    }

    [RelayCommand]
    private void Traducir()
    {
        // TODO (dominio): legacy btnTraducir_Click.
        // Validar con FiltroFormatos123.CompruebaColumna y llamar a
        // FiltroFormatos123.Col1X2ToCol123(Columna1X2) usando la valoración
        // transformada (TransformarValoracion sobre controlPorcentajes1.Valores).
        Columna123 = string.Empty;
    }

    [RelayCommand]
    private void Analizar()
    {
        // TODO (dominio): legacy btnAnalisis_Click.
        // Abre AnalisisFormatos123Frm con filtro.ArrayFormatos (requiere
        // que el filtro tenga formatos guardados).
    }

    [RelayCommand]
    private void Aceptar()
    {
        // TODO (dominio): legacy menuCondiciones1_BOk.
        // CompruebaEntradas + ObtenerAciertosPermitidos, volcar a FiltroFormatos123,
        // ActivaFiltro en el Grupo y cerrar la ventana.
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO (dominio): legacy menuCondiciones2_BEstadisticas.
        // ObtenerFiltroTemporal -> CalculadorEstadisticas.EstadisticasFiltro
        // -> abrir VisorEstadisticas.
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO (dominio): legacy menuCondiciones1_BGuardar.
        // ActualizarDatos + ArchivoCondiciones.GuardaArchivo(filtro) (*.123/*.xml).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO (dominio): legacy menuCondiciones1_BAbrir.
        // ArchivoCondiciones.AbrirArchivoCombinacion + LeeCondicion -> MarcarValores.
    }

    [RelayCommand]
    private void Borrar()
    {
        // TODO (dominio): legacy menuCondiciones1_BBorrar (confirmación + reset filtro).
        Formatos.Clear();
        for (int i = 1; i <= 8; i++)
        {
            Formatos.Add(new Formato123Item { Numero = i.ToString() });
        }
        AciertosPermitidos = string.Empty;
        IgnorarRepeticiones = false;
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO (dominio): legacy menuCondiciones1_BCancelar -> CerrarVentana().
    }
}
