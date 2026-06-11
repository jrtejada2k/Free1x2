using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

// ViewModel para ImprimirBoletoFrmPage.
// Replica los inputs del WinForms legacy Free1X2.UI.ImprimirBoletoFrm:
//   - tbmgsup / tbmgizq : margenes de impresion (superior / izquierdo)
//   - tbminbol / tbmaxbol: rango de boletos a imprimir (desde / hasta)
//   - lfile / lcols      : fichero de columnas leido y numero de columnas
//   - lblImpresora       : modelo de impresora seleccionada
//   - controlador.Rotar  : girar el boleto al imprimir
public partial class ImprimirBoletoFrmViewModel : ObservableObject
{
    // --- Posicion de impresion (margenes en puntos) ---
    [ObservableProperty]
    private double _margenSuperior = 100;

    [ObservableProperty]
    private double _margenIzquierdo = 30;

    // --- Rango de boletos ---
    [ObservableProperty]
    private double _desdeBoleto = 1;

    [ObservableProperty]
    private double _hastaBoleto = 1;

    // --- Girar boleto (legacy controlador.Rotar) ---
    [ObservableProperty]
    private bool _girarBoleto;

    // --- Estado del fichero de columnas leido ---
    [ObservableProperty]
    private string _ficheroEntrada = "-";

    [ObservableProperty]
    private int _numeroColumnas;

    // TextBlock no debe bindear int directo (regla anti-crash #2):
    // se expone como string.
    public string NumeroColumnasTexto => NumeroColumnas.ToString();

    partial void OnNumeroColumnasChanged(int value) => OnPropertyChanged(nameof(NumeroColumnasTexto));

    // --- Impresora seleccionada ---
    [ObservableProperty]
    private string _impresora = "(ninguna)";

    // ---------------------------------------------------------------------
    // Acciones (botones del form legacy). Logica de dominio marcada como TODO.
    // ---------------------------------------------------------------------

    [RelayCommand]
    private void LeerColumnas()
    {
        // TODO: portar Free1X2.UI.ImprimirBoletoFrm.LeerCols():
        //   - abrir OpenFileDialog (FileOpenPicker en WinUI) filtrando *.txt en /Columnas/
        //   - leer columnas con Free1X2.EntradaSalida.ArchivoColumnasTexto (IArchivoColumnas)
        //   - rellenar la lista 'cols', actualizar FicheroEntrada y NumeroColumnas
        //   - boletos = (cols.Count - 1) / 8 + 1; DesdeBoleto = 1; HastaBoleto = boletos
    }

    [RelayCommand]
    private void Imprimir()
    {
        // TODO: portar Free1X2.UI.ImprimirBoletoFrm.Preparar() + Imprimir(PrintPageEventArgs):
        //   - validar que hay columnas leidas
        //   - usar MargenSuperior/MargenIzquierdo y rango DesdeBoleto..HastaBoleto
        //   - generar PrintDocument que dibuja las marcas 1/X/2 de cada columna sobre el boleto
        //   - en WinUI usar Windows.Graphics.Printing / PrintManager
    }

    [RelayCommand]
    private void GuardarConfiguracion()
    {
        // TODO: portar Free1X2.UI.ImprimirBoletoFrm.SetConfig():
        //   escribir /Impresion/imprebol.cfg con margen superior, margen izquierdo,
        //   modelo (Impresora) y girar (GirarBoleto -> "si"/"no")
    }

    [RelayCommand]
    private void RecuperarConfiguracion()
    {
        // TODO: portar Free1X2.UI.ImprimirBoletoFrm.GetConfig():
        //   leer /Impresion/imprebol.cfg y poblar MargenSuperior, MargenIzquierdo,
        //   Impresora y GirarBoleto
    }

    [RelayCommand]
    private void ImpresorasConocidas()
    {
        // TODO: portar Free1X2.UI.ImprimirBoletoFrm.btnVerImpresoras_Click():
        //   abrir Free1X2.UI.ListaImpresoras(controlador) y, si hay modelo,
        //   actualizar MargenSuperior/MargenIzquierdo/Impresora/GirarBoleto
        //   desde Free1X2.MotorCalculo.ControladorImpresion
    }
}
