using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "EstimadorPremiosFrm".
/// Estima acertantes y premios por categoría (14..10) a partir de recaudación, bote y
/// porcentajes de reparto, y permite registrar el escrutinio real oficial de la jornada.
/// </summary>
public sealed partial class EstimadorPremiosFrmPage : Page
{
    public EstimadorPremiosFrmViewModel ViewModel { get; } = new();

    public EstimadorPremiosFrmPage()
    {
        InitializeComponent();
    }
}
