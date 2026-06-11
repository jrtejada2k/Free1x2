using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Analizar fichero de columnas" (legacy: AnalizarFicheroFrm).
/// Permite seleccionar un fichero de columnas (.txt/.cols), leer cuántas columnas
/// contiene y lanzar el análisis de la combinación, con opción de incluir el pleno al 15.
/// </summary>
public partial class AnalizarFicheroFrmViewModel : ObservableObject
{
    /// <summary>Ruta del fichero de columnas de entrada (legacy: txFicheroEntrada).</summary>
    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    /// <summary>Texto informativo con el nº de columnas leídas (legacy: lblColsEntrada).</summary>
    [ObservableProperty]
    private string _resumenColumnas = string.Empty;

    /// <summary>Incluir pleno al 15 en el análisis (legacy: chkPleno).</summary>
    [ObservableProperty]
    private bool _incluirPleno;

    /// <summary>
    /// Habilita el check "Incluir pleno al 15" solo cuando las columnas tienen 15 signos
    /// (legacy: chkPleno.Enabled = (columnas[0].Length == 15)).
    /// </summary>
    [ObservableProperty]
    private bool _puedeIncluirPleno;

    /// <summary>
    /// Habilita el botón Analizar solo cuando hay columnas cargadas
    /// (legacy: btnOk.Enabled = (columnas.Length > 0)).
    /// </summary>
    [ObservableProperty]
    private bool _puedeAnalizar;

    /// <summary>
    /// Selecciona el fichero de columnas y lee cuántas columnas contiene.
    /// </summary>
    [RelayCommand]
    private void AbrirFichero()
    {
        // TODO[dominio]: abrir diálogo de fichero y leer las columnas.
        //   Legacy: AnalizarFicheroFrm.btnAbrirEntrada_Click
        //     - abreFiltroDialog (OpenFileDialog) filtro "*.txt|*.cols|*.*",
        //       directorio inicial Application.StartupPath + "/Columnas/".
        //       En WinUI usar Windows.Storage.Pickers.FileOpenPicker.
        //     - cols = new Free1X2.EntradaSalida.ArchivoColumnasTexto(ruta);
        //       columnas = cols.LeerTodasCols(false); cols.Cerrar();
        //     - ResumenColumnas = columnas.Length + " columnas.";
        //     - PuedeAnalizar = columnas.Length > 0;
        //     - PuedeIncluirPleno = columnas.Length > 0 && columnas[0].Length == 15;
        //   El dominio (Free1X2.EntradaSalida.IArchivoColumnas) aún no está migrado.
    }

    /// <summary>
    /// Lanza el análisis de la combinación del fichero seleccionado.
    /// </summary>
    [RelayCommand]
    private void Analizar()
    {
        // TODO[dominio]: ejecutar el análisis del fichero de columnas.
        //   Legacy: AnalizarFicheroFrm.btnOk_Click
        //     - IArchivoColumnas aCol = new ArchivoColumnasTexto(FicheroEntrada);
        //       int partidos = aCol.ObtenNumSignos(); aCol.Cerrar();
        //     - var analizador = new Free1X2.MotorCalculo.Analizador(partidos);
        //       analizador.ArchivoColumnasBase = FicheroEntrada;
        //       for (i=0..partidos) analizador.SetPronostico(i, "1,X,2");
        //       analizador.AnalizaCombinacion(true, true);
        //     - El flag IncluirPleno (chkPleno) condiciona el tratamiento del pleno al 15.
        //   Free1X2.MotorCalculo.Analizador aún no está migrado a Free1X2.Domain.
    }
}
