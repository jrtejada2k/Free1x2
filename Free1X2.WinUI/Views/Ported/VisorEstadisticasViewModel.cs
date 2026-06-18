// Free1X2 · WinUI 3 — WIN3
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Free1X2.MotorCalculo.Estadisticas;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila del visor de estadísticas (legacy: cada DataRow de la DataTable "Estadísticas"
/// construida en VisorEstadisticas.LlenarEstadisticas, con sus columnas Archivo y
/// Cumplimiento). Los dos campos se exponen como string para enlazarlos directamente
/// a TextBlock.Text en el DataTemplate (regla anti-crash nº 2).
/// </summary>
public sealed class EstadisticaItem
{
    /// <summary>Fichero analizado (legacy: columna "Archivo" = Estadistica.Archivo).</summary>
    public string Archivo { get; set; } = string.Empty;

    /// <summary>
    /// Porcentaje de cumplimiento ya formateado con el símbolo "%"
    /// (legacy: columna "Cumplimiento" = Estadistica.Cumplimiento + "%").
    /// </summary>
    public string Cumplimiento { get; set; } = string.Empty;

    /// <summary>
    /// Valor numérico de cumplimiento (0..100) para la barra de progreso. Es el mismo
    /// Estadistica.Cumplimiento, sólo acotado al rango visible de la barra; no se inventa nada.
    /// </summary>
    public double CumplimientoValor { get; set; }
}

/// <summary>
/// ViewModel para la pantalla "Estadísticas" (legacy WinForms: VisorEstadisticas).
/// Es un visor de solo lectura: recibe una lista de <c>Estadistica</c> y la presenta
/// en una rejilla de dos columnas (Archivo, Cumplimiento). No tiene campos de entrada
/// ni acciones; únicamente mantiene la colección a mostrar.
/// </summary>
public partial class VisorEstadisticasViewModel : ObservableObject
{
    /// <summary>
    /// Handoff estático con la lista de estadísticas que el formulario llamante calcula
    /// (CalculadorEstadisticas.EstadisticasFiltro) y deja para el visor. Equivale al argumento
    /// del ctor legacy VisorEstadisticas(List&lt;Estadistica&gt; est). El visor lo lee al navegar
    /// (mismo patrón que EstucolFrmViewModel.UltimoInforme).
    /// Productores cableados: los VMs de condición (Contactos, Distancias, ...) fijan
    /// UltimasEstadisticas en su comando Estadísticas y navegan aquí.
    /// </summary>
    public static List<Estadistica>? UltimasEstadisticas { get; set; }

    /// <summary>
    /// Colección de estadísticas a mostrar en la rejilla
    /// (legacy: DataSet/DataTable "Estadísticas" enlazado a dgEstadisticas.DataSource,
    /// alimentado por el formulario que abría este diálogo a través del constructor
    /// VisorEstadisticas(List&lt;Estadistica&gt; est)).
    /// </summary>
    public ObservableCollection<EstadisticaItem> Estadisticas { get; } = new();

    /// <summary>
    /// Indica si NO hay estadísticas que mostrar, para alternar el mensaje de estado vacío.
    /// </summary>
    public bool SinDatos => Estadisticas.Count == 0;

    public VisorEstadisticasViewModel()
    {
        // Réplica 1:1 de VisorEstadisticas.LlenarEstadisticas(): por cada Estadistica de la
        // lista, una fila con Archivo y Cumplimiento + "%". La lista llega por el handoff
        // estático (la Page se reconstruye en cada navegación, así que basta con leerlo aquí).
        var lista = UltimasEstadisticas;
        if (lista != null)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                Estadistica estadistica = lista[i];
                double valor = estadistica.Cumplimiento;
                if (valor < 0) valor = 0;
                if (valor > 100) valor = 100;
                Estadisticas.Add(new EstadisticaItem
                {
                    Archivo = estadistica.Archivo,
                    Cumplimiento = estadistica.Cumplimiento + "%",
                    CumplimientoValor = valor,
                });
            }
        }
        OnPropertyChanged(nameof(SinDatos));
    }
}
