using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Free1X2.EntradaSalida;
using Free1X2.Utils;
using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class Compresor : Form
    {
        protected BitArray arrayColumnas = new BitArray(4782969, false);

        protected ConvertidorDeBases conv;
        protected int numPartidos = 0;
        public Compresor()
        {
            InitializeComponent();
            lblEstado.Text = "Preparado";
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        private void btnAbreArchivo_Click(object sender, EventArgs e)
        {
            EntradaFichero();
        }
        protected void EntradaFichero()
        {
            string columna;
            OpenFileDialog abreArchivo = new OpenFileDialog();
            abreArchivo.InitialDirectory = "*\\";
            abreArchivo.Filter = "Columnas(*.txt)|*.txt|Todos los archivos(*.*)|*.*";
            if (abreArchivo.ShowDialog() == DialogResult.OK)
            {
                lblNombreArchivo.Text = Path.GetFileName(abreArchivo.FileName);
                IArchivoColumnas ac = new ArchivoColumnasTexto(Path.GetFileName(abreArchivo.FileName));
                ac.SiguienteColumna();
                string col = ac.LeeColumnaSinComas();
                conv = new ConvertidorDeBases((byte)col.Length);
                arrayColumnas = new BitArray(conv.ObtenTamañoBitArray(col.Length), false);
                ac.Cerrar();
                ac = new ArchivoColumnasTexto(Path.GetFileName(abreArchivo.FileName));
                while (ac.SiguienteColumna())
                {
                    columna = ac.LeeColumnaSinComas();
                          
                    if ((columna.Length > 15) || (columna.Length < 14))
                    {
                        MessageBox.Show("Error leyendo columnas");
                        arrayColumnas.SetAll(false);
                        lblNombreArchivo.Text = "";
                        break;
                    }
                    arrayColumnas[conv.ConvColumnaANumero(columna)] = true;
                }
                ac.Cerrar();

                GuardarFicheroComprimido();
            }
        }
        protected void EntradaFicheroComprimido()
        {
            OpenFileDialog abreArchivo = new OpenFileDialog();
            abreArchivo.InitialDirectory = "*\\";
            abreArchivo.Filter = "Columnas Comprimidas(*.z3q)|*.z3q|Todos los archivos(*.*)|*.*";
            if (abreArchivo.ShowDialog() == DialogResult.OK)
            {
                lblNombreArchivo.Text = Path.GetFileName(abreArchivo.FileName);                

                this.arrayColumnas = CompresorZip.Descomprimir(abreArchivo.FileName);

                GuardarFicheroDescomprimido();
            }
        }

        protected void GuardarFicheroComprimido()
        {
            lblEstado.Text = "Guardando";
            SaveFileDialog guardaArchivo = new SaveFileDialog();
            guardaArchivo.InitialDirectory = "*\\";
            guardaArchivo.Filter = "Columnas Comprimidas(*.z3q)|*.z3q";
            if (guardaArchivo.ShowDialog() == DialogResult.OK)
            {
                //lblArchivoSalida.Text = Path.GetFileName(guardaArchivo.FileName);
                string a = Path.GetFileNameWithoutExtension(guardaArchivo.FileName);
                string path = Path.GetDirectoryName(guardaArchivo.FileName);
                string nombreFinal = path + "/" + a + ".z3q";
                CompresorZip.Comprimir(arrayColumnas, nombreFinal, guardaArchivo.FileName, arrayColumnas.Length,Convert.ToInt32(cbbNivel.Text));
                lblEstado.Text = "Guardado";
            }
        }
        protected void GuardarFicheroDescomprimido()
        {
            lblEstado.Text = "Guardando";
            SaveFileDialog guardaArchivo = new SaveFileDialog();
            guardaArchivo.InitialDirectory = "*\\";
            guardaArchivo.Filter = "Columnas(*.txt)|*.txt";

            conv = new ConvertidorDeBases();
            conv = new ConvertidorDeBases((byte)conv.ObtenNumeroPartidosColBin(arrayColumnas.Count));
            if (guardaArchivo.ShowDialog() == DialogResult.OK)
            {
                IArchivoColumnas aColsTxt = new ArchivoColumnasTexto(guardaArchivo.FileName);
                for (int i = 0; i < arrayColumnas.Count; i++)
                {
                    if (arrayColumnas[i])
                    {
                        aColsTxt.GuardarCols(conv.ConvNumAColumna(i));
                    }
                }
                aColsTxt.Cerrar();
                lblEstado.Text = "Guardado";
            }
            guardaArchivo.Dispose();
        }

        private void btnAbreArchivoComp_Click(object sender, EventArgs e)
        {
            EntradaFicheroComprimido();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
