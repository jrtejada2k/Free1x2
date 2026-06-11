using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Compresor *.z3q" (legacy: Free1X2.UI.Compresor).
/// Comprime un archivo de columnas de texto (*.txt) a formato propio *.z3q
/// aplicando un nivel de compresión (0-9), y descomprime *.z3q de vuelta a
/// texto. Equivalente legacy: Compresor.EntradaFichero / EntradaFicheroComprimido
/// usando Free1X2.Utils.CompresorZip y ConvertidorDeBases.
/// </summary>
public partial class CompresorViewModel : ObservableObject
{
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
    private void ComprimirArchivo()
    {
        // TODO[dominio]: abrir selector (FileOpenPicker) filtro "Columnas (*.txt)".
        //   Legacy Compresor.EntradaFichero():
        //     - NombreArchivoTexto = Path.GetFileName(ruta)
        //     - Leer la primera columna con ArchivoColumnasTexto para dimensionar:
        //         conv = new ConvertidorDeBases((byte)col.Length);
        //         arrayColumnas = new BitArray(conv.ObtenTamañoBitArray(col.Length), false);
        //     - Recorrer todas las columnas (ac.SiguienteColumna / LeeColumnaSinComas):
        //         · validar longitud (14 o 15); si no, avisar "Error leyendo columnas",
        //           limpiar arrayColumnas y NombreArchivoTexto.
        //         · arrayColumnas[conv.ConvColumnaANumero(columna)] = true;
        //     - Luego GuardarFicheroComprimido():
        //         Estado = "Guardando";
        //         FileSavePicker filtro "Columnas Comprimidas (*.z3q)";
        //         CompresorZip.Comprimir(arrayColumnas, nombreFinal, ruta,
        //             arrayColumnas.Length, NivelSeleccionado);  // nivel 0-9
        //         Estado = "Guardado";
    }

    /// <summary>
    /// Selecciona un archivo comprimido (*.z3q), lo descomprime y solicita guardar
    /// el resultado como columnas de texto (*.txt).
    /// Legacy: btnAbreArchivoComp_Click -> EntradaFicheroComprimido().
    /// </summary>
    [RelayCommand]
    private void DescomprimirArchivo()
    {
        // TODO[dominio]: abrir selector (FileOpenPicker) filtro "Columnas Comprimidas (*.z3q)".
        //   Legacy Compresor.EntradaFicheroComprimido():
        //     - NombreArchivoComprimido = Path.GetFileName(ruta)
        //     - arrayColumnas = CompresorZip.Descomprimir(ruta);
        //     - Luego GuardarFicheroDescomprimido():
        //         Estado = "Guardando";
        //         FileSavePicker filtro "Columnas (*.txt)";
        //         conv = new ConvertidorDeBases(
        //             (byte)conv.ObtenNumeroPartidosColBin(arrayColumnas.Count));
        //         Recorrer arrayColumnas: si bit activo ->
        //             aColsTxt.GuardarCols(conv.ConvNumAColumna(i));
        //         Estado = "Guardado";
    }

    /// <summary>
    /// Cierra/regresa de la pantalla. Legacy: btnCerrar_Click -> Close().
    /// </summary>
    [RelayCommand]
    private void Cerrar()
    {
        // TODO[dominio]: navegación WinUI — Frame.GoBack() o cerrar el host contenedor
        //   (equivale a Compresor.Close()).
    }
}
