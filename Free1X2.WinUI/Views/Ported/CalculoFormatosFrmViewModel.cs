using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para la Page portada de "CalculoFormatosFrm" (WinForms legacy: Free1X2.UI.CalculoFormatosFrm).
    /// Propósito legacy: dada una columna de 14 signos (1/X/2), extrae los formatos que aparecen
    /// (pares, tríos, cuartetos, quintetos) y los contactos, y vuelca el informe a un fichero de texto.
    /// </summary>
    public partial class CalculoFormatosFrmViewModel : ObservableObject
    {
        // Entrada principal: la columna de 14 signos. En el legacy era textBox1 (CharacterCasing.Upper, 14 chars).
        [ObservableProperty]
        private string _columna = "";

        // Ruta del fichero de salida seleccionado. En el legacy era el campo 'archivoFinal'.
        [ObservableProperty]
        private string _archivoSalida = "";

        // Texto mostrado en el label de estado del fichero (legacy label2, por defecto "Falta Fichero Salida").
        [ObservableProperty]
        private string _ficheroSalidaTexto = "Falta Fichero Salida";

        // Informe textual generado (resultado del cálculo). En el legacy se escribía directo al StreamWriter.
        [ObservableProperty]
        private string _informe = "";

        /// <summary>
        /// Legacy: BtnFileOutClick -> SaveFileDialog (filtro "Informe(*.txt)").
        /// </summary>
        [RelayCommand]
        private void SeleccionarFicheroSalida()
        {
            // TODO: portar Free1X2.UI.CalculoFormatosFrm.BtnFileOutClick
            //       Abrir un FileSavePicker (equivalente WinUI del SaveFileDialog), filtro *.txt,
            //       carpeta inicial .../Columnas/. Asignar la ruta a ArchivoSalida y
            //       FicheroSalidaTexto = Path.GetFileName(ruta).
        }

        /// <summary>
        /// Legacy: Button2Click + AnalizaColumna/InicializaContadores.
        /// Valida longitud 14, calcula pares/tríos/cuartetos/quintetos/contactos y vuelca al fichero.
        /// </summary>
        [RelayCommand]
        private void SacarFormato()
        {
            // TODO: portar Free1X2.UI.CalculoFormatosFrm.Button2Click
            //       - Validar que ArchivoSalida no esté vacío (legacy mostraba MessageBox "Falta seleccionar archivo de salida").
            //       - Validar que Columna tenga 14 caracteres (legacy: "numero de caracteres en la columna distinto de 14").
            //       - Recorrer la columna extrayendo subcadenas de longitud 2..5 contando ocurrencias
            //         (pares máx 9, tríos máx 12, cuartetos máx 11, quintetos máx 10).
            //       - Calcular contactos vía AnalizaColumna (1X,12,X2,11,XX,22,1V,XV,2V,VV).
            //       - Escribir el informe con StreamWriter(ArchivoSalida).
            //       Esta lógica de dominio NO se implementa aquí; reside en el legacy / futura capa de dominio.
        }
    }
}
