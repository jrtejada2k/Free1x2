using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Compresor *.z3q" (legacy: Free1X2.UI.Compresor).
/// Comprime un archivo de columnas de texto (*.txt) a formato propio *.z3q
/// aplicando un nivel de compresión (0-9), y descomprime *.z3q de vuelta a
/// texto. Equivalente legacy: Compresor.EntradaFichero / EntradaFicheroComprimido.
///
/// Los selectores de archivo, la lectura/dimensionado de columnas con
/// ArchivoColumnasTexto + ConvertidorDeBases (ambos en Free1X2.Domain) y el volcado
/// de la descompresión a texto están cableados. El núcleo de (des)compresión
/// (Free1X2.Utils.CompresorZip, basado en SharpZipLib) permanece en el proyecto
/// WinForms y NO está en Free1X2.Domain, que es la única referencia de Free1X2.WinUI;
/// por eso ese paso concreto queda como TODO preciso (no se fabrica la lógica).
/// </summary>
public partial class CompresorViewModel : ObservableObject
{
    // Matriz de columnas activas (legacy: BitArray arrayColumnas de 4.782.969 = 3^14).
    private BitArray _arrayColumnas = new(4782969, false);

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). Compresor.Close() legacy.</summary>
    public Action? Volver { get; set; }

    // Niveles de compresión disponibles (legacy: cbbNivel.Items "0".."9", default "5").
    public IReadOnlyList<string> NivelesCompresion { get; } = new[]
    {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
    };

    // Índice del nivel de compresión seleccionado (legacy: cbbNivel.Text, default "5").
    [ObservableProperty]
    private int _nivelSeleccionado = 5;

    // Nombre del archivo de columnas (*.txt) a comprimir (legacy: lblNombreArchivo del groupBox1).
    [ObservableProperty]
    private string _nombreArchivoTexto = "(ningún archivo)";

    // Nombre del archivo comprimido (*.z3q) a descomprimir (legacy: lblArchivoSalida del groupBox2).
    [ObservableProperty]
    private string _nombreArchivoComprimido = "(ningún archivo)";

    // Mensaje de estado del proceso (legacy: lblEstado: "Preparado"/"Guardando"/"Guardado").
    [ObservableProperty]
    private string _estado = "Preparado";

    /// <summary>
    /// Selecciona un archivo de columnas (*.txt), lo carga y solicita guardar el
    /// resultado comprimido (*.z3q). Legacy: btnAbreArchivo_Click -> EntradaFichero().
    /// </summary>
    [RelayCommand]
    private async Task ComprimirArchivo()
    {
        var abrir = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        abrir.FileTypeFilter.Add(".txt");
        abrir.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(abrir, AppServices.WindowHandle);

        StorageFile? entrada = await abrir.PickSingleFileAsync();
        if (entrada is null) return;

        try
        {
            // Réplica de EntradaFichero() (Free1X2/UI/Compresor.cs línea 40):
            // dimensiona el BitArray con la longitud de la primera columna y marca cada
            // columna leída. ArchivoColumnasTexto y ConvertidorDeBases están en Free1X2.Domain.
            NombreArchivoTexto = Path.GetFileName(entrada.Path);
            bool ok = await Task.Run(() =>
            {
                IArchivoColumnas ac = new ArchivoColumnasTexto(entrada.Path);
                ac.SiguienteColumna();
                string col = ac.LeeColumnaSinComas();
                var conv = new ConvertidorDeBases((byte)col.Length);
                _arrayColumnas = new BitArray(conv.ObtenTamañoBitArray(col.Length), false);
                ac.Cerrar();

                ac = new ArchivoColumnasTexto(entrada.Path);
                while (ac.SiguienteColumna())
                {
                    string columna = ac.LeeColumnaSinComas();
                    if (columna.Length > 15 || columna.Length < 14)
                    {
                        _arrayColumnas.SetAll(false);
                        return false; // "Error leyendo columnas" (legacy línea 62).
                    }
                    _arrayColumnas[conv.ConvColumnaANumero(columna)] = true;
                }
                ac.Cerrar();
                return true;
            });

            if (!ok)
            {
                NombreArchivoTexto = "(ningún archivo)";
                AppServices.MostrarError("Error leyendo columnas");
                return;
            }

            await GuardarFicheroComprimido(entrada.Path);
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("Error al leer el archivo de columnas: " + ex.Message);
        }
    }

    /// <summary>Réplica de GuardarFicheroComprimido() (Free1X2/UI/Compresor.cs línea 89).</summary>
    private async Task GuardarFicheroComprimido(string rutaEntrada)
    {
        Estado = "Guardando";
        var guardar = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = Path.GetFileNameWithoutExtension(rutaEntrada),
        };
        guardar.FileTypeChoices.Add("Columnas Comprimidas", new List<string> { ".z3q" });
        WinRT.Interop.InitializeWithWindow.Initialize(guardar, AppServices.WindowHandle);

        StorageFile? salida = await guardar.PickSaveFileAsync();
        if (salida is null)
        {
            Estado = "Preparado";
            return;
        }

        // TODO[dominio]: compresión real — falta Free1X2.Utils.CompresorZip.Comprimir,
        //   que vive en el proyecto WinForms (usa ICSharpCode.SharpZipLib) y NO está en
        //   Free1X2.Domain (única referencia de Free1X2.WinUI). Equivalente legacy
        //   (Free1X2/UI/Compresor.cs línea 101):
        //     CompresorZip.Comprimir(_arrayColumnas, salida.Path, salida.Path,
        //         _arrayColumnas.Length, NivelSeleccionado);  // nivel 0-9
        //   Cablear cuando CompresorZip se porte a Free1X2.Domain (o se referencie SharpZipLib).
        Estado = "Compresión pendiente: CompresorZip (SharpZipLib) no está en Free1X2.Domain.";
    }

    /// <summary>
    /// Selecciona un archivo comprimido (*.z3q), lo descomprime y solicita guardar
    /// el resultado como columnas de texto (*.txt).
    /// Legacy: btnAbreArchivoComp_Click -> EntradaFicheroComprimido().
    /// </summary>
    [RelayCommand]
    private async Task DescomprimirArchivo()
    {
        var abrir = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
        };
        abrir.FileTypeFilter.Add(".z3q");
        abrir.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(abrir, AppServices.WindowHandle);

        StorageFile? entrada = await abrir.PickSingleFileAsync();
        if (entrada is null) return;

        // Réplica de EntradaFicheroComprimido() (Free1X2/UI/Compresor.cs línea 74).
        NombreArchivoComprimido = Path.GetFileName(entrada.Path);

        // TODO[dominio]: descompresión real — falta Free1X2.Utils.CompresorZip.Descomprimir,
        //   que vive en el proyecto WinForms (usa ICSharpCode.SharpZipLib) y NO está en
        //   Free1X2.Domain (única referencia de Free1X2.WinUI). Equivalente legacy
        //   (Free1X2/UI/Compresor.cs línea 83):
        //     _arrayColumnas = CompresorZip.Descomprimir(entrada.Path);
        //   Con el BitArray devuelto, GuardarFicheroDescomprimido() (ya cableado abajo) vuelca
        //   las columnas a texto con ConvertidorDeBases + ArchivoColumnasTexto (ambos en Domain).
        //   Cablear cuando CompresorZip se porte a Free1X2.Domain (o se referencie SharpZipLib).
        Estado = "Descompresión pendiente: CompresorZip (SharpZipLib) no está en Free1X2.Domain.";

        // await GuardarFicheroDescomprimido();  // habilitar tras obtener _arrayColumnas real.
    }

    /// <summary>
    /// Réplica de GuardarFicheroDescomprimido() (Free1X2/UI/Compresor.cs línea 105): vuelca el
    /// BitArray a un .txt de columnas. Sólo usa Domain (ConvertidorDeBases + ArchivoColumnasTexto),
    /// pero depende del BitArray que produce CompresorZip.Descomprimir (TODO arriba), por lo que
    /// hoy no se invoca. Se deja listo para cuando ese paso esté disponible.
    /// </summary>
    private async Task GuardarFicheroDescomprimido()
    {
        Estado = "Guardando";
        var guardar = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "Columnas",
        };
        guardar.FileTypeChoices.Add("Columnas", new List<string> { ".txt" });
        WinRT.Interop.InitializeWithWindow.Initialize(guardar, AppServices.WindowHandle);

        StorageFile? salida = await guardar.PickSaveFileAsync();
        if (salida is null)
        {
            Estado = "Preparado";
            return;
        }

        await Task.Run(() =>
        {
            var conv = new ConvertidorDeBases();
            conv = new ConvertidorDeBases((byte)conv.ObtenNumeroPartidosColBin(_arrayColumnas.Count));
            IArchivoColumnas aColsTxt = new ArchivoColumnasTexto(salida.Path);
            for (int i = 0; i < _arrayColumnas.Count; i++)
            {
                if (_arrayColumnas[i])
                {
                    aColsTxt.GuardarCols(conv.ConvNumAColumna(i));
                }
            }
            aColsTxt.Cerrar();
        });
        Estado = "Guardado";
    }

    /// <summary>
    /// Cierra/regresa de la pantalla. Legacy: btnCerrar_Click -> Close().
    /// </summary>
    [RelayCommand]
    private void Cerrar()
    {
        Volver?.Invoke();
    }
}
