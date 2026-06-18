// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Pickers;

using Free1X2;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>BoletoFrm</c> ("Boleto").
/// Visor/navegador de boletos: abre un archivo de combinación, calcula cuántos boletos
/// imprimibles contiene y permite recorrerlos (primero / anterior / ir a nº / siguiente /
/// último), mostrando cada uno en el <c>BoletoControl</c>.
/// </summary>
public sealed partial class BoletoFrmPage : Page
{
    /// <summary>
    /// Handoff estático con los parámetros del boleto a abrir (legacy: VerBoletos fijaba
    /// <c>boleto.ArchivoCombinacion</c>, <c>boleto.ordenarPor</c> y <c>boleto.tipoOrden</c>
    /// antes de <c>boleto.ShowDialog()</c>). Lo deja el productor (VerBoletosViewModel) antes
    /// de navegar y lo consume <see cref="OnNavigatedTo"/>. Mismo patrón que
    /// EstucolFrmViewModel.UltimoInforme. Null = apertura autónoma (selección manual de fichero).
    /// </summary>
    public static (string fichero, OrdenarMatriz orden, TipoOrden tipo)? ParametrosBoleto { get; set; }

    public BoletoFrmViewModel ViewModel { get; } = new();

    public BoletoFrmPage()
    {
        this.InitializeComponent();

        // Sincroniza el boleto visual con el boleto activo del ViewModel. Reemplaza el
        // volcado que el WinForms hacía sobre las 8 ControlColumnaBoleto en LlenarBoleto.
        ViewModel.BoletoCambiado += OnBoletoCambiado;
    }

    /// <summary>
    /// Recibe el handoff de VerBoletos (legacy: boleto.ArchivoCombinacion/ordenarPor/tipoOrden
    /// antes de ShowDialog). Si está presente, fija los parámetros y carga el boleto, igual que
    /// hacía el BoletoFrm legacy en su Load. Sin él, la página funciona de forma autónoma
    /// (selección manual con OnSeleccionarCombinacion).
    /// </summary>
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ParametrosBoleto is { } p)
        {
            ParametrosBoleto = null; // se consume una sola vez
            ViewModel.ArchivoCombinacion = p.fichero;
            ViewModel.OrdenarPor = p.orden;
            ViewModel.TipoOrden = p.tipo;
            await ViewModel.CargarCommand.ExecuteAsync(null);
        }
    }

    private void OnBoletoCambiado(object? sender, (string[] signos, int[] numerosColumna) e)
    {
        BoletoVisual.Llenar(e.signos, e.numerosColumna);
    }

    // Reemplaza la asignación de ArchivoCombinacion del WinForms (BoletoFrm_Load esperaba
    // la ruta en el campo público ArchivoCombinacion antes de cargar).
    private async void OnSeleccionarCombinacion(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        StorageFile? file = await AbrirSelectorAsync();
        if (file is not null)
        {
            ViewModel.ArchivoCombinacion = file.Path;
        }
    }

    private static async System.Threading.Tasks.Task<StorageFile?> AbrirSelectorAsync()
    {
        var window = App.MainWindow;
        if (window is null)
        {
            return null;
        }
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        picker.FileTypeFilter.Add(".txt");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        return await picker.PickSingleFileAsync();
    }
}
