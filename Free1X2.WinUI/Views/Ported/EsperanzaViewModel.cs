// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Analisis;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>Una fila de la tabla de resultados, ya formateada.</summary>
public sealed record FilaEsperanza(string Aciertos, string ProbabilidadReal, string ProbabilidadApostada,
    string PremioMedio, string Esperanza);

/// <summary>
/// «Esperanza matemática» (W14, DT-91; pantalla nueva, no existe en el WinForms): EM exacta de una columna por
/// categoría, con precio, reparto, retención y mínimos configurables. El precio y la recaudación salen de
/// Configuración ▸ Parámetros LAE y se pueden cambiar aquí. Mismos textos que la app Mac (Esperanza.swift).
/// </summary>
public partial class EsperanzaViewModel : ObservableObject
{
    private static readonly CultureInfo Es = new("es-ES");

    public EsperanzaViewModel()
    {
        var lae = ParametrosLAEActuales.Calculo();
        _recaudacion = lae.Recaudacion;
        _precio = lae.Precio;
        Moneda = lae.Moneda;
        AplicarReparto(ParametrosEsperanza.RepartoExcel);
    }

    public string Moneda { get; }

    // ---- Columna ----
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CalcularCommand))]
    private string _columna = "";

    partial void OnColumnaChanged(string value)
    {
        string u = (value ?? "").ToUpperInvariant();
        if (u.Length > 14) u = u.Substring(0, 14);
        if (u != value) Columna = u;
    }

    // ---- Jornada ----
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ApuestasTexto))]
    private double _recaudacion;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ApuestasTexto))]
    private double _precio;

    public string ApuestasTexto =>
        "Apuestas: recaudación ÷ precio = " + (Precio > 0 ? Numero(Recaudacion / Precio, 0) : "—");

    // ---- Reparto (% de la recaudación para 14…10) ----
    public string[] OpcionesReparto { get; } =
        { "Excel: 16 · 7,5 · 7,5 · 7,5 · 9", "Free1X2: 12 · 8 · 8 · 8 · 9", "Personalizado" };

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(RepartoElegido))] private double _reparto14;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(RepartoElegido))] private double _reparto13;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(RepartoElegido))] private double _reparto12;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(RepartoElegido))] private double _reparto11;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(RepartoElegido))] private double _reparto10;

    private double[] Reparto => new[] { Reparto14, Reparto13, Reparto12, Reparto11, Reparto10 };

    /// <summary>El selector muestra el reparto que coincide con los campos; elegir uno los rellena.</summary>
    public int RepartoElegido
    {
        get => Reparto.SequenceEqual(ParametrosEsperanza.RepartoExcel) ? 0
             : Reparto.SequenceEqual(ParametrosEsperanza.RepartoFree1X2) ? 1 : 2;
        set
        {
            if (value == 0) AplicarReparto(ParametrosEsperanza.RepartoExcel);
            else if (value == 1) AplicarReparto(ParametrosEsperanza.RepartoFree1X2);
            OnPropertyChanged();
        }
    }

    private void AplicarReparto(double[] r)
    {
        Reparto14 = r[0]; Reparto13 = r[1]; Reparto12 = r[2]; Reparto11 = r[3]; Reparto10 = r[4];
    }

    // ---- Cobro ----
    [ObservableProperty] private double _retencion = 20;
    [ObservableProperty] private double _exento = 40000;
    [ObservableProperty] private double _minimo = 1;
    [ObservableProperty] private double _minimo10 = 1.5;
    [ObservableProperty] private bool _masUno = true;

    // ---- Valoraciones (porcentajes) ----
    public ObservableCollection<FilaPorcentaje> Apostados { get; } = PorcentajesHelper.Crear(14);
    public ObservableCollection<FilaPorcentaje> Reales { get; } = PorcentajesHelper.Crear(14);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VerApostados), nameof(VerReales))]
    private int _pestana;

    public bool VerApostados => Pestana == 0;
    public bool VerReales => Pestana == 1;

    // ---- Resultado ----
    public ObservableCollection<FilaEsperanza> Resultado { get; } = new();

    [ObservableProperty] private bool _hayResultado;
    [ObservableProperty] private string _textoTotal = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CalcularCommand))]
    private bool _calculando;

    private bool PuedeCalcular() => !Calculando && Columna.Length == 14;

    [RelayCommand(CanExecute = nameof(PuedeCalcular))]
    private async Task Calcular()
    {
        var p = new ParametrosEsperanza(Recaudacion, Precio, Reparto)
        {
            Retencion = Retencion,
            Exento = Exento,
            Minimo = Minimo,
            Minimo10 = Minimo10,
            ContarmeComoAcertante = MasUno,
        };
        string c = Columna;
        double[,] a = PorcentajesHelper.AMatriz(Apostados), r = PorcentajesHelper.AMatriz(Reales);

        Calculando = true;
        try
        {
            var res = await Task.Run(() => EsperanzaMatematica.Calcular(c, a, r, p));
            Resultado.Clear();
            foreach (var cat in res.Categorias)
            {
                Resultado.Add(new FilaEsperanza(cat.Aciertos.ToString(), Probabilidad(cat.ProbabilidadReal),
                    Probabilidad(cat.ProbabilidadApostada), Dinero(cat.PremioMedio), Dinero(cat.Esperanza, 4)));
            }
            TextoTotal = "EM total: " + Dinero(res.Total, 4) + " por cada apuesta de " + Dinero(res.Precio) +
                         ", el " + Numero(res.Rendimiento * 100, 1) + " % de lo apostado (" + Numero(res.Apuestas, 0) +
                         " apuestas).";
            HayResultado = true;
        }
        catch (Exception ex)
        {
            Resultado.Clear();
            HayResultado = false;
            AppServices.MostrarError(ex.Message);
        }
        finally
        {
            Calculando = false;
        }
    }

    private static string Numero(double v, int decimales) => v.ToString("N" + decimales, Es);

    private string Dinero(double v, int decimales = 2) => Numero(v, decimales) + " " + Moneda;

    /// <summary>«0,0043 % (1 entre 23.142)».</summary>
    private static string Probabilidad(double p) =>
        p > 0 ? Numero(p * 100, 4) + " % (1 entre " + Numero(1 / p, 0) + ")" : "0 %";
}
