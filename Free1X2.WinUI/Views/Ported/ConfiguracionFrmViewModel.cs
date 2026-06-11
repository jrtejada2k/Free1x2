using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    /// <summary>Carga la configuración persistida.</summary>
    public void Cargar()
    {
        // TODO: dominio legacy — instanciar Free1X2.EntradaSalida.AConfiguracion(Application.StartupPath)
        //       y poblar los campos con:
        //         ObtenValoresUtilSeparadorJB()  -> Jb1..Jb6
        //         ObtenValoresLAE(...)           -> Recaudacion, PrecioApuesta, PorcentajeAl14, SimboloMoneda
        //         ObtenPuntosCP(...)             -> ValorFijos, ValorDobles, ValorTriples
        //         ObtenDesplazamiento(...)       -> Desplazamiento
        //         ObtenNumPartidos(...)          -> NumPartidos, Separador
        //         ObtenConfiguracionActualizador(...) -> ComprobarActualizacionesAlInicio
        //         ObtenInfoIdioma(...)           -> IdiomaSeleccionado
        //         ObtenConfiguracionAdvertenciaSalir(...) -> PedirConfirmacionAlSalir
        // TODO: poblar Idiomas con Free1X2.EntradaSalida.ArchivoIdioma.ObtenerListaDeIdiomasDisponibles()
    }

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
                // TODO: mostrar ContentDialog de error
                //       ("El valor del Nº campo no puede ser mayor que el/los siguiente/s.")
                return;
            }
        }

        // TODO: dominio legacy — Free1X2.EntradaSalida.AConfiguracion(Application.StartupPath):
        //         GuardarValoresSeparadorJB(jb)
        //         GuardarValoresLAE(PrecioApuesta, PorcentajeAl14, Recaudacion, SimboloMoneda)
        //         GuardarPuntosCP((int)ValorFijos, (int)ValorDobles, (int)ValorTriples)
        //         GuardarConfiguracionBoleto((int)NumPartidos, Separador)
        //         GuardarDesplazamiento(Desplazamiento.ToString())
        //         GuardarConfiguracionActualizador(ComprobarActualizacionesAlInicio)
        //         GuardarIdioma(IdiomaSeleccionado)
        //         GuardarConfiguracionAdvertenciaSalir(PedirConfirmacionAlSalir)
        // TODO: Free1X2.VariablesGlobales.ReinicializarVariables()
        // TODO: ofrecer reinicio de la aplicación (Application.Restart() en legacy)
    }
}
