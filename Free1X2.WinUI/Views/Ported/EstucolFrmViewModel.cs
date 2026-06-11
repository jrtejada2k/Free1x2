using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "EstuCol - Generador / Analizador de Columnas Probables"
/// (legacy: EstucolFrm). El usuario selecciona dos archivos de texto (columnas reducidas y
/// columnas ganadoras), elige un modo de agrupación/emparejamiento de columnas y genera un
/// informe del escrutinio de las columnas emparejadas frente a las ganadoras.
/// </summary>
public partial class EstucolFrmViewModel : ObservableObject
{
    /// <summary>Ruta del archivo de columnas reducidas (legacy: pathReducidas).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreArchivoReducidas))]
    [NotifyPropertyChangedFor(nameof(PuedeComenzar))]
    private string _pathReducidas = string.Empty;

    /// <summary>Ruta del archivo de columnas ganadoras (legacy: pathGanadoras).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NombreArchivoGanadoras))]
    [NotifyPropertyChangedFor(nameof(PuedeComenzar))]
    private string _pathGanadoras = string.Empty;

    /// <summary>
    /// Modo de agrupación seleccionado (legacy: rdbAgrupacionA/B/C -> ModoEmparejamientoColumnasABDON).
    /// A = "1,2 - 3,4..." (por defecto), B = "1,2 - 2,3...", C = "1,3 - 2,4...".
    /// </summary>
    [ObservableProperty]
    private bool _agrupacionA = true;

    /// <summary>Modo de agrupación B (legacy: rdbAgrupacionB).</summary>
    [ObservableProperty]
    private bool _agrupacionB;

    /// <summary>Modo de agrupación C (legacy: rdbAgrupacionC).</summary>
    [ObservableProperty]
    private bool _agrupacionC;

    /// <summary>Texto de estado del proceso (legacy: lblEstado, status strip).</summary>
    [ObservableProperty]
    private string _estado = "Listo";

    /// <summary>Nombre del archivo de reducidas para mostrar (legacy: lblNombreArchivoReducidas.Text).</summary>
    public string NombreArchivoReducidas =>
        string.IsNullOrEmpty(PathReducidas) ? "Sin archivo" : System.IO.Path.GetFileName(PathReducidas);

    /// <summary>Nombre del archivo de ganadoras para mostrar (legacy: lblArchivoGanadoras.Text).</summary>
    public string NombreArchivoGanadoras =>
        string.IsNullOrEmpty(PathGanadoras) ? "Sin archivo" : System.IO.Path.GetFileName(PathGanadoras);

    /// <summary>
    /// True si ambos archivos están seleccionados (legacy: ActivaBtnComenzar / ComprobarEntradas:
    /// btnComenzar.Enabled = pathGanadoras != "" &amp;&amp; pathReducidas != "").
    /// </summary>
    public bool PuedeComenzar =>
        !string.IsNullOrEmpty(PathReducidas) && !string.IsNullOrEmpty(PathGanadoras);

    /// <summary>Abre el selector de archivo de columnas reducidas (legacy: btnAbreArchivoReducidas_Click).</summary>
    [RelayCommand]
    private void AbrirArchivoReducidas()
    {
        // TODO[dominio]: mostrar selector de archivo y asignar PathReducidas.
        //   Legacy: OpenFileDialog (InitialDirectory = Application.StartupPath + "/Columnas/",
        //   Filter = "Columnas Reducidas(*.txt)|*.txt|Todos los archivos(*.*)|*.*").
        //   En WinUI usar FileOpenPicker y luego: PathReducidas = archivo.Path;
    }

    /// <summary>Abre el selector de archivo de columnas ganadoras (legacy: btnAbreArchivoGanadoras_Click).</summary>
    [RelayCommand]
    private void AbrirArchivoGanadoras()
    {
        // TODO[dominio]: mostrar selector de archivo y asignar PathGanadoras.
        //   Legacy: OpenFileDialog (Filter = "Columnas Ganadoras(*.txt)|*.txt|...").
        //   Tras seleccionar, el legacy cuenta el total de líneas para noTotalGanadoras
        //   (StreamReader recorriendo el archivo). Conservar ese conteo aquí.
        //   En WinUI usar FileOpenPicker y luego: PathGanadoras = archivo.Path;
    }

    /// <summary>
    /// Ejecuta el proceso completo de generación de informe (legacy: btnComenzar_Click).
    /// </summary>
    [RelayCommand]
    private void Comenzar()
    {
        Estado = "Comprobando Entradas";
        if (!PuedeComenzar)
        {
            // TODO[dominio]: mostrar error "Debe especificar los dos archivos".
            //   Legacy: MessageBox.Show("Debe especificar los dos archivos", ...).
            return;
        }

        // TODO[dominio]: determinar el modo de emparejamiento según AgrupacionA/B/C.
        //   Legacy: DeterminarMetodoOrdenacion() -> ModoEmparejamientoColumnasABDON.A/B/C.

        Estado = "Emparejando columnas";
        // TODO[dominio]: emparejar las columnas reducidas según el modo elegido.
        //   Legacy: EmparejarColumnasReducidas() -> lee pathReducidas con StreamReader,
        //   Free1X2.Utils.UtilColumnas.ConvStrToLong, suma columnas con OR de bits
        //   (SumaColumnas: colA | colB) y rellena List<long> ColumnasEmparejadas.

        Estado = "Escrutando Columnas";
        // TODO[dominio]: escrutar las columnas emparejadas frente a las ganadoras.
        //   Legacy: EscrutarColumnasEmparejadas() -> Free1X2.Escrutinio.Escrutador.EscrutaApuestaMultiple,
        //   rellenando la matriz int[,] contenedorAciertos[col, ganadora].

        Estado = "Generando Informe";
        // TODO[dominio]: generar el informe y abrir el visor.
        //   Legacy: GenerarInforme() -> ObtenDatosUnaColTodasGan / ObtenDatosUnaGanTodasCol
        //   construyen List<Free1X2.Analisis.InformeColumnasABDON>, luego
        //   new VisorAnalisisColumnasAbdonFrm(...).Show().
        //   En WinUI navegar a la Page equivalente del visor pasando los informes.

        Estado = "Listo";
    }
}
