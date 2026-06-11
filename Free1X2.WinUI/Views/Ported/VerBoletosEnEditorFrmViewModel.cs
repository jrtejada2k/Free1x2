using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        public VerBoletosEnEditorFrmViewModel()
        {
            // Constructor sin datos: muestra placeholder.
            // El legacy recibía string[] columnas en el constructor del Form.
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

                for (int z = 0; z < columnasB.GetLength(1); z++)
                {
                    sb.Append(salto);

                    // TODO Legacy: VerBoletosEnEditorFrm usa VariablesGlobales.Separador[0..2]
                    // para insertar líneas en blanco en los puntos de separación (jornadas/pleno).
                    // Reemplazar por el origen real de configuración cuando se porte el dominio.
                    // if (Convert.ToString(z + 1) == VariablesGlobales.Separador[k]) sb.Append(salto);

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
        /// Acción del botón "Imprimir" (legacy bImpDirec -> ImprimirBoletos).
        /// </summary>
        [RelayCommand]
        private void Imprimir()
        {
            // TODO Legacy: VerBoletosEnEditorFrm.ImprimirBoletos() usaba System.Drawing.Printing.PrintDocument
            // y dibujaba txtBoletos.Lines línea a línea (DrawString). En WinUI la impresión se hace con
            // Windows.Graphics.Printing.PrintManager / PrintDocument de WinUI. Implementar al portar el dominio.
        }
    }
}
