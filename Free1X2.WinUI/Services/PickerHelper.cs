// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Centraliza el boilerplate de los file pickers (C-17): creación del
/// <see cref="FileOpenPicker"/>/<see cref="FileSavePicker"/>, enganche del
/// <c>InitializeWithWindow</c> contra el HWND de la ventana principal
/// (<see cref="AppServices.WindowHandle"/>) y el <c>await</c> del resultado.
///
/// El refactor es puramente mecánico: conserva EXACTAMENTE las extensiones, el
/// nombre sugerido, las etiquetas de tipo, la extensión por defecto y la
/// ubicación inicial de cada picker, para no alterar lo que el usuario ve en el
/// diálogo. Los sitios que obtienen el handle de otra forma (p. ej. desde
/// <c>App.MainWindow</c> con guarda de nulos) o que usan <c>FolderPicker</c> se
/// dejan sin refactorizar por diseño.
/// </summary>
public static class PickerHelper
{
    /// <summary>
    /// Abre un <see cref="FileOpenPicker"/> de selección simple en
    /// <see cref="PickerLocationId.DocumentsLibrary"/> con los filtros indicados
    /// (en el mismo orden). Devuelve el fichero elegido o <c>null</c> si se cancela.
    /// </summary>
    public static async Task<StorageFile?> AbrirAsync(params string[] extensiones)
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        foreach (var ext in extensiones) picker.FileTypeFilter.Add(ext);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        return await picker.PickSingleFileAsync();
    }

    /// <summary>
    /// Abre un <see cref="FileOpenPicker"/> de selección múltiple en
    /// <see cref="PickerLocationId.DocumentsLibrary"/> con los filtros indicados.
    /// Devuelve la lista de ficheros elegidos (vacía si se cancela).
    /// </summary>
    public static async Task<IReadOnlyList<StorageFile>> AbrirVariosAsync(params string[] extensiones)
    {
        var picker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
        foreach (var ext in extensiones) picker.FileTypeFilter.Add(ext);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        return await picker.PickMultipleFilesAsync();
    }

    /// <summary>
    /// Guarda con un <see cref="FileSavePicker"/> en
    /// <see cref="PickerLocationId.DocumentsLibrary"/>. Cada tipo es un par
    /// (etiqueta, extensión) que se añade a <c>FileTypeChoices</c> en orden.
    /// </summary>
    public static Task<StorageFile?> GuardarAsync(
        string nombreSugerido, params (string etiqueta, string extension)[] tipos)
        => GuardarInternoAsync(PickerLocationId.DocumentsLibrary, nombreSugerido, null, tipos);

    /// <summary>
    /// Igual que <see cref="GuardarAsync(string, ValueTuple{string, string}[])"/> pero
    /// permitiendo indicar la <paramref name="ubicacion"/> inicial (p. ej.
    /// <see cref="PickerLocationId.PicturesLibrary"/> para las exportaciones de imagen).
    /// </summary>
    public static Task<StorageFile?> GuardarAsync(
        PickerLocationId ubicacion, string nombreSugerido, params (string etiqueta, string extension)[] tipos)
        => GuardarInternoAsync(ubicacion, nombreSugerido, null, tipos);

    /// <summary>
    /// Guarda con un <see cref="FileSavePicker"/> en
    /// <see cref="PickerLocationId.DocumentsLibrary"/> fijando también
    /// <see cref="FileSavePicker.DefaultFileExtension"/>.
    /// </summary>
    public static Task<StorageFile?> GuardarConExtensionPorDefectoAsync(
        string nombreSugerido, string extensionPorDefecto, params (string etiqueta, string extension)[] tipos)
        => GuardarInternoAsync(PickerLocationId.DocumentsLibrary, nombreSugerido, extensionPorDefecto, tipos);

    private static async Task<StorageFile?> GuardarInternoAsync(
        PickerLocationId ubicacion, string nombreSugerido, string? extensionPorDefecto,
        (string etiqueta, string extension)[] tipos)
    {
        var picker = new FileSavePicker
        {
            SuggestedStartLocation = ubicacion,
            SuggestedFileName = nombreSugerido,
        };
        if (extensionPorDefecto is not null) picker.DefaultFileExtension = extensionPorDefecto;
        foreach (var (etiqueta, extension) in tipos)
        {
            picker.FileTypeChoices.Add(etiqueta, new List<string> { extension });
        }
        WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);
        return await picker.PickSaveFileAsync();
    }
}
