using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Controls;

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

    // Rejilla de porcentajes de la jornada (1/X/2). Sustituye al UserControl WinForms
    // ControlPorcentajes (controlPorcentajes1); PorcentajesHelper.AMatriz(...) equivale a .Valores.
    public ObservableCollection<FilaPorcentaje> Porcentajes { get; } =
        PorcentajesHelper.Crear(Free1X2.VariablesGlobales.NumeroPartidos);

    // Columna premiada (14 signos 1/X/2). Legacy: labels lblNewBaseNN1 (default "1"*14).
    [ObservableProperty]
    private string _columnaPremiada = "11111111111111";

    // Nº de apuestas/columnas jugadas. Legacy: lbNumApuestas (default 28000000).
    [ObservableProperty]
    private double _numApuestas = 28000000;

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
        // Legacy: button2_Click -> CalcularPremios() (EstimadorPremiosFrm.cs 1885/1889).
        // v = controlPorcentajes1.Valores (1913) -> base-100 (1916). Replica completa: el cálculo
        // sólo usa la matriz de porcentajes, la columna premiada (Signos) y el nº de apuestas,
        // sin el motor de 4,7M -> totalmente cableable.
        if (ColumnaPremiada == null || ColumnaPremiada.Length < 14)
        {
            NumApuestasTexto = "Columna premiada inválida (14 signos)";
            return;
        }

        // PctDestinadoAPremiosCategoria[0..4] = % por categoría / 100 (legacy 1898-1902).
        double[] pct =
        {
            PorcentajePara14 / 100, PorcentajePara13 / 100, PorcentajePara12 / 100,
            PorcentajePara11 / 100, PorcentajePara10 / 100,
        };
        var destinado = new double[5];
        for (int i = 0; i < 5; i++) destinado[i] = Recaudacion * pct[i];
        destinado[0] += Bote;   // el bote se suma al 14 (legacy 1910).

        // Matriz de porcentajes -> base-100 (legacy: new Porcentajes(v).ValoresBase100()).
        float[,] p = ValoresBase100(PorcentajesHelper.AMatriz(Porcentajes));

        // Signos de la columna premiada (legacy: PonerSignosEnVariables, índice en "1X2").
        var signos = new int[14];
        for (int partido = 0; partido < 14; partido++)
            signos[partido] = "1X2".IndexOf(ColumnaPremiada[partido]);

        int numApuestas = (int)NumApuestas;
        var acertantes = new double[5];
        var cr = new float[14];

        // Probabilidad del 14 y valores complementarios Cr (legacy 1928-1932).
        double probCategoria14 = 1;
        for (int partido = 0; partido < 14; partido++)
        {
            int s = signos[partido] < 0 ? 0 : signos[partido];
            probCategoria14 *= p[partido, s];
            cr[partido] = (float)((1 - p[partido, s]) / p[partido, s]);
        }

        acertantes[0] = numApuestas * probCategoria14;
        var premios = new double[5];
        premios[0] = Math.Round(destinado[0] / acertantes[0], 2);

        CalcularAcertantes(acertantes, cr, acertantes[0], 0, 4);

        for (int i = 1; i < 5; i++)
        {
            premios[i] = Math.Round(destinado[i] / acertantes[i], 2);
            acertantes[i] = Math.Round(acertantes[i], 0);
        }
        acertantes[0] = Math.Round(acertantes[0], 1);

        // Correcciones (legacy CorreccionesDeCalculo, 1966).
        if (premios[0] > destinado[0]) premios[0] = destinado[0];
        if (premios[4] < 1) premios[4] = 0;

        // Volcado de resultados (legacy MostrarResultados, 1975).
        for (int i = 0; i < 5 && i < Prevision.Count; i++)
        {
            Prevision[i].AcertantesTexto = acertantes[i].ToString();
            Prevision[i].PremioTexto = premios[i].ToString();
        }
        NumApuestasTexto = numApuestas.ToString();
    }

    // Legacy: CalcularAcertantes() (EstimadorPremiosFrm.cs 1988). Recursión que reparte la
    // probabilidad entre las categorías inferiores acumulando los acertantes esperados.
    private static void CalcularAcertantes(double[] acertantes, float[] cr, double pProb, int posicionInicial, int pProfundidad)
    {
        CalcularAcertantesRec(acertantes, cr, pProb, posicionInicial, pProfundidad, 0);
    }

    private static void CalcularAcertantesRec(double[] acertantes, float[] cr, double pProb, int posicionInicial, int pProfundidad, int profundidad)
    {
        profundidad++;
        for (int partido = posicionInicial; partido < 14; partido++)
        {
            double prob = pProb * cr[partido];
            acertantes[profundidad] += prob;
            if (profundidad < pProfundidad)
                CalcularAcertantesRec(acertantes, cr, prob, partido + 1, pProfundidad, profundidad);
        }
    }

    // Legacy: Free1X2.Utils.Porcentajes.ValoresBase100() (Free1X2/Utils/Porcentajes.cs 341).
    // Se replica porque ese helper vive en el proyecto WinForms y no en Free1X2.Domain.
    private static float[,] ValoresBase100(double[,] valores)
    {
        var b100 = new float[14, 3];
        for (int i = 0; i < 14; i++)
        {
            float factor = (float)(valores[i, 0] + valores[i, 1] + valores[i, 2]);
            for (int j = 0; j < 3; j++)
                b100[i, j] = (float)(valores[i, j] / factor);
        }
        return b100;
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
        // Legacy: btGuardarEscrutinio_Click (EstimadorPremiosFrm.cs 2260). Construye un LAE con
        // temporada/jornada/columna premiada/recaudación y graba los acertantes reales por categoría.
        // Free1X2.Utils.LAE SÍ está en Free1X2.Domain -> totalmente cableable.
        if (ColumnaPremiada == null || ColumnaPremiada.Length < 14)
        {
            NumApuestasTexto = "Columna premiada inválida (14 signos)";
            return;
        }

        var jornadaLAE = new Free1X2.Utils.LAE(
            Temporada, Jornada.ToString(), ColumnaPremiada.Substring(0, 14), Recaudacion);

        var acertantesReales = new int[5];
        for (int i = 0; i < 5 && i < EscrutinioReal.Count; i++)
            acertantesReales[i] = (int)EscrutinioReal[i].Acertantes;

        try
        {
            jornadaLAE.GrabarJornada(acertantesReales);
        }
        catch (Exception ex)
        {
            Services.AppServices.MostrarError("No se ha podido grabar el escrutinio: " + ex.Message);
        }
    }

    /// <summary>
    /// Abre los resultados del escrutinio oficial (LAE). Legacy: linkLAE_LinkClicked.
    /// </summary>
    [RelayCommand]
    private async Task AbrirEscrutinioOficial()
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
