// Free1X2 · WinUI 3 — WIN3
using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.WinUI.Services;
using Windows.Storage;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Base común del cuarteto <c>Guardar</c>/<c>Abrir</c>/<c>Copiar</c>/<c>Pegar</c> de los
/// ViewModels de filtro (refactor C-18, Tanda 3 de docs/PLAN_MEJORAS.md §7ter).
///
/// La ESTRUCTURA de los cuatro comandos es idéntica en todos los filtros: pedir el fichero
/// con <see cref="PickerHelper"/>, delegar la (de)serialización concreta en cada VM, y usar
/// el mismo formato de portapapeles (fichero temporal en <c>Temp/</c>), las mismas
/// extensiones y los mismos mensajes de error vía <see cref="AppServices.MostrarError"/>.
///
/// Lo ESPECÍFICO de cada filtro queda en miembros abstractos:
/// <see cref="NombreSugerido"/>, <see cref="TiposGuardar"/>, <see cref="ExtensionesAbrir"/>,
/// <see cref="RutaTemporal"/> y los métodos <see cref="GuardarEn"/> / <see cref="AbrirDesde"/>.
///
/// Hereda de <see cref="ObservableObject"/> para que los VM derivados sigan usando
/// <c>[ObservableProperty]</c> y <c>[RelayCommand]</c> con normalidad. Los comandos generados
/// (<c>GuardarCommand</c>, <c>AbrirCommand</c>, <c>CopiarCommand</c>, <c>PegarCommand</c>) se
/// heredan y quedan disponibles para el enlace <c>x:Bind</c> de las páginas.
/// </summary>
public abstract partial class FiltroArchivoViewModelBase : ObservableObject
{
    /// <summary>Nombre de fichero sugerido en el diálogo Guardar (sin extensión).</summary>
    protected abstract string NombreSugerido { get; }

    /// <summary>
    /// Pares (etiqueta, extensión) que se añaden en orden a <c>FileTypeChoices</c> del
    /// diálogo Guardar. El primero fija la extensión por defecto.
    /// </summary>
    protected abstract (string etiqueta, string extension)[] TiposGuardar { get; }

    /// <summary>Extensiones del filtro del diálogo Abrir, en orden.</summary>
    protected abstract string[] ExtensionesAbrir { get; }

    /// <summary>
    /// Ruta del fichero temporal de Copiar/Pegar (legacy: StartupPath + "/Temp/tmp.XXX").
    /// La extensión temporal es específica de cada filtro y NO siempre coincide con la del
    /// diálogo (p. ej. Diferencias guarda en <c>tmp.rep</c> aunque su extensión es <c>.dif</c>).
    /// </summary>
    protected abstract string RutaTemporal { get; }

    /// <summary>Serializa a disco el filtro concreto (específico de cada VM).</summary>
    protected abstract void GuardarEn(string nombreArchivo);

    /// <summary>
    /// Deserializa el filtro concreto desde disco y vuelca sus valores a pantalla
    /// (específico de cada VM).
    /// </summary>
    protected abstract void AbrirDesde(string nombreArchivo);

    [RelayCommand]
    private async Task Guardar()
    {
        StorageFile? file = await PickerHelper.GuardarAsync(NombreSugerido, TiposGuardar);
        if (file == null) return;

        try
        {
            GuardarEn(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo guardar: " + ex.Message);
        }
    }

    [RelayCommand]
    private async Task Abrir()
    {
        StorageFile? file = await PickerHelper.AbrirAsync(ExtensionesAbrir);
        if (file == null) return;

        try
        {
            AbrirDesde(file.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo abrir: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Copiar()
    {
        try
        {
            string ruta = RutaTemporal;
            Directory.CreateDirectory(Path.GetDirectoryName(ruta)!);
            GuardarEn(ruta);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se pudo copiar: " + ex.Message);
        }
    }

    [RelayCommand]
    private void Pegar()
    {
        if (File.Exists(RutaTemporal))
        {
            AbrirDesde(RutaTemporal);
        }
    }
}
