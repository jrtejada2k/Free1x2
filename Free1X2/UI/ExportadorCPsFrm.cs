using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;
using System.IO;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class ExportadorCPsFrm : Form
    {
        List<ColumnaProbable> lista = new List<ColumnaProbable>();
        public ExportadorCPsFrm(List<ColumnaProbable> lista)
        {
            InitializeComponent();
            this.lista = lista;
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExportarSimples_Click(object sender, EventArgs e)
        {
            SaveFileDialog guardaArchivo = new SaveFileDialog();
            guardaArchivo.InitialDirectory = "Columnas\\";
            guardaArchivo.Filter = "Columnas Simples(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (guardaArchivo.ShowDialog() == DialogResult.OK)
            {
                string[] columnas = new string[lista.Count];

                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(guardaArchivo.FileName);
                for (int i = 0; i < lista.Count; i++)
                {
                    columnas[i] = lista[i].PronosticosString;
                }
                comBaseCols.GuardarTodasCols(columnas, true);
                comBaseCols.Cerrar();
                guardaArchivo.Dispose();
            }
            this.Close();
        }

        private void btnExportarClm_Click(object sender, EventArgs e)
        {
            SaveFileDialog guardaArchivo = new SaveFileDialog();
            guardaArchivo.InitialDirectory = "Columnas\\";
            guardaArchivo.Filter = "Columnas Con Aciertos(*.clm)|*.clm|Todos los archivos (*.*)|*.*";
            if (guardaArchivo.ShowDialog() == DialogResult.OK)
            {
                string[] columnas = new string[lista.Count];
                StreamWriter sw = new StreamWriter(guardaArchivo.FileName);
                for (int i = 0; i < lista.Count; i++)
                {
                    ColumnaProbable cp = lista[i];
                    columnas[i] = cp.PronosticosString + "#" + cp.GetAciertos() + "#" + cp.GetAciertosSeguidos() + "#" + cp.GetFallosSeguidos();
                    sw.WriteLine(columnas[i]);
                }
                sw.Close();
                sw.Dispose();
                guardaArchivo.Dispose();
            }
            this.Close();
        }
    }
}
