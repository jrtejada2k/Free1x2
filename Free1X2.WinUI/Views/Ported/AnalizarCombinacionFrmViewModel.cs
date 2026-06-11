using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Free1X2.WinUI.Controls;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Estado de evaluación de una condición/nodo del árbol de análisis.
/// Réplica de los 3 iconos de la leyenda del WinForms "AnalizarCombinacionFrm":
/// ok.gif (acertada), exclamacion_blanco.bmp (aceptada por tolerancias) y cancelar.gif (fallada).
/// </summary>
public enum EstadoCondicion
{
    /// <summary>Condición acertada (icono ok.gif).</summary>
    Acertada,
    /// <summary>Condición aceptada por tolerancias (icono exclamacion_blanco.bmp).</summary>
    AceptadaPorTolerancias,
    /// <summary>Condición fallada (icono cancelar.gif).</summary>
    Fallada
}

/// <summary>
/// Nodo del árbol de análisis. Cada nodo representa una condición o filtro evaluado
/// contra la combinación, con su estado y posibles sub-condiciones (jerarquía del TreeView legacy).
/// </summary>
public partial class NodoAnalisisViewModel : ObservableObject
{
    [ObservableProperty]
    private string _titulo = string.Empty;

    [ObservableProperty]
    private EstadoCondicion _estado = EstadoCondicion.Acertada;

    /// <summary>Sub-nodos (hijos del nodo en el TreeView original).</summary>
    public ObservableCollection<NodoAnalisisViewModel> Hijos { get; } = new();

    /// <summary>Texto accesible del estado para AutomationProperties.</summary>
    public string EstadoTexto => Estado switch
    {
        EstadoCondicion.Acertada => "Condición acertada",
        EstadoCondicion.AceptadaPorTolerancias => "Condición aceptada por tolerancias",
        _ => "Condición fallada"
    };

    /// <summary>Estado del semáforo equivalente para el indicador visual del nodo.</summary>
    public EstadoSemaforo Semaforo => Estado switch
    {
        EstadoCondicion.Acertada => EstadoSemaforo.Verde,
        EstadoCondicion.AceptadaPorTolerancias => EstadoSemaforo.Neutro,
        _ => EstadoSemaforo.Rojo
    };
}

/// <summary>
/// ViewModel de la página portada del WinForms "AnalizarCombinacionFrm".
/// Es un visor de resultados (sólo lectura): muestra el árbol de condiciones/filtros
/// evaluados contra una combinación y su estado (acertada / aceptada por tolerancias / fallada).
/// El árbol original (TreeView treeView1) se rellena desde la lógica de dominio.
/// </summary>
public partial class AnalizarCombinacionFrmViewModel : ObservableObject
{
    /// <summary>
    /// Raíz del árbol de análisis (equivale a treeView1.Nodes del WinForms).
    /// </summary>
    public ObservableCollection<NodoAnalisisViewModel> Nodos { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MensajeVacioVisibility))]
    private bool _tieneResultados;

    /// <summary>
    /// Visibilidad del mensaje "sin resultados": visible mientras el árbol esté vacío.
    /// Se expone como <see cref="Visibility"/> para no bindear un bool directo a la UI.
    /// </summary>
    public Visibility MensajeVacioVisibility =>
        TieneResultados ? Visibility.Collapsed : Visibility.Visible;

    public AnalizarCombinacionFrmViewModel()
    {
        // TODO[dominio]: poblar 'Nodos' a partir de la evaluación de la combinación.
        // En el WinForms legacy (Free1X2.UI.AnalizarCombinacionFrm) el árbol 'treeView1'
        // se rellenaba externamente: cada nodo recibía un ImageIndex que mapeaba a uno de
        // los 3 estados de la leyenda (ok / exclamación / cancelar).
        // La construcción de los nodos y la asignación de estado depende del motor de
        // cálculo/escrutinio del dominio (aún no portado a Free1X2.Domain).
    }
}
