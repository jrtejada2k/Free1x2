using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

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
        // TODO[dominio]: poblar 'Estadisticas' a partir de la lista de
        //   Free1X2.MotorCalculo.Estadisticas.Estadistica que el formulario llamante
        //   pasaba al constructor legacy VisorEstadisticas(List<Estadistica> est).
        //   Legacy: VisorEstadisticas.LlenarEstadisticas()
        //     - Por cada Estadistica e de la lista:
        //         Estadisticas.Add(new EstadisticaItem
        //         {
        //             Archivo = e.Archivo,
        //             Cumplimiento = e.Cumplimiento + "%"
        //         });
        //   La navegación a esta Page deberá entregar la lista (p. ej. vía parámetro de
        //   Navigate / inyección) ya que aquí no se construye la lógica de dominio.
    }
}
