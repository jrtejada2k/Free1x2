using System;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms <c>SubirCategoriaFrm</c> ("Subir Categoría").
/// A partir de un archivo de columnas de origen permite elegir los partidos
/// involucrados, el número de signos seguidos, los niveles de acierto y una
/// combinación externa opcional, para calcular y grabar el resultado en un
/// archivo de salida.
/// </summary>
public sealed partial class SubirCategoriaFrmPage : Page
{
    public SubirCategoriaFrmViewModel ViewModel { get; } = new();

    public SubirCategoriaFrmPage()
    {
        this.InitializeComponent();
    }

    // Reemplaza OpenFileDialog del WinForms (BtnFileInClick).
    private async void OnSeleccionarOrigen(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        StorageFile? file = await AbrirSelectorAsync(guardar: false);
        if (file is not null)
        {
            ViewModel.OnArchivoOrigenSeleccionado(file.Path);
        }
    }

    // Reemplaza SaveFileDialog del WinForms (btnFileOutClick).
    private async void OnSeleccionarSalida(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        StorageFile? file = await AbrirSelectorAsync(guardar: true);
        if (file is not null)
        {
            ViewModel.OnArchivoSalidaSeleccionado(file.Path);
        }
    }

    // Reemplaza OpenFileDialog del WinForms (btFileExternas_Click).
    private async void OnSeleccionarExternas(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        StorageFile? file = await AbrirSelectorAsync(guardar: false);
        if (file is not null)
        {
            ViewModel.OnArchivoExternasSeleccionado(file.Path);
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
