// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2008 Morrison - morrison.ne@gmail.com
// Idea original de Jose Carlos de Nova (ABDON) 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

using Free1X2.Escrutinio;
using Free1X2.Analisis;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class EstucolFrm : Form
    {
        string pathReducidas = "";
        string pathGanadoras = "";
        int[,] contenedorAciertos;
        List<long> ColumnasEmparejadas = new List<long>();
        int noTotalGanadoras;
        List<InformeColumnasABDON> informeCompletoPorCols = new List<InformeColumnasABDON>();
        List<InformeColumnasABDON> informeCompletoPorGans = new List<InformeColumnasABDON>();

        public EstucolFrm()
        {
            InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        private void EmparejarColumnasReducidas()
        {
            ModoEmparejamientoColumnasABDON modo = DeterminarMetodoOrdenacion();

            StreamReader sr = new StreamReader(pathReducidas);

            long colTemp = 0;
            long colTemp2 = 0;
            ColumnasEmparejadas.Clear();

            int noCol = 0;

            while (sr.Peek() != -1)
            {
                //Convertimos la columna leida en un long
                long columnaLeidaLong = Utils.UtilColumnas.ConvStrToLong(sr.ReadLine());

                noCol++;
                switch (modo)
                {
                    case ModoEmparejamientoColumnasABDON.A:
                        if (noCol % 2 == 0)
                        {
                            long colResultado = SumaColumnas(colTemp, columnaLeidaLong);

                            if (colResultado != columnaLeidaLong)
                            {
                                ColumnasEmparejadas.Add(colResultado);
                            }
                        }
                        colTemp = columnaLeidaLong;
                        break;
                    case ModoEmparejamientoColumnasABDON.B:
                        long colRes = SumaColumnas(colTemp, columnaLeidaLong);

                            if (colRes != columnaLeidaLong)
                            {
                                ColumnasEmparejadas.Add(colRes);
                            }                      

                        colTemp = columnaLeidaLong;
                        break;
                    case ModoEmparejamientoColumnasABDON.C:

                        if (noCol % 2 != 0)
                        {
                            //Sumar impares
                            long colResultado = SumaColumnas(colTemp, columnaLeidaLong);
                            if (colResultado != columnaLeidaLong)
                            {
                                ColumnasEmparejadas.Add(colResultado);
                            }
                            colTemp = columnaLeidaLong;
                        }
                        else
                        {
                            //Sumar pares
                            long colResultado = SumaColumnas(colTemp2, columnaLeidaLong);
                            if (colResultado != columnaLeidaLong)
                            {
                                ColumnasEmparejadas.Add(colResultado);
                            }
                            colTemp2 = columnaLeidaLong;
                        }

                        break;
                }

            }
            sr.Close();
            sr.Dispose();
        }
        
        private void EscrutarColumnasEmparejadas()
        {
            if (ColumnasEmparejadas.Count > 0)
            {
                contenedorAciertos = new int[ColumnasEmparejadas.Count, noTotalGanadoras];
                Escrutador esc = new Escrutador();

                int noColGanadora = 0;

                StreamReader sr = new StreamReader(pathGanadoras);
                while (sr.Peek() != -1)
                {
                    long colGan = Utils.UtilColumnas.ConvStrToLong(sr.ReadLine());

                    for (int i = 0; i < ColumnasEmparejadas.Count; i++)
                    {
                        int aciertos = esc.EscrutaApuestaMultiple(ColumnasEmparejadas[i], colGan);
                        contenedorAciertos[i, noColGanadora] = aciertos;
                    }
                    noColGanadora++;
                }
            }
        }
       
        private void ObtenDatosUnaColTodasGan()
        {
            informeCompletoPorCols.Clear();

            for (int i = 0; i < contenedorAciertos.GetLength(0); i++)
            { 
                int[] serie = new int[contenedorAciertos.GetLength(1)];
                for (int j = 0; j < contenedorAciertos.GetLength(1); j++)
                {
                    serie[j] = contenedorAciertos[i, j];
                }
                InformeColumnasABDON informe = new InformeColumnasABDON(serie);

                informeCompletoPorCols.Add(informe);
            }
        }
        
        private void ObtenDatosUnaGanTodasCol()
        {
            informeCompletoPorGans.Clear();

            for (int i = 0; i < contenedorAciertos.GetLength(1); i++)
            { 
                

                int[] serie = new int[contenedorAciertos.GetLength(0)];
                for (int j = 0; j < contenedorAciertos.GetLength(0); j++)
                {
                    serie[j] = contenedorAciertos[j, i];
                }
                InformeColumnasABDON informe = new InformeColumnasABDON(serie);

                informeCompletoPorGans.Add(informe);
            }
        }

        private void GenerarInforme()
        {
            ObtenDatosUnaColTodasGan();
            ObtenDatosUnaGanTodasCol();

            VisorAnalisisColumnasAbdonFrm visor = new VisorAnalisisColumnasAbdonFrm(informeCompletoPorCols, informeCompletoPorGans, ColumnasEmparejadas);
            visor.Show();
        }
        
        private long SumaColumnas(long colA, long colB)
        {
            //Recordatorio de bits:
            //100 = 4 Representa 1
            //010 = 2 Representa X
            //001 = 1 Representa 2
            //110 = 6 Representa 1X
            //101 = 5 Representa 12
            //011 = 3 Representa X2
            //111 = 7 Representa 1X2

            //Sumar las columnas:

            return (colA | colB);
        }
        
        private ModoEmparejamientoColumnasABDON DeterminarMetodoOrdenacion()
        {
            if (rdbAgrupacionA.Checked)
            {
                return ModoEmparejamientoColumnasABDON.A;
            }
            if (rdbAgrupacionB.Checked)
            {
                return ModoEmparejamientoColumnasABDON.B;
            }
            if (rdbAgrupacionC.Checked)
            {
                return ModoEmparejamientoColumnasABDON.C;
            }
            return ModoEmparejamientoColumnasABDON.A;
        }

        private bool ComprobarEntradas()
        {
            return (pathReducidas != "" && pathGanadoras != "");
        }
        
        private void btnAbreArchivoReducidas_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.InitialDirectory = Application.StartupPath + "/Columnas/";

            openFile.Filter = "Columnas Reducidas(*.txt)|*.txt|Todos los archivos(*.*)|*.*";
           
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                pathReducidas = openFile.FileName;

                lblNombreArchivoReducidas.Text = Path.GetFileName(pathReducidas);
            }

            openFile.Dispose();
            ActivaBtnComenzar();
        }
        private void ActivaBtnComenzar()
        {
            btnComenzar.Enabled = (pathGanadoras != "" && pathReducidas != "");
        }
        private void btnAbreArchivoGanadoras_Click(object sender, EventArgs e)
        {
            noTotalGanadoras = 0;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Application.StartupPath + "/Columnas/";
            openFile.Filter = "Columnas Ganadoras(*.txt)|*.txt|Todos los archivos(*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                //inicializar el path de columnas de entrada
                pathGanadoras = openFile.FileName;
                lblArchivoGanadoras.Text = Path.GetFileName(pathGanadoras);
                StreamReader sr = new StreamReader(pathGanadoras);
                while(sr.Peek() != -1)
                {
                    sr.ReadLine();
                    noTotalGanadoras++;
                }
                sr.Close();
                sr.Dispose();
            }
            openFile.Dispose();
            ActivaBtnComenzar();
        }

        private void btnComenzar_Click(object sender, EventArgs e)
        {
            lblEstado.Text = "Comprobando Entradas";
            Application.DoEvents();
            //Si las entradas son correctas...
            if (ComprobarEntradas())
            {
                lblEstado.Text = "Emparejando columnas";
                Application.DoEvents();
                //Empareja columnas:
                EmparejarColumnasReducidas();


                lblEstado.Text = "Escrutando Columnas";
                Application.DoEvents();
                //Escruta las columnas
                EscrutarColumnasEmparejadas();

                lblEstado.Text = "Generando Informe";
                Application.DoEvents();
                //A partir de los datos, generar el informe
                GenerarInforme();

            }
            else
            {
                MessageBox.Show("Debe especificar los dos archivos", "Compruebe los datos introducidos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void EstucolFrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose(true);
        }
    }
}
