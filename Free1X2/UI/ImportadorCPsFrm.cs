using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Free1X2.MotorCalculo;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class ImportadorCPsFrm : Form
    {
        List<ColumnaProbable> grupoCPtmp = new List<ColumnaProbable>();
        protected FormulariosHelper formHelper = new FormulariosHelper();

        public ImportadorCPsFrm(List<ColumnaProbable> grupoCPtmp)
        {
            this.grupoCPtmp = grupoCPtmp;
            InitializeComponent();
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        private void btnImportarSimples_Click(object sender, EventArgs e)
        {
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Columnas\\";
            abreCombDialog.Filter = "Columnas Simples(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (abreCombDialog.ShowDialog() == DialogResult.OK)
            {
                ColumnaProbable cp;

                string todosValores = formHelper.ObtenerTodosValores();

                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(abreCombDialog.FileName);

                while (comBaseCols.SiguienteColumna())
                {
                    try
                    {
                        string columna = comBaseCols.LeeColumnaSinComas();
                        if (columna != "")
                        {
                            //crear CP y poner en grupo
                            cp = new ColumnaProbable();
                            cp.Pronosticos = ObtenPronostico(columna);
                            cp.SetNoAciertos(todosValores);
                            cp.SetNoAciertosSeguidos(todosValores);
                            cp.SetNoFallosSeguidos(todosValores);
                            // Las CPs se ponen en un grupo temporal!!
                            grupoCPtmp.Add(cp);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Las columnas no tienen el formato correcto");
                        return;
                    }
                }
                comBaseCols.Cerrar();
            }
            this.Close();
        }

        private void btnImportarClm_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Columnas\\";
            abreCombDialog.Filter = "Columnas Con Aciertos(*.clm)|*.clm|Todos los archivos (*.*)|*.*";

            if (abreCombDialog.ShowDialog() == DialogResult.OK)
            {
                ColumnaProbable cp;
                StreamReader sr = new StreamReader(abreCombDialog.FileName);
                while (sr.Peek() != -1)
                {
                    try
                    {
                        //el formato es 111xxx22211xx2#1,2,3,4,5#1,2,3,4,5#1,2,3,4,5
                        string[] partes = sr.ReadLine().Split('#');
                        if (partes.Length == 4)
                        {
                            //crear CP y poner en grupo
                            cp = new ColumnaProbable();
                            cp.Pronosticos = ObtenPronostico(partes[0]);
                            cp.SetNoAciertos(partes[1]);
                            cp.SetNoAciertosSeguidos(partes[2]);
                            cp.SetNoFallosSeguidos(partes[3]);
                            // Las CPs se ponen en un grupo temporal!!
                            grupoCPtmp.Add(cp);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Las columnas no tienen el formato correcto");
                        return;
                    }
                }
            }
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            grupoCPtmp = new List<ColumnaProbable>();
            this.Close();
        }

        protected string[] ObtenPronostico(string lineaArchivo)
        {
            //covertir a mayusculas las posibles X...
            lineaArchivo = lineaArchivo.ToUpper();

            string[] pronostico;

            if (lineaArchivo.IndexOf(',') > -1)
            {
                pronostico = lineaArchivo.Split(',');
            }
            else
            {
                //asumimos linea es de NPartidos partidos pronosticados a fijo
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
