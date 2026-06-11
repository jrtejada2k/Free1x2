using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel de la pantalla "Álgebra" (legacy: AlgebraColumnasFrm).
/// Realiza operaciones de álgebra de combinaciones entre dos archivos de columnas:
/// eliminar repetidas, sumar, intersección de comunes o resta.
/// Equivalente legacy: Free1X2.SumadorCombinaciones + AlgebraCombTipo.
/// </summary>
public partial class AlgebraColumnasFrmViewModel : ObservableObject
{
    // Rutas de archivos seleccionados (legacy: archivoCols1 / archivoCols2 / archivoColsFinal).
    private string _rutaCombinacion1 = string.Empty;
    private string _rutaCombinacion2 = string.Empty;
    private string _rutaCombinacionFinal = string.Empty;

    // Número de partidos (signos) de cada archivo, para validar compatibilidad
    // (legacy: noSignos1 / noSignos2 vía IArchivoColumnas.ObtenNumSignos()).
    private int _noSignos1;
    private int _noSignos2;

    // Indica si la salida es binaria (legacy: salidaBinaria, FilterIndex==2 del SaveFileDialog).
    private bool _salidaBinaria;

    // Operaciones disponibles. El índice coincide con el enum legacy AlgebraCombTipo:
    // 0 = EliminaRepetidas, 1 = SumaEliminaRepetidas, 2 = SumaSoloComunes, 3 = RestaSegunda.
    public IReadOnlyList<string> Operaciones { get; } = new[]
    {
        "Elimina columnas repetidas de Combinación 1",
        "Suma Combinaciones: elimina columnas repetidas",
        "Suma Combinaciones: selecciona columnas comunes",
        "Resta combinaciones: (1)-(2)",
    };

    // Operación seleccionada (legacy: radOption1..radOption4 -> AlgebraCombTipo).
    [ObservableProperty]
    private int _operacionSeleccionada;

    // Nombre del archivo de la Combinación 1 mostrado en pantalla (legacy: lblComb1).
    [ObservableProperty]
    private string _nombreCombinacion1 = "(selecciona)";

    // Nombre del archivo de la Combinación 2 mostrado en pantalla (legacy: lblComb2).
    [ObservableProperty]
    private string _nombreCombinacion2 = "(selecciona)";

    // Nombre del archivo de resultado mostrado en pantalla (legacy: lblCombFinal).
    [ObservableProperty]
    private string _nombreCombinacionFinal = "(selecciona)";

    // Detalle (nº columnas) de la Combinación 1 (legacy: lblFiltro1).
    [ObservableProperty]
    private string _detalleCombinacion1 = string.Empty;

    // Detalle (nº columnas) de la Combinación 2 (legacy: lblFiltro2).
    [ObservableProperty]
    private string _detalleCombinacion2 = string.Empty;

    // Mensaje de resultado del cálculo (legacy: lblResultado / sumador.MensajeFinCalculo).
    [ObservableProperty]
    private string _resultado = string.Empty;

    // Habilita/deshabilita el botón Calcular durante el cálculo (legacy: btnCalcular.Enabled).
    [ObservableProperty]
    private bool _puedeCalcular = true;

    /// <summary>
    /// Selecciona el archivo de la Combinación 1.
    /// Legacy: BtnSelComb1Click -> OpenFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private void SeleccionarCombinacion1()
    {
        // TODO[dominio]: abrir selector de archivo (FileOpenPicker) en la carpeta Columnas (*.txt).
        //   Legacy AlgebraColumnasFrm.BtnSelComb1Click:
        //     - archivoCols1 = ruta seleccionada
        //     - NombreCombinacion1 = Path.GetFileNameWithoutExtension(ruta)
        //     - IArchivoColumnas archivo = new ArchivoColumnasTexto(ruta);
        //       noSignos1 = archivo.ObtenNumSignos();
        //       DetalleCombinacion1 = "Combinación 1: " + archivo.NumColumnas + " columnas.";
    }

    /// <summary>
    /// Selecciona el archivo de la Combinación 2.
    /// Legacy: BtnSelComb2Click -> OpenFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private void SeleccionarCombinacion2()
    {
        // TODO[dominio]: abrir selector de archivo (FileOpenPicker) en la carpeta Columnas (*.txt).
        //   Legacy AlgebraColumnasFrm.BtnSelComb2Click:
        //     - archivoCols2 = ruta seleccionada
        //     - NombreCombinacion2 = Path.GetFileNameWithoutExtension(ruta)
        //     - IArchivoColumnas archivo = new ArchivoColumnasTexto(ruta);
        //       noSignos2 = archivo.ObtenNumSignos();
        //       DetalleCombinacion2 = "Combinación 2: " + archivo.NumColumnas + " columnas.";
    }

    /// <summary>
    /// Selecciona el archivo de resultado donde se guardará la operación.
    /// Legacy: BtnSelCombFinalClick -> SaveFileDialog (carpeta Columnas, *.txt).
    /// </summary>
    [RelayCommand]
    private void SeleccionarCombinacionFinal()
    {
        // TODO[dominio]: abrir selector de guardado (FileSavePicker) en la carpeta Columnas (*.txt).
        //   Legacy AlgebraColumnasFrm.BtnSelCombFinalClick:
        //     - salidaBinaria = (FilterIndex == 2)
        //     - archivoColsFinal = ruta seleccionada
        //     - NombreCombinacionFinal = Path.GetFileNameWithoutExtension(ruta)
    }

    /// <summary>
    /// Ejecuta la operación de álgebra de combinaciones seleccionada.
    /// Legacy: BtnCalcularClick -> Calcula().
    /// </summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: validar entradas y ejecutar el cálculo.
        //   Legacy AlgebraColumnasFrm.Calcula() + SonDatosEntradaValidos():
        //     AlgebraCombTipo tipo = (AlgebraCombTipo)OperacionSeleccionada;
        //     - Validar: rutas requeridas según la operación (EliminaRepetidas usa 1+final;
        //       el resto usa 1+2+final).
        //     - Validar compatibilidad: noSignos1 == noSignos2 y ambos != 0; si no,
        //       limpiar nombres/rutas y avisar "Los archivos tienen distinto número de partidos".
        //     - sumador = new SumadorCombinaciones(noSignos2) { ArchivoCols1, ArchivoCols2, ArchivoColsFinal };
        //       PuedeCalcular = false; sumador.Calcula(tipo);
        //       Resultado = sumador.MensajeFinCalculo; PuedeCalcular = true;
        //     - Si entradas inválidas: Resultado = "No ha seleccionado los archivos".
    }

    /// <summary>
    /// Cierra/regresa sin ejecutar la operación. Legacy: BtnCancelarClick -> Close().
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: navegación WinUI — Frame.GoBack() o cerrar el host contenedor
        //   (equivale a AlgebraColumnasFrm.Close()).
    }
}
