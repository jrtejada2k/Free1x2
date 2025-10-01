// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Free1X2.MotorCalculo;

namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlDataGridViewFiguras : UserControl
    {
        List<FiguraCondicion> figuras;
        int columnas = 0;
        IFiltro _filtro;
        string tipoFigura = "Figuras";
        public CtrlDataGridViewFiguras(List<FiguraCondicion> lstFiguras, IFiltro filtro)
        {
            InitializeComponent();
            this.figuras = lstFiguras;
            _filtro = filtro;
            LlenarFiguras();
            
        }
        public CtrlDataGridViewFiguras(List<FiguraCondicion> lstFiguras, IFiltro filtro, string tipo)
        {
            InitializeComponent();
            this.figuras = lstFiguras;
            _filtro = filtro;
            tipoFigura = tipo;
            LlenarFiguras();

        }
        protected void LlenarFiguras()
        {
            //Montar un Dataset
            DataSet dsFiguras = new DataSet();
            DataTable dtFiguras = new DataTable("Figuras");

            DataColumn dtcFiguras = new DataColumn();
            dtcFiguras.DataType = System.Type.GetType("System.String");
            dtcFiguras.ColumnName = "Figuras";
            dtFiguras.Columns.Add(dtcFiguras);

            dtcFiguras = new DataColumn();
            dtcFiguras.DataType = System.Type.GetType("System.Int32");
            dtcFiguras.ColumnName = "Columnas";
            dtFiguras.Columns.Add(dtcFiguras);

            for (int i = 0; i < figuras.Count; i++)
            {
                DataRow dtRow = dtFiguras.NewRow();

                dtRow["Columnas"] = figuras[i].Apariciones.ToString();
                if (figuras[i].Figura != 0)
                {
                    dtRow["Figuras"] = Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(figuras[i].Figura);
                }
                else
                {
                    dtRow["Figuras"] = "0";
                }
                dtFiguras.Rows.Add(dtRow);
            }
            dsFiguras.Tables.Add(dtFiguras);


            dataGridView1.DataSource = dsFiguras.Tables["Figuras"];


            lblFiguras.Text = tipoFigura + ": " + figuras.Count.ToString();
        }

        protected void MostrarSeleccionadas()
        {
            columnas = 0;
            DataGridViewSelectedRowCollection col = dataGridView1.SelectedRows;

            for (int i = 0; i < col.Count; i++)
            {
                DataGridViewRow dtr = col[i];
                columnas += (int)dtr.Cells["Columnas"].Value;
            }
            lblColumnas.Text = columnas.ToString();
            float importe = (float)(columnas * 0.5);
            lblImporte.Text = importe.ToString();
        }


        private void CtrlDataGridViewFiguras_Load(object sender, EventArgs e)
        {
            MostrarSeleccionadas();
        }

        private void dataGridView1_RowStateChanged_1(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            lblSeleccionadas.Text = dataGridView1.SelectedRows.Count.ToString();
            MostrarSeleccionadas();
        }
        public List<long> ObtenerFigurasMarcadas()
        {
            List<long> lista = new List<long>();
            FiguraCondicion fig = new FiguraCondicion();
            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
            {
                DataGridViewRow dtr = dataGridView1.SelectedRows[i];
                int apariciones = (int)dtr.Cells["Columnas"].Value;
                long figuraLong = Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(dtr.Cells["Figuras"].Value.ToString());
                fig = new FiguraCondicion();
                fig.Apariciones = apariciones;
                fig.Figura = figuraLong;

                lista.Add(fig.Figura);
            }
            return lista;
        }
        private void btnMarcarCondicion_Click(object sender, EventArgs e)
        {
            //Determinar que filtro vamos a modificar
            switch (_filtro.NombreFiltro)
            {
                case Filtro.Contactos:
                    FiltroContactos filtroCont = (FiltroContactos)_filtro;
                    //Respetar los valores introducidos, si no hay ninguno, llenar todos
                    
                    filtroCont.LlenarTodosValores();
                    filtroCont.Figuras = ObtenerFigurasMarcadas();
                    break;
            }
            //Si el filtro no está activo, activarlo
            if (!_filtro.IsActive)
            {
                _filtro.IsActive = true;
            }
            if (!_filtro.ContieneDatos)
            {
                _filtro.ContieneDatos = true;
            }
        }
        protected string DeterminarCondicion()
        {
            string condicion = "";
            switch(_filtro.NombreFiltro)
            {
                case Filtro.Contactos:
                    condicion = "Contactos";
                    break;
                case Filtro.SignosSeguidos:
                    condicion = "V1X2";
                    break;
                case Filtro.PesosNumericos:
                    condicion = "PN";
                    break;
            }
            return condicion;
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            List<long> lista = ObtenerFigurasMarcadas();
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = Application.StartupPath + "/Condiciones";
            string condicion = DeterminarCondicion();
            string extension = "(*.fig" + condicion +")|*.fig" + condicion + "|Todos los archivos (*.*)|*.*";
            saveFile.Filter = "Figuras de " + condicion + extension;

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                string nombre = saveFile.FileName;
                StreamWriter writer = new StreamWriter(nombre);
                for (int i = 0; i < lista.Count; i++)
                {
                    writer.WriteLine(Utils.UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(lista[i]));
                }
                writer.Close();
            }
            saveFile.Dispose();
        }



    }
}
