// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla de Configuración (portada de WinForms <c>ConfiguracionFrm</c>).
/// Agrupa todos los parámetros editables del programa: configuración del boleto,
/// desplazamiento de grupo/condición, puntuación en CPs, separador de porcentajes JB,
/// parámetros LAE, actualizador, idioma y confirmación al salir.
///
/// La persistencia real (carga/guardado) se delega al dominio legacy
/// <c>Free1X2.EntradaSalida.AConfiguracion</c>; aquí solo se mantienen los valores.
/// Se usa la forma de CAMPO privado para [ObservableProperty] (no 'public partial').
/// </summary>
public partial class ConfiguracionFrmViewModel : ObservableObject
{
    // ===== Configuración del boleto =====
    [ObservableProperty]
    private double _numPartidos = 14;

    [ObservableProperty]
    private string _separador = "5,9,12";

    // ===== Desplazamiento de grupo/condición =====
    [ObservableProperty]
    private double _desplazamiento = 3;

    // ===== Puntuación en CPs =====
    [ObservableProperty]
    private double _valorFijos;

    [ObservableProperty]
    private double _valorDobles;

    [ObservableProperty]
    private double _valorTriples;

    // ===== Separador de porcentajes JB (6 límites crecientes) =====
    [ObservableProperty]
    private double _jb1 = 15;

    [ObservableProperty]
    private double _jb2 = 22;

    [ObservableProperty]
    private double _jb3 = 29;

    [ObservableProperty]
    private double _jb4 = 36;

    [ObservableProperty]
    private double _jb5 = 48;

    [ObservableProperty]
    private double _jb6 = 61;

    // ===== Parámetros LAE =====
    [ObservableProperty]
    private double _recaudacion = 15000000;

    [ObservableProperty]
    private double _precioApuesta = 0.50;

    [ObservableProperty]
    private double _porcentajeAl14 = 15;

    [ObservableProperty]
    private string _simboloMoneda = "€";

    // ===== Actualizador =====
    [ObservableProperty]
    private bool _comprobarActualizacionesAlInicio;

    // ===== Al salir =====
    [ObservableProperty]
    private bool _pedirConfirmacionAlSalir;

    // ===== Idioma =====
    public ObservableCollection<string> Idiomas { get; } = new() { "es-ES" };

    [ObservableProperty]
    private string? _idiomaSeleccionado = "es-ES";

    public ConfiguracionFrmViewModel()
    {
        Cargar();
    }

    /// <summary>Carga la configuración persistida (legacy AConfiguracion Obten*).</summary>
    public void Cargar()
    {
        try
        {
            // AConfiguracion() usa AppContext.BaseDirectory/parametros.free1x2. Si el archivo
            // no existe (entorno de desarrollo sin datos), se conservan los valores por defecto.
            var cfg = new AConfiguracion();

            // Separador JB ("15,22,29,36,48,61").
            string[] jb = cfg.ObtenValoresUtilSeparadorJB().Split(',');
            if (jb.Length >= 6)
            {
                Jb1 = ParseInv(jb[0]); Jb2 = ParseInv(jb[1]); Jb3 = ParseInv(jb[2]);
                Jb4 = ParseInv(jb[3]); Jb5 = ParseInv(jb[4]); Jb6 = ParseInv(jb[5]);
            }

            // Valores LAE.
            double precio = 0, pct = 0, recaud = 0; string moneda = "€";
            cfg.ObtenValoresLAE(ref precio, ref pct, ref recaud, ref moneda);
            PrecioApuesta = precio; PorcentajeAl14 = pct; Recaudacion = recaud; SimboloMoneda = moneda;

            // Puntos CP.
            int fijos = 0, dobles = 0, triples = 0;
            cfg.ObtenPuntosCP(ref fijos, ref dobles, ref triples);
            ValorFijos = fijos; ValorDobles = dobles; ValorTriples = triples;

            // Desplazamiento de grupo/condición.
            int desp = 3;
            cfg.ObtenDesplazamiento(ref desp);
            Desplazamiento = desp;

            // Configuración del boleto (nº partidos + separador).
            int np = 14; string sep = Separador;
            cfg.ObtenNumPartidos(ref np, ref sep);
            NumPartidos = np; Separador = sep;

            // Actualizador y confirmación al salir.
            bool actualizar = false;
            cfg.ObtenConfiguracionActualizador(ref actualizar);
            ComprobarActualizacionesAlInicio = actualizar;

            bool confirmarSalir = false;
            cfg.ObtenConfiguracionAdvertenciaSalir(ref confirmarSalir);
            PedirConfirmacionAlSalir = confirmarSalir;

            // Idioma.
            string idioma = "es-ES";
            cfg.ObtenInfoIdioma(ref idioma);
            if (!string.IsNullOrEmpty(idioma))
            {
                if (!Idiomas.Contains(idioma)) Idiomas.Add(idioma);
                IdiomaSeleccionado = idioma;
            }
        }
        catch
        {
            // Sin archivo de configuración accesible: se mantienen los valores por defecto.
        }
    }

    private static double ParseInv(string s) =>
        double.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var v)
            ? v
            : double.TryParse(s.Trim(), NumberStyles.Any, new CultureInfo("es-ES"), out var v2) ? v2 : 0;

    /// <summary>Valida y guarda la configuración.</summary>
    [RelayCommand]
    private void Guardar()
    {
        // Validación: los límites del separador JB deben ser estrictamente crecientes.
        double[] jb = { Jb1, Jb2, Jb3, Jb4, Jb5, Jb6 };
        for (int i = 1; i < jb.Length; i++)
        {
            if (jb[i - 1] >= jb[i])
            {
                Free1X2.Abstractions.UserDialogs.ShowError(
                    "El valor del campo " + i + " no puede ser mayor o igual que el siguiente.");
                return;
            }
        }

        try
        {
            var cfg = new AConfiguracion();

            var jbStr = new string[jb.Length];
            for (int i = 0; i < jb.Length; i++)
                jbStr[i] = ((int)jb[i]).ToString(CultureInfo.InvariantCulture);
            cfg.GuardarValoresSeparadorJB(jbStr);

            cfg.GuardarValoresLAE(PrecioApuesta, PorcentajeAl14, Recaudacion, SimboloMoneda);
            cfg.GuardarPuntosCP((int)ValorFijos, (int)ValorDobles, (int)ValorTriples);
            cfg.GuardarConfiguracionBoleto((int)NumPartidos, Separador);
            cfg.GuardarDesplazamiento(((int)Desplazamiento).ToString(CultureInfo.InvariantCulture));
            cfg.GuardarConfiguracionActualizador(ComprobarActualizacionesAlInicio);
            if (!string.IsNullOrEmpty(IdiomaSeleccionado)) cfg.GuardarIdioma(IdiomaSeleccionado);
            cfg.GuardarConfiguracionAdvertenciaSalir(PedirConfirmacionAlSalir);

            // Recarga las variables globales con los nuevos valores (legacy tras guardar).
            Free1X2.VariablesGlobales.ReinicializarVariables();

            Free1X2.Abstractions.UserDialogs.ShowInfo(
                "Configuración guardada. Algunos cambios pueden requerir reiniciar la aplicación.");
        }
        catch (Exception ex)
        {
            Free1X2.Abstractions.UserDialogs.ShowError("No se pudo guardar la configuración: " + ex.Message);
        }
    }
}
