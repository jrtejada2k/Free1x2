using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms "DialogoAnalisisMultipleDeTramosFrm"
/// (Text = "DEFINICIÓN MÚLTIPLE DE JORNADAS Y FICHEROS").
///
/// Diálogo de configuración de un análisis múltiple de tramos. El usuario elige primero un
/// fichero de valoraciones históricas y luego trabaja en uno de dos modos (las dos pestañas
/// legacy tab14T / tabFicheros):
///   • "Combinación única": aplica una misma combinación —el desarrollo de 14 triples o un
///     fichero— sobre las temporadas marcadas (legacy chkTemporadas, rb14Triples/rbFichero).
///   • "Una combinación por jornada": construye una lista de tripletes temporada/jornada/
///     fichero, que se puede guardar y volver a leer en disco (legacy dgListaFicheros,
///     btAdd/btEliminar/btGuardar/btLeer).
/// Además define el criterio de selección de jornadas por rangos de recaudación e importes
/// de los premios de 14/13/12/11/10 (legacy grSeleccionJornada).
///
/// La UI y el estado en memoria viven en
/// <see cref="DialogoAnalisisMultipleDeTramosFrmViewModel"/>. La lógica de dominio
/// (selección de ficheros, lectura/escritura, validación y cierre devolviendo datos) está
/// marcada con TODO referenciando los tipos y métodos legacy a invocar.
/// </summary>
public sealed partial class DialogoAnalisisMultipleDeTramosFrmPage : Page
{
    public DialogoAnalisisMultipleDeTramosFrmViewModel ViewModel { get; } = new();

    public DialogoAnalisisMultipleDeTramosFrmPage()
    {
        this.InitializeComponent();
    }

    // Toda la lógica de dominio se invocará desde los RelayCommand del ViewModel:
    //  - btSeleccionarFichero_Click / CargarListaDeTemporadas(): leer el fichero de
    //    valoraciones históricas con Free1X2.EntradaSalida.ArchivoColumnasTexto
    //    (SiguienteColumna / LeeColumnaSinComas), validar 44 campos por columna y agrupar
    //    por temporada para poblar Temporadas.
    //  - btSeleccionarCombi_Click: elegir el fichero de combinación (FicheroCombinación).
    //  - btAdd_Click / EsFicheroDeColumnas(): validar y crear instancias de
    //    Free1X2.Combinacion para la lista por jornada.
    //  - btGuardar_Click / btLeer_Click: persistir/leer la lista (.lst) con
    //    ArchivoColumnasTexto.GuardarCols(...).
    //  - btAceptar_Click: validar según el modo activo y cerrar devolviendo los datos
    //    (FicheroValoracionesHistoricas, Es14Triples, ListaCombinaciones, los umbrales de
    //    premios/recaudación) al MainForm que abrió el diálogo.
    // Ver los TODO detallados en DialogoAnalisisMultipleDeTramosFrmViewModel.cs.
}
