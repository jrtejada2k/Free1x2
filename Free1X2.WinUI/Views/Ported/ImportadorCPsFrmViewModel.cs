using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel portado de <c>Free1X2.UI.ImportadorCPsFrm</c>.
    ///
    /// El WinForms recibía una <c>List&lt;ColumnaProbable&gt; grupoCPtmp</c> del llamador
    /// (ColProbablesFrm) y la rellenaba leyendo un fichero. Aquí, sin form padre, las CPs
    /// importadas se acumulan en <see cref="ColumnasImportadas"/> y se expone su número; la
    /// lectura usa el motor real (ArchivoColumnasTexto + ColumnaProbable), igual que el legacy.
    /// </summary>
    public partial class ImportadorCPsFrmViewModel : ObservableObject
    {
        // Grupo temporal de CPs importadas (legacy: grupoCPtmp pasado por el llamador).
        public List<ColumnaProbable> ColumnasImportadas { get; private set; } = new();

        // Estado / feedback mostrado al usuario tras una acción.
        [ObservableProperty]
        private string _estadoTexto = "Sin importaciones realizadas.";

        // Número de CPs importadas (string para no bindear int a Text — regla anti-crash).
        [ObservableProperty]
        private string _numeroImportadasTexto = "0 columnas importadas.";

        /// <summary>
        /// Importar CPs simples desde un archivo *.txt.
        /// Legacy: ImportadorCPsFrm.btnImportarSimples_Click.
        /// </summary>
        [RelayCommand]
        private async Task ImportarSimples()
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSingleFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;
            string todosValores = UtilidadesEntradasValores.ObtenerTodosValores();

            bool formatoIncorrecto = false;
            var importadas = new List<ColumnaProbable>();

            await Task.Run(() =>
            {
                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(ruta);
                while (comBaseCols.SiguienteColumna())
                {
                    try
                    {
                        string columna = comBaseCols.LeeColumnaSinComas();
                        if (columna != "")
                        {
                            // Crear CP (igual que el legacy).
                            var cp = new ColumnaProbable();
                            cp.Pronosticos = ObtenPronostico(columna);
                            cp.SetNoAciertos(todosValores);
                            cp.SetNoAciertosSeguidos(todosValores);
                            cp.SetNoFallosSeguidos(todosValores);
                            importadas.Add(cp);
                        }
                    }
                    catch
                    {
                        formatoIncorrecto = true;
                        break;
                    }
                }
                comBaseCols.Cerrar();
            });

            if (formatoIncorrecto)
            {
                AppServices.MostrarError("Las columnas no tienen el formato correcto");
                EstadoTexto = "Error: formato de columnas incorrecto.";
                return;
            }

            ColumnasImportadas = importadas;
            ActualizarResumen();
            EstadoTexto = $"Importadas {importadas.Count} columnas simples desde {Path.GetFileName(ruta)}.";
        }

        /// <summary>
        /// Importar CPs con aciertos/aciertos seguidos/fallos seguidos desde *.clm.
        /// Legacy: ImportadorCPsFrm.btnImportarClm_Click.
        /// Formato: 111xxx22211xx2#1,2,3,4,5#1,2,3,4,5#1,2,3,4,5.
        /// </summary>
        [RelayCommand]
        private async Task ImportarClm()
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".clm");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? archivo = await picker.PickSingleFileAsync();
            if (archivo is null)
            {
                return;
            }

            string ruta = archivo.Path;

            bool formatoIncorrecto = false;
            var importadas = new List<ColumnaProbable>();

            await Task.Run(() =>
            {
                using var sr = new StreamReader(ruta);
                while (sr.Peek() != -1)
                {
                    try
                    {
                        // El formato es 111xxx22211xx2#1,2,3,4,5#1,2,3,4,5#1,2,3,4,5
                        string[] partes = sr.ReadLine()!.Split('#');
                        if (partes.Length == 4)
                        {
                            var cp = new ColumnaProbable();
                            cp.Pronosticos = ObtenPronostico(partes[0]);
                            cp.SetNoAciertos(partes[1]);
                            cp.SetNoAciertosSeguidos(partes[2]);
                            cp.SetNoFallosSeguidos(partes[3]);
                            importadas.Add(cp);
                        }
                    }
                    catch
                    {
                        formatoIncorrecto = true;
                        break;
                    }
                }
            });

            if (formatoIncorrecto)
            {
                AppServices.MostrarError("Las columnas no tienen el formato correcto");
                EstadoTexto = "Error: formato de columnas incorrecto.";
                return;
            }

            ColumnasImportadas = importadas;
            ActualizarResumen();
            EstadoTexto = $"Importadas {importadas.Count} columnas con aciertos desde {Path.GetFileName(ruta)}.";
        }

        /// <summary>
        /// Cancelar — descartar el grupo temporal.
        /// Legacy: ImportadorCPsFrm.btnCancelar_Click (grupoCPtmp = new List&lt;ColumnaProbable&gt;()).
        /// </summary>
        [RelayCommand]
        private void Cancelar()
        {
            ColumnasImportadas = new List<ColumnaProbable>();
            ActualizarResumen();
            EstadoTexto = "Importación cancelada.";
            // TODO (navegación): el WinForms hacía Close(); el llamador (ColProbablesFrm.ImportaColumnas,
            //   ver Free1X2/UI/Filtros/ColProbablesFrm.cs línea 690) leía grupoCPtmp tras ShowDialog.
            //   En WinUI, quien navegue a esta Page debe leer ColumnasImportadas al volver.
        }

        private void ActualizarResumen()
        {
            int n = ColumnasImportadas.Count;
            NumeroImportadasTexto = n == 1 ? "1 columna importada." : $"{n} columnas importadas.";
        }

        /// <summary>
        /// Copia literal de ImportadorCPsFrm.ObtenPronostico: convierte una línea a array de
        /// pronósticos (separados por comas, o asume NumeroPartidos signos a fijo).
        /// </summary>
        private static string[] ObtenPronostico(string lineaArchivo)
        {
            // Convertir a mayúsculas las posibles X...
            lineaArchivo = lineaArchivo.ToUpper();

            string[] pronostico;

            if (lineaArchivo.IndexOf(',') > -1)
            {
                pronostico = lineaArchivo.Split(',');
            }
            else
            {
                // Asumimos línea de NPartidos partidos pronosticados a fijo.
                pronostico = new string[VariablesGlobales.NumeroPartidos];

                for (int i = 0; i < pronostico.Length; i++)
                {
                    pronostico[i] = lineaArchivo[i].ToString();
                }
            }

            return pronostico;
        }
    }
}
