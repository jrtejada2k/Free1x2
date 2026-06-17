using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Free1X2;
using Free1X2.EntradaSalida;
using Free1X2.WinUI.Services;
using Windows.Storage.Pickers;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// ViewModel para VerBoletosEnEditorFrmPage.
    /// Legacy: Free1X2.UI.VerBoletosEnEditorFrm — muestra las columnas de apuesta
    /// formateadas en vertical (estilo boleto) dentro de un TextBox de solo lectura
    /// con fuente monoespaciada, y permite imprimirlas directamente.
    /// </summary>
    public partial class VerBoletosEnEditorFrmViewModel : ObservableObject
    {
        // Texto formateado de los boletos (equivale a txtBoletos.Text del legacy,
        // multiline, ReadOnly, fuente "Lucida Console").
        [ObservableProperty]
        private string _boletosTexto = string.Empty;

        /// <summary>
        /// Se solicita imprimir/exportar la vista de boletos. Lo escucha el code-behind de la Page,
        /// que renderiza el texto completo a un mapa de bits (RenderTargetBitmap) y lo lleva al
        /// portapapeles / lo guarda como imagen. El VM no referencia controles de la vista (MVVM).
        /// </summary>
        public event EventHandler? ImpresionSolicitada;

        public VerBoletosEnEditorFrmViewModel()
        {
            // Constructor sin datos: muestra placeholder.
            // El legacy recibía string[] columnas en el constructor del Form.
        }

        /// <summary>
        /// Abre un fichero de columnas y las muestra como boletos verticales. Réplica del flujo
        /// legacy verBoletosEnEditorDeTextoToolStripMenuItem_Click (MainForm.cs:781): OpenFileDialog
        /// (*.txt en /Columnas), luego aCol.LeerTodasCols(false) y new VerBoletosEnEditorFrm(cols).
        /// En el WinForms el fichero lo elegía el menú antes de abrir el form; aquí, como la página
        /// se abre por navegación, la propia página ofrece el botón "Cargar columnas…".
        /// </summary>
        [RelayCommand]
        private async Task CargarFicheroAsync()
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
            };
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add("*");
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            var file = await picker.PickSingleFileAsync();
            if (file is null) return;

            try
            {
                // Legacy: IArchivoColumnas aCol = new ArchivoColumnasTexto(ruta); cols = aCol.LeerTodasCols(false).
                IArchivoColumnas aCol = new ArchivoColumnasTexto(file.Path);
                string[] cols = aCol.LeerTodasCols(false);
                aCol.Cerrar();
                CargarColumnas(cols);
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se ha podido leer el fichero de columnas: " + ex.Message);
            }
        }

        /// <summary>
        /// Carga y formatea las columnas de apuesta en boletos verticales.
        /// Replica la lógica del constructor de VerBoletosEnEditorFrm (formateo de texto).
        /// </summary>
        public void CargarColumnas(string[] columnas)
        {
            if (columnas == null || columnas.Length == 0)
            {
                BoletosTexto = string.Empty;
                return;
            }

            string[,] columnasB = TransformarColumnas(columnas);
            int numBoletos = (int)(columnas.Length / 8);
            if (numBoletos < columnas.Length) numBoletos++;

            string salto = Environment.NewLine;
            const string linea = "__________________";
            var sb = new StringBuilder();

            for (int i = 0; i < numBoletos; i++)
            {
                int noBol = i + 1;
                if (noBol == 1)
                    sb.Append("Boleto " + noBol);
                else
                    sb.Append(salto + salto + "Boleto " + noBol);

                sb.Append(salto + linea);

                // Legacy: VariablesGlobales.Separador[0..2] marca los puntos de separación
                // (jornadas/pleno) donde se inserta una línea en blanco extra. Separador se
                // inicializa en el arranque de la app (por defecto ["1","X","2"]); se protege
                // por si aún no estuviera poblado.
                string[]? separador = VariablesGlobales.Separador;

                for (int z = 0; z < columnasB.GetLength(1); z++)
                {
                    sb.Append(salto);

                    // Legacy: if (Convert.ToString(z+1) == Separador[0|1|2]) sb.Append(salto).
                    if (separador is { Length: >= 3 })
                    {
                        string zStr = Convert.ToString(z + 1);
                        if (zStr == separador[0] || zStr == separador[1] || zStr == separador[2])
                        {
                            sb.Append(salto);
                        }
                    }

                    for (int j = (i * 8); j < (noBol * 8); j++)
                    {
                        if (j < columnas.Length)
                            sb.Append(columnasB[j, z] + " ");
                    }
                }
            }

            BoletosTexto = sb.ToString();
        }

        // Replica VerBoletosEnEditorFrm.TransformarColumnas: pone cada signo en su fila.
        private static string[,] TransformarColumnas(string[] columnas)
        {
            int longitudCol = columnas.Length > 0 ? columnas[0].Length : 0;
            var colsTransformadas = new string[columnas.Length, longitudCol];

            for (int i = 0; i < columnas.Length; i++)
            {
                char[] signos = columnas[i].ToCharArray();
                for (int j = 0; j < signos.Length; j++)
                {
                    colsTransformadas[i, j] = signos[j].ToString();
                }
            }

            return colsTransformadas;
        }

        /// <summary>
        /// Acción del botón "Imprimir" (legacy bImpDirec -> ImprimirBoletos, dibujaba txtBoletos.Lines
        /// línea a línea con System.Drawing.Printing). Equivalente funcional WinUI: renderizar el texto
        /// completo de los boletos a un mapa de bits (RenderTargetBitmap), copiarlo al portapapeles y
        /// ofrecer guardarlo como imagen PNG. El render (que necesita un UIElement) lo hace la Page.
        /// Ver Free1X2/UI/VerBoletosEnEditorFrm.cs líneas 80-97.
        /// </summary>
        [RelayCommand]
        private void Imprimir()
        {
            if (string.IsNullOrEmpty(BoletosTexto))
            {
                return; // sin boletos cargados, nada que exportar.
            }
            ImpresionSolicitada?.Invoke(this, EventArgs.Empty);
        }
    }
}
