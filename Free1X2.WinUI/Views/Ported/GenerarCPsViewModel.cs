using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de valoración por partido (signos 1 / X / 2 en porcentaje).
/// Réplica del UserControl legacy Free1X2.UI.Controls.GeneradorOptions,
/// que exponía Valor_1 / Valor_X / Valor_2 (string) + NumeroPartido.
/// </summary>
public partial class ValoracionPartidoItem : ObservableObject
{
    [ObservableProperty]
    private int _numeroPartido;

    [ObservableProperty]
    private double _valor1;

    [ObservableProperty]
    private double _valorX;

    [ObservableProperty]
    private double _valor2;

    // Texto de cabecera del partido para el TextBlock (regla anti-crash 2:
    // no se bindea el int directo a Text).
    public string NumeroPartidoTexto => NumeroPartido.ToString();
}

/// <summary>
/// ViewModel de la Page portada desde el WinForms "GenerarCPs"
/// (Generador de Columnas Probables).
/// </summary>
public partial class GenerarCPsViewModel : ObservableObject
{
    // Número de partidos de la quiniela. En el legacy se leía de
    // VariablesGlobales.NumeroPartidos.
    private const int NumeroPartidos = 14;

    public GenerarCPsViewModel()
    {
        Valoraciones = new ObservableCollection<ValoracionPartidoItem>();
        for (int i = 1; i <= NumeroPartidos; i++)
        {
            Valoraciones.Add(new ValoracionPartidoItem { NumeroPartido = i });
        }
    }

    /// <summary>
    /// Lista editable de valoraciones (1/X/2) por partido. Equivale al
    /// panelPartidos con N controles GeneradorOptions del legacy.
    /// </summary>
    public ObservableCollection<ValoracionPartidoItem> Valoraciones { get; }

    // Jornada destino. En el legacy era el NumTextBox 'numJornada'.
    [ObservableProperty]
    private double _jornada;

    // Mensaje de estado para feedback al usuario (sustituye los MessageBox).
    [ObservableProperty]
    private string _estado = string.Empty;

    /// <summary>
    /// btnOK_Click del legacy: valida la jornada, copia la valoración a la
    /// matriz y genera/guarda los ficheros de columnas probables.
    /// </summary>
    [RelayCommand]
    private void Generar()
    {
        if (Jornada <= 0)
        {
            Estado = "Especifique un número de jornada.";
            return;
        }

        // TODO (dominio): replicar GenerarCPs.btnOK_Click — copiar valoración
        // (CopiarValoracion), cargar configuración de CPs (DatosHelper.ObtenerDatos),
        // generar columnas (CPs.CrearCPs) y guardar los ficheros .txt en
        // Condiciones/ vía la clase IO. No implementado en esta capa de UI.
        Estado = "Generación pendiente de la capa de dominio (CPs / IO).";
    }

    /// <summary>
    /// btnImportarVal_Click del legacy: abre un .txt de valoración, lo parsea
    /// con Porcentajes y vuelca los porcentajes a pantalla.
    /// </summary>
    [RelayCommand]
    private void ImportarPorcentajes()
    {
        // TODO (dominio): replicar GenerarCPs.btnImportarVal_Click — FileOpenPicker
        // sobre Condiciones/*.txt, parseo con Free1X2 Porcentajes y volcado a
        // Valoraciones (PonerValoracionPantalla). No implementado aquí.
        Estado = "Importar % pendiente de la capa de dominio (Porcentajes).";
    }

    /// <summary>
    /// btnConfigurar_Click del legacy: abría el diálogo ConfigCPsFrm.
    /// </summary>
    [RelayCommand]
    private void ConfigurarColumnas()
    {
        // TODO (dominio/navegación): abrir el equivalente a ConfigCPsFrm.
        Estado = "Configurar columnas pendiente de navegación (ConfigCPsFrm).";
    }

    /// <summary>
    /// btnSeparador_Click del legacy: abría el diálogo FiltroPorcenJB.
    /// </summary>
    [RelayCommand]
    private void SeparadorPorcentajes()
    {
        // TODO (dominio/navegación): abrir el equivalente a FiltroPorcenJB.
        Estado = "Separador de porcentajes pendiente de navegación (FiltroPorcenJB).";
    }

    /// <summary>
    /// btnDiferencias_Click del legacy: abría GeneradorCPSDiferencias.
    /// </summary>
    [RelayCommand]
    private void CpsPorDiferencias()
    {
        // TODO (dominio/navegación): abrir el equivalente a GeneradorCPSDiferencias.
        Estado = "CPs por diferencias pendiente de navegación (GeneradorCPSDiferencias).";
    }
}
