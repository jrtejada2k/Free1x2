using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Fila de un partido del boleto: número, equipos y los tres signos 1 / X / 2.
/// Recalcula el resumen del boleto cada vez que cambia un signo.
/// </summary>
public partial class PartidoViewModel : ObservableObject
{
    private readonly BoletoViewModel _owner;

    public PartidoViewModel(BoletoViewModel owner, int numero, string local, string visitante)
    {
        _owner = owner;
        Numero = numero;
        Local = local;
        Visitante = visitante;
    }

    public int Numero { get; }

    public string NumeroTexto => Numero.ToString("00");

    public string Local { get; }

    public string Visitante { get; }

    public string Equipos => $"{Local} - {Visitante}";

    [ObservableProperty]
    private bool _signo1;

    [ObservableProperty]
    private bool _signoX;

    [ObservableProperty]
    private bool _signo2;

    /// <summary>Cantidad de signos marcados en esta fila (0..3).</summary>
    public int Marcados => (Signo1 ? 1 : 0) + (SignoX ? 1 : 0) + (Signo2 ? 1 : 0);

    partial void OnSigno1Changed(bool value) => _owner.RecalcularResumen();
    partial void OnSignoXChanged(bool value) => _owner.RecalcularResumen();
    partial void OnSigno2Changed(bool value) => _owner.RecalcularResumen();
}

/// <summary>
/// ViewModel reutilizable del boleto de la Quiniela: 14 partidos con signos 1/X/2
/// y un contador en vivo de fijos / dobles / triples.
/// Datos de muestra; aún no depende del dominio.
/// </summary>
public partial class BoletoViewModel : ObservableObject
{
    public const int NumPartidos = 14;

    public ObservableCollection<PartidoViewModel> Partidos { get; } = new();

    [ObservableProperty]
    private int _fijos;

    [ObservableProperty]
    private int _dobles;

    [ObservableProperty]
    private int _triples;

    public BoletoViewModel()
    {
        CargarMuestra();
        RecalcularResumen();
    }

    /// <summary>Texto resumen del boleto, p. ej. "Fijos: 0 · Dobles: 0 · Triples: 0".</summary>
    public string Resumen => $"Fijos: {Fijos} · Dobles: {Dobles} · Triples: {Triples}";

    /// <summary>Recalcula los contadores agregados a partir de los partidos.</summary>
    public void RecalcularResumen()
    {
        int fijos = 0, dobles = 0, triples = 0;
        foreach (var p in Partidos)
        {
            switch (p.Marcados)
            {
                case 1: fijos++; break;
                case 2: dobles++; break;
                case 3: triples++; break;
            }
        }
        Fijos = fijos;
        Dobles = dobles;
        Triples = triples;
        OnPropertyChanged(nameof(Resumen));
    }

    /// <summary>Limpia todos los signos marcados del boleto.</summary>
    [RelayCommand]
    private void Limpiar()
    {
        foreach (var p in Partidos)
        {
            p.Signo1 = false;
            p.SignoX = false;
            p.Signo2 = false;
        }
        RecalcularResumen();
    }

    private void CargarMuestra()
    {
        // Equipos de muestra; en la migración real vendrán del dominio.
        var equipos = new (string Local, string Visitante)[]
        {
            ("Madrid",     "Barcelona"),
            ("Sevilla",    "Valencia"),
            ("Athletic",   "R. Sociedad"),
            ("Betis",      "Villarreal"),
            ("Girona",     "Osasuna"),
            ("Celta",      "Mallorca"),
            ("Getafe",     "Rayo"),
            ("Alavés",     "Las Palmas"),
            ("Cádiz",      "Granada"),
            ("Espanyol",   "Leganés"),
            ("Levante",    "Elche"),
            ("Zaragoza",   "Oviedo"),
            ("Sporting",   "Mirandés"),
            ("Eibar",      "Huesca"),
        };

        for (int i = 0; i < NumPartidos; i++)
        {
            var (local, visitante) = equipos[i];
            Partidos.Add(new PartidoViewModel(this, i + 1, local, visitante));
        }
    }
}
