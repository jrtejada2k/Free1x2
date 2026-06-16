using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Fila de un partido del boleto base: número, nombre del equipo (local-visitante) y los
/// tres signos 1 / X / 2 seleccionables. Multi-selección → fijo (1 signo), doble (2) o triple (3).
/// Réplica de <c>PartidoBoleto</c> (WinForms) + la fila del control <c>Pronosticos</c>.
/// </summary>
public partial class PartidoBaseViewModel : ObservableObject
{
    private readonly BoletoBaseViewModel _owner;

    public PartidoBaseViewModel(BoletoBaseViewModel owner, int numero, string equipos)
    {
        _owner = owner;
        Numero = numero;
        _equipos = equipos;
    }

    public int Numero { get; }

    /// <summary>Número formateado para TextBlock.Text (regla anti-crash: proyectar int a string).</summary>
    public string NumeroTexto => Numero.ToString("00");

    // Nombre editable del partido, formato "LOCAL-VISITANTE" (igual que Pronosticos.DevolverEquipos()).
    [ObservableProperty]
    private string _equipos;

    [ObservableProperty]
    private bool _signo1 = true;

    [ObservableProperty]
    private bool _signoX = true;

    [ObservableProperty]
    private bool _signo2 = true;

    /// <summary>Cantidad de signos marcados (0..3): 1=fijo, 2=doble, 3=triple.</summary>
    public int Marcados => (Signo1 ? 1 : 0) + (SignoX ? 1 : 0) + (Signo2 ? 1 : 0);

    /// <summary>
    /// Pronóstico en el formato del motor: signos separados por coma, p. ej. "1,X,2" / "1,X" / "X".
    /// Equivale a <c>Pronosticos.GetPronostico(i)</c> (WinForms).
    /// </summary>
    public string Pronostico
    {
        get
        {
            var partes = new System.Collections.Generic.List<string>(3);
            if (Signo1) partes.Add("1");
            if (SignoX) partes.Add("X");
            if (Signo2) partes.Add("2");
            return string.Join(",", partes);
        }
    }

    partial void OnSigno1Changed(bool value) => _owner.AlCambiarSigno();
    partial void OnSignoXChanged(bool value) => _owner.AlCambiarSigno();
    partial void OnSigno2Changed(bool value) => _owner.AlCambiarSigno();
    partial void OnEquiposChanged(string value) => _owner.RecalcularResumen();
}

/// <summary>
/// ViewModel del boleto base editable, cableado al motor real (<see cref="AppState"/>).
/// Construye una fila por partido (según <c>VariablesGlobales.NumeroPartidos</c>), refleja el
/// estado del Analizador en pantalla y lo vuelca de vuelta al motor con <c>SetPronostico</c>
/// (igual que <c>MainForm.ObtenPronosticos()</c>) y los nombres con <c>DevolverEquipos()</c>.
/// </summary>
public partial class BoletoBaseViewModel : ObservableObject
{
    private bool _cargando;

    public ObservableCollection<PartidoBaseViewModel> Partidos { get; } = new();

    public int NumPartidos { get; } = Free1X2.VariablesGlobales.NumeroPartidos;

    [ObservableProperty]
    private int _fijos;

    [ObservableProperty]
    private int _dobles;

    [ObservableProperty]
    private int _triples;

    public BoletoBaseViewModel()
    {
        Construir();
        CargarDesdeMotor();
        RecalcularResumen();
    }

    /// <summary>Resumen en vivo del boleto, p. ej. "Fijos: 0 · Dobles: 0 · Triples: 14".</summary>
    public string Resumen => $"Fijos: {Fijos} · Dobles: {Dobles} · Triples: {Triples}";

    // Construye una fila por partido. Inicialmente "triple" (1,X,2), igual que
    // Pronosticos.InicializaPronosticos() (todos a "1X2") en WinForms.
    private void Construir()
    {
        Partidos.Clear();
        for (int i = 0; i < NumPartidos; i++)
        {
            Partidos.Add(new PartidoBaseViewModel(this, i + 1, "? - ?"));
        }
    }

    /// <summary>
    /// Vuelca al boleto en pantalla el estado del motor compartido (pronósticos + equipos del
    /// boleto base). Equivale a la parte de carga de MainForm (PonerPronosticosPantalla + SetEquipos).
    /// </summary>
    public void CargarDesdeMotor()
    {
        _cargando = true;
        try
        {
            var analizador = AppState.Instancia.Analizador;
            string[]? pronosticos = analizador.Pronosticos;

            for (int i = 0; i < Partidos.Count; i++)
            {
                var fila = Partidos[i];
                // Pronósticos: si el motor aún no tiene, deja "1,X,2".
                string pr = (pronosticos != null && i < pronosticos.Length && !string.IsNullOrEmpty(pronosticos[i]))
                    ? pronosticos[i]
                    : "1,X,2";
                fila.Signo1 = pr.Contains("1");
                fila.SignoX = pr.Contains("X");
                fila.Signo2 = pr.Contains("2");
            }
        }
        finally
        {
            _cargando = false;
        }
        RecalcularResumen();
    }

    /// <summary>
    /// Empuja los pronósticos del boleto en pantalla al Analizador compartido.
    /// Réplica exacta de <c>MainForm.ObtenPronosticos()</c>:
    /// <c>for i in 0..NumPartidos: analizador.SetPronostico(i, pronosticos[i+1])</c>.
    /// </summary>
    public void VolcarPronosticosAlMotor()
    {
        var analizador = AppState.Instancia.Analizador;
        for (int i = 0; i < Partidos.Count; i++)
        {
            analizador.SetPronostico(i, Partidos[i].Pronostico);
        }
    }

    /// <summary>Devuelve los equipos en formato "LOCAL-VISITANTE" (igual que Pronosticos.DevolverEquipos()).</summary>
    public string[] DevolverEquipos()
    {
        var equipos = new string[Partidos.Count];
        for (int i = 0; i < Partidos.Count; i++)
        {
            equipos[i] = Partidos[i].Equipos;
        }
        return equipos;
    }

    /// <summary>Carga los nombres de equipos en el boleto (igual que Pronosticos.SetEquipos()).</summary>
    public void SetEquipos(string[] equipos)
    {
        _cargando = true;
        try
        {
            for (int i = 0; i < Partidos.Count && i < equipos.Length; i++)
            {
                Partidos[i].Equipos = string.IsNullOrEmpty(equipos[i]) ? "? - ?" : equipos[i];
            }
        }
        finally
        {
            _cargando = false;
        }
    }

    // Cada vez que cambia un signo: vuelca al motor y refresca el resumen.
    internal void AlCambiarSigno()
    {
        if (_cargando) return;
        VolcarPronosticosAlMotor();
        RecalcularResumen();
    }

    /// <summary>Recalcula los contadores fijos/dobles/triples a partir de los partidos.</summary>
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

    /// <summary>Pone todos los partidos a triple (1,X,2). Equivale a Reiniciar14Triples().</summary>
    [RelayCommand]
    private void Reiniciar() => ReiniciarTriples();

    /// <summary>
    /// Reinicia el boleto a 14 triples (todos 1,X,2) y vuelca al motor. Público para que la
    /// MainPage lo invoque al crear una combinación nueva (MNuevaComb → Reiniciar14Triples).
    /// </summary>
    public void ReiniciarTriples()
    {
        _cargando = true;
        try
        {
            foreach (var p in Partidos)
            {
                p.Signo1 = true;
                p.SignoX = true;
                p.Signo2 = true;
            }
        }
        finally
        {
            _cargando = false;
        }
        VolcarPronosticosAlMotor();
        RecalcularResumen();
    }

    /// <summary>Vacía los nombres de equipos del boleto (Pronosticos.SetEquiposVacio()).</summary>
    public void VaciarEquipos()
    {
        _cargando = true;
        try
        {
            foreach (var p in Partidos)
            {
                p.Equipos = "? - ?";
            }
        }
        finally
        {
            _cargando = false;
        }
    }
}
