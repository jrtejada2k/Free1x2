using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de la tabla de previsión de premios (solo lectura): categoría, acertantes y premio
/// estimados por el motor. Legacy: labels lblPrevAcertantesNN / lblPrevPremioNN en EstimadorPremiosFrm.
/// </summary>
public partial class PrevisionPremioItem : ObservableObject
{
    [ObservableProperty]
    private string _categoria = string.Empty;

    [ObservableProperty]
    private string _acertantesTexto = "-";

    [ObservableProperty]
    private string _premioTexto = "-";
}

/// <summary>
/// Fila editable del escrutinio real: el usuario introduce acertantes y premio oficiales
/// por categoría. Legacy: textboxes txAcertantesNN / txPremioNN en EstimadorPremiosFrm.
/// </summary>
public partial class EscrutinioRealItem : ObservableObject
{
    [ObservableProperty]
    private string _categoria = string.Empty;

    [ObservableProperty]
    private double _acertantes;

    [ObservableProperty]
    private double _premio;
}

/// <summary>
/// ViewModel para la pantalla "Estimación de premios" (legacy: EstimadorPremiosFrm).
/// Estima los acertantes y premios por categoría (14..10) a partir de recaudación, bote,
/// porcentajes destinados a cada categoría y el reparto de porcentajes de la jornada.
/// Permite además registrar el escrutinio real oficial.
/// </summary>
public partial class EstimadorPremiosFrmViewModel : ObservableObject
{
    // --- Cabecera: temporada / jornada (legacy: txTemporada, txJornada) ---
    [ObservableProperty]
    private string _temporada = string.Empty;

    [ObservableProperty]
    private double _jornada;

    // --- Parámetros económicos (legacy: txRecaudacion, txBote, lbNumApuestas, txPrecioApuesta) ---
    [ObservableProperty]
    private double _recaudacion = 13825260.50;

    [ObservableProperty]
    private double _bote;

    [ObservableProperty]
    private double _precioApuesta = 0.5;

    // Nº de columnas / apuestas (calculado por el dominio). Texto para evitar bind int->TextBlock.
    [ObservableProperty]
    private string _numApuestasTexto = "-";

    // --- Reparto de premios (legacy: txPorcentajeDestinadoAPremios + txPorcentajeParaElNN) ---
    [ObservableProperty]
    private double _porcentajeDestinadoAPremios = 55;

    [ObservableProperty]
    private double _porcentajePara14 = 12;

    [ObservableProperty]
    private double _porcentajePara13 = 8;

    [ObservableProperty]
    private double _porcentajePara12 = 8;

    [ObservableProperty]
    private double _porcentajePara11 = 8;

    [ObservableProperty]
    private double _porcentajePara10 = 9;

    // --- Tablas ---
    public ObservableCollection<PrevisionPremioItem> Prevision { get; } = new();

    public ObservableCollection<EscrutinioRealItem> EscrutinioReal { get; } = new();

    public EstimadorPremiosFrmViewModel()
    {
        var categorias = new[] { "14", "13", "12", "11", "10" };
        foreach (var cat in categorias)
        {
            Prevision.Add(new PrevisionPremioItem { Categoria = cat });
            EscrutinioReal.Add(new EscrutinioRealItem { Categoria = cat });
        }

        // Temporada por defecto según el mes actual (legacy: EstimadorPremiosFrm ctor):
        // mes < 7 -> año-1/año ; en otro caso año/año+1.
        var ahora = DateTime.Now;
        Temporada = ahora.Month < 7
            ? (ahora.Year - 1) + "/" + ahora.Year
            : ahora.Year + "/" + (ahora.Year + 1);
    }

    /// <summary>
    /// Recalcula la previsión de acertantes y premios por categoría.
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO: lógica en Free1X2/UI/EstimadorPremiosFrm.cs línea 1885 (button2_Click).
        //   El cálculo lee la matriz de porcentajes de la jornada del UserControl
        //   Free1X2.UI.Controls.ControlPorcentajes (controlPorcentajes1.Valores, línea 1913),
        //   aún no portado a WinUI, junto con ApuestaProbableCentral/Porcentajes. Sin ese
        //   control no hay datos de entrada que cablear al motor sin inventar valores.
    }

    /// <summary>
    /// Navega a la jornada anterior cargando sus datos. Legacy: btJornadaAnterior_Click.
    /// </summary>
    [RelayCommand]
    private void JornadaAnterior()
    {
        // Decrementa la jornada (legacy: btJornadaAnterior_Click). La recarga de los
        // porcentajes persistidos depende del UserControl ControlPorcentajes (no portado).
        if (Jornada > 1) Jornada--;
        // TODO: recarga de datos/porcentajes en Free1X2/UI/EstimadorPremiosFrm.cs (btJornadaAnterior_Click)
        //   — requiere ControlPorcentajes, aún no portado a WinUI.
    }

    /// <summary>
    /// Navega a la jornada posterior cargando sus datos. Legacy: btJornadaPosterior_Click.
    /// </summary>
    [RelayCommand]
    private void JornadaPosterior()
    {
        // Incrementa la jornada (legacy: btJornadaPosterior_Click).
        Jornada++;
        // TODO: recarga de datos/porcentajes en Free1X2/UI/EstimadorPremiosFrm.cs (btJornadaPosterior_Click)
        //   — requiere ControlPorcentajes, aún no portado a WinUI.
    }

    /// <summary>
    /// Copia los valores de previsión estimados a las celdas del escrutinio real.
    /// (Reinterpretación WinUI del legacy btCopiar_Click, que copiaba al portapapeles.)
    /// </summary>
    [RelayCommand]
    private void CopiarPrevision()
    {
        for (int i = 0; i < Prevision.Count && i < EscrutinioReal.Count; i++)
        {
            EscrutinioReal[i].Acertantes = ParseDouble(Prevision[i].AcertantesTexto);
            EscrutinioReal[i].Premio = ParseDouble(Prevision[i].PremioTexto);
        }
    }

    private static double ParseDouble(string texto) =>
        double.TryParse(texto, out double v) ? v : 0;

    /// <summary>
    /// Guarda el escrutinio real introducido por el usuario. Legacy: btGuardarEscrutinio_Click.
    /// </summary>
    [RelayCommand]
    private void GuardarEscrutinio()
    {
        // TODO: lógica en Free1X2/UI/EstimadorPremiosFrm.cs línea 2260 (btGuardarEscrutinio_Click).
        //   Usa Free1X2...LAE (GrabarJornada) con la columna premiada leída de la matriz de
        //   labels del form legacy y los acertantes reales; depende de tipos/IO no expuestos en
        //   este flujo aislado de utilidades file→file.
    }

    /// <summary>
    /// Abre los resultados del escrutinio oficial (LAE). Legacy: linkLAE_LinkClicked.
    /// </summary>
    [RelayCommand]
    private async void AbrirEscrutinioOficial()
    {
        // Legacy linkLAE_LinkClicked abría el navegador con el recurso oficial LAE.
        try
        {
            await Windows.System.Launcher.LaunchUriAsync(
                new Uri("https://www.loteriasyapuestas.es/es/la-quiniela"));
        }
        catch
        {
            // Lanzador no disponible: se ignora (equivale a no poder abrir el navegador).
        }
    }
}
