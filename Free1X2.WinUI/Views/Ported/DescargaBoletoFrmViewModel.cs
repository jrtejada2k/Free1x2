using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Descargar Boleto" (legacy: DescargaBoletoFrm).
/// Permite elegir Jornada y Temporada para descargar el boleto oficial online.
/// </summary>
public partial class DescargaBoletoFrmViewModel : ObservableObject
{
    /// <summary>
    /// Lista de jornadas (1..60). Se expone como <c>ItemsSource</c> para alimentar
    /// el ComboBox con <c>SelectedItem</c> enlazado, evitando elementos
    /// <c>&lt;x:String&gt;</c> en línea (regla anti-crash del XamlCompiler 1.6).
    /// Legacy: bucle for i=1..60 en InicializarComboBoxes().
    /// </summary>
    public IReadOnlyList<string> Jornadas { get; }

    /// <summary>
    /// Lista de temporadas (formato "AAAA-AAAA"). Legacy: bucle for i=2005..2010.
    /// </summary>
    public IReadOnlyList<string> Temporadas { get; }

    [ObservableProperty]
    private string _jornadaSeleccionada;

    [ObservableProperty]
    private string _temporadaSeleccionada;

    [ObservableProperty]
    private string _mensaje = string.Empty;

    public DescargaBoletoFrmViewModel()
    {
        var jornadas = new List<string>();
        for (int i = 1; i <= 60; i++)
        {
            jornadas.Add(i.ToString());
        }
        Jornadas = jornadas;

        var temporadas = new List<string>();
        for (int i = 2005; i <= 2010; i++)
        {
            temporadas.Add(i + "-" + (i + 1));
        }
        Temporadas = temporadas;

        // Selección inicial (legacy: cbbJornada.Text = "1" y temporada según mes actual).
        _jornadaSeleccionada = jornadas.Count > 0 ? jornadas[0] : string.Empty;

        string temporadaActual = DateTime.Now.Month <= 6
            ? (DateTime.Now.Year - 1) + "-" + DateTime.Now.Year
            : DateTime.Now.Year + "-" + (DateTime.Now.Year + 1);

        _temporadaSeleccionada = temporadas.Contains(temporadaActual)
            ? temporadaActual
            : (temporadas.Count > 0 ? temporadas[temporadas.Count - 1] : string.Empty);
    }

    /// <summary>
    /// Descarga el boleto de la jornada/temporada elegida desde el servicio online.
    /// </summary>
    [RelayCommand]
    private void Descargar()
    {
        Mensaje = string.Empty;

        // Validación de la temporada (legacy: partes = cbbTemporada.Text.Split('-'); if length != 2 -> error).
        string[] partes = (TemporadaSeleccionada ?? string.Empty).Split('-');
        if (partes.Length != 2)
        {
            Mensaje = "La Temporada elegida no es correcta";
            return;
        }

        if (string.IsNullOrWhiteSpace(JornadaSeleccionada) || !int.TryParse(JornadaSeleccionada, out int jornada))
        {
            Mensaje = "La Jornada elegida no es correcta";
            return;
        }

        // Parámetros ya validados y listos para el servicio (legacy: ObtenerBoleto(jornada, partes[0])).
        string anioTemporada = partes[0];

        // El servicio web Free1X2WService (SOAP) NO está disponible sin conexión: en el WinForms
        // original ObtenerBoleto(int, string) es un stub que devuelve "" (modo offline,
        // ver Free1X2/Utils/ControlCompatibility.cs línea 738). Por eso el legacy entra siempre por
        // la rama "boleto == \"\"" de DescargaBoletoFrm.btnActualizar_Click (Free1X2/UI/DescargaBoletoFrm.cs
        // línea 52) y muestra "El Boleto elegido no está disponible". Reproducimos ese mismo mensaje
        // de runtime: sin servicio online no hay boleto que descargar (no se inventan datos).
        _ = jornada;            // parámetros ya parseados (los usaría el servicio si estuviera disponible)
        _ = anioTemporada;
        Mensaje = "El Boleto elegido no está disponible (servicio online no disponible sin conexión).";
    }
}
