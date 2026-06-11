using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    public partial class ImportadorCPsFrmViewModel : ObservableObject
    {
        // Estado / feedback mostrado al usuario tras una acción.
        [ObservableProperty]
        private string _estadoTexto = "Sin importaciones realizadas.";

        // Comando: Importar CPs simples desde un archivo *.txt.
        // Legacy: ImportadorCPsFrm.btnImportarSimples_Click
        [RelayCommand]
        private void ImportarSimples()
        {
            // TODO (dominio): replicar ImportadorCPsFrm.btnImportarSimples_Click.
            //  1. Mostrar selector de archivo (*.txt) con FileOpenPicker (legacy: OpenFileDialog,
            //     InitialDirectory "Columnas\\", filtro "Columnas Simples(*.txt)").
            //  2. Leer columnas con Free1X2.EntradaSalida.ArchivoColumnasTexto (LeeColumnaSinComas).
            //  3. Por cada columna no vacía crear Free1X2.MotorCalculo.ColumnaProbable:
            //       cp.Pronosticos = ObtenPronostico(columna);
            //       cp.SetNoAciertos / SetNoAciertosSeguidos / SetNoFallosSeguidos con
            //       FormulariosHelper.ObtenerTodosValores().
            //  4. Añadir cada cp al grupo temporal (grupoCPtmp) pasado por el llamador.
            //  5. Manejar formato incorrecto mostrando aviso (legacy: MessageBox).
            EstadoTexto = "Pendiente de implementar: importación de CPs simples (*.txt).";
        }

        // Comando: Importar CPs con aciertos/aciertos seguidos/fallos seguidos desde *.clm.
        // Legacy: ImportadorCPsFrm.btnImportarClm_Click
        [RelayCommand]
        private void ImportarClm()
        {
            // TODO (dominio): replicar ImportadorCPsFrm.btnImportarClm_Click.
            //  1. Selector de archivo (*.clm) con FileOpenPicker (legacy: OpenFileDialog,
            //     filtro "Columnas Con Aciertos(*.clm)").
            //  2. Leer línea a línea; formato: pronostico#Ac#Acs#Fs (4 partes separadas por '#').
            //  3. Por cada línea válida crear ColumnaProbable:
            //       cp.Pronosticos = ObtenPronostico(partes[0]);
            //       cp.SetNoAciertos(partes[1]); SetNoAciertosSeguidos(partes[2]); SetNoFallosSeguidos(partes[3]);
            //  4. Añadir cada cp al grupo temporal (grupoCPtmp).
            //  5. Manejar formato incorrecto mostrando aviso (legacy: MessageBox).
            EstadoTexto = "Pendiente de implementar: importación de CPs con aciertos (*.clm).";
        }

        // Comando: Cancelar — descartar el grupo temporal y cerrar.
        // Legacy: ImportadorCPsFrm.btnCancelar_Click
        [RelayCommand]
        private void Cancelar()
        {
            // TODO (dominio): replicar ImportadorCPsFrm.btnCancelar_Click.
            //  Vaciar el grupo temporal (grupoCPtmp = new List<ColumnaProbable>()) y cerrar/navegar atrás.
            EstadoTexto = "Importación cancelada.";
        }
    }
}
