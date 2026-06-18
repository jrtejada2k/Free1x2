using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Cambiar Puntuación" (legacy: CambioPuntosFrm).
/// Gestiona los puntos asignados a Fijos, Dobles y Triples al evaluar columnas.
/// Lee/escribe vía Free1X2.EntradaSalida.AConfiguracion (ObtenPuntosCP/GuardarPuntosCP).
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
    /// Legacy: AConfiguracion.ObtenPuntosCP(ref valorFijos, ref valorDobles, ref valorTriples).
    /// </summary>
    public void CargarPuntos()
    {
        try
        {
            int fijos = 0, dobles = 0, triples = 0;
            new AConfiguracion().ObtenPuntosCP(ref fijos, ref dobles, ref triples);
            ValorFijos = fijos;
            ValorDobles = dobles;
            ValorTriples = triples;
        }
        catch
        {
            // Sin archivo de configuración accesible: valores por defecto.
            ValorFijos = 0;
            ValorDobles = 0;
            ValorTriples = 0;
        }
    }

    /// <summary>
    /// Persiste los valores actuales de puntuación.
    /// Legacy: AConfiguracion.GuardarPuntosCP(valorFijos, valorDobles, valorTriples).
    /// </summary>
    [RelayCommand]
    private void Guardar()
    {
        try
        {
            new AConfiguracion().GuardarPuntosCP((int)ValorFijos, (int)ValorDobles, (int)ValorTriples);
            Free1X2.Abstractions.UserDialogs.ShowInfo("Puntuación guardada.");
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudo guardar la puntuación: " + ex.Message);
        }
    }
}
