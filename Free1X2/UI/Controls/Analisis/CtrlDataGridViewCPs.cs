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

using Free1X2.Analisis;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlDataGridViewCPs : UserControl
    {
        List<ContenedorColumnasProbables> contenedor;
        public CtrlDataGridViewCPs(List<ContenedorColumnasProbables> cont)
        {
            InitializeComponent();
            this.contenedor = cont;
            LlenarAciertos();
            LlenarAciertosSeguidos();
            LlenarFallosSeguidos();
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

        protected void LlenarAciertos()
        {
            //Montar un Dataset
            DataSet dsFiguras = new DataSet();
            DataTable dtFiguras = new DataTable("CPs");

            DataColumn dtcFiguras = new DataColumn();
            dtcFiguras.DataType = System.Type.GetType("System.Int32");
            dtcFiguras.ColumnName = "Nº";
            dtFiguras.Columns.Add(dtcFiguras);

            for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                dtcFiguras = new DataColumn();
                dtcFiguras.DataType = System.Type.GetType("System.Int32");
                dtcFiguras.ColumnName = i.ToString();
                dtFiguras.Columns.Add(dtcFiguras);
            }
            

            for (int i = 0; i < contenedor.Count; i++)
            {
                DataRow dtRow = dtFiguras.NewRow();

                dtRow["Nº"] = i + 1;
                for (int j = 0; j <= VariablesGlobales.NumeroPartidos; j++)
                {
                    dtRow[j.ToString()] = contenedor[i].NoAC[j];
                }
                dtFiguras.Rows.Add(dtRow);
            }
            dsFiguras.Tables.Add(dtFiguras);


            dataGridView1.DataSource = dsFiguras.Tables["CPs"];
        }
        protected void LlenarAciertosSeguidos()
        {
            DataSet dsFiguras = new DataSet();
            DataTable dtFiguras = new DataTable("CPs");

            DataColumn dtcFiguras = new DataColumn();
            dtcFiguras.DataType = System.Type.GetType("System.Int32");
            
            dtcFiguras.ColumnName = "Nº";
            dtFiguras.Columns.Add(dtcFiguras);
            for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                dtcFiguras = new DataColumn();
                dtcFiguras.DataType = System.Type.GetType("System.Int32");
                dtcFiguras.ColumnName = i.ToString();
                dtFiguras.Columns.Add(dtcFiguras);
            }
            

            for (int i = 0; i < contenedor.Count; i++)
            {
                DataRow dtRow = dtFiguras.NewRow();

                dtRow["Nº"] = i + 1;
                for (int j = 0; j <= VariablesGlobales.NumeroPartidos; j++)
                {
                    dtRow[j.ToString()] = contenedor[i].NoACS[j];
                }
                dtFiguras.Rows.Add(dtRow);
            }
            dsFiguras.Tables.Add(dtFiguras);


            dataGridView2.DataSource = dsFiguras.Tables["CPs"];
        }
        protected void LlenarFallosSeguidos()
        {
            DataSet dsFiguras = new DataSet();
            DataTable dtFiguras = new DataTable("CPs");

            DataColumn dtcFiguras = new DataColumn();
            dtcFiguras.DataType = System.Type.GetType("System.Int32");
            dtcFiguras.ColumnName = "Nº";
            dtFiguras.Columns.Add(dtcFiguras);
            for (int i = 0; i <= VariablesGlobales.NumeroPartidos; i++)
            {
                dtcFiguras = new DataColumn();
                dtcFiguras.DataType = System.Type.GetType("System.Int32");
                dtcFiguras.ColumnName = i.ToString();
                dtFiguras.Columns.Add(dtcFiguras);
            }

            for (int i = 0; i < contenedor.Count; i++)
            {
                DataRow dtRow = dtFiguras.NewRow();

                dtRow["Nº"] = i + 1;
                for (int j = 0; j <= VariablesGlobales.NumeroPartidos; j++)
                {
                    dtRow[j.ToString()] = contenedor[i].NoFS[j];
                }
                dtFiguras.Rows.Add(dtRow);
            }
            dsFiguras.Tables.Add(dtFiguras);


            dataGridView3.DataSource = dsFiguras.Tables["CPs"];
        }

    }
}
