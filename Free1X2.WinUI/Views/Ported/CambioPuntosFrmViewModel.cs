using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Cambiar Puntuación" (legacy: CambioPuntosFrm).
/// Gestiona los puntos asignados a Fijos, Dobles y Triples al evaluar columnas.
/// </summary>
public partial class CambioPuntosFrmViewModel : ObservableObject
{
    [ObservableProperty]
    private double _valorFijos;

    [ObservableProperty]
    private double _valorDobles;

    [ObservableProperty]
    private double _valorTriples;

    public CambioPuntosFrmViewModel()
    {
        CargarPuntos();
    }

    /// <summary>
    /// Carga los valores actuales de puntuación desde la configuración persistida.
    /// </summary>
    public void CargarPuntos()
    {
        // TODO[dominio]: leer puntuación desde la configuración legacy.
        //   Legacy: Free1X2.EntradaSalida.AConfiguracion.ObtenPuntosCP(ref valorFijos, ref valorDobles, ref valorTriples)
        //   El dominio aún no está migrado a Free1X2.Domain; placeholders por defecto.
        ValorFijos = 0;
        ValorDobles = 0;
        ValorTriples = 0;
    }

    /// <summary>
    /// Persiste los valores actuales de puntuación. Devuelve true si se guardó correctamente.
    /// </summary>
    [RelayCommand]
    private void Guardar()
    {
        // TODO[dominio]: guardar puntuación en la configuración legacy.
        //   Legacy: Free1X2.EntradaSalida.AConfiguracion.GuardarPuntosCP(valorFijos, valorDobles, valorTriples)
        //   Convertir ValorFijos/ValorDobles/ValorTriples (double) a int antes de persistir.
    }
}
