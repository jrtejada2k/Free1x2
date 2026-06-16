using System;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>BoletoFrm</c> ("Boleto").
/// Visor/navegador de boletos: abre un archivo de combinación, calcula cuántos boletos
/// imprimibles contiene y permite recorrerlos (primero / anterior / ir a nº / siguiente /
/// último), mostrando cada uno en el <c>BoletoControl</c>.
/// </summary>
public sealed partial class BoletoFrmPage : Page
{
    public BoletoFrmViewModel ViewModel { get; } = new();

    public BoletoFrmPage()
    {
        this.InitializeComponent();

        // Sincroniza el boleto visual con el boleto activo del ViewModel. Reemplaza el
        // volcado que el WinForms hacía sobre las 8 ControlColumnaBoleto en LlenarBoleto.
        ViewModel.BoletoCambiado += OnBoletoCambiado;
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
