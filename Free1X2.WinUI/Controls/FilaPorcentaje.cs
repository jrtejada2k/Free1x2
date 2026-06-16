using CommunityToolkit.Mvvm.ComponentModel;

namespace Free1X2.WinUI.Controls;

/// <summary>
/// Una fila de la rejilla de porcentajes/valoraciones: el partido y sus tres valores
/// (1 / X / 2). Es el modelo que rellena <see cref="PorcentajesControl"/>, equivalente
/// a una fila de los UserControls WinForms <c>valors</c> y <c>ControlPorcentajes</c>.
/// </summary>
public partial class FilaPorcentaje : ObservableObject
{
    [ObservableProperty] private int _partido;
    [ObservableProperty] private double _uno;
    [ObservableProperty] private double _equis;
    [ObservableProperty] private double _dos;

    public FilaPorcentaje() { }

    public FilaPorcentaje(int partido, double uno = 0, double equis = 0, double dos = 0)
    {
        _partido = partido;
        _uno = uno;
        _equis = equis;
        _dos = dos;
    }
}
