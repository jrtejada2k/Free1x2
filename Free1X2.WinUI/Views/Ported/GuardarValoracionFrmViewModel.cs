using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para GuardarValoracionFrmPage.
    /// Porta el WinForms legacy Free1X2.UI.GuardarValoracionFrm.
    /// Permite elegir el formato del fichero de salida y el separador de campos
    /// para exportar valoraciones (porcentajes) a un .txt.
    /// </summary>
    public partial class GuardarValoracionFrmViewModel : ObservableObject
    {
        // ----- Formato del fichero -----
        // Legacy: RadioButton rb3ValoresPorFila / rb42ValoresPorFila / rb1ValorPorFila.
        // Opciones de la lista corresponden a los indices 0/1/2.

        public IReadOnlyList<string> FormatoOpciones { get; } = new[]
        {
            "3 valores por línea",
            "42 valores en una línea",
            "1 valor por línea",
        };

        // Legacy: rb3ValoresPorFila.Checked = true (opcion por defecto -> indice 0).
        [ObservableProperty]
        private string _formatoSeleccionado = "3 valores por línea";

        // ----- Separador de campos -----
        // Legacy: RadioButton rbTabulador / rbComa / rbEspacio.
        // Solo aplica cuando NO es "1 valor por línea".

        public IReadOnlyList<string> SeparadorOpciones { get; } = new[]
        {
            "[Tabulador]",
            "[Coma]",
            "[Espacio en blanco]",
        };

        // Legacy: rbTabulador.Checked = true (separador por defecto).
        [ObservableProperty]
        private string _separadorSeleccionado = "[Tabulador]";

        // El separador solo tiene sentido si se exportan varios valores por línea.
        // Legacy: en btAceptar_Click el separador solo se lee si rb3/rb42 estan marcados.
        public bool SeparadorHabilitado => FormatoSeleccionado != "1 valor por línea";

        partial void OnFormatoSeleccionadoChanged(string value)
        {
            OnPropertyChanged(nameof(SeparadorHabilitado));
        }

        [RelayCommand]
        private void Aceptar()
        {
            // Legacy: GuardarValoracionFrm.btAceptar_Click(...)
            //   short formato3ValoresPorFila;  // 3, 42 o 1
            //   char separador;                // ',', ' ' o (char)9 [tab]
            //
            //   Mapeo de FormatoSeleccionado -> formato3ValoresPorFila:
            //     "3 valores por línea"      => 3
            //     "42 valores en una línea"  => 42
            //     "1 valor por línea"        => 1
            //
            //   Mapeo de SeparadorSeleccionado -> separador:
            //     "[Tabulador]"            => (char)9
            //     "[Coma]"                 => ','
            //     "[Espacio en blanco]"    => ' '
            //
            // TODO dominio: mostrar SaveFileDialog (FileSavePicker en WinUI), filtro "*.txt",
            //   directorio inicial "Combinaciones\\", y delegar a Free1X2.Utils.Porcentajes:
            //     Pct.GuardarValoraciones(archivoSalida, formato3ValoresPorFila, separador, _valores1x2)        // matriz 2D
            //     Pct.GuardarValoraciones(archivoSalida, formato3ValoresPorFila, separador, _valores1, _valoresX, _valores2)  // 3 vectores
            //   Los datos (_valores1/_valoresX/_valores2 o _valores1x2) llegan por constructor en el legacy.
            // TODO: cerrar la página / cuadro de diálogo tras guardar (this.Close()).
        }

        [RelayCommand]
        private void Cancelar()
        {
            // Legacy: GuardarValoracionFrm.btCancelar_Click(...) -> this.Close();
            // TODO dominio: cerrar la página / cuadro de diálogo sin guardar.
        }
    }
}
