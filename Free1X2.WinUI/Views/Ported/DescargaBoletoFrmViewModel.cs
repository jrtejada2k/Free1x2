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

        if (string.IsNullOrWhiteSpace(TemporadaSeleccionada) || !TemporadaSeleccionada.Contains('-'))
        {
            Mensaje = "La Temporada elegida no es correcta";
            return;
        }

        if (string.IsNullOrWhiteSpace(JornadaSeleccionada))
        {
            Mensaje = "La Jornada elegida no es correcta";
            return;
        }

        // TODO[dominio]: descargar el boleto online y publicarlo en el form principal.
        //   Legacy: DescargaBoletoFrm.btnActualizar_Click
        //     Free1X2WService fWS = new Free1X2WService();
        //     string[] partes = cbbTemporada.Text.Split('-');
        //     boleto = fWS.ObtenerBoleto(Convert.ToInt32(cbbJornada.Text), partes[0]);
        //     if (boleto == "") -> mostrar "El Boleto elegido no está disponible";
        //     else -> MainForm.BoletoOnline = boleto; Close();
        //   El servicio Free1X2WService aún no está migrado a Free1X2.Domain.
        //   Al integrarlo: parsear int.Parse(JornadaSeleccionada) y TemporadaSeleccionada.Split('-')[0].
    }
}
