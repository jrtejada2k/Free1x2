using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Boleto" (visor/navegador de boletos).
/// Replica el WinForms <c>BoletoFrm</c>: abre un archivo de columnas, calcula cuántos
/// boletos imprimibles contiene y permite navegar entre ellos (primero / anterior /
/// ir a nº / siguiente / último), mostrando cada boleto en el <c>BoletoControl</c>.
/// </summary>
public partial class BoletoFrmViewModel : ObservableObject
{
    // Ruta del archivo de combinación de entrada (WinForms: ArchivoCombinacion).
    [ObservableProperty]
    private string _archivoCombinacion = "";

    // Total de boletos imprimibles del archivo (WinForms: campo 'boletos' / totalBoletos.Text).
    // NotifyPropertyChangedFor mantiene la proyección en cadena y la disponibilidad de navegación.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalBoletosTexto))]
    private int _totalBoletos = 1;

    /// <summary>Proyección en cadena de <see cref="TotalBoletos"/> para enlazar a TextBlock.Text.</summary>
    public string TotalBoletosTexto => TotalBoletos.ToString();

    // Número de boleto actualmente mostrado (1-based, como el WinForms 'boletoActual').
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BoletoActualTexto))]
    private int _boletoActual = 1;

    /// <summary>Proyección en cadena de <see cref="BoletoActual"/> (x:Bind no convierte int -&gt; string).</summary>
    public string BoletoActualTexto => BoletoActual.ToString();

    // Destino del cuadro "Ir a nº" (WinForms: boletoActual TextBox editable). NumberBox -> double.
    [ObservableProperty]
    private double _irABoleto = 1;

    // Mensaje de estado / errores de rango (WinForms: MessageBox "fuera de rango").
    [ObservableProperty]
    private string _estado = "Selecciona un archivo de combinación para ver sus boletos.";

    /// <summary>
    /// Equivale a <c>BoletoFrm_Load</c>: lee el archivo, crea la matriz de columnas,
    /// calcula el total de boletos y muestra el primero.
    /// </summary>
    [RelayCommand]
    private void Cargar()
    {
        if (string.IsNullOrEmpty(ArchivoCombinacion))
        {
            Estado = "No se ha seleccionado ningún archivo.";
            return;
        }

        // TODO(dominio): portar la carga de Free1X2.UI.BoletoFrm.BoletoFrm_Load.
        //   - Free1X2.EntradaSalida.ArchivoColumnasTexto (IArchivoColumnas):
        //       ObtenNumCols / SiguienteColumna / LeeColumnaSinComas / Cerrar.
        //   - Free1X2.UI.Controls.ControlBoleto: CreaMatriz(numCols), campo 'boletos',
        //       'apuestas', matriz 'columna[]', OrdenarMatrizColumnas(ordenarPor, tipoOrden),
        //       LlenarBoleto(numBol).
        //   - Aplicar OrdenarMatriz/TipoOrden (campos ordenarPor / tipoOrden del WinForms).
        //   Al terminar: TotalBoletos = nº calculado e invocar LlenarBoleto(0).

        Estado = "Carga pendiente de portar dominio.";
    }

    /// <summary>Equivale a <c>btnPrimero_Click</c>: muestra el primer boleto.</summary>
    [RelayCommand]
    private void Primero() => LlenarBoleto(0);

    /// <summary>Equivale a <c>btnAnterior_Click</c>: retrocede un boleto.</summary>
    [RelayCommand]
    private void Anterior() => LlenarBoleto(BoletoActual - 2);

    /// <summary>Equivale a <c>btnSiguiente_Click</c>: avanza un boleto.</summary>
    [RelayCommand]
    private void Siguiente() => LlenarBoleto(BoletoActual);

    /// <summary>Equivale a <c>btnUltimo_Click</c>: muestra el último boleto.</summary>
    [RelayCommand]
    private void Ultimo() => LlenarBoleto(TotalBoletos - 1);

    /// <summary>Equivale a <c>btnIr_Click</c>: salta al boleto indicado, validando el rango.</summary>
    [RelayCommand]
    private void Ir()
    {
        int destino = (int)IrABoleto;
        if (destino < 1 || destino > TotalBoletos)
        {
            Estado = "El número de boleto está fuera de rango.";
            return;
        }
        LlenarBoleto(destino - 1);
    }

    /// <summary>
    /// Equivale a <c>LlenarBoleto(int)</c> del WinForms: fija el boleto mostrado (0-based)
    /// y actualiza el contador 1-based. La pintura real del boleto la hará el dominio.
    /// </summary>
    private void LlenarBoleto(int numBol)
    {
        if (numBol < 0)
        {
            numBol = 0;
        }
        if (numBol > TotalBoletos - 1)
        {
            numBol = TotalBoletos - 1;
        }

        // TODO(dominio): controlBoleto1.LlenarBoleto(numBol) -> volcar la columna numBol
        //   en el BoletoControl (ViewModel.Partidos) para reflejar los signos del boleto.

        BoletoActual = numBol + 1;
        IrABoleto = BoletoActual;
        Estado = $"Mostrando boleto {BoletoActual} de {TotalBoletos}.";
    }
}
