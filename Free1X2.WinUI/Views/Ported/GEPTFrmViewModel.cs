using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de la columna estadística "GEPT" de Abdon (legacy: GEPTFrm).
/// Para cada partido se registran las dos últimas ocasiones (G = Ganó, E = Empató, P = Perdió)
/// y se deriva el pronóstico de signos (1 / X / 2 / 12 / 1X / X2).
/// </summary>
public partial class GEPTFila : ObservableObject
{
    /// <summary>Número de partido (1..N) como texto. Anti-crash: no se bindea int a TextBlock.Text.</summary>
    public string NumeroTexto { get; }

    /// <summary>Opciones permitidas en cada celda de entrada (legacy admitía G / E / P).</summary>
    public IReadOnlyList<string> OpcionesSigno { get; } = new[] { "", "G", "E", "P" };

    public GEPTFila(int numero)
    {
        NumeroTexto = numero.ToString("00");
    }

    /// <summary>Primera ocasión: una de G / E / P (legacy: primeraColumna[i]).</summary>
    [ObservableProperty]
    private string _primera = string.Empty;

    /// <summary>Segunda ocasión: una de G / E / P (legacy: segundaColumna[i]).</summary>
    [ObservableProperty]
    private string _segunda = string.Empty;

    /// <summary>Pronóstico resultante para el partido (legacy: columnaResultados[i]).</summary>
    [ObservableProperty]
    private string _resultado = string.Empty;

    /// <summary>
    /// Fondo de la celda "Última vez" (legacy: primeraColumna[i].BackColor, Fondo(idx)).
    /// G -> GreenYellow, E -> Yellow, P -> Pink, vacío -> White (Transparent en WinUI para no
    /// romper el tema de la celda; equivale al "White" del WinForms sobre fondo blanco).
    /// </summary>
    [ObservableProperty]
    private Brush _fondoPrimera = FondoNeutro;

    /// <summary>Fondo de la celda "Penúltima vez" (legacy: segundaColumna[i].BackColor, Fondo(idx)).</summary>
    [ObservableProperty]
    private Brush _fondoSegunda = FondoNeutro;

    // Cuando cambia la selección de una celda, recolorea como hacía Resultado_TextChanged del legacy.
    partial void OnPrimeraChanged(string value) => FondoPrimera = Fondo(value);
    partial void OnSegundaChanged(string value) => FondoSegunda = Fondo(value);

    private static readonly Brush FondoNeutro = new SolidColorBrush(Colors.Transparent);
    private static readonly Brush FondoG = new SolidColorBrush(Colors.GreenYellow);
    private static readonly Brush FondoE = new SolidColorBrush(Colors.Yellow);
    private static readonly Brush FondoP = new SolidColorBrush(Colors.Pink);

    /// <summary>Color de fondo por valor (réplica de GEPTFrm.Fondo: G/E/P -> verde/amarillo/rosa).</summary>
    internal static Brush Fondo(string valor) => (valor ?? string.Empty).Trim().ToUpperInvariant() switch
    {
        "G" => FondoG,
        "E" => FondoE,
        "P" => FondoP,
        _ => FondoNeutro,
    };
}

/// <summary>
/// ViewModel para la pantalla "Columna estadística de ABDON (GEPT)" (legacy: GEPTFrm).
/// Cada partido toma las dos últimas veces que se repitió el enfrentamiento (G/E/P)
/// y produce un pronóstico de signos según una tabla de traducción fija.
/// </summary>
public partial class GEPTFrmViewModel : ObservableObject
{
    public ObservableCollection<GEPTFila> Partidos { get; } = new();

    public GEPTFrmViewModel()
    {
        // Legacy GEPTFrm usaba VariablesGlobales.NumeroPartidos (ya disponible en Free1X2.Domain).
        for (int i = 1; i <= VariablesGlobales.NumeroPartidos; i++)
        {
            Partidos.Add(new GEPTFila(i));
        }
    }

    /// <summary>
    /// Calcula el pronóstico de cada partido a partir de las dos últimas ocasiones.
    /// Replica GEPTFrm.Calcular() -> Verificar() + Pronosticar() + Traductor().
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // Legacy Calcular() = Verificar() + Pronosticar(). Verificar recolorea las dos celdas de
        // entrada (G verde / E amarillo / P rosa) y Pronosticar deriva el signo. La recoloración ya
        // es reactiva (OnPrimera/OnSegundaChanged), pero se reasegura aquí para igualar a Verificar().
        foreach (var fila in Partidos)
        {
            fila.FondoPrimera = GEPTFila.Fondo(fila.Primera);
            fila.FondoSegunda = GEPTFila.Fondo(fila.Segunda);
            fila.Resultado = Traductor(fila.Primera, fila.Segunda);
        }
    }

    /// <summary>
    /// Traduce la combinación de las dos últimas ocasiones a un pronóstico de signos.
    /// Tabla idéntica a GEPTFrm.Traductor().
    /// </summary>
    private static string Traductor(string primera, string segunda)
    {
        string ch = ((primera ?? string.Empty) + (segunda ?? string.Empty)).ToUpperInvariant();
        return ch switch
        {
            "GG" => "12",
            "GE" => "1X",
            "GP" => "1",
            "EG" => "X2",
            "EE" => "X",
            "EP" => "1X",
            "PG" => "2",
            "PE" => "X2",
            "PP" => "12",
            _ => " ",
        };
    }
}
