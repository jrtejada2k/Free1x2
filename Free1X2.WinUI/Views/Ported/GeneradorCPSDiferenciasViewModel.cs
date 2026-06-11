using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la página portada de <c>Free1X2.UI.GeneradorCPSDiferencias</c>
/// (Generador de CPs por diferencias).
///
/// Replica las entradas del WinForms legacy:
///  - txtColumna1 -> ColumnaInicial (cadena de 14 signos 1/X/2, mayúsculas)
///  - txtColumna2 -> ColumnaAlternativa (cadena de 14 signos 1/X/2, mayúsculas)
///  - txtArchivo  -> ArchivoDestino
///  - groupBox1 (rFijos/rDobles)  -> TipoColumnaIndex (0 = Fijos, 1 = Dobles)
///  - groupBox2 (rDif1/rDif2)     -> NumeroDiferenciasIndex (0 = 1, 1 = 2)
/// </summary>
public partial class GeneradorCPSDiferenciasViewModel : ObservableObject
{
    [ObservableProperty]
    private string _columnaInicial = string.Empty;

    [ObservableProperty]
    private string _columnaAlternativa = string.Empty;

    [ObservableProperty]
    private string _archivoDestino = string.Empty;

    // groupBox1 "Columnas": 0 = Fijos, 1 = Dobles. Legacy default rDobles.Checked = true.
    [ObservableProperty]
    private int _tipoColumnaIndex = 1;

    // groupBox2 "Diferencias": 0 = 1, 1 = 2. Legacy default rDif2.Checked = true.
    [ObservableProperty]
    private int _numeroDiferenciasIndex = 1;

    // Mensaje de estado para la barra inferior (sustituye los MessageBox legacy).
    [ObservableProperty]
    private string _estadoTexto = string.Empty;

    public IReadOnlyList<string> TiposColumna { get; } = new[] { "Fijos", "Dobles" };

    public IReadOnlyList<string> NumerosDiferencias { get; } = new[] { "1", "2" };

    [RelayCommand]
    private void Examinar()
    {
        // TODO: legacy GeneradorCPSDiferencias.button1_Click -> SaveFileDialog (fd).
        // Mostrar un FileSavePicker (WinUI) con filtro "*.txt", extensión por defecto ".txt",
        // y asignar la ruta seleccionada a ArchivoDestino.
        EstadoTexto = "TODO: seleccionar archivo destino (SaveFileDialog legacy).";
    }

    [RelayCommand]
    private void Generar()
    {
        // TODO: legacy GeneradorCPSDiferencias.button2_Click.
        // 1. Validar ColumnaInicial y ColumnaAlternativa con esValida(...) (14 chars, solo 1/X/2).
        // 2. Validar que ArchivoDestino no esté vacío.
        // 3. Según TipoColumnaIndex (Fijos/Dobles) y NumeroDiferenciasIndex (1/2) invocar el método
        //    de combinación correspondiente del legacy:
        //      - combinarFijos1 / combinarFijos2
        //      - combinarDobles1 / combinarDobles2
        // 4. Persistir las columnas resultantes vía
        //    Free1X2.EntradaSalida.IArchivoColumnas / ArchivoColumnasTexto.GuardarColsComa(...).
        EstadoTexto = "TODO: generar columnas (button2_Click legacy).";
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO: legacy GeneradorCPSDiferencias.button3_Click -> Close().
        // En WinUI esto debería navegar atrás o cerrar el contenedor de la Page.
        EstadoTexto = string.Empty;
    }
}
