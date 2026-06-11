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

        // TODO[dominio]: inicializar temporada por defecto.
        //   Legacy: EstimadorPremiosFrm ctor calcula la temporada según el mes actual
        //   (mes<7 -> año-1/año ; sino año/año+1) y rellena txTemporada.
    }

    /// <summary>
    /// Recalcula la previsión de acertantes y premios por categoría.
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: ejecutar el cálculo de estimación de premios.
        //   Legacy: EstimadorPremiosFrm.button2_Click -> usa Free1X2.Analisis.ApuestaProbableCentral,
        //   Free1X2.Analisis.Porcentajes (controlPorcentajes1), los arrays p/Cr/v/Signos y los
        //   porcentajes DestinadoAPremiosCategoria para calcular Acertantes[] y Premios[].
        //   Volcar el resultado en cada PrevisionPremioItem.AcertantesTexto / PremioTexto
        //   y actualizar NumApuestasTexto. El dominio aún no está migrado.
    }

    /// <summary>
    /// Navega a la jornada anterior cargando sus datos. Legacy: btJornadaAnterior_Click.
    /// </summary>
    [RelayCommand]
    private void JornadaAnterior()
    {
        // TODO[dominio]: decrementar jornada y recargar porcentajes/datos.
        //   Legacy: EstimadorPremiosFrm.btJornadaAnterior_Click.
    }

    /// <summary>
    /// Navega a la jornada posterior cargando sus datos. Legacy: btJornadaPosterior_Click.
    /// </summary>
    [RelayCommand]
    private void JornadaPosterior()
    {
        // TODO[dominio]: incrementar jornada y recargar porcentajes/datos.
        //   Legacy: EstimadorPremiosFrm.btJornadaPosterior_Click.
    }

    /// <summary>
    /// Copia los valores de previsión al escrutinio real. Legacy: btCopiar_Click.
    /// </summary>
    [RelayCommand]
    private void CopiarPrevision()
    {
        // TODO[dominio]: copiar Acertantes/Premios estimados a las celdas de escrutinio real.
        //   Legacy: EstimadorPremiosFrm.btCopiar_Click.
    }

    /// <summary>
    /// Guarda el escrutinio real introducido por el usuario. Legacy: btGuardarEscrutinio_Click.
    /// </summary>
    [RelayCommand]
    private void GuardarEscrutinio()
    {
        // TODO[dominio]: persistir el escrutinio real (acertantes/premios oficiales) de la jornada.
        //   Legacy: EstimadorPremiosFrm.btGuardarEscrutinio_Click + Free1X2.EntradaSalida.
    }

    /// <summary>
    /// Abre/descarga los resultados del escrutinio oficial (LAE). Legacy: linkLAE_LinkClicked.
    /// </summary>
    [RelayCommand]
    private void AbrirEscrutinioOficial()
    {
        // TODO[dominio]: abrir el origen de "Resultados escrutinio oficial".
        //   Legacy: EstimadorPremiosFrm.linkLAE_LinkClicked (LinkLabel -> recurso oficial LAE).
    }
}
