using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para ImportExportFrmPage.
    /// Porta el WinForms legacy "ImportExportFrm" (UI/ImportExportFrm.cs):
    /// herramienta de conversion de ficheros de columnas entre CSV y TXT.
    /// </summary>
    public partial class ImportExportFrmViewModel : ObservableObject
    {
        // Opciones de conversion (legacy: RadioButton rConv3 / rConv4).
        // Indice 0 -> CSV a TXT (legacy rConv3, conversion=2, checked por defecto).
        // Indice 1 -> TXT a CSV (legacy rConv4, conversion=3).
        public IReadOnlyList<string> OpcionesConversion { get; } = new[]
        {
            "Separadas por comas (*.csv) a TXT (*.txt)",
            "TXT (*.txt) a Separadas por comas (*.csv)"
        };

        [ObservableProperty]
        private int _conversionSeleccionada = 0;

        // Fichero de entrada (legacy: txFicheroEntrada).
        [ObservableProperty]
        private string _ficheroEntrada = string.Empty;

        // Fichero de salida (legacy: txtFicheroSalida).
        [ObservableProperty]
        private string _ficheroSalida = string.Empty;

        // Contadores informativos (legacy: lblColsEntrada / lblColsSalida).
        [ObservableProperty]
        private string _colsEntradaTexto = string.Empty;

        [ObservableProperty]
        private string _colsSalidaTexto = string.Empty;

        // Mensaje de estado para sustituir los MessageBox del legacy.
        [ObservableProperty]
        private string _estadoTexto = string.Empty;

        // Legacy: btnAbrirEntrada_Click. Abre OpenFileDialog filtrando por
        // csv/txt segun la conversion, lee las columnas y rellena el contador.
        [RelayCommand]
        private void AbrirEntrada()
        {
            // TODO[dominio]: replicar ImportExportFrm.btnAbrirEntrada_Click.
            // - Mostrar selector de archivo (FileOpenPicker) con filtro segun ConversionSeleccionada.
            // - Leer columnas con Free1X2.EntradaSalida.ArchivoColumnasTexto
            //   (IArchivoColumnas.LeerTodasColsSeparadasPorComas() / LeerTodasCols(true)).
            // - Proponer FicheroSalida cambiando la extension (.csv<->.txt).
            // - Actualizar ColsEntradaTexto con el numero de columnas.
        }

        // Legacy: btnAbrirSalida_Click. Selecciona el fichero de salida.
        [RelayCommand]
        private void AbrirSalida()
        {
            // TODO[dominio]: replicar ImportExportFrm.btnAbrirSalida_Click.
            // - Mostrar selector de archivo con filtro txt/csv segun ConversionSeleccionada.
            // - Asignar la ruta elegida a FicheroSalida.
        }

        // Legacy: btnOk_Click ("Hacer"). Ejecuta la conversion y guarda.
        [RelayCommand]
        private void Hacer()
        {
            // TODO[dominio]: replicar ImportExportFrm.btnOk_Click.
            // - Validar que FicheroSalida no este vacio y que existan columnas leidas.
            // - Validar que entrada != salida.
            // - Confirmar sobreescritura si el fichero de salida ya existe.
            // - Guardar con Free1X2.EntradaSalida.ArchivoColumnasTexto
            //   (IArchivoColumnas.GuardarTodasCols(columnas, true/false)).
            // - Actualizar ColsSalidaTexto y EstadoTexto con el resultado.
        }

        // Legacy: btnCancel_Click. Cierra el formulario.
        [RelayCommand]
        private void Cancelar()
        {
            // TODO[dominio]: replicar ImportExportFrm.btnCancel_Click (this.Close()).
            // En WinUI, navegar atras o cerrar la pagina segun el host de navegacion.
        }
    }
}
