using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Free1X2.UI
{
    public partial class VerBoletosEnEditorFrm : Form
    {
        public VerBoletosEnEditorFrm(string[] columnas)
        {
            InitializeComponent();

            //Colocar las columnas en vertical
            string[,] columnasB = TransformarColumnas(columnas);
            int numBoletos = (int)(columnas.Length / 8);
            if (numBoletos < columnas.Length) numBoletos++;
            string salto = Environment.NewLine;
            string linea = "__________________";
            for (int i = 0; i < numBoletos; i++)
            {
                int noBol = i + 1;
                if (noBol == 1)
                {
                    txtBoletos.Text += "Boleto " + noBol;
                }
                else
                {
                    txtBoletos.Text += salto + salto + "Boleto " + noBol;
                }
                txtBoletos.Text += salto + linea;
                for (int z = 0; z < columnasB.GetLength(1); z++)
                {
                    txtBoletos.Text += salto;
                    if (Convert.ToString(z + 1) == VariablesGlobales.Separador[0] ||
                        Convert.ToString(z + 1) == VariablesGlobales.Separador[1] ||
                        Convert.ToString(z + 1) == VariablesGlobales.Separador[2])
                    {
                        txtBoletos.Text += salto;
                    }
                    for (int j = (i * 8); j < (noBol * 8); j++)
                    {
                        if (j < columnas.Length)
                        {
                            txtBoletos.Text += columnasB[j, z] + " ";
                        }
                    }
                }
            }
        }

        private string[,] TransformarColumnas(string[] columnas)
        {
            int longitudCol = 0;
            if (columnas.Length > 0)
            {
                longitudCol = columnas[0].Length;
            }

            string[,] colsTransformadas = new string[columnas.Length,longitudCol];
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

        private void ImprimirBoletos()
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += Imprimir;

            pd.Print();
        }
        void Imprimir(object sender, PrintPageEventArgs ev)
        {
            float x = 1;
            float y = 1;
                SolidBrush myBrush = new SolidBrush(Color.Black);
                for (int i = 0; i < txtBoletos.Lines.Length; i++)
                {
                    ev.Graphics.DrawString(txtBoletos.Lines[i],txtBoletos.Font,myBrush, new PointF(x,y));
                    y+=10;
                }
        }

        private void bImpDirec_Click(object sender, EventArgs e)
        {
            ImprimirBoletos();
        }

    }
}
