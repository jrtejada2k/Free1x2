// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.WinUI.Controls;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para GuardarValoracionFrmPage.
    /// Porta el WinForms legacy Free1X2.UI.GuardarValoracionFrm.
    /// Permite elegir el formato del fichero de salida y el separador de campos
    /// para exportar valoraciones (porcentajes) a un .txt.
    ///
    /// En el WinForms la matriz de valoraciones llegaba por constructor (desde el form invocante).
    /// En WinUI se edita en una rejilla <see cref="PorcentajesControl"/> (la colección <see cref="Filas"/>),
    /// que el motor consume como double[14,3] vía <see cref="PorcentajesHelper.AMatriz"/>.
    /// La grabación replica fielmente Free1X2.Utils.Porcentajes.Guardar (clase del proyecto WinForms,
    /// no portada al dominio; aquí se transcribe su escritura de texto, sin inventar lógica).
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

        /// <summary>
        /// Matriz editable de valoraciones (14 partidos x 1/X/2) que alimenta la
        /// <see cref="PorcentajesControl"/>. En el WinForms llegaba por constructor.
        /// </summary>
        public ObservableCollection<FilaPorcentaje> Filas { get; } =
            PorcentajesHelper.Crear(VariablesGlobales.NumeroPartidos);

        // Estado / feedback (sustituye los MessageBox del WinForms).
        [ObservableProperty]
        private string _estado = string.Empty;

        partial void OnFormatoSeleccionadoChanged(string value)
        {
            OnPropertyChanged(nameof(SeparadorHabilitado));
        }

        [RelayCommand]
        private async Task Aceptar()
        {
            // Legacy GuardarValoracionFrm.btAceptar_Click: mapea formato y separador.
            short formato;
            char separador = (char)9; // por defecto tabulador (rbTabulador.Checked)

            if (FormatoSeleccionado != "1 valor por línea")
            {
                formato = FormatoSeleccionado == "42 valores en una línea" ? (short)42 : (short)3;
                separador = SeparadorSeleccionado switch
                {
                    "[Coma]" => ',',
                    "[Espacio en blanco]" => ' ',
                    _ => (char)9, // [Tabulador]
                };
            }
            else
            {
                formato = 1;
            }

            // SaveFileDialog -> FileSavePicker (legacy: filtro "Valoraciones(*.txt)", carpeta Combinaciones).
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                DefaultFileExtension = ".txt",
                SuggestedFileName = "valoracion",
            };
            picker.FileTypeChoices.Add("Valoraciones", new List<string> { ".txt" });
            picker.FileTypeChoices.Add("Todos los archivos", new List<string> { "." });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSaveFileAsync();
            if (archivo is null)
            {
                return;
            }

            double[,] valores = PorcentajesHelper.AMatriz(Filas);

            try
            {
                await Task.Run(() => Guardar(archivo.Path, formato, separador, valores));
            }
            catch (Exception ex)
            {
                Estado = "Error al guardar la valoración: " + ex.Message;
                AppServices.MostrarError("Error al guardar la valoración: " + ex.Message);
                return;
            }

            Estado = $"Valoración guardada en {archivo.Name}.";
            AppServices.MostrarInfo("Valoración guardada correctamente");
        }

        [RelayCommand]
        private void Cancelar()
        {
            // Legacy: GuardarValoracionFrm.btCancelar_Click -> this.Close().
            // En WinUI no se cierra una ventana propia; sólo se limpia el feedback.
            Estado = "Cancelado.";
        }

        /// <summary>
        /// Réplica de Free1X2.Utils.Porcentajes.Guardar (Free1X2/Utils/Porcentajes.cs línea 354):
        /// escribe la matriz double[N,3] en texto según el formato (3 / 1 / 42) y el separador.
        /// </summary>
        private static void Guardar(string nombreArchivo, short formatoFichero, char separador, double[,] valores)
        {
            int n = VariablesGlobales.NumeroPartidos;
            using StreamWriter sw = File.CreateText(nombreArchivo);

            switch (formatoFichero)
            {
                case 3:
                    for (int i = 0; i < n; i++)
                    {
                        string linea = "";
                        for (int j = 0; j < 3; j++)
                        {
                            linea += valores[i, j].ToString().Replace(",", ".");
                            if (j < 2) linea += separador;
                        }
                        sw.WriteLine(linea);
                    }
                    break;

                case 1:
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            sw.WriteLine(valores[i, j].ToString());
                        }
                    }
                    break;

                case 42:
                    {
                        string linea = "";
                        for (int i = 0; i < n; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                linea += valores[i, j].ToString().Replace(",", ".") + separador;
                            }
                        }
                        sw.WriteLine(linea);
                    }
                    break;
            }
        }
    }
}
