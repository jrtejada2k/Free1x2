// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>AgregaP15Frm</c> ("Añade P15 (Indeciso)").
/// Permite generar el Pleno al 15 sobre un archivo de columnas de entrada y
/// guardar el resultado en un archivo de salida, con cuatro modos de cálculo.
/// </summary>
public sealed partial class AgregaP15FrmPage : Page
{
    public AgregaP15FrmViewModel ViewModel { get; } = new();

    public AgregaP15FrmPage()
    {
        this.InitializeComponent();
    }

    // Reemplaza OpenFileDialog del WinForms (BtnFileInClick).
    private async void OnSeleccionarEntrada(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        StorageFile? file = await AbrirSelectorAsync(guardar: false);
        if (file is not null)
        {
            ViewModel.ArchivoEntrada = file.Path;
        }
    }

    // Reemplaza SaveFileDialog del WinForms (BtnFileOutClick).
    private async void OnSeleccionarSalida(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        StorageFile? file = await AbrirSelectorAsync(guardar: true);
        if (file is not null)
        {
            ViewModel.ArchivoSalida = file.Path;
        }
    }

    private static async System.Threading.Tasks.Task<StorageFile?> AbrirSelectorAsync(bool guardar)
    {
        var window = App.MainWindow;
        if (window is null)
        {
            return null;
        }
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        if (guardar)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "columnas",
            };
            picker.FileTypeChoices.Add("Columnas", new[] { ".txt" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            return await picker.PickSaveFileAsync();
        }
        else
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".txt");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            return await picker.PickSingleFileAsync();
        }
    }
}
